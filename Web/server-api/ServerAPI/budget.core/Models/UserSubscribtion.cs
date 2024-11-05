namespace budget.core.Models {
  public class UserSubscribtion {
    public string PlanId { get; set; }
    public bool IsActive { get; set; }
    public DateTime Date { get; set; }
    public string SubscribtionId { get; set; }

    public UserSubscribtion(UserSubscribtionMessage userSubscriptionMessage){
      PlanId = userSubscriptionMessage.PlanId;
      IsActive = userSubscriptionMessage.Status?.Equals("subscribed") ?? false;
      Date = userSubscriptionMessage.Date;
      SubscribtionId = userSubscriptionMessage.SubscribtionId;
    }
  }
}
