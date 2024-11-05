using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using budget.core.Attributes;

namespace budget.core.Entities {
  public abstract class BaseItem {
    [BsonId]
    [FixedFieldDuringUpdate]
    public ObjectId Id { get; set; }

    [FixedFieldDuringUpdate]
    public string? CreateUserId { get; set; }

    [FixedFieldDuringUpdate]
    public DateTime CreateDate { get; set; }

    public string? ModifyUserId { get; set; }

    public DateTime ModifyDate { get; set; }

    [FixedFieldDuringUpdate]
    public string? OwnerUserId { get; set; }

    [FixedFieldDuringUpdate]
    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("accountId")]
    public string? AccountId { get; set; }

    [BsonElement("transactionId")]
    public string? TransactionId { get; set; }
  }
}