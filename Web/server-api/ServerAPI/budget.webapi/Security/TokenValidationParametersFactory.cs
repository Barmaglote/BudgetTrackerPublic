using Microsoft.IdentityModel.Tokens;

namespace budget.webapi.Security {
  public class TokenValidationParametersFactory {
    public static TokenValidationParameters Create(string provider, IConfiguration configuration) {
      if (string.IsNullOrWhiteSpace(provider)) {
        throw new ArgumentNullException(nameof(provider), "Provider is missing");
      }

      var validationParameters = provider.ToLower() switch {
        "google" => new GoogleTokenValidationParameters(configuration),
        "budgettracker" => new BudgetTrackerTokenValidationParameters(configuration),
        "facebook" => new FacebookTokenValidationParameters(),
        _ => new TokenValidationParameters(),
      };

      return validationParameters;
    }
  }
}
