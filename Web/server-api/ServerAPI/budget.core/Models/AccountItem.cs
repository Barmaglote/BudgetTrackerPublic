using budget.core.Services.Interfaces;

namespace budget.core.Models {
  public class AccountItem : Identifiable {
    public required string Title { get; set; }
    public decimal? Initial { get; set; }
    public decimal? Limit { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Goal { get; set; }
    public string? Comment { get; set; }    
    public required string Currency { get; set; }
    public required AccountType AccountType { get; set; }
  }
}