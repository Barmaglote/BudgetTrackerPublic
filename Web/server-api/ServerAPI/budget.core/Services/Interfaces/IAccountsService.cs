using budget.core.DB.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;

namespace budget.core.Services.Interfaces
{
  public interface IAccountsService {
    bool ChangeQuantity(AppUser appUser, string accountID, decimal difference, ITransactionManager? transactionManager);
    bool ChangeQuantity(AppUser appUser, string accountID1, decimal difference1, string accountID2, decimal difference2, ITransactionManager? transactionManager);
    AccountItem? GetItemById(AppUser appUser, string accountID);
  }
}