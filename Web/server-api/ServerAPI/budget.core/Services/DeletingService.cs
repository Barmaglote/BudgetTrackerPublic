using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models.Users;
using budget.core.Repositories.Interfaces;
using budget.core.Services.Interfaces;
using MongoDB.Driver;

namespace budget.core.Services {
  public class DeletingService<T, TView> : IDeletingService<T, TView>
    where T : BaseItem, ITransformable<TView>
    where TView : class, ITransformable<T> 
{
    private readonly IPostingRepository<T> _postingRepository;
    private readonly IEnumerableRepository<T> _enumerableRepository;
    private readonly IFactory<T, TView> _factory;

    public DeletingService(IPostingRepository<T> postingRepository, IEnumerableRepository<T> enumerableRepository, IFactory<T, TView> factory) {
      _postingRepository = postingRepository;
      _enumerableRepository = enumerableRepository;
      _factory = factory;
    }

    public bool DeleteItem(AppUser appUser, TView itemView, string collectionName, IClientSessionHandle? session = null) {
      ArgumentNullException.ThrowIfNull(appUser);
      ArgumentNullException.ThrowIfNull(itemView);
      ArgumentException.ThrowIfNullOrEmpty(collectionName);
      var _item = _factory.CreateItem(itemView);

      var item = _enumerableRepository.GetItem(_item.Id, collectionName);
      if (item == null) {
        return false;
      }

      var result = _postingRepository.DeleteItem(appUser, item, collectionName, session);
      return result;
    }

    public bool DeleteItemsByAccountId(string accountId, string collectionName, IClientSessionHandle? session = null) {
      ArgumentException.ThrowIfNullOrEmpty(accountId);
      ArgumentException.ThrowIfNullOrEmpty(collectionName);
      var result = _postingRepository.DeleteItemByAccountId(accountId, collectionName, session);
      return result;
    }
  }
}
