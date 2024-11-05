using budget.core.Factories.Interfaces;
using budget.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;
using utils.Exceptions;

namespace webapi.Controllers;

[Route("[controller]")]
public class HealthController : BaseController
{
  public HealthController(IUserFactory _userFactory) : base(_userFactory){
  }

  [HttpGet("state")]
  public bool GetState()
  {
    return true;
  }

  [HttpGet("error")]
  public bool GetError() {
    throw new ApiException("Error");
  }
}
