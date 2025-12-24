
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using shipping_app.Models;
using shipping_app.Repositories;

namespace shipping_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IepCrossingDockShipmentsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IIepCrossingDockShipmentRepository _repo;
        private readonly ILogger<IepCrossingDockShipmentsController> _logger;

        public IepCrossingDockShipmentsController(
            IIepCrossingDockShipmentRepository repo,
            ILogger<IepCrossingDockShipmentsController> logger,
            IConfiguration configuration)
        {
            _repo = repo;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("shipping_appCon")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'shipping_appCon'.");
        }

        // GET api/IepCrossingDockShipments
        [HttpGet]
        public ActionResult<List<IepCrossingDockShipment>> GetAll()
        {
            try
            {
                var shipments = _repo.GetListShippment(_connectionString);
                return Ok(shipments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo lista de IEP Crossing Dock Shipments.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/IepCrossingDockShipments/{id}
        [HttpGet("{id}")]
        public ActionResult<List<IepCrossingDockShipment>> GetById([FromRoute] string id)
        {
            try
            {
                var shipments = _repo.GetListShippmentById(id, _connectionString);

                if (shipments == null)
                {
                    return StatusCode(500, "Internal server error");
                }

                if (shipments.Count == 0)
                {
                    return NotFound($"No se encontró envío con ID '{id}'.");
                }

                return Ok(shipments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo IEP Crossing Dock Shipment con ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }


        // POST api/IepCrossingDockShipments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IepCrossingDockShipmentCreateDto dto)
        {
            try
            {
                if (dto is null || string.IsNullOrWhiteSpace(dto.ID))
                    return BadRequest("Se requiere un objeto válido y un ID no vacío.");

                if (await _repo.ExistsAsync(dto.ID, _connectionString))
                    return Conflict($"Ya existe un envío con ID '{dto.ID}'.");

                try
                {
                    var ok = await _repo.InsertAsync(dto, _connectionString);
                    if (!ok) return StatusCode(500, "No se pudo insertar el registro.");
                }
                catch (SqlException sqlEx)
                {
                    // Log detallado para ver causa real
                    _logger.LogError(sqlEx, "SqlException en INSERT");
                    var pd = new ProblemDetails
                    {
                        Title = "Error al insertar en BD",
                        Detail = $"SQL {sqlEx.Number}: {sqlEx.Message}",
                        Status = StatusCodes.Status500InternalServerError
                    };
                    return StatusCode(pd.Status!.Value, pd); // devuelve el mensaje concreto
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Excepción no-SQL en INSERT");
                    return StatusCode(500, ex.Message);
                }

                IepCrossingDockShipment? resource = null;
                try
                {
                    var created = _repo.GetListShippmentById(dto.ID, _connectionString);
                    resource = created?.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Insert ok, pero falló el read-back.");
                }

                return CreatedAtAction(nameof(GetById), new { id = dto.ID }, (object?)resource ?? dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando IEP Crossing Dock Shipment.");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/IepCrossingDockShipments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("ID es requerido.");

                var exists = await _repo.ExistsAsync(id, _connectionString);
                if (!exists) return NotFound($"No existe envío con ID '{id}'.");

                try
                {
                    var ok = await _repo.DeleteAsync(id, _connectionString);
                    if (!ok)
                        return StatusCode(500, "No se pudo eliminar el registro."); // si por alguna razón no afectó filas
                }
                catch (SqlException sqlEx)
                {
                    // Por si el usuario no tiene permisos o hay FK/trigger
                    _logger.LogError(sqlEx, "SqlException en DELETE");
                    return StatusCode(500, $"SQL {sqlEx.Number}: {sqlEx.Message}");
                }

                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error eliminando ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/IepCrossingDockShipments/{id}/status
        [HttpPut("{id}/scanout")]
        public async Task<IActionResult> ScanOut([FromRoute] string id, [FromBody] StatusUpdateDto dto)
        {
            try
            {
                var idNorm = (id ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("ID es requerido.");

                // Validación simple: para Scan Out esperamos Status = 2
                if (dto == null || dto.Status != "2")
                    return BadRequest("Status inválido. Para Scan Out debe ser 2.");

                var exists = await _repo.ExistsAsync(id, _connectionString);
                if (!exists) return NotFound($"No existe envío con ID '{id}'.");

                try
                {
                    var ok = await _repo.UpdateStatusWithShipOutAsync(id, dto, _connectionString);
                    if (!ok) return StatusCode(500, "No se pudo actualizar el status/fecha de salida.");
                }
                catch (SqlException sqlEx)
                {
                    _logger.LogError(sqlEx, "SqlException en ScanOut");
                    return StatusCode(500, $"SQL {sqlEx.Number}: {sqlEx.Message}");
                }

                // Opcional: devolver la fila actualizada
                var updated = _repo.GetListShippmentById(id, _connectionString)?.FirstOrDefault();
                return updated is not null
                    ? Ok(updated)
                    : Ok(new { id, Status = dto.Status, ShipOutDate = dto.ShipOutDate });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en Scan Out (ID {id})");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] IepCrossingDockShipmentUpdateDto dto)
        {
            try
            {
                var idNorm = (id ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(idNorm))
                    return BadRequest("ID es requerido.");

                var exists = await _repo.ExistsAsync(idNorm, _connectionString);
                if (!exists) return NotFound($"No existe envío con ID '{idNorm}'.");

                try
                {
                    var ok = await _repo.UpdateAsync(idNorm, dto, _connectionString);
                    if (!ok) return StatusCode(500, "No se pudo actualizar el registro.");
                }
                catch (SqlException sqlEx)
                {
                    _logger.LogError(sqlEx, "SqlException en UPDATE");
                    return StatusCode(500, $"SQL {sqlEx.Number}: {sqlEx.Message}");
                }

                var updated = _repo.GetListShippmentById(idNorm, _connectionString)?.FirstOrDefault();
                return updated is not null
                    ? Ok(updated)
                    : Ok(dto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error actualizando ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
