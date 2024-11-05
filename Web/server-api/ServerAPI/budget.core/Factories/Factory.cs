using budget.core.Entities;
using budget.core.Factories.Interfaces;

namespace budget.core.Factories
{
    public class Factory<T, TView> : IFactory<T, TView>
    where T : BaseItem, ITransformable<TView>
    where TView : class, ITransformable<T> {
    public T CreateItem(TView view) {
      return view.Transform();
    }

    public TView CreateView(T entity) {
      return entity.Transform();
    }
  }
}
