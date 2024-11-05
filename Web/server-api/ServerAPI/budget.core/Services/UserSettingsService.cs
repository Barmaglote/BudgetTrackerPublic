using budget.core.DB.Interfaces;
using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services.Interfaces;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;
using utils.Exceptions;

namespace budget.core.Services
{
    public class UserSettingsService : EntityService<UserSettings, UserSettingsView>, IUserSettingsService {
    protected override string _collectionName { get; set; } = "usersettings";
    public static readonly int MAX_ACCOUNTS = 10;
    public static readonly int MAX_TEMPLATES = 50;
    public static readonly int MAX_CATEGORIES = 100;

    public UserSettingsService(
      IEnumerableService<UserSettings, UserSettingsView> enumerableService,
      IPostingService<UserSettings, UserSettingsView> postingService,
      IDeletingService<UserSettings, UserSettingsView> deletingService,
      IFilteringService<UserSettings, UserSettingsView> filteringService
      ) : base(enumerableService, postingService, deletingService, filteringService) {
    }

    public UserSettingsView GetUserSettings(AppUser appUser) {
      var items = FilterByField(appUser, "OwnerUserId", appUser.Id);
      if (items == null || !items.Any() || items.Count() > 1) {
        return new UserSettingsViewEmpty();
      }

      return items.First();
    }

    public UserSettingsView GetUserSettings(string userId) {
      var items = FilterByField("OwnerUserId", userId);
      if (items == null || !items.Any() || items.Count() > 1) {
        return new UserSettingsViewEmpty();
      }

      return items.First();
    }

    public bool UpsertUserSettings(AppUser appUser, UserSettingsView userSettingsView, ITransactionManager? transactionManager = null) {
      var items = FilterByField(appUser, "OwnerUserId", appUser.Id);

      if (items != null && items.Any() && items.Count() > 1) {
        throw new ApiException("Wrong user settings");
      }

      if (!AreSettingsValid(appUser, userSettingsView)) {
        throw new ApiException("Wrong user settings");
      }

      return UpsertItem(appUser, userSettingsView, out var isAdded, transactionManager);
    }

    public bool AreSettingsValid(AppUser appUser, UserSettingsView userSettingsView) {
      if (userSettingsView == null) { return false; }

      int maxAccounts = MAX_ACCOUNTS;
      int maxTemplates = MAX_TEMPLATES;
      int maxCategories = MAX_CATEGORIES;
      var t01Subscription = userSettingsView.Subscribtions?.FirstOrDefault(s => s.PlanId == "T01" && s.IsActive);

      if (t01Subscription != null) {
        maxAccounts = 50;
        maxTemplates = 200;
        maxCategories = 200;
      }


      if (userSettingsView.Accounts != null && (userSettingsView.Accounts.Count() + 1) > maxAccounts) {
        return false;
      }

      if (!string.IsNullOrEmpty(userSettingsView.Email)) {
        return false;
      }

      if (userSettingsView.Templates != null && userSettingsView.Templates.Any(x => x.Value.Length + 1 > maxTemplates)) {
        return false;
      }

      if (userSettingsView.Categories != null && userSettingsView.Categories.Sum(category => category.Value.Length) > maxCategories) {
        return false;
      }

      return true;
    }

    public bool DeleteAccountItem(AppUser appUser, string id, ITransactionManager? transactionManager) {
      if (string.IsNullOrWhiteSpace(id)) {
        return false;
      }

      var items = FilterByField(appUser, "OwnerUserId", appUser.Id);

      if (items == null || !items.Any()) {
        return false;
      }

      var userSettings = items.Single();

      if (userSettings?.Accounts == null || !userSettings.Accounts.Any()) {
        return false;
      }

      var accounts = userSettings.Accounts.Where(x => !string.IsNullOrEmpty(x.ID) && x.ID != id).ToList();
      userSettings.Accounts = accounts;
      return UpsertItem(appUser, userSettings, out var isAdded, transactionManager);
    }

    public AccountItem? GetAccountItem(AppUser appUser, string id) {
      if (string.IsNullOrWhiteSpace(id)) {
        return null;
      }

      var items = FilterByField(appUser, "OwnerUserId", appUser.Id);

      if (items == null || !items.Any()) {
        return null;
      }

      var userSettings = items.Single();

      if (userSettings == null || userSettings.Accounts == null) {
        return null;
      }

      var accounts = (userSettings.Accounts != null) ? userSettings.Accounts.ToList() : new List<AccountItem>();

      return accounts.Find(x => x.ID == id);
    }

    public bool Upsert<T>(AppUser appUser, T item) where T: Identifiable {
      if (!string.IsNullOrWhiteSpace(item.ID)) {
        return Update(appUser, item);
      } else {
        return Add(appUser, item);
      }
    }

