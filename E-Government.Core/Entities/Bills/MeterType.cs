using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Government.Domain.Entities.Bills
{
    public enum MeterType
    {
        [JsonPropertyName("Electricity")]
        Electricity = 1,

        [JsonPropertyName("Water")]
        Water = 2,

        [JsonPropertyName("Gas")]
        Gas = 3
    }

}
