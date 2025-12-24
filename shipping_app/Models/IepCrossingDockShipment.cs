using Microsoft.Data.SqlClient;
using shipping_app.Repositories;
using System;
using System.Data;

namespace shipping_app.Models
{
    public class IepCrossingDockShipment : IIepCrossingDockShipmentRepository
    {
        public string ID { get; set; } = default!;
        public string? HAWB { get; set; }
        public string? InvRefPo { get; set; }
        public string? HpPartNum { get; set; }
        public string? IecPartNum { get; set; }
        public int? Qty { get; set; }
        public string? Bulks { get; set; }
        public string? BoxPlt { get; set; }
        public DateTime? RcvdDate { get; set; }
        public string? Status { get; set; }
        public string? Carrier { get; set; }
        public string? Shipper { get; set; }
        public string? Bin { get; set; }
        public string? ShipOutStatus { get; set; }
        public int? RemainQty { get; set; }
        public DateTime? ShipOutDate { get; set; }
        public string? TruckNum { get; set; }
        public string? SealNum { get; set; }
        public string? ContainerNum { get; set; }
        public string? ImxInvNum { get; set; }
        public string? Operator { get; set; }
        public DateTime? Cdt { get; set; }
        public DateTime? Udt { get; set; }
        public string? Remark { get; set; }
        public string? Weight { get; set; }


        public List<IepCrossingDockShipment> GetListShippment(string _connectionString)
        {
            var list = new List<IepCrossingDockShipment>();
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                const string query = @"
                    SELECT *
                    FROM IEP_Crossing_Dock_Shipment
                    ORDER BY [ID] DESC;";

                using var command = new SqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var s = new IepCrossingDockShipment
                    {
                        ID = reader["ID"].ToString(),
                        HAWB = reader["HAWB"].ToString(),
                        InvRefPo = reader["INVREFPO"].ToString(),
                        HpPartNum = reader["HPPARTNUM"].ToString(),
                        IecPartNum = reader["IECPARTNUM"].ToString(),
                        Qty = reader["QTY"] != DBNull.Value ? Convert.ToInt32(reader["QTY"]) : (int?)null,
                        Bulks = reader["BULKS"].ToString(),
                        BoxPlt = reader["BOXPLT"].ToString(),
                        RcvdDate = reader["RCVDDATE"] != DBNull.Value ? Convert.ToDateTime(reader["RCVDDATE"]) : (DateTime?)null,
                        Status = reader["STATUS"].ToString(),
                        Carrier = reader["CARRIER"].ToString(),
                        Shipper = reader["SHIPPER"].ToString(),
                        Bin = reader["Bin"].ToString(),
                        ShipOutStatus = reader["ShipOutStatus"].ToString(),
                        RemainQty = reader["RemainQty"] != DBNull.Value ? Convert.ToInt32(reader["RemainQty"]) : (int?)null,
                        ShipOutDate = reader["ShipOutDate"] != DBNull.Value ? Convert.ToDateTime(reader["ShipOutDate"]) : (DateTime?)null,
                        TruckNum = reader["Truck#"].ToString(),
                        SealNum = reader["SEAL#"].ToString(),
                        ContainerNum = reader["Container#"].ToString(),
                        ImxInvNum = reader["IMX_INV#"].ToString(),
                        Operator = reader["Operator"].ToString(),
                        Cdt = reader["Cdt"] != DBNull.Value ? Convert.ToDateTime(reader["Cdt"]) : (DateTime?)null,
                        Udt = reader["Udt"] != DBNull.Value ? Convert.ToDateTime(reader["Udt"]) : (DateTime?)null,
                        Remark = reader["REMARK"].ToString(),
                        Weight = reader["WEIGHT"].ToString(),
                    };
                    list.Add(s);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new List<IepCrossingDockShipment>();
            }

            return list;
        }


