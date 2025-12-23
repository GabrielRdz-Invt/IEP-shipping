namespace shipping_app.Models
{
    public class StatusUpdateDto
    {
        public string? Status { get; set; }
        public DateTime? ShipOutDate { get; set; }
        public bool SetUdtFromShipOutDate { get; set; } = true;
    }
}
