using MongoDB.Driver;
using budget.core.Factories.Interfaces;
using budget.core.Repositories.Interfaces;
using budget.core.Services.Interfaces;
using budget.core.Entities;
using budget.core.Models.Users;

namespace budget.core.Services {
  public class FilteringService<T, TView> : IFilteringService<T, TView>
    where T : BaseItem, ITransformable<TView>
    where TView : class, ITransformable<T> 
  {

    private readonly IFilteringRepository<T> _filteringRepository;
    private readonly IFactory<T, TView> _factory;

    public FilteringService(IFilteringRepository<T> filteringRepository, IFactory<T, TView> factory) {
      _filteringRepository = filteringRepository;
      _factory = factory;
    }

    public IEnumerable<TView> GetItemByMonth(AppUser appUser, string collectionName, int year, int month) {
      return _filteringRepository.GetItemByMonth(appUser, collectionName, year, month).Select(_factory.CreateView);
    }

    public IEnumerable<TView> GetItemViews(AppUser appUser, FilterDefinition<T> filterDefinition, string collectionName) {
      return _filteringRepository.ExecuteFilter(appUser, filterDefinition, collectionName).Select(_factory.CreateView);
    }

    public IEnumerable<TView> GetItemViews(FilterDefinition<T> filterDefinition, string collectionName) {
      return _filteringRepository.ExecuteFilter(filterDefinition, collectionName).Select(_factory.CreateView);
    }
  }
}
