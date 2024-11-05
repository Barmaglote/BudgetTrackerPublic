namespace budget.webapi.Middlewares {
  public class AntiForgeryTokenLoggingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<AntiForgeryTokenLoggingMiddleware> _logger;

    public AntiForgeryTokenLoggingMiddleware(RequestDelegate next, ILogger<AntiForgeryTokenLoggingMiddleware> logger) {
      _next = next;
      _logger = logger;
    }

    public async Task Invoke(HttpContext context) {
      // Log request tokens
      if (context.Request.Cookies.TryGetValue("XSRF-TOKEN", out var xsrfToken)) {
        _logger.LogInformation("Request XSRF-TOKEN: {Token}", xsrfToken);
      }
      if (context.Request.Headers.TryGetValue("X-XSRF-TOKEN", out var headerToken)) {
        if (headerToken.Any()) {
          _logger.LogInformation("Request X-XSRF-TOKEN: {Token}", headerToken!);
        } else {
          _logger.LogInformation("Request X-XSRF-TOKEN: NULL");
        }
      }

      await _next(context);
    }
  }

}
