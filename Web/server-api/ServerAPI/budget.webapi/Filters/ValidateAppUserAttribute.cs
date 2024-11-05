using budget.webapi.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace budget.utils.ApiFilters {
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
 public class ValidateAppUserAttribute : ActionFilterAttribute {

    public override void OnActionExecuting(ActionExecutingContext context) {
      var controller = context.Controller as BaseController;

      if (controller?.CurrentAppUser == null) {
        context.Result = new ObjectResult("CurrentAppUser is null") {
          StatusCode = 500 // Set the appropriate HTTP status code
        };
      }

      base.OnActionExecuting(context);
    }
  }
}
