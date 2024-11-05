using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services;
using budget.core.Services.Interfaces;
using Moq;

namespace budget.test.core {
  public class IncomeServiceTest {
    private Mock<IEnumerableService<Item, ItemView>> EnumerableServiceMock { get; } = new Mock<IEnumerableService<Item, ItemView>>();
    private Mock<IPostingService<Item, ItemView>> PostingServiceMock { get; } = new Mock<IPostingService<Item, ItemView>>();
    private Mock<IDeletingService<Item, ItemView>> DeletingServiceMock { get; } = new Mock<IDeletingService<Item, ItemView>>();
    private Mock<IFilteringService<Item, ItemView>> FilteringServiceMock { get; } = new Mock<IFilteringService<Item, ItemView>>();

    [Fact]
    public void GetLastItem() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var entityService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns<AppUser, string, TableLazyLoadEvent>((appUser, collectionName, tableLazyLoadEvent) => {
        return new(new List<ItemView>() {
          new ItemView(new Item() { Quantity = 10, IsRegular = true, Category = "salary" }),
        }, 1);
      });

      // Action
      var result = entityService.GetLastItem(appUser);

      // Assert
      Assert.NotNull(result);
      Assert.Equal("salary", result.Category);
    }

    [Fact]
    public void GetRegularPayments() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var entityService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns<AppUser, string, TableLazyLoadEvent>((appUser, collectionName, tableLazyLoadEvent) => {
        return new(new List<ItemView>() {
          new ItemView(new Item() { Quantity = 10, IsRegular = true, Category = "salary", Date = new DateTime(2020, 01, 01) }),
          new ItemView(new Item() { Quantity = 10, IsRegular = true, Category = "gift", Date = new DateTime(2020, 01, 01) }),
          new ItemView(new Item() { Quantity = 10, IsRegular = true, Category = "gift", Date = new DateTime(2020, 01, 01) }),
          new ItemView(new Item() { Quantity = 10, IsRegular = true, Category = "kids", Date = new DateTime(2020, 01, 01) }),
        }, 3);
      });

      // Action
      var result = entityService.GetRegularPayments(appUser, 2020);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(4, result.Count());
    }
  }
}