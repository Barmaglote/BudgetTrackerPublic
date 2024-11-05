using budget.core.Entities;

namespace budget.core.Factories.Interfaces
{
    public interface IFactory<T, TView> : IViewFactory<T, TView>, IItemFactory<T, TView>
    where T : BaseItem, ITransformable<TView>
    where TView : class, ITransformable<T> {
  }
}
