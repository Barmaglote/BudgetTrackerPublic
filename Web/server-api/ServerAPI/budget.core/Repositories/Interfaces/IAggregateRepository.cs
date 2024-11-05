using budget.core.Entities;
using MongoDB.Driver;

namespace budget.core.Repositories.Interfaces
{
  public interface IAggregateRepository<T> where T : BaseItem  {
    List<TOutput> ExecuteAggregate<TOutput>(PipelineDefinition<T, TOutput> pipeline, string collectionName);
  }
}
