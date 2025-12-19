using System.Collections.Generic;
using System.Threading.Tasks;
using shipping_app.Models;

namespace shipping_app.Repositories
{
    public interface IIepCrossingDockShipmentRepository
    {
        List<IepCrossingDockShipment> GetListShippment(string _connectionString);
        List<IepCrossingDockShipment?> GetListShippmentById(string id, string connectionString);
    }
}
