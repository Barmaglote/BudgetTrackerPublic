using budget.core.Factories.Interfaces;
using budget.core.Models.Users;
using System.Security.Claims;

namespace budget.core.Factories {
  public class UserFactory : IUserFactory {
    public AppUser? CreateUser(ClaimsPrincipal claimsPrincipal) {

      var issClaim = claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "iss");

      if (issClaim == null) {
        return null;
      }

      if (issClaim.Value.Equals("https://accounts.google.com")) {
        return GoogleUser.Create(claimsPrincipal);
      }

      if (issClaim.Value.Equals("loginapi")) {
        return BudgetTrackerUser.Create(claimsPrincipal);
      }

      return null;
    }
  }
}
