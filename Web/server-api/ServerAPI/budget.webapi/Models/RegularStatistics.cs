using budget.core.Models;

namespace budget.webapi.Models
{
  public class RegularStatistics {
    public IEnumerable<ItemView>? Incomes { get; set; } = new List<ItemView>();
    public IEnumerable<ItemView>? Expenses { get; set; } = new List<ItemView>();
  }
}