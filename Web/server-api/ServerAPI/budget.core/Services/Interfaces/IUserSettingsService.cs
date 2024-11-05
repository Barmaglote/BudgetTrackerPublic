using budget.core.Models.Users;
using budget.core.Models;
using budget.core.DB.Interfaces;

namespace budget.core.Services.Interfaces
{
  public interface IUserSettingsService
  {
    UserSettingsView GetUserSettings(AppUser appUser);
    UserSettingsView GetUserSettings(string userId);
    bool UpsertUserSettings(AppUser appUser, UserSettingsView userSettingsView, ITransactionManager? transactionManager = null);    
    bool Upsert<T>(AppUser appUser, T item) where T: Identifiable;
    bool UpdateField<T>(AppUser appUser, string fieldName, T fieldValue);
    bool RemoveUserSubscription(string userId, string subscriptionId);
    bool UpsertUserSubscription(string userId, UserSubscribtion userSubscription);
  }
}