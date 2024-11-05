using budget.core.Models;
using budget.core.Repositories.Interfaces;
using budget.core.Services.Interfaces;
using budget.core.Models.Users;
using budget.core.Entities;
using MongoDB.Driver;

namespace budget.core.Services
{
  public class StatisticsService<T, TView> : IStatisticsService<T, TView>
    where T : BaseItem, ICategorized
    where TView : class {
    private readonly IEnumerableRepository<T> _enumerableRepository;
    private readonly IFilterBuilder<T> _filterBuilder;

    public StatisticsService(IEnumerableRepository<T> enumerableRepository, IFilterBuilder<T> filterBuilder) {
      _enumerableRepository = enumerableRepository;
      _filterBuilder = filterBuilder;
    }

  public IEnumerable<StatsByCategoryView> GetStatsByCategory(AppUser appUser, string collectionName, TableLazyLoadEvent tableLazyLoadEvent) {

      var filter = _filterBuilder.BuildFilerDefinition(tableLazyLoadEvent);
      var (items, totalCount) = _enumerableRepository.GetItems(appUser, collectionName, filter);
      var itemViews = items.ToList()
            .GroupBy(item => new { item.Category, item.AccountId} )
            .Select(group => new StatsByCategoryView() {
              Category = group.Key.Category,
              AccountId = group.Key.AccountId,
              Quantity = group.Sum(item => item.Quantity)
          });

      return itemViews.AsEnumerable();
    }

    public IEnumerable<StatsByDateView> GetStatsByDate(AppUser appUser, string collectionName, TableLazyLoadEvent tableLazyLoadEvent) {
      var filter = _filterBuilder.BuildFilerDefinition(tableLazyLoadEvent);
      var (items, totalCount) = _enumerableRepository.GetItems(appUser, collectionName, filter);
      var itemViews = items.ToList()
            .GroupBy(item => new { item.Category, item.Date, item.AccountId })
            .Select(group => new StatsByDateView() {
              Category = group.Key.Category,
              Date = group.Key.Date,
              AccountId = group.Key.AccountId,
              Quantity = group.Sum(item => item.Quantity)
            });

      if (itemViews.Any()) {
        return itemViews.AsEnumerable();
      } else {
        return new List<StatsByDateView>();
      }
    }
  }
}
