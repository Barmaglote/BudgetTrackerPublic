using budget.core.DB.Interfaces;
using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace budget.core.Services
{
  public abstract class EntityService<T, TView> : IEntityService<TView>
    where T : BaseItem
    where TView : class
  {

    protected virtual string _collectionName { get; set; } = "income";

    private readonly IEnumerableService<T, TView> _enumerableService;
    private readonly IPostingService<T, TView> _postService;
    private readonly IDeletingService<T, TView> _deletingService;
    private readonly IFilteringService<T, TView> _filteringService;

    public EntityService(
      IEnumerableService<T, TView> enumerableService,
      IPostingService<T, TView> postService,
      IDeletingService<T, TView> deletingService,
      IFilteringService<T, TView> filteringService
      ) {
      _enumerableService = enumerableService;
      _postService = postService;
      _deletingService = deletingService;
      _filteringService = filteringService;
    }

    public (IEnumerable<TView> Items, long TotalCount) GetItems(AppUser appUser, TableLazyLoadEvent tableLazyLoadEvent) {
      return _enumerableService.GetItemViews(appUser, _collectionName, tableLazyLoadEvent);
    }

    public bool UpsertItem(AppUser appUser, TView view, out bool isAdded, ITransactionManager? transactionManager = null) {
      bool result;
      IClientSessionHandle? session = null;
      if (transactionManager != null) {
        session = transactionManager.GetSession();
      }

      result = _postService.UpsertItem(appUser, view, _collectionName, out isAdded, session);
      return result;
    }
    public bool UpsertItem(string userId, TView view, out bool isAdded, ITransactionManager? transactionManager = null) {
      bool result;
      IClientSessionHandle? session = null;
      if (transactionManager != null) {
        session = transactionManager.GetSession();
      }

      result = _postService.UpsertItem(userId, view, _collectionName, out isAdded, session);
      return result;
    }

    public bool DeleteItem(AppUser appUser, TView view, ITransactionManager? transactionManager = null) {
      IClientSessionHandle? session = null;

      if (transactionManager != null) {
        session = transactionManager.GetSession();        
      }

      return _deletingService.DeleteItem(appUser, view, _collectionName, session);  
    }

    public bool DeleteItemByAccountId(string accountId, ITransactionManager? transactionManager) {
      IClientSessionHandle? session = null;

      if (transactionManager != null) {
        session = transactionManager.GetSession();
      }

      return _deletingService.DeleteItemsByAccountId(accountId, _collectionName, session);
    }

    public IEnumerable<TView> FilterByField(AppUser appUser, string field, string value) {
      var filter = Builders<T>.Filter.Eq(field, value);
      return _filteringService.GetItemViews(appUser, filter, _collectionName);
    }

    public IEnumerable<TView> FilterByField(string field, string value) {
      var filter = Builders<T>.Filter.Eq(field, value);
      return _filteringService.GetItemViews(filter, _collectionName);
    }

    public IEnumerable<TView> GetItemByMonth(AppUser appUser, int year, int month) {
      return _filteringService.GetItemByMonth(appUser, _collectionName, year, month);
    }

    public TView? GetViewById(AppUser appUser, string id) {
      TView? itemView = null;

      if (ObjectId.TryParse(id, out var objectId)) {
        itemView = _enumerableService.GetItemView(objectId, _collectionName);
      }

      return itemView;
    }

    public TView? GetLastItem(AppUser appUser) {
      var tableLazyLoadEvent = new TableLazyLoadEvent() {
        First = 0,
        Rows = 1,
        SortField = "day",
        SortOrder = 1
      };
      var (result, totalCount) = _enumerableService.GetItemViews(appUser, _collectionName, tableLazyLoadEvent);

      if (result == null || totalCount != 1 || !result.Any()) {
        return null;
      }

      return result.FirstOrDefault();
    }

    public IEnumerable<TView>? GetRegularPayments(AppUser appUser, int year) {

      DateTime firstDayOfYear = new DateTime(year, 1, 1, 0, 0, 0);
      DateTime lastDayOfYear = new DateTime(year, 12, 31, 23, 59, 59);

      var tableLazyLoadEvent = new TableLazyLoadEvent() {
        First = 0,
        Rows = 360,
        SortField = "category",
        SortOrder = 1,
        Filters = new Dictionary<string, FilterMetadata[]> {
          { "isRegular", new FilterMetadata[] { new FilterMetadata() { Value = true, MatchMode = "Eqbool", OperatorMode = "And" } } },
          { "date", new FilterMetadata[] {
            new FilterMetadata() { Value = firstDayOfYear, MatchMode = "Gte", OperatorMode = "And" },
            new FilterMetadata() { Value = lastDayOfYear, MatchMode = "Lte", OperatorMode = "And" }
          } }
        }
      };

      var (result, totalCount) = _enumerableService.GetItemViews(appUser, _collectionName, tableLazyLoadEvent);

      if (result == null || totalCount == 0 || !result.Any()) {
        return null;
      }

      return result;
    }
  }
}
