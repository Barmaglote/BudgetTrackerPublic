using budget.core.Entities;
using budget.core.Factories.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace budget.core.Models {
  public class ItemView : ITransformable<Item> {
    public ObjectId Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdString {
      get { return Convert.ToString(Id); }
      set { Id = ObjectId.Parse(value); }
    }

    public string Category { get; set; }
    public decimal Quantity { get; set; }
    public DateTime Date { get; set; }
    public string? Comment { get; set; }
    public bool IsRegular { get; set; }
    public string? OwnerUserId { get; set; }
    public string? AccountId { get; set; }
    public string? TransactionId { get; set; }

    public ItemView(Item item) {
      (Category, Quantity, Date, Comment, IsRegular, Id, OwnerUserId, AccountId, TransactionId) = item;
    }

    [JsonConstructor]
    public ItemView(string category, decimal quantity, DateTime date, bool isRegular, string? comment, string? ownerUserId, string? accountId, string? transactionId) {
      Category = category;
      Quantity = quantity;
      Date = date;
      Comment = comment;
      IsRegular = isRegular;
      OwnerUserId = ownerUserId;
      AccountId = accountId;
      TransactionId = transactionId;
    }

    public Item Transform() => new(this);

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
