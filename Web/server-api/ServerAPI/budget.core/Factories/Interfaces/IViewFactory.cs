namespace budget.core.Factories.Interfaces
{
    public interface IViewFactory<T, TView> 
    where TView : class, ITransformable<T>
    where T : class {
    TView CreateView(T entity);
  }
}
