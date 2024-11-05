namespace budget.core.Services.Interfaces
{
  public interface IGrouppingService<TKey, TEntity>
  {
    IDictionary<TKey, TResultType> GetGrouped<TResultType>(IEnumerable<TEntity> items, string propertyName, Func<IEnumerable<TEntity>, TResultType> aggrigateFunction);
  }
}
