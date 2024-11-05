using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services;
using budget.core.Services.Interfaces;
using MongoDB.Driver;
using Moq;

namespace budget.test.core {
  public class AccountsServiceTest {
    private Mock<IEnumerableService<UserSettings, UserSettingsView>> EnumerableServiceMock { get; } = new();
    private Mock<IDeletingService<UserSettings, UserSettingsView>> DeletingServiceMock { get; } = new();
    private Mock<IPostingService<UserSettings, UserSettingsView>> PostingServiceMock { get; } = new();
    private Mock<IFilteringService<UserSettings, UserSettingsView>> FilteringServiceMock { get; } = new();

    [Fact]
    public void ChangeQuantity_ValidInput_ReturnsTrue() {
      // Arrange      
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accountID = "123";
      var difference = 10.0m;
      var accounts = new List<AccountItem> { 
        new AccountItem { ID = accountID, Quantity = 100.0m, Title = "My Salary Card", AccountType = AccountType.DebitCard, Currency = "rub" } 
      };
      var userSettings = new UserSettings { Accounts = accounts };

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { new UserSettingsView(userSettings) };
      });

      var service = new AccountsService(userSettingsServiceMock);

      // Act
      var result = service.ChangeQuantity(appUser, accountID, difference);

      // Assert
      Assert.True(result);
      Assert.Equal(110.0m, accounts[0].Quantity);
    }

    [Fact]
    public void ChangeQuantity2_ValidInput_ReturnsTrue() {
      // Arrange      
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accountID1 = "123";
      var accountID2 = "321";
      var difference1 = 10.0m;
      var difference2 = 20.0m;
      var accounts = new List<AccountItem> {
        new AccountItem { ID = accountID1, Quantity = 100.0m, Title = "My Salary Card", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = accountID2, Quantity = 100.0m, Title = "My Salary Card", AccountType = AccountType.DebitCard, Currency = "rub" }
      };
      var userSettings = new UserSettings { Accounts = accounts };

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { new UserSettingsView(userSettings) };
      });

      var service = new AccountsService(userSettingsServiceMock);

      // Act
      var result = service.ChangeQuantity(appUser, accountID1, -difference1, accountID2, difference2);

      // Assert
      Assert.True(result);
      Assert.Equal(90.0m, accounts[0].Quantity);
      Assert.Equal(120.0m, accounts[1].Quantity);
    }

    [Fact]
    public void GetItemById_ValidInput_ReturnAccount() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accountID = "123";

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);      
      var service = new AccountsService(userSettingsServiceMock);

      var accounts = new List<AccountItem> {
        new AccountItem { ID = "123", Quantity = 100.0m, Title = "Card 1", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = "456", Quantity = 100.0m, Title = "Card 2", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = "789", Quantity = 100.0m, Title = "Card 3", AccountType = AccountType.DebitCard, Currency = "rub" }
      };

      var userSettings = new UserSettings { Accounts = accounts };

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { new UserSettingsView(userSettings) };
      });

      // Act
      var result = service.GetItemById(appUser, accountID);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(accountID, result.ID);
    }

    [Fact]
    public void GetItemById_InvalidInput_ReturnNull() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accountID = "753";

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var service = new AccountsService(userSettingsServiceMock);

      var accounts = new List<AccountItem> {
        new AccountItem { ID = "123", Quantity = 100.0m, Title = "Card 1", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = "456", Quantity = 100.0m, Title = "Card 2", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = "789", Quantity = 100.0m, Title = "Card 3", AccountType = AccountType.DebitCard, Currency = "rub" }
      };

      var userSettings = new UserSettings { Accounts = accounts };

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { new UserSettingsView(userSettings) };
      });

      // Act
      var result = service.GetItemById(appUser, accountID);

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public void GetItemById_NotValidUser_ReturnNull() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accountID = "753";

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var service = new AccountsService(userSettingsServiceMock);

      var accounts = new List<AccountItem> {
        new AccountItem { ID = "123", Quantity = 100.0m, Title = "Card 1", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = "456", Quantity = 100.0m, Title = "Card 2", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = "789", Quantity = 100.0m, Title = "Card 3", AccountType = AccountType.DebitCard, Currency = "rub" }
      };

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return null;
      });

      // Act
      var result = service.GetItemById(appUser, accountID);

      // Assert
      Assert.Null(result);
    }
  }
}