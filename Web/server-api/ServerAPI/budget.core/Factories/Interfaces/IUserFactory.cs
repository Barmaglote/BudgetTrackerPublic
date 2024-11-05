using budget.core.Models.Users;
using System.Security.Claims;

namespace budget.core.Factories.Interfaces {
  public interface IUserFactory {
    AppUser? CreateUser(ClaimsPrincipal claimsPrincipal);
  }
}
