using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using Stripe;

namespace budget.core.Entities {
  public class PaymentIntentResult : BaseItem, ITransformable<PaymentIntentResultView> {

    [BsonElement("paymentIntent")]
    public PaymentIntent? PaymentIntent { get; set; }

    public PaymentIntentResult() {
    }

    public PaymentIntentResult(PaymentIntentResultView view) {
      (Id, PaymentIntent) = view;
    }

    public PaymentIntentResultView Transform() => new(this);

    public void Deconstruct(
      out ObjectId id,
      out PaymentIntent? paymentIntent
    ) {
      id = Id;
      paymentIntent = PaymentIntent;
    }
  }
}
