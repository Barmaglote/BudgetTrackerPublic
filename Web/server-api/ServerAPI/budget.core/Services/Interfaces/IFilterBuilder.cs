using budget.core.Models;
using MongoDB.Driver;

namespace budget.core.Services.Interfaces {
  public interface IFilterBuilder<T> {
    FilterDefinition<T>? BuildFilerDefinition(TableLazyLoadEvent tableLazyLoadEvent);
  }
}
