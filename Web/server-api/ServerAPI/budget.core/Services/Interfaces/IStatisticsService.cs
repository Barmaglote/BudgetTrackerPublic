using budget.core.Models;
using budget.core.Models.Users;

namespace budget.core.Services.Interfaces {
  public interface IStatisticsService<T, TView>
    where T: class
    where TView : class
  {
    IEnumerable<StatsByCategoryView> GetStatsByCategory(AppUser appUser, string collectionName, TableLazyLoadEvent tableLazyLoadEvent);
    IEnumerable<StatsByDateView> GetStatsByDate(AppUser appUser, string collectionName, TableLazyLoadEvent tableLazyLoadEvent);
  }
}
