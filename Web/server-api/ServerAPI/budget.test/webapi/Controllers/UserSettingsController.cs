using budget.core.DB.Interfaces;
using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services;
using budget.core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System.Security.Claims;
using webapi.Controllers;

namespace budget.test.webapi {
  public class UserSettingsControllerTest {
    private Mock<IEnumerableService<UserSettings, UserSettingsView>> EnumerableUserSettingsServiceMock { get; } = new();
    private Mock<IDeletingService<UserSettings, UserSettingsView>> DeletingUserSettingsServiceMock { get; } = new();
    private Mock<IPostingService<UserSettings, UserSettingsView>> PostingUserSettingsServiceMock { get; } = new();
    private Mock<IFilteringService<UserSettings, UserSettingsView>> FilteringUserSettingsServiceMock { get; } = new();
    private Mock<IEnumerableService<Item, ItemView>> EnumerableServiceMock { get; } = new();
    private Mock<IPostingService<Item, ItemView>> PostingServiceMock { get; } = new();
    private Mock<IDeletingService<Item, ItemView>> DeletingServiceMock { get; } = new();
    private Mock<IFilteringService<Item, ItemView>> FilteringServiceMock { get; } = new();
    private Mock<IEnumerableService<CreditItem, CreditItemView>> EnumerableCreditServiceMock { get; } = new();
    private Mock<IDeletingService<CreditItem, CreditItemView>> DeletingCreditServiceMock { get; } = new();
    private Mock<IPostingService<CreditItem, CreditItemView>> PostingCreditServiceMock { get; } = new();
    private Mock<IFilteringService<CreditItem, CreditItemView>> FilteringCreditServiceMock { get; } = new();
    private Mock<IUserFactory> IUserFactoryMock { get; } = new();
    private Mock<ITransactionManager> ITransactionManagerMock { get; } = new();

    [Fact]
    public void GetUserSettings_ValidInput_ReturnsUserSettings() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Language="ru", Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0" }
          }
        })
      });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      userSettingsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = userSettingsController.GetUserSettings();
      var okResult = result.Result as OkObjectResult;
      UserSettingsView? httpResponse = okResult?.Value != null ? (UserSettingsView)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal("ru", httpResponse.Language);
    }

    [Fact]
    public void GetUserSettings_AccessViloation_ReturnsBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Language="ru", Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0" }
          }
        })
      });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>((appUser) => null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      // Act
      var result = userSettingsController.GetUserSettings();

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void UpsertAccountItem_ValidInput_ReturnsTrue() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, It.IsAny<IClientSessionHandle>())).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      userSettingsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };

      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Language="ru", Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0" }
          }
        })
      });

      // Act
      var result = userSettingsController.UpsertAccountItem(accountItem);
      var okResult = result.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);
    }

    [Fact]
    public void UpsertAccountItem_AccessViloation_ReturnsBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Language="ru", Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0" }
          }
        })
      });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>((appUser) => null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };

      // Act
      var result = userSettingsController.UpsertAccountItem(accountItem);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteAccountItem_ValidInput_ReturnsTrue() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      DeletingCreditServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      userSettingsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };
      DeletingUserSettingsServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);

      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Language="ru", Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0" }
          }
        })
      });

      // Act
      var result = await userSettingsController.DeleteAccountItem(id.ToString());
      var okResult = result.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);
    }

    [Fact]
    public async Task DeleteAccount_AccessViloation_ReturnsBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);
      DeletingUserSettingsServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>((appUser) => null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };

      // Act
      var result = await userSettingsController.DeleteAccountItem(id.ToString());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }
   
    [Fact]
    public void GetAccountItem_ValidInput_ReturnsAccountItem() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      DeletingCreditServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      userSettingsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };
      DeletingUserSettingsServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);

      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Language="ru", Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0", Initial = 100 }
          }
        })
      });

      // Act
      var result = userSettingsController.GetAccountItem("0");
      var okResult = result.Result as OkObjectResult;
      AccountItem? httpResponse = okResult?.Value != null ? (AccountItem)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(100, httpResponse.Initial);
    }

    [Fact]
    public void GetAccountItem_AccessViloation_ReturnsBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);
      DeletingUserSettingsServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>((appUser) => null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };

      // Act
      var result = userSettingsController.GetAccountItem(id.ToString());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void UpdateLanguage_ValidInput_ReturnsAccountItem() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      DeletingCreditServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      userSettingsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };
      DeletingUserSettingsServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);

      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Language="eng", Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0", Initial = 100 }
          }
        })
      });

      // Act
      var result = userSettingsController.UpdateLanguage("rus");
      var okResult = result.Result as OkObjectResult;
      UserSettingsView? httpResponse = okResult?.Value != null ? (UserSettingsView)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal("rus", httpResponse.Language);
    }

    [Fact]
    public void UpdateLanguage_AccessViloation_ReturnsBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);
      DeletingUserSettingsServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>((appUser) => null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };

      // Act
      var result = userSettingsController.UpdateLanguage(id.ToString());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void UpdateLocale_ValidInput_ReturnsAccountItem() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      DeletingCreditServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      userSettingsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };
      DeletingUserSettingsServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);

      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Language="eng", Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0", Initial = 100 }
          }
        })
      });

      // Act
      var result = userSettingsController.UpdateLocale("rus");
      var okResult = result.Result as OkObjectResult;
      UserSettingsView? httpResponse = okResult?.Value != null ? (UserSettingsView)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal("rus", httpResponse.Locale);
    }

    [Fact]
    public void UpdateLocale_AccessViloation_ReturnsBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var creditService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var userSettingsController = new UserSettingsController(userSettingsService, incomeService, expensesService, creditService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var isAdded = true;
      PostingUserSettingsServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, It.IsAny<IClientSessionHandle>())).Returns(true);
      DeletingUserSettingsServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), It.IsAny<IClientSessionHandle>())).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), It.IsAny<IClientSessionHandle>())).Returns(true);
      DeletingCreditServiceMock.Setup(x => x.DeleteItemsByAccountId(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(true);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), It.IsAny<IClientSessionHandle>())).Returns(true);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<ClaimsPrincipal>((claims) => null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "TitleTest" };

      // Act
      var result = userSettingsController.UpdateLocale(id.ToString());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }
  }
}