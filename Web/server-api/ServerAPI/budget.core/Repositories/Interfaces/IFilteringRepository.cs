using budget.core.Models.Users;
using MongoDB.Driver;

namespace budget.core.Repositories.Interfaces
{
  public interface IFilteringRepository<T> where T : class  
  {
    List<T> ExecuteFilter(AppUser appUser, FilterDefinition<T> filterDefinition, string collectionName);
    List<T> ExecuteFilter(FilterDefinition<T> filterDefinition, string collectionName);
    List<T> GetItemByMonth(AppUser appUser, string collectionName, int year, int month);
  }
}
