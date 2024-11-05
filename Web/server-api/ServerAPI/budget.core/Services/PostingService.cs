using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models.Users;
using budget.core.Repositories.Interfaces;
using budget.core.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using utils.Exceptions;

namespace budget.core.Services {
  public class PostingService<T, TView> : IPostingService<T, TView>
    where T : BaseItem, ITransformable<TView>
    where TView : class, ITransformable<T>
  {

    private readonly IFactory<T, TView> _factory;
    private readonly IPostingRepository<T> _postingRepository;
    private readonly IEnumerableRepository<T> _enumerableRepository;

    public PostingService(
      IPostingRepository<T> postingRepository, 
      IFactory<T, TView> factory, 
      IEnumerableRepository<T> enumerableRepository
    ) {
      _factory = factory;
      _postingRepository = postingRepository;
      _enumerableRepository = enumerableRepository;
    }

    public bool UpsertItem(AppUser appUser, TView view, string collectionName, out bool isAdded, IClientSessionHandle? sessionHandle = null) {
      if (view == null) {
        throw new ApiException("Nothing to add"); // Завернуть в AssertCode и потом вывести в логи через ExceptionFilter
      }

      var isExist = ((view.Id.Timestamp == 0 || view.Id.ToString().Equals("000000000000000000000000") || view.Id == ObjectId.Empty) ? null : _enumerableRepository.GetItem(view.Id, collectionName)) != null;
      bool result;

      var item = _factory.CreateItem(view);
      if (isExist) {
        result = _postingRepository.UpdateItem(appUser, item, collectionName, sessionHandle);
        isAdded = false;
      } else {
        item.OwnerUserId = appUser.Id;
        result = _postingRepository.AddItem(appUser, item, collectionName, sessionHandle);
        isAdded = true;
      }

      return result;
    }

    public bool UpsertItem(string userId, TView view, string collectionName, out bool isAdded, IClientSessionHandle? sessionHandle = null) {
      if (view == null) {
        throw new ApiException("Nothing to add"); // Завернуть в AssertCode и потом вывести в логи через ExceptionFilter
      }

      var isExist = ((view.Id.Timestamp == 0 || view.Id.ToString().Equals("000000000000000000000000") || view.Id == ObjectId.Empty) ? null : _enumerableRepository.GetItem(view.Id, collectionName)) != null;
      bool result;

      var item = _factory.CreateItem(view);
      if (isExist) {
        result = _postingRepository.UpdateItem(userId, item, collectionName, sessionHandle);
        isAdded = false;
      } else {
        item.OwnerUserId = userId;
        result = _postingRepository.AddItem(userId, item, collectionName, sessionHandle);
        isAdded = true;
      }

      return result;
    }
  }
}
