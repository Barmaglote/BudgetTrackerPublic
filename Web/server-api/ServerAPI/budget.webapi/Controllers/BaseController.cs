using Asp.Versioning;
using budget.core.Factories.Interfaces;
using budget.core.Models.Users;
using budget.utils.ApiFilters;
using Microsoft.AspNetCore.Mvc;

namespace budget.webapi.Controllers {
  [ApiController]
  [ApiVersion("1.0")] //To use another version set Header "api-version" with new version
  [Route("[controller]")]
  [ServiceFilter(typeof(UnhandledExceptionFilterAttribute))]
  [ServiceFilter(typeof(AddCustomHeaderResultFilter))]
  public class BaseController : ControllerBase {

    private readonly IUserFactory _userFactory;

    public BaseController(IUserFactory userFactory) {
      _userFactory = userFactory;
    }

    public AppUser? CurrentAppUser {
      get {
        if (User == null) {
          return null;
        }

        return _userFactory?.CreateUser(User); 
      }
    }
  }
}
