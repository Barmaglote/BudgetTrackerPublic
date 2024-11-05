namespace budget.utils.ApiFilters.Models {
  public class ApiExceptionResult {
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public string? LanguageCode { get; set; }
    public Exception? InnerException { get; set; }
    public object[]? Args { get; set; }

    public ApiExceptionResult(string message) {
      Message = message;
    }
  }
}
