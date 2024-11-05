using System.Security.Claims;

namespace budget.core.Models.Users 
{
  public class GoogleUser : AppUser 
  {
    public static readonly string CLAIMS_NAMEIDENTIFIER = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    private static readonly string CLAIMS_EMAILADDRESS = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    private static readonly string CLAIMS_GIVENNAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
    private static readonly string CLAIMS_SURNAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";

    private ClaimsPrincipal? _claimsPrincipal;
    public override ClaimsPrincipal? ClaimsPrincipal => _claimsPrincipal;
    public override string Provider { get; } = "GG";
    private string _id { get; set; } = "0";
    public override string Id => _id;
    private GoogleUser() { }

    private GoogleUser(ClaimsPrincipal claimsPrincipal) {
      _claimsPrincipal = claimsPrincipal;
      _id = string.Format("{0}-{1}", Provider, claimsPrincipal.FindFirst(CLAIMS_NAMEIDENTIFIER)?.Value ?? "0");
      Email = claimsPrincipal.FindFirst(CLAIMS_EMAILADDRESS)?.Value;
      FirstName = claimsPrincipal.FindFirst(CLAIMS_GIVENNAME)?.Value;
      LastName = claimsPrincipal.FindFirst(CLAIMS_SURNAME)?.Value;
    }

    public static GoogleUser? Create(ClaimsPrincipal claimsPrincipal) {
      if (claimsPrincipal == null) return null;

      var userIdClaim = claimsPrincipal.FindFirst(CLAIMS_NAMEIDENTIFIER)?.Value;
      if (userIdClaim == null) return null;

      return new GoogleUser(claimsPrincipal);
    }
  }
}
