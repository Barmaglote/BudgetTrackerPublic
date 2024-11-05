using Microsoft.IdentityModel.Tokens;

namespace budget.webapi.Security {

  public class GoogleTokenValidationParameters : TokenValidationParameters {
    private string? jwksUrl { get; set; }
    private HttpClient httpClient = new HttpClient();

    public GoogleTokenValidationParameters(IConfiguration configuration) {
      if (configuration == null) { 
        throw new ArgumentNullException(nameof(configuration), "Configuration is not set");
      }

      if (string.IsNullOrWhiteSpace(configuration["ValidationTokenParameters:GoogleTokenValidationParameters:JwksUrl"])) {
        throw new ArgumentNullException(nameof(configuration), "Configuration is not set");
      }

      jwksUrl = configuration["ValidationTokenParameters:GoogleTokenValidationParameters:JwksUrl"];

      if (string.IsNullOrEmpty(jwksUrl)) {
        throw new ArgumentNullException(nameof(configuration), "Configuration is not set");
      }

      var jwksJson = httpClient.GetStringAsync(jwksUrl).Result;
      var jwks = new JsonWebKeySet(jwksJson);
      IssuerSigningKeys = jwks.Keys;

      ValidIssuer = configuration["ValidationTokenParameters:GoogleTokenValidationParameters:ValidIssuer"];
      ValidAudience = configuration["ValidationTokenParameters:GoogleTokenValidationParameters:ValidAudience"];

      if (bool.TryParse(configuration["ValidationTokenParameters:GoogleTokenValidationParameters:ValidateIssuer"], out var validateIssuer)) {
        ValidateIssuer = validateIssuer;
      }

      if (bool.TryParse(configuration["ValidationTokenParameters:GoogleTokenValidationParameters:ValidateAudience"], out var validateAudience)) {
        ValidateAudience = validateAudience;
      }

      if (bool.TryParse(configuration["ValidationTokenParameters:GoogleTokenValidationParameters:ValidateLifetime"], out var validateLifetime)) {
        ValidateLifetime = validateLifetime;
      }
    }
  }
}
