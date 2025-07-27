using E_Government.Domain.Entities.Bills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.Bills
{
    public class RegisterMeterDto
    {
        [Required]
        public string NID { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MeterType Type { get; set; }
    }
}
