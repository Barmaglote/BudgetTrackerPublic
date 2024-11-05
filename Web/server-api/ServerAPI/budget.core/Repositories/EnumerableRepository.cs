using MongoDB.Driver;
using budget.core.DB.Interfaces;
using budget.core.Repositories.Interfaces;
using budget.core.Models.Users;
using budget.core.Entities;
using MongoDB.Bson;
using budget.core.Configurations;
using Microsoft.Extensions.Options;

namespace budget.core.Repositories
{
  public class EnumerableRepository<T> : IEnumerableRepository<T> where T : BaseItem  {

    private IMongoDatabase _database { get; set; }
    private readonly ApiSettings _appSettings;

    public EnumerableRepository(IDBClient dbClient, IOptions<ApiSettings> appSettings)
    {
      _database = dbClient.GetMongoDatabase();
      _appSettings = appSettings.Value;
    }

    public (IAsyncCursor<T> Items, long TotalCount) GetItems(AppUser appUser, string collectionName, FilterDefinition<T>? filterDefinition) {
      var collection = _database.GetCollection<T>(collectionName);

      var filter = Builders<T>.Filter.Where(item => item.OwnerUserId == appUser.Id);

      if (filterDefinition != null) {
        filter &= filterDefinition;
      }

      var totalCount = collection.CountDocuments(filter);
      var sortDefinition = Builders<T>.Sort.Descending("Date");

      var items = collection.Find(filter)
                  .Sort(sortDefinition)
                  .Limit(_appSettings.LimitRecordsPerQuery)
                  .ToCursor();
      return (items, totalCount);
    }

    public T? GetItem(ObjectId objectId, string collectionName) {
      var collection = _database.GetCollection<T>(collectionName);
      var filter = Builders<T>.Filter.Eq(p => p.Id, objectId);
      var item = collection.Find(filter).Limit(_appSettings.LimitRecordsPerQuery);
      if (item == null) {
        return null;
      }

      return item.FirstOrDefault();
    }
  }
}
