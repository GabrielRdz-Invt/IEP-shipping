using System.ComponentModel.DataAnnotations;

namespace shipping_app.Models
{
    public class SecondScanValidateRequestDto
    {
        [Required]
        public string TrackingNumber { get; set; } = string.Empty;

        [Required]
        public string HpPartNum { get; set; } = string.Empty;

    }
}
