using System.Security.Claims;

namespace budget.core.Models.Users 
{
  public class BudgetTrackerUser : AppUser 
  {
    public static readonly string CLAIMS_NAMEIDENTIFIER = "id";
    private static readonly string CLAIMS_EMAILADDRESS = "login";
    private static readonly string CLAIMS_GIVENNAME = "username";

    private ClaimsPrincipal? _claimsPrincipal;
    public override ClaimsPrincipal? ClaimsPrincipal => _claimsPrincipal;
    public override string Provider { get; } = "BT";
    private string _id { get; set; } = "0";
    public override string Id => _id;
    private BudgetTrackerUser() { }

    private BudgetTrackerUser(ClaimsPrincipal claimsPrincipal) {
      _claimsPrincipal = claimsPrincipal;
      _id = string.Format("{0}-{1}", Provider, claimsPrincipal.FindFirst(CLAIMS_NAMEIDENTIFIER)?.Value ?? "0");
      Email = claimsPrincipal.FindFirst(CLAIMS_EMAILADDRESS)?.Value;
      FirstName = claimsPrincipal.FindFirst(CLAIMS_GIVENNAME)?.Value;
    }

    public static BudgetTrackerUser? Create(ClaimsPrincipal claimsPrincipal) {
      if (claimsPrincipal == null) return null;

      var userIdClaim = claimsPrincipal.FindFirst(CLAIMS_NAMEIDENTIFIER)?.Value;
      if (userIdClaim == null) return null;

      return new BudgetTrackerUser(claimsPrincipal);
    }

    public static BudgetTrackerUser CreateEmpty() {
      return new BudgetTrackerUser();
    }
  }
}
