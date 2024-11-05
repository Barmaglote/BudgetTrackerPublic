using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace budget.utils.ApiFilters {
  public class AddCustomHeaderResultFilter : IResultFilter {
    private readonly string _uniqueIdentifier = Guid.NewGuid().ToString();
    private readonly string _hostName = Dns.GetHostName();
    private readonly string _headerName = "X-Unique-Identifier-host";
    private readonly string _headerValue = "UNDEFINED";

    public AddCustomHeaderResultFilter() {
      string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
      
      if (environment.Equals("Production", StringComparison.OrdinalIgnoreCase)) {
        _headerValue = _uniqueIdentifier;
      } else {
        _headerValue = string.Format("{0}-{1}", _hostName, _uniqueIdentifier);
      }
    }

    public void OnResultExecuting(ResultExecutingContext context) {
      if (context.HttpContext.Response.Headers.ContainsKey(_headerName)) { return; }

      context.HttpContext.Response.Headers.Add(_headerName, _headerValue);
    }

    public void OnResultExecuted(ResultExecutedContext context) {
    }
  }
}
