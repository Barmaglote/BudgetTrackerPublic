using budget.core.Entities;
using budget.core.Factories.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace budget.core.Models {
  public class CreditItemView : ITransformable<CreditItem> {
    public ObjectId Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdString {
      get { return Convert.ToString(Id); }
      set { Id = ObjectId.Parse(value); }
    }

    public string Category { get; set; }
    public decimal Quantity { get; set; }
    public decimal Months { get; set; }
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }
    public string? Comment { get; set; }
    public string? OwnerUserId { get; set; }
    public string? AccountId { get; set; }
    public bool IsActive { get; set; }
    public string? TransactionId { get; set; }
    public decimal Mandatory { get; set; }
    public bool IsIncluded { get; set; }
    public List<Payment>? Plan { get; set; }

    public CreditItemView(CreditItem item) {
      (Category, Quantity, Months, Rate, Date, Comment, Id, OwnerUserId, AccountId, IsActive, Mandatory, IsIncluded, Plan) = item;
    }

    [JsonConstructor]
    public CreditItemView(
      string category, 
      decimal quantity, 
      decimal months, 
      decimal rate, 
      DateTime date, 
      string? comment, 
      string? ownerUserId, 
      string? accountId,   
      bool isActive,
      decimal mandatory,
      bool isIncluded,
      List<Payment> plan
      ) {
      Category = category;
      Quantity = quantity;
      Months = months;
      Rate = rate;
      Date = date;
      Comment = comment;
      OwnerUserId = ownerUserId;
      AccountId = accountId;
      IsActive = isActive;
      Mandatory = mandatory;
      IsIncluded = isIncluded;
      Plan = plan;
    }

    public CreditItem Transform() => new(this);

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
