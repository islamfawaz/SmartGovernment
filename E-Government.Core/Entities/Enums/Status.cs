﻿using System.Text.Json.Serialization;

namespace E_Government.Domain.Entities.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Civil,
       Resident
    }

}
