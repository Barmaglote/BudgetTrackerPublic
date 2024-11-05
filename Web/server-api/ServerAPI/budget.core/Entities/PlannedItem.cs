using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using budget.core.Factories.Interfaces;
using budget.core.Models;

namespace budget.core.Entities {
  public class PlannedItem : BaseItem, ITransformable<PlannedItemView> {

    [BsonElement("quantity")]
    public decimal Quantity { get; set; }

    [BsonElement("comment")]
    public string? Comment { get; set; }

    [BsonElement("isPaid")]
    public bool IsPaid { get; set; }

    [BsonElement("currency")]
    public string? Currency { get; set; }
    [BsonElement("area")]
    public string? Area { get; set; }

    public PlannedItem() {
    }

    public PlannedItem(PlannedItemView view) {
      (Quantity, Date, Comment, Id, OwnerUserId, AccountId, IsPaid, Currency, Area) = view;
    }

    public PlannedItemView Transform() => new(this);

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
