
using Microsoft.AspNetCore.Mvc;
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
    }
}
