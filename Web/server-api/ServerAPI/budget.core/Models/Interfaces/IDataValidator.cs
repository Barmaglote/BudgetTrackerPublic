using budget.core.Entities;
using budget.core.Models.Users;

namespace budget.core.Models.Interfaces {
  public interface IDataValidator {
    bool IsValidToAdd(AppUser appUser, string collectionName, BaseItem item);
    bool IsValidToUpdate(AppUser appUser, string collectionName, BaseItem item);
    bool IsValidToAdd(string userId, string collectionName, BaseItem item);
    bool IsValidToUpdate(string userId, string collectionName, BaseItem item);
  }
}
