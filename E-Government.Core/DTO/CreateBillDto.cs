using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class CreateBillDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int MeterId { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MeterType Type { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CurrentReading { get; set; }

        public decimal PreviousReading { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
