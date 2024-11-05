using budget.core.Models;

namespace budget.webapi.Models {
  public class CreditsResponse {
    public required IEnumerable<CreditItemView> Items { get; set; }
    public decimal TotalCount { get; set; }
  }
}
