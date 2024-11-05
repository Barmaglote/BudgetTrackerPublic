using MongoDB.Driver;
using budget.core.DB.Interfaces;
using budget.core.Repositories.Interfaces;
using budget.core.Entities;

namespace budget.core.Repositories
{
  public class AggregateRepository<T> : IAggregateRepository<T> where T : BaseItem {

    private IMongoDatabase Database { get; set; }

    public AggregateRepository(IDBClient dbClient)
    {
      Database = dbClient.GetMongoDatabase();
    }

    public List<TOutput> ExecuteAggregate<TOutput>(PipelineDefinition<T, TOutput> pipeline, string collectionName) {
      var collection = Database.GetCollection<T>(collectionName);
      var result = collection.Aggregate(pipeline).ToList();
      return result;
    }
  }
}
