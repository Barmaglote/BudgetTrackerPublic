using MongoDB.Driver;

namespace budget.core.DB.Interfaces {
  public interface IDBClient {
    IMongoDatabase GetMongoDatabase();
  }
}
