using budget.core.Entities;
using budget.core.Models;
using budget.core.Services.Interfaces;

namespace budget.core.Services
{
  public class PlanningService : EntityService<PlannedItem, PlannedItemView> {
    protected override string _collectionName{ get; set; } = "planning";

    public PlanningService(
      IEnumerableService<PlannedItem, PlannedItemView> enumerableService,
      IPostingService<PlannedItem, PlannedItemView> postingService,
      IDeletingService<PlannedItem, PlannedItemView> deletingService,
      IFilteringService<PlannedItem, PlannedItemView> filteringService
      ) : base(enumerableService, postingService, deletingService, filteringService) {
    }
  }
}
