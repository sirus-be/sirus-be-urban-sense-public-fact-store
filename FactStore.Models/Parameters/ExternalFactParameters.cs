using System.Text.Json.Serialization;

namespace FactStore.Models.Parameters
{
    public class ExternalFactParameters: Parameters
    {
        [JsonPropertyName("sorting")]
        public string Sorting { get; set; } = "Key asc";
    }
}
