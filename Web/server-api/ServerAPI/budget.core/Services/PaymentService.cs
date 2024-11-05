using budget.core.Entities;
using budget.core.Models;
using budget.core.Services.Interfaces;

namespace budget.core.Services
{
  public class PaymentService : EntityService<PaymentIntentResult, PaymentIntentResultView> {
    protected override string _collectionName{ get; set; } = "payment";

    public PaymentService(
      IEnumerableService<PaymentIntentResult, PaymentIntentResultView> enumerableService,
      IPostingService<PaymentIntentResult, PaymentIntentResultView> postingService,
      IDeletingService<PaymentIntentResult, PaymentIntentResultView> deletingService,
      IFilteringService<PaymentIntentResult, PaymentIntentResultView> filteringService
      ) : base(enumerableService, postingService, deletingService, filteringService) {
    }
  }
}
