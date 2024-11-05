using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Net;

public class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute {

  public override void OnException(ExceptionContext context) {

    var result = new ObjectResult(new {
      context.Exception.Message, // Or a different generic message
      context.Exception.Source,
      ExceptionType = context.Exception.GetType().FullName,
    }) {
      StatusCode = (int)HttpStatusCode.InternalServerError
    };

    Log.Error("Unhandled exception occurred while executing request: {ex}", context.Exception);
    context.Result = result;
  }
}
