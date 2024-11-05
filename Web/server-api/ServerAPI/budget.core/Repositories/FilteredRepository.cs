using MongoDB.Driver;
using budget.core.DB.Interfaces;
using budget.core.Repositories.Interfaces;
using budget.core.Models.Users;
using budget.core.Entities;

namespace budget.core.Repositories
{
  public class FilteringRepository<T> : IFilteringRepository<T> where T : BaseItem {

    private IMongoDatabase _database { get; set; }

    public FilteringRepository(IDBClient dbClient)
    {
      _database = dbClient.GetMongoDatabase();
    }

    public List<T> ExecuteFilter(AppUser appUser, FilterDefinition<T> filterDefinition, string collectionName) {
      var filter = Builders<T>.Filter.And(
          Builders<T>.Filter.Eq(x => x.OwnerUserId, appUser.Id),
          filterDefinition
      );
      var collection = _database.GetCollection<T>(collectionName);
      var result = collection.Find(filter).ToList();
      return result;
    }

    public List<T> ExecuteFilter(FilterDefinition<T> filterDefinition, string collectionName) {
      var filter = Builders<T>.Filter.And(
          filterDefinition
      );
      var collection = _database.GetCollection<T>(collectionName);
      var result = collection.Find(filter).ToList();
      return result;
    }

    public List<T> GetItemByMonth(AppUser appUser, string collectionName, int year, int month) {
      var firstDayOfMonth = new DateTime(year, month, 1);
      var lastDayOfMonth = new DateTime(year, month, 1).AddMonths(1).AddMilliseconds(-1);

      var filter = Builders<T>.Filter.And(
        Builders<T>.Filter.Eq(x => x.OwnerUserId, appUser.Id),
        Builders<T>.Filter.Gte(x => x.Date, firstDayOfMonth),
        Builders<T>.Filter.Lte(x => x.Date, lastDayOfMonth)
      );

      var collection = _database.GetCollection<T>(collectionName);
      var result = collection.Find(filter).ToList();
      return result;
    }
  }
}
