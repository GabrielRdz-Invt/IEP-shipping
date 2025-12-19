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
            List<IepCrossingDockShipment> ListShippment = new List<IepCrossingDockShipment>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = @"Select * from IEP_Crossing_Dock_Shipment order by ID desc;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            IepCrossingDockShipment shippment = new IepCrossingDockShipment
                            {
                                ID = reader["ID"].ToString(),
                                HAWB = reader["HAWB"].ToString(),
                                InvRefPo = reader["INVREFPO"].ToString(),
                                HpPartNum = reader["HPPARTNUM"].ToString(),
                                IecPartNum = reader["IECPARTNUM"].ToString(),
                                Qty = reader["QTY"] != DBNull.Value ? Convert.ToInt32(reader["Qty"]) : (int?)null,
                                Bulks = reader["BULKS"].ToString(),
                                BoxPlt = reader["BOXPLT"].ToString(),
                                RcvdDate = reader["RCVDDATE"] != DBNull.Value ? Convert.ToDateTime(reader["RcvdDate"]) : (DateTime?)null,
                                Status = reader["STATUS"].ToString(),
                                Carrier = reader["CARRIER"].ToString(),
                                Shipper = reader["SHIPPER"].ToString(),
                                Bin = reader["Bin"].ToString(),
                                ShipOutStatus = reader["ShipOutStatus"].ToString(),
                                RemainQty = reader["RemainQty"] != DBNull.Value ? Convert.ToInt32(reader["RemainQty"]) : (int?)null,
                                ShipOutDate = reader["ShipOutDate"] != DBNull.Value ? Convert.ToDateTime(reader["ShipOutDate"]) : (DateTime?)null,
                                // << Campo# -> CampoNum >>
                                TruckNum = reader["Truck#"].ToString(),
                                SealNum = reader["SEAL#"].ToString(),
                                ContainerNum = reader["Container#"].ToString(),
                                ImxInvNum = reader["IMX_INV#"].ToString(),
                                // << Campo# -> CampoNum >>
                                Operator = reader["Operator"].ToString(),
                                Cdt = reader["Cdt"] != DBNull.Value ? Convert.ToDateTime(reader["Cdt"]) : (DateTime?)null,
                                Udt = reader["Udt"] != DBNull.Value ? Convert.ToDateTime(reader["Udt"]) : (DateTime?)null,
                                Remark = reader["REMARK"].ToString(),
                                Weight = reader["WEIGHT"].ToString(),
                            };
                            ListShippment.Add(shippment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            // var results = new List<IepCrossingDockShipment>(); -- comentado porque no se usa
            return ListShippment;
        }


        public List<IepCrossingDockShipment> GetListShippmentById(string id, string _connectionString)
        {
            var listShipment = new List<IepCrossingDockShipment>();

            if (string.IsNullOrWhiteSpace(id))
                return listShipment;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    const string query = @"Select * IEP_Crossing_Dock_Shipment WHERE[ID] = @Id order by ID desc ";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 50) { Value = id });

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var shipment = new IepCrossingDockShipment
                                {
                                    ID = reader["ID"].ToString(),
                                    HAWB = reader["HAWB"].ToString(),
                                    InvRefPo = reader["InvRefPo"].ToString(),
                                    HpPartNum = reader["HpPartNum"].ToString(),
                                    IecPartNum = reader["IecPartNum"].ToString(),
                                    Qty = reader["Qty"] != DBNull.Value ? Convert.ToInt32(reader["Qty"]) : (int?)null,
                                    Bulks = reader["Bulks"].ToString(),
                                    BoxPlt = reader["BoxPlt"].ToString(),
                                    RcvdDate = reader["RcvdDate"] != DBNull.Value ? Convert.ToDateTime(reader["RcvdDate"]) : (DateTime?)null,
                                    Status = reader["Status"].ToString(),
                                    Carrier = reader["Carrier"].ToString(),
                                    Shipper = reader["Shipper"].ToString(),
                                    Bin = reader["Bin"].ToString(),
                                    ShipOutStatus = reader["ShipOutStatus"].ToString(),
                                    RemainQty = reader["RemainQty"] != DBNull.Value ? Convert.ToInt32(reader["RemainQty"]) : (int?)null,
                                    ShipOutDate = reader["ShipOutDate"] != DBNull.Value ? Convert.ToDateTime(reader["ShipOutDate"]) : (DateTime?)null,
                                    // << Campo# -> CampoNum >>
                                    TruckNum = reader["TruckNum"].ToString(),
                                    SealNum = reader["SealNum"].ToString(),
                                    ContainerNum = reader["ContainerNum"].ToString(),
                                    ImxInvNum = reader["ImxInvNum"].ToString(),
                                    // << Campo# -> CampoNum >>
                                    Operator = reader["Operator"].ToString(),
                                    Cdt = reader["Cdt"] != DBNull.Value ? Convert.ToDateTime(reader["Cdt"]) : (DateTime?)null,
                                    Udt = reader["Udt"] != DBNull.Value ? Convert.ToDateTime(reader["Udt"]) : (DateTime?)null,
                                    Remark = reader["Remark"].ToString(),
                                    Weight = reader["Weight"].ToString(),
                                };

                                listShipment.Add(shipment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return listShipment;
        }

    }
}
