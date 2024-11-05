using budget.core.DB.Interfaces;
using MongoDB.Driver;

namespace budget.core.DB {
  public class MongoTransactionManager : IDisposable, ITransactionManager {
    private readonly IClientSessionHandle _session;

    public MongoTransactionManager(IDBClient dbClient) {
      _session = dbClient.GetMongoDatabase().Client.StartSession();
    }

    public void StartTransaction() {
      _session.StartTransaction();
    }

    public async Task CommitTransactionAsync() {
      await _session.CommitTransactionAsync();
    }

    public async Task AbortTransactionAsync() {
      await _session.AbortTransactionAsync();
    }

    public IClientSessionHandle GetSession() {
      return _session;
    }

    public void Dispose() {
      _session.Dispose();
    }
  }
}
