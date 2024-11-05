namespace budget.core.Factories.Interfaces 
{
    public interface IItemFactory<T, TView>
    where T : class, ITransformable<TView>
    where TView : class {
    T CreateItem(TView entity);
  }
}
