namespace utils.Exceptions
{
  public class ApiException : Exception {
    public int StatusCode { get; set; }
    public string? LanguageCode { get; set; }
    public object[]? Args { get; set; }
    public Exception? InnerException2 { get; set; }
    /// <summary>
    /// Here the language code will be translated into the corresponding language later in the ApiExcepctionFilter
    /// </summary>
    /// <param name="languageCode"></param>
    /// <param name="args"></param>
    /// <param name="statusCode"></param>
    /// <param name="innerEx"></param>
    public ApiException(string languageCode, object[] args, int statusCode = 400, Exception? innerEx = null)
          : base(languageCode) {
      LanguageCode = languageCode;
      StatusCode = statusCode;
      Args = args;
      InnerException2 = innerEx;
    }

    /// <summary>
    /// Here the message is already translated and will not be translated separately in the ApiExcepctionFilter later
    /// </summary>
    /// <param name="message"></param>
    /// <param name="statusCode"></param>
    /// <param name="innerEx"></param>
    public ApiException(string message, int statusCode = 400, Exception? innerEx = null) : base(message) {
        StatusCode = statusCode;
        InnerException2 = innerEx;
    }
  }
}
