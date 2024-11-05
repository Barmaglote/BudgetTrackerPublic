using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using budget.core.Factories.Interfaces;
using budget.core.Models;

namespace budget.core.Entities {
  public class CreditItem : BaseItem, ITransformable<CreditItemView>, ICategorized {

    [BsonElement("category")]
    public string Category { get; set; } = "";

    [BsonElement("quantity")]
    public decimal Quantity { get; set; }

    [BsonElement("months")]
    public decimal Months { get; set; }

    [BsonElement("rate")]
    public decimal Rate { get; set; }

    [BsonElement("comment")]
    public string? Comment { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("mandatory")]
    public decimal Mandatory { get; set; } //Mandatory payment

    [BsonElement("isIncluded")]   
    public bool IsIncluded { get; set; } // Include mandatory payment in lumpSumPayment

    [BsonElement("plan")]
    public List<Payment>? Plan { get; set; } 

    public CreditItem() {
    }

    public CreditItem(CreditItemView view) {
      (Category, Quantity, Months, Rate, Date, Comment, Id, OwnerUserId, AccountId, IsActive, Mandatory, IsIncluded, Plan) = view;
    }

    public CreditItemView Transform() => new(this);

    public void Deconstruct(
      out string category,
      out decimal quantity,
      out decimal months,
      out decimal rate,
      out DateTime date,
      out string? comment,
      out ObjectId id,
      out string? ownerUserId,
      out string? accountId,
      out bool isActive,
      out decimal mandatory,
      out bool isIncluded,
      out List<Payment>? plan
    ) {
      category = Category;
      quantity = Quantity;
      months = Months;
      rate = Rate;
      date = Date;
      comment = Comment;
      id = Id;
      ownerUserId = OwnerUserId;
      accountId = AccountId;
      isActive = IsActive;
      mandatory = Mandatory;
      isIncluded = IsIncluded;
      plan = Plan;
    }
  }
}
