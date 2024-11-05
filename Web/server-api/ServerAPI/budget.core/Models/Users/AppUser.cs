using System.Security.Claims;

namespace budget.core.Models.Users
{
  abstract public class AppUser
  {
    public abstract string Id { get; }
    public abstract string Provider { get; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public virtual ClaimsPrincipal? ClaimsPrincipal { get; }
    public string UID { 
      get {
        return string.Format("{0}:{1}", Provider, Id);
      }
    }
  }
}
