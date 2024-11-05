using budget.core.Models.Users;
using MongoDB.Driver;

namespace budget.core.Repositories.Interfaces
{
  public interface IPostingRepository<T> where T : class
  {
    bool AddItem(AppUser appUser, T item, string collectionName, IClientSessionHandle? _session = null);
    bool UpdateItem(AppUser appUser, T item, string collectionName, IClientSessionHandle? _session = null);
    bool DeleteItem(AppUser appUser, T item, string collectionName, IClientSessionHandle? session = null);
    bool DeleteItemByAccountId(string accountId, string collectionName, IClientSessionHandle? session = null);
    bool AddItem(string userId, T item, string collectionName, IClientSessionHandle? _session = null);
    bool UpdateItem(string userId, T item, string collectionName, IClientSessionHandle? _session = null);

  }
}
