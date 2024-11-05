using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Repositories.Interfaces;
using budget.core.Services;
using MongoDB.Driver;
using Moq;

namespace budget.test.core {
  public class StatisticsServiceTest {
    private Mock<IEnumerableRepository<Item>> IEnumerableRepositoryMock { get; } = new Mock<IEnumerableRepository<Item>>();

    [Fact]
    public void GetStatsByCategory_ValidInput_ReturnStatistics() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var statisticsService = new StatisticsService<Item, ItemView>(IEnumerableRepositoryMock.Object, new FilterBuilder<Item>());

      var items = new List<Item>() {
        new Item() { AccountId = "1", Category = "salary", Quantity = 10 },
        new Item() { AccountId = "1", Category = "salary", Quantity = 25 },
        new Item() { AccountId = "2", Category = "salary", Quantity = 10 },
        new Item() { AccountId = "1", Category = "gift", Quantity = 25 },
        new Item() { AccountId = "2", Category = "gift", Quantity = 35 },
        new Item() { AccountId = "2", Category = "gift", Quantity = 45 },
        new Item() { AccountId = "3", Category = "salary", Quantity = 40 }
      };

      var itemsAsyncCursorMock = new ListAsyncCursor<Item>(items);

      IEnumerableRepositoryMock
        .Setup(x => x.GetItems(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<FilterDefinition<Item>>()))
        .Returns(() => new(itemsAsyncCursorMock, 10));

      // Act
      var result = statisticsService.GetStatsByCategory(appUser, "collectionName", new TableLazyLoadEvent()).ToList();

      // Assert
      Assert.NotNull(result);
      Assert.Equal(5, result.Count());
      Assert.Equal(35, result.Where(x => x.AccountId == "1" && x.Category == "salary").Sum(x => x.Quantity));
      Assert.Equal(10, result.Where(x => x.AccountId == "2" && x.Category == "salary").Sum(x => x.Quantity));
      Assert.Equal(40, result.Where(x => x.AccountId == "3" && x.Category == "salary").Sum(x => x.Quantity));
      Assert.Equal(80, result.Where(x => x.AccountId == "2" && x.Category == "gift").Sum(x => x.Quantity));
    }

    [Fact]
    public void GetStatsByDate_ValidInput_ReturnStatistics() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var statisticsService = new StatisticsService<Item, ItemView>(IEnumerableRepositoryMock.Object, new FilterBuilder<Item>());

      var items = new List<Item>() {
        new Item() { AccountId = "1", Category = "salary", Quantity = 10, Date = new DateTime(2020, 01, 01) },
        new Item() { AccountId = "1", Category = "salary", Quantity = 25, Date = new DateTime(2020, 01, 01) },
        new Item() { AccountId = "1", Category = "salary", Quantity = 15, Date = new DateTime(2020, 02, 01) },
        new Item() { AccountId = "2", Category = "salary", Quantity = 10, Date = new DateTime(2020, 02, 01) },
        new Item() { AccountId = "1", Category = "gift", Quantity = 25, Date = new DateTime(2020, 03, 01) },
        new Item() { AccountId = "2", Category = "gift", Quantity = 35, Date = new DateTime(2020, 04, 01) },
        new Item() { AccountId = "2", Category = "gift", Quantity = 45, Date = new DateTime(2020, 04, 01) },
        new Item() { AccountId = "3", Category = "salary", Quantity = 40, Date = new DateTime(2020, 05, 01) },
        new Item() { AccountId = "3", Category = "salary", Quantity = 10, Date = new DateTime(2020, 05, 01) }
      };

      var itemsAsyncCursorMock = new ListAsyncCursor<Item>(items);

      IEnumerableRepositoryMock
        .Setup(x => x.GetItems(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<FilterDefinition<Item>>()))
        .Returns(() => new(itemsAsyncCursorMock, 10));

      // Act
      var result = statisticsService.GetStatsByDate(appUser, "collectionName", new TableLazyLoadEvent()).ToList();

      // Assert
      Assert.NotNull(result);
      Assert.Equal(6, result.Count());
      Assert.Equal(35, result.Where(x => x.AccountId == "1" && x.Category == "salary" && x.Date == new DateTime(2020, 01, 01)).Sum(x => x.Quantity));
      Assert.Equal(15, result.Where(x => x.AccountId == "1" && x.Category == "salary" && x.Date == new DateTime(2020, 02, 01)).Sum(x => x.Quantity));
      Assert.Equal(10, result.Where(x => x.AccountId == "2" && x.Category == "salary" && x.Date == new DateTime(2020, 02, 01)).Sum(x => x.Quantity));
      Assert.Equal(0, result.Where(x => x.AccountId == "2" && x.Category == "salary" && x.Date == new DateTime(2020, 01, 01)).Sum(x => x.Quantity));
      Assert.Equal(0, result.Where(x => x.AccountId == "3" && x.Category == "salary" && x.Date == new DateTime(2020, 01, 01)).Sum(x => x.Quantity));
      Assert.Equal(50, result.Where(x => x.AccountId == "3" && x.Category == "salary" && x.Date == new DateTime(2020, 05, 01)).Sum(x => x.Quantity));
    }
  }
}