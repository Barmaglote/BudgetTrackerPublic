using System.Text.Json.Serialization;

namespace budget.core.Models {
  public class TableLazyLoadEvent {
    [JsonPropertyName("first")]
    public int? First { get; set; }
    [JsonPropertyName("rows")]
    public int? Rows { get; set; }
    [JsonPropertyName("sortField")]
    public string? SortField { get; set; }
    [JsonPropertyName("sortOrder")]
    public int? SortOrder { get; set; }
    [JsonPropertyName("filters")]
    public Dictionary<string, FilterMetadata[]>? Filters { get; set; }
  }
}
