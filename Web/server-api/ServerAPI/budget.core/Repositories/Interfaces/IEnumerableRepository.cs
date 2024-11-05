using budget.core.Entities;
using budget.core.Models.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace budget.core.Repositories.Interfaces
{
  public interface IEnumerableRepository<T> where T : BaseItem
  {
    (IAsyncCursor<T> Items, long TotalCount) GetItems(AppUser appUser, string collectionName, FilterDefinition<T>? filterDefinition);
    T? GetItem(ObjectId objectId, string collectionName);
  }
}
