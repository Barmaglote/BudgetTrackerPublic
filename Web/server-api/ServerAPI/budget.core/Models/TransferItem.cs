namespace budget.core.Models {
  public class TransferItem {
    public string? TransactionId { get; set; }
    public DateTime Date { get; set; }
    public required AccountItem FromAccount { get; set; }
    public required AccountItem ToAccount { get; set; }
    public decimal FromQuantity { get; set; }
    public decimal ToQuantity { get; set; }
  }
}