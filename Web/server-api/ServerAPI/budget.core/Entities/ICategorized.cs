namespace budget.core.Entities
{
  public interface ICategorized
  {
    public string Category { get; set; }
    public decimal Quantity { get; set; }
  }
}