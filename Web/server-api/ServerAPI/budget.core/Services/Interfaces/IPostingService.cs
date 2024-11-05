using budget.core.Entities;
using budget.core.Models.Users;
using MongoDB.Driver;

namespace budget.core.Services.Interfaces {
  public interface IPostingService<T, TView>
      where T : BaseItem
      where TView : class {
    bool UpsertItem(AppUser appUser, TView view, string collectionName, out bool isAdded, IClientSessionHandle? sessionHandle = null);
    bool UpsertItem(string userId, TView view, string collectionName, out bool isAdded, IClientSessionHandle? sessionHandle = null);
  }
}
