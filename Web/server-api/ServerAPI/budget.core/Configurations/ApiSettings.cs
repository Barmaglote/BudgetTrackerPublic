namespace budget.core.Configurations
{
  public class ApiSettings
  {
    public string[]? CorsOrigin { get; set; }
    public string? MongoDBConnection { get; set; }
    public string? PathToSSL { get; set; }
    public int MaxCredits { get; set; }
    public int MaxT01Credits { get; set; }
    public int MaxTransaction{ get; set; }
    public int MaxAccounts { get; set; }
    public int MaxT01Accounts { get; set; }
    public int MaxPlannings { get; set; }
    public int MaxCategories { get; set; }
    public int MaxT01Categories { get; set; }
    public int MaxTemplates { get; set; }
    public int MaxT01Templates { get; set; }
    public string? BudgetDBName { get; set; }
    public int LimitRecordsPerQuery { get; set; }
  }
}
