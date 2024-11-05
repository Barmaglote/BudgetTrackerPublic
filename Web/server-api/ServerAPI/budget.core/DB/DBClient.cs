using MongoDB.Driver;
using budget.core.DB.Interfaces;
using Microsoft.Extensions.Options;
using budget.core.Configurations;

namespace budget.core.DB {
  public class DBClient: IDBClient {

    private readonly ApiSettings _appSettings;
    private MongoClient _client { get; set; }

    public DBClient(IOptions<ApiSettings> appSettings)
    {
      _appSettings = appSettings.Value;
      _client = new MongoClient(_appSettings.MongoDBConnection);
    }

    public IMongoDatabase GetMongoDatabase() {
      return _client.GetDatabase(_appSettings.BudgetDBName);
    }
  }
}
