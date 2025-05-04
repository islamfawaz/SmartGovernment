using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CustomerCategory
    {
        Residential,
        Commercial
    }

}
