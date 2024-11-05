using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace budget.webapi.Security {
  public class AddRequiredProviderHeaderParameter : IOperationFilter {

    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
      if (operation.Parameters == null) {
        operation.Parameters = new List<OpenApiParameter>();
      }

      operation.Parameters.Add(new OpenApiParameter() {
        Name = "Provider",
        In = ParameterLocation.Header,
        Description = "Provider of authorization",
        Required = true,
        AllowEmptyValue = false
      });
    }
  }
}
