using E_Government.Domain.Entities.Bills;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_Government.Application.DTO.Bills
{
    public class GenerateBillRequestDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public string NID { get; set; }

        [Required(ErrorMessage = "Meter type is required")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MeterType Type { get; set; }

        [Required(ErrorMessage = "Current reading is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Reading must be positive")]
        public decimal CurrentReading { get; set; }
    }
}