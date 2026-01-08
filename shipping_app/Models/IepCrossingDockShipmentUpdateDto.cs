namespace shipping_app.Models
{
    public class IepCrossingDockShipmentUpdateDto
    {
        public string? Carrier { get; set; }
        public string? HAWB { get; set; }
        public string? InvRefPo { get; set; }
        public string? HpPartNum { get; set; }
        public string? IecPartNum { get; set; }
        public int? Qty { get; set; }
        public string? Bulks { get; set; }
        public string? BoxPlt { get; set; }
        public DateTime? RcvdDate { get; set; }
        public string? Weight { get; set; }
        public string? Shipper { get; set; }
        public string? Bin { get; set; }
        public string? Remark { get; set; }
        public string? Operator { get; set; }
    }
}
