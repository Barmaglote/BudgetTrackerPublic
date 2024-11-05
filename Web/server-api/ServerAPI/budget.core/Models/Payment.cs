namespace budget.core.Models {
  public class Payment {
    public int Month { get; set; }
    public decimal Quantity { get; set; }
    public DateTime Date { get; set; }
    public bool isPaid { get; set; }
  }
}
