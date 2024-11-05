using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using budget.core.Models;
using budget.core.Factories.Interfaces;

namespace budget.core.Entities {
  public class Item : BaseItem, ITransformable<ItemView>, ICategorized {

    [BsonElement("category")]
    public string Category { get; set; } = "";

    [BsonElement("quantity")]
    public decimal Quantity { get; set; }

    [BsonElement("comment")]
    public string? Comment { get; set; }

    [BsonElement("isRegular")]
    public bool IsRegular { get; set; }

    public Item() {
    }

    public Item(ItemView view) {
      (Category, Quantity, Date, Comment, IsRegular, Id, OwnerUserId, AccountId, TransactionId) = view;
    }

    public ItemView Transform() => new(this);

    public void Deconstruct(
      out string category,
      out decimal quantity,
      out DateTime date,
      out string? comment,
      out bool isRegular,
      out ObjectId id,
      out string? ownerUserId,
      out string? accountId,
      out string? transactionId
    ) {
      category = Category;
      quantity = Quantity;
      date = Date;
      comment = Comment;
      isRegular = IsRegular;
      id = Id;
      ownerUserId = OwnerUserId;
      accountId = AccountId;
      transactionId = TransactionId;
    }
  }
}
