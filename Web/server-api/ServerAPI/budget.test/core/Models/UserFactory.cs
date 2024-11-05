using budget.core.Factories;
using budget.core.Models.Users;
using System.Security.Claims;

namespace budget.test.core {
  public class UserFactoryTest {
    [Fact]
    public void CreateUser_ValudGoogleClaimInput_ReturnGoogleUser() {
      // Arrange      
      var googleClaim = new Claim("iss", "https://accounts.google.com");
      var googleClaimID = new Claim(GoogleUser.CLAIMS_NAMEIDENTIFIER, "xxxxxxxxxxxxxxxx");
     
      var ClaimsIdentity = new ClaimsIdentity(new List<Claim> { googleClaim, googleClaimID });
      var googleClaims = new ClaimsPrincipal(ClaimsIdentity);

      var factory = new UserFactory();

      // Action
      var user = factory.CreateUser(googleClaims);

      // Assert
      Assert.NotNull(user);
    }

    [Fact]
    public void CreateUser_ValudGoogleClaimInput_ReturnBudgetTrackerUser() {
      // Arrange      
      var googleClaim = new Claim("iss", "loginapi");
      var googleClaimID = new Claim(BudgetTrackerUser.CLAIMS_NAMEIDENTIFIER, "xxxxxxxxxxxxxxxx");

      var ClaimsIdentity = new ClaimsIdentity(new List<Claim> { googleClaim, googleClaimID });
      var googleClaims = new ClaimsPrincipal(ClaimsIdentity);

      var factory = new UserFactory();

      // Action
      var user = factory.CreateUser(googleClaims);

      // Assert
      Assert.NotNull(user);
    }
  }
}