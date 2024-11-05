using budget.core.Services.Interfaces;
using budget.core.Services;
using Moq;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Entities;
using MongoDB.Driver;
using budget.core.DB.Interfaces;

namespace budget.test.core {
  public class TransferServiceTest {   
    private Mock<IAccountsService> IAccountsServiceMock { get; } = new Mock<IAccountsService>();
    private Mock<IEnumerableService<Item, ItemView>> EnumerableServiceMock { get; } = new();
    private Mock<IPostingService<Item, ItemView>> PostingServiceMock { get; } = new();
    private Mock<IDeletingService<Item, ItemView>> DeletingServiceMock { get; } = new();
    private Mock<IFilteringService<Item, ItemView>> FilteringServiceMock { get; } = new();
    private Mock<IEnumerableService<UserSettings, UserSettingsView>> EnumerableUserServiceMock { get; } = new();
    private Mock<IDeletingService<UserSettings, UserSettingsView>> DeletingUserServiceMock { get; } = new();
    private Mock<IPostingService<UserSettings, UserSettingsView>> PostingUserServiceMock { get; } = new();
    private Mock<IFilteringService<UserSettings, UserSettingsView>> FilteringUserServiceMock { get; } = new();

    [Fact]
    public void AddTransfer_ValidInput_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var transferItem = new TransferItem() { 
        FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" }, 
        ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc02", ID = "2" },
        ToQuantity = 100,
        FromQuantity = 100
      };
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var isAdded = true;

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);
      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      IAccountsServiceMock.Setup(x => x.ChangeQuantity(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<ITransactionManager>())).Returns(true);

      // Act
      var result = transferService.AddTransfer(appUser, transferItem);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void AddTransfer_InvalidInput1_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var transferItem = new TransferItem() {
        FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc02", ID = "2" },
        ToQuantity = 50,
        FromQuantity = 100
      };
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var isAdded = true;

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);
      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      // Act
      var result = transferService.AddTransfer(appUser, transferItem);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void AddTransfer_InvalidInput2_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var transferItem = new TransferItem() {
        FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        ToQuantity = 100,
        FromQuantity = 100
      };
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var isAdded = true;

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);
      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      // Act
      var result = transferService.AddTransfer(appUser, transferItem);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void GetItemById_ValidInput_ReturnTransferItem() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accounts = new List<AccountItem>() {
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc02", ID = "2" },
      };

      var items = new List<Item>() {
        new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 },
      };

      var itemViews = new List<ItemView>() {
        new ItemView(new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 }),
      };

      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var id = "10";

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<Item>>(), It.IsAny<string>())).Returns(itemViews);
      IAccountsServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>())).Returns(new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" });

      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      // Act
      var result = transferService.GetItemById(appUser, id);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(50, result.FromQuantity);
      Assert.Equal(50, result.ToQuantity);
      Assert.Equal("1", result.FromAccount.ID);
      Assert.Equal("1", result.ToAccount.ID);
    }

    [Fact]
    public void GetItemById_InvalidInput_ReturnNull() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accounts = new List<AccountItem>() {
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc02", ID = "2" },
      };

      var items = new List<Item>() {
        new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 },
      };

      var itemViews = new List<ItemView>() {
        new ItemView(new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 }),
      };

      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var id = "20";

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<Item>>(), It.IsAny<string>())).Returns(new List<ItemView>());
      IAccountsServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>())).Returns(new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" });

      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      // Act
      var result = transferService.GetItemById(appUser, id);

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public void DeleteItemById_ValidInput_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accounts = new List<AccountItem>() {
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc02", ID = "2" },
      };

      var items = new List<Item>() {
        new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 },
      };

      var itemViews = new List<ItemView>() {
        new ItemView(new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 }),
      };

      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var id = "10";

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<Item>>(), It.IsAny<string>())).Returns(itemViews);
      IAccountsServiceMock.Setup(x => x.ChangeQuantity(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<ITransactionManager>())).Returns(true);
      IAccountsServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>())).Returns(new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" });
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      IAccountsServiceMock.Setup(x => x.ChangeQuantity(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<ITransactionManager>())).Returns(true);

      // Act
      var result = transferService.DeleteItemById(appUser, id);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void DeleteItemById_InvalidInput_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accounts = new List<AccountItem>() {
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc02", ID = "2" },
      };

      var items = new List<Item>() {
        new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 },
      };

      var itemViews = new List<ItemView>() {
        new ItemView(new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 }),
      };

      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var id = "10";

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<Item>>(), It.IsAny<string>())).Returns(new List<ItemView>());
      IAccountsServiceMock.Setup(x => x.ChangeQuantity(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<ITransactionManager>())).Returns(true);
      IAccountsServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>())).Returns(new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" });
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      // Act
      var result = transferService.DeleteItemById(appUser, id);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void GetItems_ValidInput_ReturnTransferItems() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accounts = new List<AccountItem>() {
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc02", ID = "2" },
      };

      var items = new List<Item>() {
        new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 },
      };

      var itemViews = new List<ItemView>() {
        new ItemView(new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 }),
      };

      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<Item>>(), It.IsAny<string>())).Returns(itemViews);
      IAccountsServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>())).Returns(new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" });
      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(() => new (itemViews.AsEnumerable(), 1));

      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      // Act
      var result = transferService.GetItems(appUser, tableLazyLoadEvent);

      // Assert
      Assert.Equal(1, result.TotalCount);
      Assert.NotNull(result.Items);
      Assert.Single(result.Items);
    }

    [Fact]
    public void GetItems_InvalidInput_ReturnNull() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accounts = new List<AccountItem>() {
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" },
        new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc02", ID = "2" },
      };

      var items = new List<Item>() {
        new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 },
      };

      var itemViews = new List<ItemView>() {
        new ItemView(new Item() { AccountId = "1", TransactionId = "1", Quantity = 50 }),
      };

      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<Item>>(), It.IsAny<string>())).Returns(itemViews);
      IAccountsServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>())).Returns(new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Acc01", ID = "1" });
      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(() => new(new List<ItemView>(), 0));

      var userSettingsService = new UserSettingsService(EnumerableUserServiceMock.Object, PostingUserServiceMock.Object, DeletingUserServiceMock.Object, FilteringUserServiceMock.Object);
      var transferService = new TransferService(incomeService, expensesService, IAccountsServiceMock.Object, userSettingsService);

      // Act
      var result = transferService.GetItems(appUser, tableLazyLoadEvent);

      // Assert
      Assert.Null(result.Items);
      Assert.Equal(0, result.TotalCount);
    }
  }
}