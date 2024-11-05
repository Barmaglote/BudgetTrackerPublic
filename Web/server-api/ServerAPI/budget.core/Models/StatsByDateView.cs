namespace budget.core.Models {
  public class StatsByDateView {
    public string? Category { get; set; }
    public decimal Quantity { get; set; }
    public DateTime Date { get; set; }
    public string? AccountId { get; set; }
  }
}