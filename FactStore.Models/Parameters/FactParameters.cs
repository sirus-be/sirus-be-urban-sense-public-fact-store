using System.Text.Json.Serialization;

namespace FactStore.Models.Parameters
{
    public class FactParameters: Parameters
    {
        [JsonPropertyName("sorting")]
        public string Sorting { get; set; } = "Key asc";
    }
}
