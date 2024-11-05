using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services.Interfaces;

namespace budget.core.Services
{
    public class CreditsService : EntityService<CreditItem, CreditItemView>, ICreditService {
    protected override string _collectionName{ get; set; } = "credits";
    private readonly int _maxAllowedCredits = 100;
    private readonly int _maxAllowedPaymens = 10;
    private readonly UserSettingsService _userSettingsService;

    public CreditsService(
      IEnumerableService<CreditItem, CreditItemView> enumerableService,
      IPostingService<CreditItem, CreditItemView> postingService,
      IDeletingService<CreditItem, CreditItemView> deletingService,
      IFilteringService<CreditItem, CreditItemView> filteringService,
      UserSettingsService userSettingsService
      ) : base(enumerableService, postingService, deletingService, filteringService) {
      _userSettingsService = userSettingsService;
    }

    public GeneralCreditsStatistics GetGeneralCreditsStatistics(AppUser appUser) {
      var tableLazyLoadEvent = new TableLazyLoadEvent() {
        First = 0,
        Rows = _maxAllowedCredits,
        Filters = new Dictionary<string, FilterMetadata[]> {
          { "IsActive", new FilterMetadata[] { new FilterMetadata() { Value = true, MatchMode = "Eq", OperatorMode = "And" } } }
        }
      };

      var (items, totalCount) = GetItems(appUser, tableLazyLoadEvent);

      var statistics = new GeneralCreditsStatistics() {
        ActiveCredits = (int)totalCount
      };

      var settings = _userSettingsService.GetUserSettings(appUser);

      if (settings == null || settings.Accounts == null || !settings.Accounts.Any()) {
        return statistics;
      }

      var debtByCurrency = items
        .Where(creditItem => creditItem != null)
        .Where(creditItem => creditItem.IsActive && creditItem.Plan != null)
        .SelectMany(creditItem => creditItem.Plan.Select(payment => new {
          Payment = payment,
          IsPaid = payment.isPaid,
          Amount = payment.Quantity,
          Currency = settings.Accounts.Where(x => !string.IsNullOrEmpty(x.ID)).FirstOrDefault(x => x.ID == creditItem.AccountId)?.Currency ?? null
        }))
        .Where(paymentInfo => paymentInfo.Payment != null && !paymentInfo.IsPaid && !string.IsNullOrEmpty(paymentInfo.Currency))
        .GroupBy(paymentInfo => paymentInfo.Currency)
        .ToDictionary(
          group => group.Key,
          group => group.Sum(paymentInfo => paymentInfo.Amount)
        );

      if (debtByCurrency != null) {
        statistics.Debt = debtByCurrency.Where(pair => !string.IsNullOrEmpty(pair.Key)).ToDictionary(pair => pair.Key!, pair => pair.Value);
      }

      return statistics;
    }

    public IEnumerable<PaymentInfo>? GetTopPaymentsForActiveCredits(AppUser appUser, int top = 5) {
      if (top > _maxAllowedPaymens) { return null; }
      var tableLazyLoadEvent = new TableLazyLoadEvent() {
        First = 0,
        Rows = _maxAllowedCredits,
        Filters = new Dictionary<string, FilterMetadata[]> {
          { "IsActive", new FilterMetadata[] { new FilterMetadata() { Value = true, MatchMode = "Eq", OperatorMode = "And" } } }
        }
      };

      var (items, totalCount) = GetItems(appUser, tableLazyLoadEvent);

      if (items == null || totalCount == 0 || !items.Any()) {
        return null;
      }

      var paymentInfos = items
          .Where(creditItem => creditItem.IsActive && !string.IsNullOrEmpty(creditItem.AccountId) && creditItem.Plan != null)
          .SelectMany(creditItem =>
              creditItem.Plan?.Select(payment => new PaymentInfo {
                Payment = payment,
                AccountId = creditItem.AccountId,
                CreditId = creditItem.IdString,
                CreditTitle = creditItem.Comment
              }) ?? Enumerable.Empty<PaymentInfo>())
          .Where(paymentInfo => paymentInfo.Payment != null && !paymentInfo.Payment.isPaid)
          .OrderBy(x => x.Payment?.Date)
          .Take(top)
          .ToList();

      return paymentInfos; 
    }

    public IEnumerable<CreditItemView> GetViewByAccountId(AppUser appUser, string accountId) {
      var tableLazyLoadEvent = new TableLazyLoadEvent() {
        First = 0,
        Rows = _maxAllowedCredits,
        Filters = new Dictionary<string, FilterMetadata[]> {
          { "AccountId", new FilterMetadata[] { new FilterMetadata() { Value = accountId, MatchMode = "Eq", OperatorMode = "And" } } }
        }
      };

      var (items, totalCount) = GetItems(appUser, tableLazyLoadEvent);

      if (items == null || totalCount == 0 || !items.Any()) {
        return new List<CreditItemView>();
      }

      return items.Where(credit => string.Equals(credit.AccountId, accountId));
    }
  }
}
