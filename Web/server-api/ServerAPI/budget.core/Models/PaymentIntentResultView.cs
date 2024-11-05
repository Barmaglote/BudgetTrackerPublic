using budget.core.Entities;
using budget.core.Factories.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Stripe;
using System.Text.Json.Serialization;

namespace budget.core.Models {
  public class PaymentIntentResultView : ITransformable<PaymentIntentResult> {

    public ObjectId Id { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdString {
      get { return Convert.ToString(Id); }
      set { Id = ObjectId.Parse(value); }
    }

    public PaymentIntent? PaymentIntent { get; set; }

    public PaymentIntentResultView(PaymentIntentResult item) {
      (Id, PaymentIntent) = item;     
    }

    [JsonConstructor]
    public PaymentIntentResultView(ObjectId id, PaymentIntent paymentIntent) {
      Id = id;
      PaymentIntent = paymentIntent;
    }

    public PaymentIntentResult Transform() => new(this);

    public void Deconstruct(
      out ObjectId id,
      out PaymentIntent? paymentIntent
    ) {
      id = Id;
      paymentIntent = PaymentIntent;
    }
  }

}