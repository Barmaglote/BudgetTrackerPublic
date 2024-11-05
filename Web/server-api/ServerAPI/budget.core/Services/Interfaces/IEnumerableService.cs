using budget.core.Models;
using budget.core.Models.Users;
using MongoDB.Bson;

namespace budget.core.Services.Interfaces {
  public interface IEnumerableService<T, TView>
      where T : class
      where TView : class
  {
    (IEnumerable<TView> ItemViews, long TotalCount) GetItemViews(AppUser appUser, string collectionName, TableLazyLoadEvent tableLazyLoadEvent);
    TView? GetItemView(ObjectId objectId, string collectionName);
  }
}
