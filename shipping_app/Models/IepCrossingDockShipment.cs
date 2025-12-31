// using Microsoft.Data.SqlClient;
using Npgsql;
using NpgsqlTypes;
using shipping_app.Repositories;
using System;
using System.Data;
using System.Globalization;

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
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                const string sql = @"
                SELECT *
                FROM public.iep_crossing_dock_shipment
                ORDER BY id DESC;";

                using var cmd = new NpgsqlCommand(sql, connection);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var s = new IepCrossingDockShipment
                    {
                        ID = reader["id"]?.ToString(),
                        HAWB = reader["hawb"]?.ToString(),
                        InvRefPo = reader["invrefpo"]?.ToString(),
                        HpPartNum = reader["hppartnum"]?.ToString(),
                        IecPartNum = reader["iecpartnum"]?.ToString(),
                        Qty = reader["qty"] != DBNull.Value ? Convert.ToInt32(reader["qty"]) : (int?)null,
                        Bulks = reader["bulks"]?.ToString(),
                        BoxPlt = reader["boxplt"]?.ToString(),
                        RcvdDate = reader["rcvddate"] != DBNull.Value ? Convert.ToDateTime(reader["rcvddate"]) : (DateTime?)null,
                        Status = reader["status"]?.ToString(),
                        Carrier = reader["carrier"]?.ToString(),
                        Shipper = reader["shipper"]?.ToString(),
                        Bin = reader["bin"]?.ToString(),
                        ShipOutStatus = reader["shipoutstatus"]?.ToString(),
                        RemainQty = reader["remainqty"] != DBNull.Value ? Convert.ToInt32(reader["remainqty"]) : (int?)null,
                        ShipOutDate = reader["shipoutdate"] != DBNull.Value ? Convert.ToDateTime(reader["shipoutdate"]) : (DateTime?)null,
                        TruckNum = reader["truck_num"]?.ToString(),
                        SealNum = reader["seal_num"]?.ToString(),
                        ContainerNum = reader["container_num"]?.ToString(),
                        ImxInvNum = reader["imx_inv_num"]?.ToString(),
                        Operator = reader["operator_name"]?.ToString(),
                        Cdt = reader["cdt"] != DBNull.Value ? Convert.ToDateTime(reader["cdt"]) : (DateTime?)null,
                        Udt = reader["udt"] != DBNull.Value ? Convert.ToDateTime(reader["udt"]) : (DateTime?)null,
                        Remark = reader["remark"]?.ToString(),
                        Weight = reader["weight"]?.ToString(),
                    };
                    list.Add(s);
                }
            }
            catch (PostgresException pgEx)
            {
                Console.Error.WriteLine(pgEx);
                return new List<IepCrossingDockShipment>();
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
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                const string query = @"
                    SELECT *
                    FROM public.iep_crossing_dock_shipment
                    WHERE id = @Id;";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.Add(new NpgsqlParameter("@Id", NpgsqlDbType.Varchar, 50) { Value = id });

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var shipment = new IepCrossingDockShipment
                    {
                        ID = reader["id"]?.ToString(),
                        HAWB = reader["hawb"]?.ToString(),
                        InvRefPo = reader["invrefpo"]?.ToString(),
                        HpPartNum = reader["hppartnum"]?.ToString(),
                        IecPartNum = reader["iecpartnum"]?.ToString(),
                        Qty = reader["qty"] != DBNull.Value ? Convert.ToInt32(reader["qty"]) : (int?)null,
                        Bulks = reader["bulks"]?.ToString(),
                        BoxPlt = reader["boxplt"]?.ToString(),
                        RcvdDate = reader["rcvddate"] != DBNull.Value ? Convert.ToDateTime(reader["rcvddate"]) : (DateTime?)null,
                        Status = reader["status"]?.ToString(),
                        Carrier = reader["carrier"]?.ToString(),
                        Shipper = reader["shipper"]?.ToString(),
                        Bin = reader["bin"]?.ToString(),
                        ShipOutStatus = reader["shipoutstatus"]?.ToString(),
                        RemainQty = reader["remainqty"] != DBNull.Value ? Convert.ToInt32(reader["remainqty"]) : (int?)null,
                        ShipOutDate = reader["shipoutdate"] != DBNull.Value ? Convert.ToDateTime(reader["shipoutdate"]) : (DateTime?)null,
                        TruckNum = reader["truck_num"]?.ToString(),
                        SealNum = reader["seal_num"]?.ToString(),
                        ContainerNum = reader["container_num"]?.ToString(),
                        ImxInvNum = reader["imx_inv_num"]?.ToString(),
                        Operator = reader["operator_name"]?.ToString(),
                        Cdt = reader["cdt"] != DBNull.Value ? Convert.ToDateTime(reader["cdt"]) : (DateTime?)null,
                        Udt = reader["udt"] != DBNull.Value ? Convert.ToDateTime(reader["udt"]) : (DateTime?)null,
                        Remark = reader["remark"]?.ToString(),
                        Weight = reader["weight"]?.ToString(),
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
            const string sql = @"SELECT 1 From public.iep_crossing_dock_shipment WHERE id = @Id;";
            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@Id", NpgsqlDbType.Varchar, 50) { Value = id });
            var result = await cmd.ExecuteScalarAsync();
            return result != null && result != DBNull.Value;
        }



        public async Task<bool> InsertAsync(IepCrossingDockShipmentCreateDto dto, string connectionString)
        {
            var nowLocal = DateTime.Now;
            var nowUnspec = DateTime.SpecifyKind(nowLocal, DateTimeKind.Unspecified);


            const string sql = @"
            INSERT INTO public.iep_crossing_dock_shipment (
                id, hawb, invrefpo, iecpartnum, qty, bulks, boxplt, rcvddate, weight,
                shipper, bin, remark, carrier, operator_name, cdt, udt, status
            ) VALUES (
                @id, @hawb, @invrefpo, @iecpartnum, @qty, @bulks, @boxplt, @rcvddate, @weight,
                @shipper, @bin, @remark, @carrier, @operator_name, @cdt, @udt, @status
            );";

            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);

            // Requerido
            cmd.Parameters.Add(new NpgsqlParameter("@id", NpgsqlDbType.Varchar) { Value = dto.ID });

            // Strings y opcionales
            cmd.Parameters.Add(new NpgsqlParameter("@hawb", NpgsqlDbType.Varchar) { Value = (object?)dto.HAWB ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@invrefpo", NpgsqlDbType.Varchar) { Value = (object?)dto.InvRefPo ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@iecpartnum", NpgsqlDbType.Varchar) { Value = (object?)dto.IecPartNum ?? DBNull.Value });

            // FALTABAN (ya están)
            cmd.Parameters.Add(new NpgsqlParameter("@bulks", NpgsqlDbType.Varchar) { Value = (object?)dto.Bulks ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@boxplt", NpgsqlDbType.Varchar) { Value = (object?)dto.BoxPlt ?? DBNull.Value });

            // Numéricos y fechas
            cmd.Parameters.Add(new NpgsqlParameter("@qty", NpgsqlDbType.Integer) { Value = (object?)dto.Qty ?? DBNull.Value });
            if (dto.RcvdDate.HasValue)
            {
                var rcvdUnspec = DateTime.SpecifyKind(dto.RcvdDate.Value, DateTimeKind.Unspecified);
                cmd.Parameters.Add(new NpgsqlParameter("@rcvddate", NpgsqlDbType.Timestamp) { Value = rcvdUnspec });
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("@rcvddate", NpgsqlDbType.Timestamp) { Value = DBNull.Value });
            }


            // Más strings
            cmd.Parameters.Add(new NpgsqlParameter("@weight", NpgsqlDbType.Varchar) { Value = (object?)dto.Weight ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@shipper", NpgsqlDbType.Varchar) { Value = (object?)dto.Shipper ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@bin", NpgsqlDbType.Varchar) { Value = (object?)dto.Bin ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@remark", NpgsqlDbType.Varchar) { Value = (object?)dto.Remark ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@carrier", NpgsqlDbType.Varchar) { Value = (object?)dto.Carrier ?? DBNull.Value });

            // OJO: "operator_name" en SQL; el parámetro aquí es @operator
            cmd.Parameters.Add(new NpgsqlParameter("@operator_name", NpgsqlDbType.Varchar) { Value = (object?)dto.Operator ?? DBNull.Value });

            // Auditoría
            cmd.Parameters.Add(new NpgsqlParameter("@cdt", NpgsqlDbType.Timestamp) { Value = nowUnspec });
            cmd.Parameters.Add(new NpgsqlParameter("@udt", NpgsqlDbType.Timestamp) { Value = nowUnspec });


            cmd.Parameters.Add(new NpgsqlParameter("@status", NpgsqlDbType.Varchar) { Value = "1" });

            try
            {
                var affected = await cmd.ExecuteNonQueryAsync();
                return affected > 0;
            }
            catch (PostgresException pgEx)
            {
                // Log: si es 23505 => duplicado; 42703 => columna inexistente; 23502 => not null violation
                Console.Error.WriteLine($"PG SqlState={pgEx.SqlState} Message={pgEx.MessageText}");
                Console.Error.WriteLine($"Detail={pgEx.Detail} Where={pgEx.Where}");
                throw; // Deja que el controller atrape y devuelva ProblemDetails con pgEx.SqlState
            }
        }


        // Delete shipment record by ID
        public async Task<bool> DeleteAsync(string id, string connectionString)
        {
            const string sql = @"DELETE FROM public.iep_crossing_dock_shipment WHERE id = @Id;";
            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@Id", NpgsqlDbType.Varchar, 50) { Value = id });
            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }

        // Update shipment status by ID

        public async Task<bool> UpdateStatusWithShipOutAsync(string id, StatusUpdateDto dto, string connectionString)
        {
            const string sql = @"
                UPDATE public.iep_crossing_dock_shipment
                SET status      = @Status,
                    shipoutdate = @ShipOutDate,
                    udt         = @Udt
                WHERE id = @Id;";

            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.Add("@Id", NpgsqlDbType.Varchar, 50).Value = id;
            cmd.Parameters.Add("@Status", NpgsqlDbType.Varchar, 50).Value = dto.Status;

            if (dto.ShipOutDate.HasValue)
            {
                var shipOutUnspec = DateTime.SpecifyKind(dto.ShipOutDate.Value, DateTimeKind.Unspecified);
                cmd.Parameters.Add("@ShipOutDate", NpgsqlDbType.Timestamp).Value = shipOutUnspec;
            }
            else
            {
                cmd.Parameters.Add("@ShipOutDate", NpgsqlDbType.Timestamp).Value = DBNull.Value;
            }


            // Si quieres que Udt sea la misma fecha elegida, úsala; si no, usa UtcNow.
            var udtSource = (dto.SetUdtFromShipOutDate && dto.ShipOutDate.HasValue)
                    ? dto.ShipOutDate.Value
                    : DateTime.Now;
            var udtUnspec = DateTime.SpecifyKind(udtSource, DateTimeKind.Unspecified);
            cmd.Parameters.Add("@Udt", NpgsqlDbType.Timestamp).Value = udtUnspec;

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }

        public async Task<bool> UpdateAsync(string id, IepCrossingDockShipmentUpdateDto dto, string connectionString)
        {
            const string sql = @"
            UPDATE public.iep_crossing_dock_shipment
            SET carrier = @Carrier,
                hawb = @HAWB,
                invrefpo = @InvRefPo,
                iecpartnum = @IecPartNum,
                qty = @Qty,
                bulks = @Bulks,
                boxplt = @BoxPlt,
                rcvddate = @RcvdDate,
                weight = @Weight,
                shipper = @Shipper,
                bin = @Bin,
                remark = @Remark,
                operator_name = @Operator,
                udt = @Udt
            WHERE id = @IdTrim;";


            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.Add("@IdTrim", NpgsqlDbType.Varchar, 100).Value = (id ?? string.Empty).Trim();
            cmd.Parameters.Add("@Carrier", NpgsqlDbType.Varchar, 50).Value = (object?)dto.Carrier ?? DBNull.Value;
            cmd.Parameters.Add("@HAWB", NpgsqlDbType.Varchar, 50).Value = (object?)dto.HAWB ?? DBNull.Value;
            cmd.Parameters.Add("@InvRefPo", NpgsqlDbType.Varchar, 50).Value = (object?)dto.InvRefPo ?? DBNull.Value;
            cmd.Parameters.Add("@IecPartNum", NpgsqlDbType.Varchar, 50).Value = (object?)dto.IecPartNum ?? DBNull.Value;
            cmd.Parameters.Add("@Qty", NpgsqlDbType.Integer).Value = (object?)dto.Qty ?? DBNull.Value;
            cmd.Parameters.Add("@Bulks", NpgsqlDbType.Varchar, 50).Value = (object?)dto.Bulks ?? DBNull.Value;
            cmd.Parameters.Add("@BoxPlt", NpgsqlDbType.Varchar, 50).Value = (object?)dto.BoxPlt ?? DBNull.Value;

            cmd.Parameters.Add("@RcvdDate", NpgsqlDbType.Timestamp).Value =
                dto.RcvdDate.HasValue
                ? DateTime.SpecifyKind(dto.RcvdDate.Value, DateTimeKind.Unspecified)
                : (object)DBNull.Value;

            cmd.Parameters.Add("@Weight", NpgsqlDbType.Varchar, 50).Value = (object?)dto.Weight ?? DBNull.Value;
            cmd.Parameters.Add("@Shipper", NpgsqlDbType.Varchar, 50).Value = (object?)dto.Shipper ?? DBNull.Value;
            cmd.Parameters.Add("@Bin", NpgsqlDbType.Varchar, 50).Value = (object?)dto.Bin ?? DBNull.Value;
            cmd.Parameters.Add("@Remark", NpgsqlDbType.Varchar, 250).Value = (object?)dto.Remark ?? DBNull.Value;
            cmd.Parameters.Add("@Operator", NpgsqlDbType.Varchar, 50).Value = (object?)dto.Operator ?? DBNull.Value;

            var udtUnspec = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            cmd.Parameters.Add("@Udt", NpgsqlDbType.Timestamp).Value = udtUnspec;

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }


        public async Task<string> GenerateNextIdAsync(DateTime nowLocal, string connectionString)
        {

            string prefix = $"{nowLocal:yyyy}-{nowLocal:MM}-{nowLocal:dd}-";

            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT id
                FROM public.iep_crossing_dock_shipment
                WHERE id LIKE (@prefix || '%');";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@prefix", NpgsqlDbType.Varchar, 50) { Value = prefix });

            int maxConsecutive = 0;

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var id = reader.GetString(0);
                    // Extraer sufijo después del último '-'
                    int dash = id.LastIndexOf('-');
                    if (dash >= 0 && dash < id.Length - 1)
                    {
                        var suffix = id.Substring(dash + 1);
                        if (int.TryParse(suffix, NumberStyles.None, CultureInfo.InvariantCulture, out var num) && num >= 0)
                        {
                            if (num > maxConsecutive) maxConsecutive = num;
                        }
                    }
                }
            }
            int next = (maxConsecutive >= 0 ? maxConsecutive + 1 : 1);
            return prefix + next.ToString("D4", CultureInfo.InvariantCulture);
        }



        public async Task<List<object>> GetReportRowsAsync(string dateField, DateTime from, DateTime to, string connectionString)
        {
            var list = new List<object>();

            string dateExpr = (dateField ?? "rcvd").Trim().ToLowerInvariant() switch
            {
                "shipout" => "shipoutdate",
                "shipoutdate" => "shipoutdate",
                "rcvd" => "rcvddate"   // por defecto
            };

            var fromUnspec = DateTime.SpecifyKind(from, DateTimeKind.Unspecified);
            var toUnspec = DateTime.SpecifyKind(to, DateTimeKind.Unspecified);

            string sql = $@"
            SELECT
                id,
                status,
                hawb,
                invrefpo,
                iecpartnum,
                qty,
                bulks,
                carrier,
                bin,
                rcvddate,
                shipoutdate,
                operator_name,
                cdt
            FROM public.iep_crossing_dock_shipment
            WHERE {dateExpr} IS NOT NULL
                AND {dateExpr} >= @from
                AND {dateExpr} <= @to
            ORDER BY {dateExpr} ASC, id ASC;";

            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.Add("@from", NpgsqlDbType.Timestamp).Value = fromUnspec;
            cmd.Parameters.Add("@to", NpgsqlDbType.Timestamp).Value = toUnspec;

            using var reader = await cmd.ExecuteReaderAsync();

            int idxId = reader.GetOrdinal("id");
            int idxStatus = reader.GetOrdinal("status");
            int idxHawb = reader.GetOrdinal("hawb");
            int idxInvRefPo = reader.GetOrdinal("invrefpo");
            int idxIecPartNum = reader.GetOrdinal("iecpartnum");
            int idxQty = reader.GetOrdinal("qty");
            int idxBulks = reader.GetOrdinal("bulks");
            int idxCarrier = reader.GetOrdinal("carrier");
            int idxBin = reader.GetOrdinal("bin");
            int idxRcvdDate = reader.GetOrdinal("rcvddate");
            int idxShipOut = reader.GetOrdinal("shipoutdate");

            int idxOperator = reader.GetOrdinal("operator_name");

            int idxCdt = reader.GetOrdinal("cdt");

            while (await reader.ReadAsync())
            {
                list.Add(new
                {
                    id = reader.IsDBNull(idxId) ? null : reader.GetString(idxId),
                    status = reader.IsDBNull(idxStatus) ? null : reader.GetString(idxStatus),
                    hawb = reader.IsDBNull(idxHawb) ? null : reader.GetString(idxHawb),
                    invRefPo = reader.IsDBNull(idxInvRefPo) ? null : reader.GetString(idxInvRefPo),
                    iecPartNum = reader.IsDBNull(idxIecPartNum) ? null : reader.GetString(idxIecPartNum),
                    qty = reader.IsDBNull(idxQty) ? (int?)null : reader.GetInt32(idxQty),
                    bulks = reader.IsDBNull(idxBulks) ? null : reader.GetString(idxBulks),
                    carrier = reader.IsDBNull(idxCarrier) ? null : reader.GetString(idxCarrier),
                    bin = reader.IsDBNull(idxBin) ? null : reader.GetString(idxBin),

                    // [CAMBIO] Serialización ISO "s"
                    rcvdDate = reader.IsDBNull(idxRcvdDate) ? null : reader.GetDateTime(idxRcvdDate).ToString("s"),
                    shipOutDate = reader.IsDBNull(idxShipOut) ? null : reader.GetDateTime(idxShipOut).ToString("s"),

                    // [CAMBIO] operador correcto
                    operatorName = reader.IsDBNull(idxOperator) ? null : reader.GetString(idxOperator),

                    cdt = reader.IsDBNull(idxCdt) ? null : reader.GetDateTime(idxCdt).ToString("s"),
                });
            }

            return list;


        }

        private static string ResolveDateExpr(string dateField)
        {
            // Admite "shipoutdate" o "shipout"
            if (dateField.Equals("shipoutdate", StringComparison.OrdinalIgnoreCase) ||
                dateField.Equals("shipout", StringComparison.OrdinalIgnoreCase))
            {
                return "shipoutdate";
            }

            // Por defecto: COALESCE(rcvddate, cdt)
            // (Ambas columnas en minúsculas, sin comillas)
            return "coalesce(rcvddate, cdt)";
        }



    }
}
