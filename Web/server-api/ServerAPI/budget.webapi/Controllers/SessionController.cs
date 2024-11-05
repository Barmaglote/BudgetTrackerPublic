using budget.core.Factories.Interfaces;
using budget.utils.ApiFilters;
using budget.webapi.Controllers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace webapi.Controllers;

[Authorize]
[IgnoreAntiforgeryToken]
[ValidateAppUser]
[Route("[controller]")]
public class SessionController : BaseController {

  private readonly IAntiforgery _antiforgery;

  public SessionController(IUserFactory _userFactory, IAntiforgery antiforgery) : base(_userFactory) {
    _antiforgery = antiforgery;
  }

  [HttpGet("csrf")]
  public ActionResult GetCSRF() {
    Log.Information("csrf 01");
    var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
    var token = tokens.RequestToken!;
    Log.Information("csrf 02 " + token);
    HttpContext.Response.Cookies.Append("XSRF-TOKEN", token, new CookieOptions {
      HttpOnly = false,
      Secure = true,
      SameSite = SameSiteMode.None,
      Expires = DateTimeOffset.UtcNow.AddDays(14)
    });
    return Ok(new { CSRFToken = token });
  }
}
