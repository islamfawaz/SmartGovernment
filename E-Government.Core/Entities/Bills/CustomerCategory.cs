using System.Text.Json.Serialization;

namespace E_Government.Domain.Entities.Bills
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CustomerCategory
    {
        Residential,
        Commercial
    }

}
