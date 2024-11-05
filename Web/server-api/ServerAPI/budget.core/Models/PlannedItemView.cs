using budget.core.Entities;
using budget.core.Factories.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace budget.core.Models {
  public class PlannedItemView : ITransformable<PlannedItem> {
    public ObjectId Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdString {
      get { return Convert.ToString(Id); }
      set { Id = ObjectId.Parse(value); }
    }

    public decimal Quantity { get; set; }
    public DateTime Date { get; set; }
    public string? Comment { get; set; }
    public string? OwnerUserId { get; set; }
    public string? AccountId { get; set; }
    public bool IsPaid { get; set; }
    public string? Currency { get; set; }
    public string? Area { get; set; }
    public string? TransactionId { get; set; }

    public PlannedItemView(PlannedItem item) {
      (Quantity, Date, Comment, Id, OwnerUserId, AccountId, IsPaid, Currency, Area) = item;
    }

    [JsonConstructor]
    public PlannedItemView(
      decimal quantity, 
      DateTime date, 
      string? comment, 
      string? ownerUserId, 
      string? accountId,   
      bool isPaid,
      string currency,
      string area
      ) {
      Quantity = quantity;
      Date = date;
      Comment = comment;
      OwnerUserId = ownerUserId;
      AccountId = accountId;
      IsPaid = isPaid;
      Currency = currency;
      Area = area;
    }

    public PlannedItem Transform() => new(this);

    public void Deconstruct(
      out decimal quantity,
      out DateTime date,
      out string? comment,
      out ObjectId id,
      out string? ownerUserId,
      out string? accountId,
      out bool isPaid,
      out string? currency,
      out string? area
    ) {
      quantity = Quantity;
      date = Date;
      comment = Comment;
      id = Id;
      ownerUserId = OwnerUserId;
      accountId = AccountId;
      isPaid = IsPaid;
      currency = Currency;
      area = Area;
    }
  }
}
