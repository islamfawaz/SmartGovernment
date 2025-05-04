using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities
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
