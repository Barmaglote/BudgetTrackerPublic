using budget.core.Models.Users;
using budget.core.Models;

namespace budget.core.Services.Interfaces
{
    public  interface ICreditService
    {
      IEnumerable<PaymentInfo>? GetTopPaymentsForActiveCredits(AppUser appUser, int top = 5);
      GeneralCreditsStatistics GetGeneralCreditsStatistics(AppUser appUser);
      IEnumerable<CreditItemView> GetViewByAccountId(AppUser appUser, string accountId);
  }
}