using budget.core.Models;

namespace budget.webapi.Models
{
    public class BriefStatistics
    {
      public ItemView? LastIncome { get; set; }
      public ItemView? LastExpense { get; set; }
    }
}