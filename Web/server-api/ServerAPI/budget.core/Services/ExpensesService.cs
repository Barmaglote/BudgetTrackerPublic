using budget.core.Entities;
using budget.core.Models;
using budget.core.Services.Interfaces;

namespace budget.core.Services
{
  public class ExpensesService : EntityService<Item, ItemView> {
    protected override string _collectionName{ get; set; } = "expenses";

    public ExpensesService(
      IEnumerableService<Item, ItemView> enumerableService,
      IPostingService<Item, ItemView> postingService,
      IDeletingService<Item, ItemView> deletingService,
      IFilteringService<Item, ItemView> filteringService
      ) : base(enumerableService, postingService, deletingService, filteringService) {
    }
  }
}
