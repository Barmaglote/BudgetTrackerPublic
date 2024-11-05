using budget.core.Models;

namespace budget.webapi.Models {
  public class ItemsResponse {
    public required IEnumerable<ItemView> Items { get; set; }
    public decimal TotalCount { get; set; }
  }
}
