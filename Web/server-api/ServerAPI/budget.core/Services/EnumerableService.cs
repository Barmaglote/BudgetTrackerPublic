using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Repositories.Interfaces;
using budget.core.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace budget.core.Services {
  public class EnumerableService<T, TView> : IEnumerableService<T, TView>
    where T : BaseItem, ITransformable<TView>
    where TView : class, ITransformable<T>
  {
    private IEnumerableRepository<T> EnumerableRepository { get; set; }
    private IFilterBuilder<T> FilterBuilder { get; set; }
    private IFactory<T, TView> Factory { get; set; }
    public EnumerableService(IEnumerableRepository<T> enumerableRepository,
                             IFactory<T, TView> factory,
                             IFilterBuilder<T> filterBuilder) {
      EnumerableRepository = enumerableRepository;
      Factory = factory;
      FilterBuilder = filterBuilder;
    }
    public (IEnumerable<TView> ItemViews, long TotalCount) GetItemViews(AppUser appUser, string collectionName, TableLazyLoadEvent tableLazyLoadEvent) {

      var filter = FilterBuilder.BuildFilerDefinition(tableLazyLoadEvent);
      var (items, totalCount) = EnumerableRepository.GetItems(appUser, collectionName, filter);

      var itemViews = items.ToList() // Switch to ToEnumerable(), in case CPU and Memory is overloaded
              .OrderByDescending(x => x.Date)
              .Skip(tableLazyLoadEvent.First ?? 0)
              .Take(tableLazyLoadEvent.Rows ?? 0)
              .ToList()
              .Select(Factory.CreateView);

      return (itemViews, totalCount);
    }

    public TView? GetItemView(ObjectId objectId, string collectionName) {
      var item = EnumerableRepository.GetItem(objectId, collectionName);
      if (item == null) { return null; }

      var itemView = Factory.CreateView(item);
      return itemView;
    }
  }
}
