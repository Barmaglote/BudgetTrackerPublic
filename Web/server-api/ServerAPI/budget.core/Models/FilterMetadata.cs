using System.Text.Json.Serialization;

namespace budget.core.Models {
  public class FilterMetadata {
    [JsonPropertyName("value")]
    public object? Value { get; set; }
    [JsonPropertyName("matchMode")]
    public string? MatchMode { get; set; }

    [JsonPropertyName("operator")]
    public string? OperatorMode { get; set; }
  }
}