using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace budget.webapi.Security {

  public class BudgetTrackerTokenValidationParameters : TokenValidationParameters {
    private string jwksUrl { get; set; }
    private HttpClient httpClient = new HttpClient();
    private readonly string KEY = "ValidationTokenParameters:BudgetTrackerTokenValidationParameters";

    public BudgetTrackerTokenValidationParameters(IConfiguration configuration) {
      if (configuration == null) {
        throw new ArgumentNullException(nameof(configuration), "Configuration is not set");
      }

      if (string.IsNullOrWhiteSpace(configuration[string.Format("{0}:{1}", KEY, "JwksUrl")])) {
        throw new ArgumentNullException(nameof(configuration), "Configuration is not set");
      }

      jwksUrl = configuration[string.Format("{0}:{1}", KEY, "JwksUrl")] ?? string.Empty;

      if (string.IsNullOrEmpty(jwksUrl)) {
        throw new ArgumentNullException(nameof(configuration), "Configuration is not set");
      }

      var jwksJson = httpClient.GetStringAsync(jwksUrl).Result;
      var jwks = new JsonWebKeySet(jwksJson);
      IssuerSigningKeys = jwks.Keys;

      ValidIssuer = configuration[string.Format("{0}:{1}", KEY, "ValidIssuer")];
      ValidAudience = configuration[string.Format("{0}:{1}", KEY, "ValidAudience")];

      if (bool.TryParse(configuration[string.Format("{0}:{1}", KEY, "ValidateIssuer")], out var validateIssuer)) {
        ValidateIssuer = validateIssuer;
      }

      if (bool.TryParse(configuration[string.Format("{0}:{1}", KEY, "ValidateAudience")], out var validateAudience)) {
        ValidateAudience = validateAudience;
      }

      if (bool.TryParse(configuration[string.Format("{0}:{1}", KEY, "ValidateLifetime")], out var validateLifetime)) {
        ValidateLifetime = validateLifetime;
      }

      ValidateIssuerSigningKey = true;
      var section = configuration[string.Format("{0}:{1}", KEY, "AccessKey")];
      var key = section == null ? null : Encoding.ASCII.GetBytes(section);
      IssuerSigningKey = new SymmetricSecurityKey(key);
    }
  }
}
