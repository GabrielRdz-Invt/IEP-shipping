using System.Collections.Generic;
using System.Threading.Tasks;
using shipping_app.Models;

namespace shipping_app.Repositories
{
    public interface IIepCrossingDockShipmentRepository
    {
        List<IepCrossingDockShipment> GetListShippment(string _connectionString);
        List<IepCrossingDockShipment?> GetListShippmentById(string id, string connectionString);

        // Insert new shipment record
        Task<bool> InsertAsync(IepCrossingDockShipmentCreateDto dto, string connectionString);
        Task<bool> ExistsAsync(string id, string connectionString);

        // Delete shipment record by ID
        Task<bool> DeleteAsync(string id, string connectionString);

        // Update shipment status by ID
        Task<bool> UpdateStatusWithShipOutAsync(string id, StatusUpdateDto statusUpdateDto, string connectionString);

        Task<bool> UpdateAsync(string id, IepCrossingDockShipmentUpdateDto dto, string connectionString);

        Task<string> GenerateNextIdAsync(DateTime nowLocal, string connectionString);

        Task<List<object>> GetReportRowsAsync(string dateColumn, DateTime from, DateTime to, string connectionString);
    }
}
