using budget.core.Entities;
using budget.core.Models;
using budget.core.Services.Interfaces;
using budget.core.Services;
using webapi.Controllers;
using Moq;
using budget.core.Factories.Interfaces;
using budget.core.Models.Users;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using budget.webapi.Models;

namespace budget.test.webapi {
  public class StatisticsControllerTest {
    private Mock<IStatisticsService<Item, ItemView>> IStatisticsServiceMock { get; } = new();
    private Mock<IUserFactory> IUserFactoryMock { get; } = new();
    private Mock<IEnumerableService<Item, ItemView>> EnumerableServiceMock { get; } = new();
    private Mock<IDeletingService<Item, ItemView>> DeletingServiceMock { get; } = new();
    private Mock<IPostingService<Item, ItemView>> PostingServiceMock { get; } = new();
    private Mock<IFilteringService<Item, ItemView>> FilteringServiceMock { get; } = new();
    private Mock<IEnumerableService<CreditItem, CreditItemView>> EnumerableCreditServiceMock { get; } = new();
    private Mock<IPostingService<CreditItem, CreditItemView>> PostingCreditServiceMock { get; } = new();
    private Mock<IDeletingService<CreditItem, CreditItemView>> DeletingCreditServiceMock { get; } = new();
    private Mock<IFilteringService<CreditItem, CreditItemView>> FilteringCreditServiceMock { get; } = new();
    private Mock<IEnumerableService<UserSettings, UserSettingsView>> EnumerableUserSettingsServiceMock { get; } = new();
    private Mock<IPostingService<UserSettings, UserSettingsView>> PostingUserSettingsServiceMock { get; } = new();
    private Mock<IDeletingService<UserSettings, UserSettingsView>> DeletingUserSettingsServiceMock { get; } = new();
    private Mock<IFilteringService<UserSettings, UserSettingsView>> FilteringUserSettingsServiceMock { get; } = new();

