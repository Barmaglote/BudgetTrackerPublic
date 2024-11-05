using budget.core.Models;

namespace budget.webapi.Models {
  public class TransfersResponse {
    public required IEnumerable<TransferItem> Items { get; set; }
    public decimal TotalCount { get; set; }
  }
}
