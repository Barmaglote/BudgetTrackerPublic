namespace budget.core.Models {
  public class GeneralCreditsStatistics {
    public int ActiveCredits { get; set; }
    public Dictionary<string, decimal>? Debt { get; set; }
  }
}