    [Fact]
    public void GetStatisticsByCategory_ValidInput_ReturnsStatistics() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      IStatisticsServiceMock.Setup(x => x.GetStatsByCategory(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(
        new List<StatsByCategoryView>() {
          new StatsByCategoryView(),
          new StatsByCategoryView(),
          new StatsByCategoryView()
        }
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = statisticsController.GetStatisticsByCategory("area", tableLazyLoadEvent);
      var actionResult = (OkObjectResult)result.Result;
      var httpResponse = actionResult.Value != null ? (List<StatsByCategoryView>)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(3, httpResponse.Count);
    }

    [Fact]
    public void GetStatisticsByCategory_AccessViolation_ReturnsBadRequest() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      IStatisticsServiceMock.Setup(x => x.GetStatsByCategory(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(
        new List<StatsByCategoryView>() {
          new StatsByCategoryView(),
          new StatsByCategoryView(),
          new StatsByCategoryView()
        }
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = statisticsController.GetStatisticsByCategory("area", tableLazyLoadEvent);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }
    
    [Fact]
    public void GetStatisticsByDate_ValidInput_ReturnsStatistics() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      IStatisticsServiceMock.Setup(x => x.GetStatsByDate(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(
        new List<StatsByDateView>() {
          new StatsByDateView(),
          new StatsByDateView(),
          new StatsByDateView()
        }
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = statisticsController.GetStatisticsByDate("area", tableLazyLoadEvent);
      var actionResult = (OkObjectResult)result.Result;
      var httpResponse = actionResult.Value != null ? (List<StatsByDateView>)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(3, httpResponse.Count);
    }

    [Fact]
    public void GetStatisticsByDate_AccessViolation_ReturnsBadRequest() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      IStatisticsServiceMock.Setup(x => x.GetStatsByDate(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(
        new List<StatsByDateView>() {
          new StatsByDateView(),
          new StatsByDateView(),
          new StatsByDateView()
        }
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = statisticsController.GetStatisticsByDate("area", tableLazyLoadEvent);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }
    
    [Fact]
    public void GetGeneralCreditsStatistics_ValidInput_ReturnsStatistics() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      EnumerableCreditServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(() => 
        new (new List<CreditItemView>() {
          new CreditItemView(new CreditItem() { AccountId="0", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false, Quantity = 100 }, new Payment() { isPaid = true, Quantity = 100 } } } ),
          new CreditItemView(new CreditItem() { AccountId="0", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false, Quantity = 100 }, new Payment() { isPaid = true, Quantity = 100 } } } ),
          new CreditItemView(new CreditItem() { AccountId="0", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false, Quantity = 100 }, new Payment() { isPaid = true, Quantity = 100 } } } ),
        }, 3)
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() { 
          new UserSettingsView(new UserSettings() { Accounts = new List<AccountItem>() { 
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0" } 
          } 
        }) 
      });

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = statisticsController.GetGeneralCreditsStatistics();
      var actionResult = (OkObjectResult)result.Result;
      var httpResponse = actionResult.Value != null ? (GeneralCreditsStatistics)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(1, httpResponse.Debt.Count);
      Assert.Equal(300, httpResponse.Debt["rub"]);
    }

    [Fact]
    public void GetGeneralCreditsStatistics_AccessViolation_ReturnsBadRequest() {
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      EnumerableCreditServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(() =>
        new(new List<CreditItemView>() {
          new CreditItemView(new CreditItem() { AccountId="0", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false, Quantity = 100 }, new Payment() { isPaid = true, Quantity = 100 } } } ),
          new CreditItemView(new CreditItem() { AccountId="0", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false, Quantity = 100 }, new Payment() { isPaid = true, Quantity = 100 } } } ),
          new CreditItemView(new CreditItem() { AccountId="0", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false, Quantity = 100 }, new Payment() { isPaid = true, Quantity = 100 } } } ),
        }, 3)
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>(null);
      FilteringUserSettingsServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>()))
        .Returns(new List<UserSettingsView>() {
          new UserSettingsView(new UserSettings() { Accounts = new List<AccountItem>() {
            new AccountItem() { AccountType= AccountType.Credit, Currency="rub", Title="TEST", ID="0" }
          }
        })
      });

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = statisticsController.GetGeneralCreditsStatistics();

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void GetBriefStatistics_ValidInput_ReturnsStatistics() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      IStatisticsServiceMock.Setup(x => x.GetStatsByDate(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(
        new List<StatsByDateView>() {
          new StatsByDateView(),
          new StatsByDateView(),
          new StatsByDateView()
        }
      );

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(() =>
        new(new List<ItemView>() {
          new ItemView(new Item() { Quantity = 100 })
        }, 1)
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = statisticsController.GetBriefStatistics();
      var actionResult = (OkObjectResult)result.Result;
      var httpResponse = actionResult.Value != null ? (BriefStatistics)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.NotNull(httpResponse.LastIncome);
      Assert.NotNull(httpResponse.LastExpense);
      Assert.Equal(100, httpResponse.LastIncome.Quantity);
      Assert.Equal(100, httpResponse.LastExpense.Quantity);
    }

    [Fact]
    public void GetBriefStatistics_AccessViolation_ReturnsBadRequest() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      IStatisticsServiceMock.Setup(x => x.GetStatsByDate(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(
        new List<StatsByDateView>() {
          new StatsByDateView(),
          new StatsByDateView(),
          new StatsByDateView()
        }
      );

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(() =>
        new(new List<ItemView>() {
          new ItemView(new Item() { Quantity = 100 })
        }, 1)
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>(null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = statisticsController.GetBriefStatistics();

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void GetRegualarPaymnets_ValidInput_ReturnsStatistics() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      IStatisticsServiceMock.Setup(x => x.GetStatsByDate(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(
        new List<StatsByDateView>() {
          new StatsByDateView(),
          new StatsByDateView(),
          new StatsByDateView()
        }
      );

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(() =>
        new(new List<ItemView>() {
          new ItemView(new Item() { Quantity = 100, Date = new DateTime(2024, 1, 1) }),
          new ItemView(new Item() { Quantity = 100, Date = new DateTime(2024, 1, 1) }),
          new ItemView(new Item() { Quantity = 100, Date = new DateTime(2024, 1, 1) }),
          new ItemView(new Item() { Quantity = 100, Date = new DateTime(2024, 1, 1) }),
        }, 4)
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = statisticsController.GetRegualarPaymnets(2024);
      var actionResult = (OkObjectResult)result.Result;
      var httpResponse = actionResult.Value != null ? (RegularStatistics)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.NotNull(httpResponse.Expenses);
      Assert.NotNull(httpResponse.Incomes);
      Assert.Equal(4, httpResponse.Expenses.Count());
      Assert.Equal(4, httpResponse.Incomes.Count());
      Assert.Equal(400, httpResponse.Expenses.Sum(x => x.Quantity));
      Assert.Equal(400, httpResponse.Incomes.Sum(x => x.Quantity));
    }

    [Fact]
    public void GetRegualarPaymnets_AccessViolation_ReturnsBadRequest() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var expensesService = new ExpensesService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var statisticsController = new StatisticsController(IStatisticsServiceMock.Object, creditsService, incomeService, expensesService, IUserFactoryMock.Object);
      var tableLazyLoadEvent = new TableLazyLoadEvent();
      IStatisticsServiceMock.Setup(x => x.GetStatsByDate(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(
        new List<StatsByDateView>() {
          new StatsByDateView(),
          new StatsByDateView(),
          new StatsByDateView()
        }
      );

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns(() =>
        new(new List<ItemView>() {
          new ItemView(new Item() { Quantity = 100 })
        }, 1)
      );

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>(null);

      var identity = new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      statisticsController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = statisticsController.GetRegualarPaymnets(100);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }
  }
}