using budget.webapi.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace budget.webapi.Middlewares {
  public class TokenValidationMiddleware {
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration; // Добавьте поле для IConfiguration
    private readonly ILogger<TokenValidationMiddleware> _logger; // Добавьте поле для IConfiguration

    public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<TokenValidationMiddleware> logger) {
      _next = next;
      _configuration = configuration;
      _logger = logger;
    }

    public async Task Invoke(HttpContext context) {
      if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) && context.Request.Headers.TryGetValue("Provider", out var authorizationProvider)) {
        var firstHeaderValue = authorizationHeader.FirstOrDefault();
        var authorizationProviderValue = authorizationProvider.FirstOrDefault();
        if (!string.IsNullOrEmpty(firstHeaderValue) && TryGetBearerToken(firstHeaderValue, out var idToken)) {
          var isValid = ValidateToken(idToken, authorizationProviderValue, out ClaimsPrincipal? claimsPrincipal);
          if (!string.IsNullOrEmpty(idToken) && !string.IsNullOrEmpty(authorizationProviderValue) && isValid && claimsPrincipal != null) {
            // Токен прошел проверку, выполните аутентификацию пользователя здесь, например, используя Identity или другую систему аутентификации.
            context.User = claimsPrincipal;
            await _next(context);
            return;
          }
        }
      }

      context.Response.StatusCode = 401; // Неавторизовано
    }

    private bool TryGetBearerToken(string headerValue, out string? token) {
      if (headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) {
        var authorizationHeaderParts = headerValue.ToString().Split(' ');
        token = authorizationHeaderParts[1];
        return true;
      }
      token = null;
      return false;
    }

    public bool ValidateToken(string? idToken, string? provider, out ClaimsPrincipal? claimsPrincipal) {

      if (string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(provider)) {
        claimsPrincipal = null;
        return false;
      }
      var validationParameters = TokenValidationParametersFactory.Create(provider, _configuration);
      var tokenHandler = new JwtSecurityTokenHandler();

      try {
        SecurityToken validatedToken;
        claimsPrincipal = tokenHandler.ValidateToken(idToken, validationParameters, out validatedToken);

        if (validatedToken is JwtSecurityToken jwtSecurityToken) {
          // Проверка даты истечения срока действия (expiration)
          if (DateTime.UtcNow > jwtSecurityToken.ValidTo || DateTime.UtcNow < jwtSecurityToken.ValidFrom ) {
            _logger.LogWarning("Токен истек");
            claimsPrincipal = null;
            return false;
          }

          // Проверка издателя (Issuer)
          if (jwtSecurityToken.Issuer != validationParameters.ValidIssuer) {
            _logger.LogWarning("Wrong Token Issuer");
            claimsPrincipal = null;
            return false;
          }

          // Проверка аудитории (Audience)
          if (!jwtSecurityToken.Audiences.Any(aud => aud == validationParameters.ValidAudience)) {
            _logger.LogWarning("Wrong Token Audience");
            claimsPrincipal = null;
            return false;
          }

          // Проверка наличия подписи токена
          if (jwtSecurityToken.SigningKey == null) {
            _logger.LogWarning("No signing key on Token");
            claimsPrincipal = null;
            return false;
          }
        } else {
          _logger.LogWarning("False JWT token");
          claimsPrincipal = null;
          return false;
        }

        return true;
      } catch (Exception e) {
        _logger.LogError(new EventId(), e.Message);
        claimsPrincipal = null;
        return false;
      }
    }
  }
}
