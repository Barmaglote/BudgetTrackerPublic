using budget.core.Models.Users;
using MongoDB.Driver;

namespace budget.core.Services.Interfaces {
  public interface IDeletingService<T, TView>
      where T : class
      where TView : class {
    bool DeleteItem(AppUser appUser, TView itemView, string collectionName, IClientSessionHandle? session = null);
    bool DeleteItemsByAccountId(string accountId, string collectionName, IClientSessionHandle? session = null);
  }
}
