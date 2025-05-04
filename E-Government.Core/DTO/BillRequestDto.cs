// BillRequestDto.cs
using E_Government.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class BillRequestDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Bill number is required")]
    public string BillNumber { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
    public decimal Amount { get; set; }

    public string Status { get; set; } // Make optional

    public decimal PreviousReading { get; set; }
    public decimal CurrentReading { get; set; }
    public decimal Consumption => CurrentReading - PreviousReading;
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount => Consumption * UnitPrice;

    [Required(ErrorMessage = "Meter type is required")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MeterType Type { get; set; }

    public string PdfUrl { get; set; } // Make optional
    public string PaymentId { get; set; } // Make optional

    // Customer information
    [Required(ErrorMessage = "Customer ID is required")]
    public string UserNID { get; set; }

    public string UserName { get; set; } // Make optional
    public string CustomerAddress { get; set; } // Make optional

    // Meter information
    [Required(ErrorMessage = "Meter ID is required")]
    public int MeterId { get; set; }

    public string MeterNumber { get; set; } // Make optional
}