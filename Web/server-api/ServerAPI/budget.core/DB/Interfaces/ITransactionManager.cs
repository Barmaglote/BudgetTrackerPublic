using MongoDB.Driver;

namespace budget.core.DB.Interfaces {
  public interface ITransactionManager {
    void StartTransaction();
    Task CommitTransactionAsync();
    Task AbortTransactionAsync();
    IClientSessionHandle GetSession();
  }
}
