using budget.core.DB.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;

namespace budget.core.Services.Interfaces
{
  public interface IEntityService<TView> where TView : class {
    (IEnumerable<TView> Items, long TotalCount) GetItems(AppUser appUser, TableLazyLoadEvent tableLazyLoadEvent);
    bool UpsertItem(AppUser appUser, TView view, out bool isAdded, ITransactionManager? transactionManager = null);
    bool DeleteItem(AppUser appUser, TView view, ITransactionManager? transactionManager = null);
    IEnumerable<TView> FilterByField(AppUser appUser, string field, string value);
    TView? GetViewById(AppUser appUser, string id);
    IEnumerable<TView> GetItemByMonth(AppUser appUser, int year, int month);
    TView? GetLastItem(AppUser appUser);
  }
}