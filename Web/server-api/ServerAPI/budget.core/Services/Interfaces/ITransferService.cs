using budget.core.DB.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;

namespace budget.core.Services.Interfaces
{
  public interface ITransferService
  {
    TransferItem? GetItemById(AppUser appUser, string id);
    bool AddTransfer(AppUser appUser, TransferItem transferItem, ITransactionManager? transactionManager = null);
    (IEnumerable<TransferItem>? Items, long TotalCount) GetItems(AppUser appUser, TableLazyLoadEvent tableLazyLoadEvent);
    bool DeleteItemById(AppUser appUser, string id, ITransactionManager? transactionManager = null);
  }
}