    private bool Update<T>(AppUser appUser, [Required] T item) where T : Identifiable {
      if (string.IsNullOrWhiteSpace(item.ID) || item == null) {
        return false;
      }

      var items = FilterByField(appUser, "OwnerUserId", appUser.Id);

      if (items == null || !items.Any()) {
        return false;
      }

      var userSettings = items.Single();

      if (userSettings == null || userSettings.Accounts == null) {
        return false;
      }

      if (typeof(T) == typeof(AccountItem)) {

        var accounts = (userSettings.Accounts != null) ? userSettings.Accounts : new List<AccountItem>();
        if (accounts.Count > MAX_ACCOUNTS) {
          return false;
        }

        accounts = accounts.Where(x => !string.IsNullOrEmpty(x.ID)).Where(x => x.ID != item.ID).ToList();
        var accountItem = item as AccountItem;
        if (accountItem != null && accountItem.Initial == null) {
          accountItem.Initial = 0;
        }

        if (accountItem != null) {
          accounts.Add(accountItem);
        }

        userSettings.Accounts = accounts;
      }

      return UpsertItem(appUser, userSettings, out var isAdded);
    }

    public bool UpdateField<T>(AppUser appUser, string fieldName, T fieldValue) {
      if (string.IsNullOrEmpty(fieldName)) { return false; }
      var items = FilterByField(appUser, "OwnerUserId", appUser.Id);

      var userSettings = new UserSettingsView();

      if (items != null && items.Count() > 1) {
        throw new ApiException("Wrong user settings");
      }

      if (items != null && items.Any()) {
        userSettings = items.First();
      }

      if (userSettings == null) {
        userSettings = new UserSettingsView();
      }

      var property = typeof(UserSettingsView).GetProperty(fieldName);
      if (property != null) {
        property.SetValue(userSettings, fieldValue);
        return UpsertItem(appUser, userSettings, out bool isAdded);
      } else {
        return false;
      }
    }

    private bool Add<T>(AppUser appUser, [Required] T item) where T : Identifiable {
      if (!string.IsNullOrWhiteSpace(item!.ID) || item == null) {
        return false;
      }

      var items = FilterByField(appUser, "OwnerUserId", appUser.Id);
      var userSettings = new UserSettingsView();

      if (items != null && items.Count() > 1) {
        throw new ApiException("Wrong user settings");
      }

      if (items != null && items.Any()) {
        userSettings = items.First();
      }

      if (userSettings == null) {
        userSettings = new UserSettingsView() {
          Accounts = new List<AccountItem>()
        };
      }

      if (typeof(T) == typeof(AccountItem)) {
        if (userSettings.Accounts == null) {
          userSettings.Accounts = new List<AccountItem>();
        }

        var accountItem = item as AccountItem;

        if (accountItem != null && accountItem.Initial == null) {
          accountItem.Initial = 0;
        }

        if (accountItem != null) {
          accountItem.Quantity = accountItem.Initial;
          accountItem.ID = Guid.NewGuid().ToString();
          userSettings.Accounts.Add(accountItem);
        }
      }

      if (!AreSettingsValid(appUser, userSettings)) {
        throw new ApiException("Wrong user settings");
      }

      return UpsertItem(appUser, userSettings, out bool isAdded);
    }

    public bool UpsertUserSubscription(string userId, UserSubscribtion userSubscription) {
      var items = FilterByField("OwnerUserId", userId);

      if (items == null || !items.Any()) {
        return false;
      }

      var userSettings = items.First();

      if (userSettings.Subscribtions == null) {
        userSettings.Subscribtions = new List<UserSubscribtion>();
      }

      var existingSubscription = userSettings.Subscribtions
          .FirstOrDefault(s => s.PlanId == userSubscription.PlanId);

      if (existingSubscription != null) {
        userSettings.Subscribtions.Remove(existingSubscription);
      }

      userSettings.Subscribtions.Add(userSubscription);

      return UpsertItem(userId, userSettings, out var isAdded);
    }

    public bool RemoveUserSubscription(string userId, string subscriptionId) {
      var items = FilterByField("OwnerUserId", userId);

      if (items == null || !items.Any()) {
        return false;
      }

      var userSettings = items.First();

      if (userSettings.Subscribtions == null || !userSettings.Subscribtions.Any()) {
        return false;
      }

      var subscription = userSettings.Subscribtions
          .FirstOrDefault(s => s.SubscribtionId == subscriptionId);

      if (subscription == null) {
        return false;
      }

      userSettings.Subscribtions.Remove(subscription);

      return UpsertItem(userId, userSettings, out var isAdded);
    }
  }
}
