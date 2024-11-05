using MongoDB.Driver;
using budget.core.DB.Interfaces;
using budget.core.Repositories.Interfaces;
using budget.core.Entities;
using budget.core.Models.Users;
using budget.core.Attributes;
using budget.core.Models.Interfaces;
using Serilog;

namespace budget.core.Repositories
{
  public class PostingRepository<T> : IPostingRepository<T> where T : BaseItem {
    private IMongoDatabase _database { get; set; }
    private IDataValidator _dataValidator { get; set; }

    public PostingRepository(IDBClient dbClient, IDataValidator dataValidator) {
      _database = dbClient.GetMongoDatabase();
      _dataValidator = dataValidator;
    }

    public bool AddItem(AppUser appUser, T item, string collectionName, IClientSessionHandle? session = null) {
      return AddItem(appUser.Id, item, collectionName, session);
    }

    public bool AddItem(string userId, T item, string collectionName, IClientSessionHandle? session = null) {

      var isValid = _dataValidator.IsValidToAdd(userId, collectionName, item);
      if (!isValid) { return false; }

      var collection = _database.GetCollection<T>(collectionName);
      item.CreateDate = DateTime.Now;
      item.CreateUserId = userId;
      if (session == null) {
        collection.InsertOne(item);
      } else {
        collection.InsertOne(session, item);
      }

      return true;
    }

    public bool DeleteItem(AppUser appUser, T item, string collectionName, IClientSessionHandle? session = null) {
      Log.Debug("Postin Repository, DeleteItem", appUser, item, collectionName, session);
      var collection = _database.GetCollection<T>(collectionName);
      var filter = Builders<T>.Filter.Eq("Id", item.Id);
      return session == null ? collection.DeleteOne(filter).DeletedCount > 0 : collection.DeleteOne(session, filter).DeletedCount > 0;
    }

    public bool UpdateItem(AppUser appUser, T item, string collectionName, IClientSessionHandle? session = null) {
      return UpdateItem(appUser.Id, item, collectionName, session = null);
    }

    public bool UpdateItem(string userId, T item, string collectionName, IClientSessionHandle? session = null) {

      var isValid = _dataValidator.IsValidToUpdate(userId, collectionName, item);
      if (!isValid) { return false; }

      var filter = Builders<T>.Filter.Eq("_id", item.Id);
      var list = new List<UpdateDefinition<T>>();

      foreach (var t in item.GetType().GetProperties()) {
        if (Attribute.IsDefined(t, typeof(FixedFieldDuringUpdateAttribute))) continue;

        list.Add(Builders<T>.Update.Set(t.Name, t.GetValue(item)));
      }

      list.Add(Builders<T>.Update.Set("ModifyDate", DateTime.Now));
      list.Add(Builders<T>.Update.Set("ModifyUserId", userId));

      var updateDefinition = Builders<T>.Update.Combine(list);
      var collection = _database.GetCollection<T>(collectionName);
      return session == null ? collection.UpdateOne(filter, updateDefinition).ModifiedCount > 0 : collection.UpdateOne(session, filter, updateDefinition).ModifiedCount > 0;
    }

    public bool DeleteItemByAccountId(string accountId, string collectionName, IClientSessionHandle? session = null) {
      var collection = _database.GetCollection<T>(collectionName);
      var filter = Builders<T>.Filter.Eq("accountId", accountId);
      return session == null ? collection.DeleteMany(filter).DeletedCount > 0 : collection.DeleteMany(session, filter).DeletedCount > 0;
    }
  }
}
