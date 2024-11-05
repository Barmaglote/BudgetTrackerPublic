namespace budget.core.Models {
  public class UserSubscribtionMessage {
    public required string PlanId { get; set; }
    public required string Status { get; set; }
    public required string Email { get; set; }
    public required string Provider { get; set; }
    public required string UserId { get; set; }
    public DateTime Date { get; set; }
    public required string SubscribtionId { get; set; }
  }
}
