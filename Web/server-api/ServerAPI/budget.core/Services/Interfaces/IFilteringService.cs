using budget.core.Models.Users;
using MongoDB.Driver;

namespace budget.core.Services.Interfaces {
  public interface IFilteringService<T, TView>
      where T : class
      where TView : class
  {
    IEnumerable<TView> GetItemViews(AppUser appUser, FilterDefinition<T> filterDefinition, string collectionName);
    IEnumerable<TView> GetItemViews(FilterDefinition<T> filterDefinition, string collectionName);
    IEnumerable<TView> GetItemByMonth(AppUser appUser, string collectionName, int year, int month);
  }
}
