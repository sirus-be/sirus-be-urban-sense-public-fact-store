using System.Text.Json.Serialization;

namespace FactStore.Models.Parameters
{
    public class FactTypeParameters: Parameters
    {
        [JsonPropertyName("sorting")]
        public string Sorting { get; set; } = "Name asc";
    }
}
