using budget.core.DB.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services.Interfaces;

namespace budget.core.Services
{
  public class AccountsService : IAccountsService {
    private readonly UserSettingsService _userSettingsService;

    public AccountsService(UserSettingsService userSettingsService) {
      _userSettingsService = userSettingsService;
    }

    public bool ChangeQuantity(AppUser appUser, string accountID, decimal difference, ITransactionManager? transactionManager = null) {
      var settings = _userSettingsService.GetUserSettings(appUser);

      if (settings == null || settings?.Accounts == null || settings?.Accounts.Count == 0) {
        return false;
      }

      var index = settings?.Accounts.FindIndex(x => x.ID == accountID);

      if (index == -1 || index == null) {
        return false;
      }

      var account = settings?.Accounts[index.Value];

      if (account == null || settings?.Accounts[index.Value] == null) {
        return false;
      }

      account.Quantity += difference;

      settings.Accounts[index.Value] = account;
      _userSettingsService.UpsertUserSettings(appUser, settings, transactionManager);

      return true;
    }

    public bool ChangeQuantity(AppUser appUser, string accountID1, decimal difference1, string accountID2, decimal difference2, ITransactionManager? transactionManager = null) {
      var settings = _userSettingsService.GetUserSettings(appUser);

      if (settings == null || settings?.Accounts == null || settings?.Accounts.Count == 0) {
        return false;
      }

      #region Account 1
      var index1 = settings?.Accounts.FindIndex(x => x.ID == accountID1);

      if (index1 == -1 || index1 == null) {
        return false;
      }

      var account1 = settings?.Accounts[index1.Value];

      if (account1 == null || settings?.Accounts[index1.Value] == null) {
        return false;
      }

      account1.Quantity += difference1;
      #endregion

      #region Account 2
      var index2 = settings?.Accounts.FindIndex(x => x.ID == accountID2);

      if (index2 == -1 || index2 == null) {
        return false;
      }

      var account2 = settings?.Accounts[index2.Value];

      if (account2 == null || settings?.Accounts[index2.Value] == null) {
        return false;
      }

      account2.Quantity += difference2;
      #endregion


      settings.Accounts[index1.Value] = account1;
      settings.Accounts[index2.Value] = account2;
      _userSettingsService.UpsertUserSettings(appUser, settings, transactionManager);

      return true;
    }

    public AccountItem? GetItemById(AppUser appUser, string accountID) {
      var settings = _userSettingsService.GetUserSettings(appUser);
      return settings?.Accounts?.FirstOrDefault(x => x.ID == accountID);
    }
  }
}
