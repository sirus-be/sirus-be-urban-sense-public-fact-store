using System.Text.Json.Serialization;

namespace FactStore.Models.Parameters
{
    public abstract class Parameters
    {
        [JsonPropertyName("search")]
        public string Search { get; set; }

        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; } = 0;

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;
    }
}
