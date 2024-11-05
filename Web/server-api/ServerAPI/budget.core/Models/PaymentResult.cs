namespace budget.core.Models {
  public class PaymentResult {
    public int Amount { get; set; }
    public AmountDetails? AmountDetails { get; set; }
    public AutomaticPaymentMethods? AutomaticPaymentMethods { get; set; }
    public DateTime? CanceledAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? Currency { get; set; }
    public CaptureMethod? CaptureMethod { get; set; }
    public string? ClientSecret { get; set; }
    public string? ConfirmationMethod { get; set; }
    public long Created { get; set; }
    public string? Description { get; set; }
    public LastPaymentError? LastPaymentError { get; set; }
    public bool Livemode { get; set; }
    public NextAction? NextAction { get; set; }
    public string? Object { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public PaymentMethodConfigurationDetails? PaymentMethodConfigurationDetails { get; set; }
    public string[]? PaymentMethodTypes { get; set; }
    public Processing? Processing { get; set; }
    public string? ReceiptEmail { get; set; }
    public SetupFutureUsage? SetupFutureUsage { get; set; }
    public Shipping? Shipping { get; set; }
    public Source? Source { get; set; }
    public PaymentIntentStatus? Status { get; set; }

  }

  public enum CaptureMethod {
    Automatic,
    AutomaticAsync,
    Manual
  }

  public enum ConfirmationMethod {
    Automatic,
    Manual
  }

  public enum PaymentIntentStatus {
    RequiresPaymentMethod,
    RequiresConfirmation,
    RequiresAction,
    Processing,
    RequiresCapture,
    Canceled,
    Succeeded
  }

  public class Tip {
    public int Amount { get; set; }
  }

  public class AmountDetails {
    public Tip? Tip { get; set; }
  }

  public class AutomaticPaymentMethods {
    public bool Enabled { get; set; }
  }

  public class LastPaymentError {
    public string? Code { get; set; }
    public string? DeclineCode { get; set; }
    public string? DocUrl { get; set; }
    public string? Message { get; set; }
    public string? Param { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? Type { get; set; }
  }

  public class RedirectToUrl {
    public string? ReturnUrl { get; set; }
    public string? Url { get; set; }
  }

  public class NextAction {
    public string? Type { get; set; }
    public RedirectToUrl? RedirectToUrl { get; set; }
  }

  public class PaymentMethodConfigurationDetails {
    public string? Type { get; set; }
  }

  public class Processing {
    public string? Type { get; set; }
  }

  public class SetupFutureUsage {
    public string? Type { get; set; }
  }

  public class Shipping {
    public Address? Address { get; set; }
    public string? Carrier { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? TrackingNumber { get; set; }
  }

  public class Source {
    public string? Id { get; set; }
    public string? Type { get; set; }
  }


  public enum AllowRedirects {
    always,
    never
  }

  public class Card {
    public string? Brand { get; set; }
    public Checks? Checks { get; set; }
    public string? Country { get; set; }
    public int ExpMonth { get; set; }
    public int ExpYear { get; set; }
    public string? Fingerprint { get; set; }
    public string? Funding { get; set; }
    public string? Installments { get; set; }
    public string? Last4 { get; set; }
    public string? Network { get; set; }
    public string? ThreeDSecure { get; set; }
    public string? Wallet { get; set; }
  }

  public class Checks {
    public string? AddressLine1Check { get; set; }
    public string? AddressPostalCodeCheck { get; set; }
    public string? CvcCheck { get; set; }
  }

  public class Address {
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? PostalCode { get; set; }
    public string? State { get; set; }
  }

  public class BillingDetails {
    public Address? Address { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
  }

  public class PaymentMethod {
    public string? Id { get; set; }
    public string? Object { get; set; }
    public BillingDetails? BillingDetails { get; set; }
    public Card? Card { get; set; }
    public long? Created { get; set; }
    public string? Customer { get; set; }
    public bool? Livemode { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public string? Type { get; set; }
  }

}