        public List<IepCrossingDockShipment> GetListShippmentById(string id, string _connectionString)
        {
            var listShipment = new List<IepCrossingDockShipment>();
            if (string.IsNullOrWhiteSpace(id)) return listShipment;

            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                const string query = @"
                    SELECT *
                    FROM IEP_Crossing_Dock_Shipment
                    WHERE [ID] = @Id;";

                using var command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 50) { Value = id });

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var shipment = new IepCrossingDockShipment
                    {
                        ID = reader["ID"].ToString(),
                        HAWB = reader["HAWB"].ToString(),
                        InvRefPo = reader["INVREFPO"].ToString(),
                        HpPartNum = reader["HPPARTNUM"].ToString(),
                        IecPartNum = reader["IECPARTNUM"].ToString(),
                        Qty = reader["QTY"] != DBNull.Value ? Convert.ToInt32(reader["QTY"]) : (int?)null,
                        Bulks = reader["BULKS"].ToString(),
                        BoxPlt = reader["BOXPLT"].ToString(),
                        RcvdDate = reader["RCVDDATE"] != DBNull.Value ? Convert.ToDateTime(reader["RCVDDATE"]) : (DateTime?)null,
                        Status = reader["STATUS"].ToString(),
                        Carrier = reader["CARRIER"].ToString(),
                        Shipper = reader["SHIPPER"].ToString(),
                        Bin = reader["Bin"].ToString(),
                        ShipOutStatus = reader["ShipOutStatus"].ToString(),
                        RemainQty = reader["RemainQty"] != DBNull.Value ? Convert.ToInt32(reader["RemainQty"]) : (int?)null,
                        ShipOutDate = reader["ShipOutDate"] != DBNull.Value ? Convert.ToDateTime(reader["ShipOutDate"]) : (DateTime?)null,
                        TruckNum = reader["Truck#"].ToString(),
                        SealNum = reader["SEAL#"].ToString(),
                        ContainerNum = reader["Container#"].ToString(),
                        ImxInvNum = reader["IMX_INV#"].ToString(),
                        Operator = reader["Operator"].ToString(),
                        Cdt = reader["Cdt"] != DBNull.Value ? Convert.ToDateTime(reader["Cdt"]) : (DateTime?)null,
                        Udt = reader["Udt"] != DBNull.Value ? Convert.ToDateTime(reader["Udt"]) : (DateTime?)null,
                        Remark = reader["REMARK"].ToString(),
                        Weight = reader["WEIGHT"].ToString(),
                    };

                    listShipment.Add(shipment);
                }
            }
            catch (Exception ex)
            {
                // Loguea para ver el error real y NO regreses null.
                Console.Error.WriteLine(ex);
                return new List<IepCrossingDockShipment>();
            }

            return listShipment;
        }


        // Insert new shipment record
        public async Task<bool> ExistsAsync(string id, string connectionString)
        {
            const string sql = @"SELECT 1 From IEP_Crossing_Dock_Shipment WHERE [ID] = @Id;";
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 50) { Value = id });
            var result = await cmd.ExecuteScalarAsync();
            return result != null && result != DBNull.Value;
        }


        public async Task<bool> InsertAsync(IepCrossingDockShipmentCreateDto dto, string connectionString)
        {
            // Sane defaults para fechas de auditoría
            var now = DateTime.UtcNow;

            const string sql = @"
                INSERT INTO IEP_Crossing_Dock_Shipment (
                    [ID],
                    [HAWB],
                    [INVREFPO],
                    [IECPARTNUM],
                    [QTY],
                    [BULKS],
                    [BOXPLT],
                    [RCVDDATE],
                    [WEIGHT],
                    [SHIPPER],
                    [Bin],
                    [REMARK],
                    [CARRIER],
                    [Operator],
                    [Cdt],
                    [Udt],
                    [STATUS]
                    -- Opcional: [HPPARTNUM]
                )
                VALUES (
                    @ID,
                    @HAWB,
                    @InvRefPo,
                    @IecPartNum,
                    @Qty,
                    @Bulks,
                    @BoxPlt,
                    @RcvdDate,
                    @Weight,
                    @Shipper,
                    @Bin,
                    @Remark,
                    @Carrier,
                    @Operator,
                    @Cdt,
                    @Udt,
                    @Status
                    -- Opcional: @HpPartNum
            );";

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            // Requerido
            cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50) { Value = dto.ID });

            // Strings y opcionales
            cmd.Parameters.AddWithValue("@HAWB", (object?)dto.HAWB ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InvRefPo", (object?)dto.InvRefPo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IecPartNum", (object?)dto.IecPartNum ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Bulks", (object?)dto.Bulks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BoxPlt", (object?)dto.BoxPlt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Weight", (object?)dto.Weight ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Shipper", (object?)dto.Shipper ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Bin", (object?)dto.Bin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Remark", (object?)dto.Remark ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Carrier", (object?)dto.Carrier ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Operator", (object?)dto.Operator ?? DBNull.Value);

            // Numéricos y fechas
            cmd.Parameters.AddWithValue("@Qty", (object?)dto.Qty ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RcvdDate", (object?)dto.RcvdDate ?? DBNull.Value);

            // Auditoría (server-side)
            cmd.Parameters.AddWithValue("@Cdt", now);
            cmd.Parameters.AddWithValue("@Udt", now);

            // Status inicial
            cmd.Parameters.AddWithValue("@Status", "1");

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }

        // Delete shipment record by ID
        public async Task<bool> DeleteAsync(string id, string connectionString)
        {
            const string sql = @"DELETE FROM IEP_Crossing_Dock_Shipment WHERE [ID] = @Id;";
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 50) { Value = id });
            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }

        // Update shipment status by ID

        public async Task<bool> UpdateStatusWithShipOutAsync(string id, StatusUpdateDto dto, string connectionString)
        {
            // Ajusta tipos según tu tabla:
            // - Si STATUS es INT -> SqlDbType.Int y dto.Status int.
            // - Si STATUS es VARCHAR -> SqlDbType.VarChar y dto.Status string.
            const string sql = @"
                UPDATE dbo.IEP_Crossing_Dock_Shipment
                SET [STATUS]      = @Status,
                    [ShipOutDate] = @ShipOutDate,
                    [Udt]         = @Udt
                WHERE [ID] = @Id;";

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Id", SqlDbType.VarChar, 50).Value = id;

            // Cambia este tipo si STATUS es varchar en BD.
            cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = dto.Status;

            cmd.Parameters.Add("@ShipOutDate", SqlDbType.DateTime).Value =
                (object?)dto.ShipOutDate ?? DBNull.Value;

            // Si quieres que Udt sea la misma fecha elegida, úsala; si no, usa UtcNow.
            var udt = (dto.SetUdtFromShipOutDate && dto.ShipOutDate.HasValue)
                ? dto.ShipOutDate.Value
                : DateTime.UtcNow;

            cmd.Parameters.Add("@Udt", SqlDbType.DateTime).Value = udt;

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }

        public async Task<bool> UpdateAsync(string id, IepCrossingDockShipmentUpdateDto dto, string connectionString)
        {
            const string sql = @"
            UPDATE dbo.IEP_Crossing_Dock_Shipment
            SET [CARRIER]   = @Carrier,
                [HAWB]      = @HAWB,
                [INVREFPO]  = @InvRefPo,
                [IECPARTNUM]= @IecPartNum,
                [QTY]       = @Qty,
                [BULKS]     = @Bulks,
                [BOXPLT]    = @BoxPlt,
                [RCVDDATE]  = @RcvdDate,
                [WEIGHT]    = @Weight,
                [SHIPPER]   = @Shipper,
                [Bin]       = @Bin,
                [REMARK]    = @Remark,
                [Operator]  = @Operator,
                [Udt]       = @Udt
                -- Opcional: [HPPARTNUM] = @HpPartNum
            WHERE RTRIM(LTRIM([ID])) = @IdTrim;";

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@IdTrim", SqlDbType.VarChar, 100).Value = (id ?? string.Empty).Trim();
            cmd.Parameters.Add("@Carrier", SqlDbType.VarChar, 50).Value = (object?)dto.Carrier ?? DBNull.Value;
            cmd.Parameters.Add("@HAWB", SqlDbType.VarChar, 50).Value = (object?)dto.HAWB ?? DBNull.Value;
            cmd.Parameters.Add("@InvRefPo", SqlDbType.VarChar, 50).Value = (object?)dto.InvRefPo ?? DBNull.Value;
            cmd.Parameters.Add("@IecPartNum", SqlDbType.VarChar, 50).Value = (object?)dto.IecPartNum ?? DBNull.Value;
            cmd.Parameters.Add("@Qty", SqlDbType.Int).Value = (object?)dto.Qty ?? DBNull.Value;
            cmd.Parameters.Add("@Bulks", SqlDbType.VarChar, 50).Value = (object?)dto.Bulks ?? DBNull.Value;
            cmd.Parameters.Add("@BoxPlt", SqlDbType.VarChar, 50).Value = (object?)dto.BoxPlt ?? DBNull.Value;
            cmd.Parameters.Add("@RcvdDate", SqlDbType.DateTime).Value = (object?)dto.RcvdDate ?? DBNull.Value;
            cmd.Parameters.Add("@Weight", SqlDbType.VarChar, 50).Value = (object?)dto.Weight ?? DBNull.Value;
            cmd.Parameters.Add("@Shipper", SqlDbType.VarChar, 50).Value = (object?)dto.Shipper ?? DBNull.Value;
            cmd.Parameters.Add("@Bin", SqlDbType.VarChar, 50).Value = (object?)dto.Bin ?? DBNull.Value;
            cmd.Parameters.Add("@Remark", SqlDbType.VarChar, 250).Value = (object?)dto.Remark ?? DBNull.Value;
            cmd.Parameters.Add("@Operator", SqlDbType.VarChar, 50).Value = (object?)dto.Operator ?? DBNull.Value;
            cmd.Parameters.Add("@Udt", SqlDbType.DateTime).Value = DateTime.UtcNow;
            // Opcional:
            // cmd.Parameters.Add("@HpPartNum", SqlDbType.VarChar, 50).Value = (object?)dto.HpPartNum ?? DBNull.Value;

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;

        }
    }
}
