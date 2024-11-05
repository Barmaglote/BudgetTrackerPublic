using budget.core.Services.Interfaces;

namespace budget.core.Models {
  public class TemplateItem : Identifiable {
    public string? Title { get; set; }
    public string? Category { get; set; }
    public decimal? Quantity { get; set; }
    public string? Comment { get; set; }
    public bool isRegular { get; set; }
    public required string AccountId { get; set; }
  }
}