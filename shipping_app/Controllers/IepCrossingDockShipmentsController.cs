using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using shipping_app.Models;
using shipping_app.Repositories;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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

        [HttpGet("next-id")]
        public async Task<ActionResult<string>> GetNextId()
        {
            try
            {
                var nowLocal = DateTime.Now;
                var nextId = await _repo.GenerateNextIdAsync(nowLocal, _connectionString);
                return Ok(new { id = nextId });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando Next ID.");
                return StatusCode(500, "Internal server error");

            }
        }


        [HttpGet("report")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string? from,
            [FromQuery] string? to,
            [FromQuery] string? dateField = "rcvd")
        {
            try
            {
                // [CAMBIO] Validación de params y parseo seguro
                if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                    return BadRequest("Parámetros 'from' y 'to' son requeridos.");

                // Aceptamos ISO o 'yyyy-MM-dd'. Usamos límites inclusivos por día.
                if (!DateTime.TryParse(from, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var fromDt))
                    return BadRequest("Formato inválido en 'from'. Usa ISO o yyyy-MM-dd.");
                if (!DateTime.TryParse(to, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var toDt))
                    return BadRequest("Formato inválido en 'to'. Usa ISO o yyyy-MM-dd.");

                // Cerramos rango (inclusive). Opción A: <= finDelDía
                var fromBound = new DateTime(fromDt.Year, fromDt.Month, fromDt.Day, 0, 0, 0, DateTimeKind.Local);
                var toBound = new DateTime(toDt.Year, toDt.Month, toDt.Day, 23, 59, 59, 999, DateTimeKind.Local);

                // [CAMBIO] Selección de columna de fecha permitida (evita inyección)
                var col = (dateField ?? "rcvd").Trim().ToLowerInvariant() switch
                {
                    "shipout" => "ShipOutDate",
                    _ => "RCVDDATE"
                };

                // [CAMBIO] Consulta con parámetros (ADO.NET)
                var rows = await GetReportRowsAsync(col, fromBound, toBound, _connectionString);

                return Ok(rows); // JSON
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SqlException en Report.");
                var pd = new ProblemDetails
                {
                    Title = "Error generando Reporte",
                    Detail = $"SQL {sqlEx.Number}: {sqlEx.Message}",
                    Status = StatusCodes.Status500InternalServerError
                };
                return StatusCode(pd.Status!.Value, pd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando Reporte.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("report.csv")]
        public async Task<IActionResult> GetReportCsv(
            [FromQuery] string? from,
            [FromQuery] string? to,
            [FromQuery] string? dateField = "rcvd")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                    return BadRequest("Parámetros 'from' y 'to' son requeridos.");

                if (!DateTime.TryParse(from, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var fromDt))
                    return BadRequest("Formato inválido en 'from'. Usa ISO o yyyy-MM-dd.");
                if (!DateTime.TryParse(to, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var toDt))
                    return BadRequest("Formato inválido en 'to'. Usa ISO o yyyy-MM-dd.");

                var fromBound = new DateTime(fromDt.Year, fromDt.Month, fromDt.Day, 0, 0, 0, DateTimeKind.Local);
                var toBound = new DateTime(toDt.Year, toDt.Month, toDt.Day, 0, 0, 0, DateTimeKind.Local);

                var col = (dateField ?? "rcvd").Trim().ToLowerInvariant() switch
                {
                    "shipout" => "ShipOutDate",
                    _ => "RCVDDATE"
                };


                var rows = await GetReportRowsAsync(col, fromBound, toBound, _connectionString);

                // [CAMBIO] Serialización CSV segura
                var csv = BuildCsv(rows);

                var fileName = $"report_{fromBound:yyyy-MM-dd}_to_{toBound:yyyy-MM-dd}_{col}.csv";
                return File(Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SqlException en Report CSV.");
                var pd = new ProblemDetails
                {
                    Title = "Error generando CSV",
                    Detail = $"SQL {sqlEx.Number}: {sqlEx.Message}",
                    Status = StatusCodes.Status500InternalServerError
                };
                return StatusCode(pd.Status!.Value, pd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando CSV.");
                return StatusCode(500, ex.Message);
            }
        }

        // [CAMBIO] Helper ADO.NET para obtener filas del reporte
        private async Task<List<object>> GetReportRowsAsync(string dateColumn, DateTime from, DateTime to, string connectionString)
        {
            var list = new List<object>();

            // Campos habituales; ajusta según tu tabla
            var sql = $@"
            SELECT
                [ID],
                [STATUS],
                [HAWB],
                [INVREFPO],
                [IECPARTNUM],
                [QTY],
                [BULKS],
                [CARRIER],
                [Bin],
                [RCVDDATE],
                [ShipOutDate],
                [Operator]
            FROM dbo.IEP_Crossing_Dock_Shipment
            WHERE {dateColumn} IS NOT NULL
                AND {dateColumn} >= @from
                AND {dateColumn} = @to
            ORDER BY {dateColumn} ASC, [ID] ASC;";

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@from", System.Data.SqlDbType.DateTime).Value = from;
            cmd.Parameters.Add("@to", System.Data.SqlDbType.DateTime).Value = to;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // [CAMBIO] Proyección ligera a objeto anónimo para JSON/CSV
                list.Add(new
                {
                    id = reader["ID"]?.ToString(),
                    status = reader["STATUS"]?.ToString(),
                    hawb = reader["HAWB"]?.ToString(),
                    invRefPo = reader["INVREFPO"]?.ToString(),
                    iecPartNum = reader["IECPARTNUM"]?.ToString(),
                    qty = reader["QTY"] != DBNull.Value ? Convert.ToInt32(reader["QTY"]) : (int?)null,
                    bulks = reader["BULKS"]?.ToString(),
                    carrier = reader["CARRIER"]?.ToString(),
                    bin = reader["Bin"]?.ToString(),
                    rcvdDate = reader["RCVDDATE"] != DBNull.Value ? Convert.ToDateTime(reader["RCVDDATE"]).ToString("s") : null, // ISO sin zona
                    shipOutDate = reader["ShipOutDate"] != DBNull.Value ? Convert.ToDateTime(reader["ShipOutDate"]).ToString("s") : null,
                    operatorName = reader["Operator"]?.ToString(),
                });
            }

            return list;
        }

        // [CAMBIO] Helper para CSV (escapa comas, comillas y saltos de línea)
        private static string BuildCsv(List<object> rows)
        {
            var sb = new StringBuilder();

            // Encabezados
            sb.AppendLine("ID,Status,HAWB,INV Ref PO,IEC Part Num,Qty,Bulks,Carrier,Bin,RcvdDate,ShipOutDate,Operator");

            foreach (dynamic r in rows)
            {
                string Esc(string? v)
                {
                    if (string.IsNullOrEmpty(v)) return "";
                    var needsQuotes = v.Contains(',') || v.Contains('"') || v.Contains('\n') || v.Contains('\r');
                    var s = v.Replace("\"", "\"\"");
                    return needsQuotes ? $"\"{s}\"" : s;
                }

                sb.AppendLine(string.Join(",", new[]
                {
                    Esc(r.id),
                    Esc(r.status),
                    Esc(r.hawb),
                    Esc(r.invRefPo),
                    Esc(r.iecPartNum),
                    r.qty?.ToString() ?? "",
                    Esc(r.bulks),
                    Esc(r.carrier),
                    Esc(r.bin),
                    Esc(r.rcvdDate),
                    Esc(r.shipOutDate),
                    Esc(r.operatorName),
                }));
            }

            return sb.ToString();
        }


    }
}
