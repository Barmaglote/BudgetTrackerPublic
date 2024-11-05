using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models.Users;
using budget.core.Models;
using budget.core.Services.Interfaces;
using budget.core.Services;
using budget.webapi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using System.Security.Claims;
using utils.Exceptions;
using webapi.Controllers;
using budget.core.DB.Interfaces;
using MongoDB.Driver;

namespace budget.test.webapi {
  public class IncomeControllerTest {
    private Mock<IAccountsService> IAccountsServiceMock { get; } = new();
    private Mock<IUserFactory> IUserFactoryMock { get; } = new();
    private Mock<IEnumerableService<Item, ItemView>> EnumerableServiceMock { get; } = new();
    private Mock<IDeletingService<Item, ItemView>> DeletingServiceMock { get; } = new();
    private Mock<IPostingService<Item, ItemView>> PostingServiceMock { get; } = new();
    private Mock<IFilteringService<Item, ItemView>> FilteringServiceMock { get; } = new();
    private Mock<IEnumerableService<PlannedItem, PlannedItemView>> EnumerablePlanningServiceMock { get; } = new();
    private Mock<IPostingService<PlannedItem, PlannedItemView>> PostingPlanningServiceMock { get; } = new();
    private Mock<IDeletingService<PlannedItem, PlannedItemView>> DeletingPlanningServiceMock { get; } = new();
    private Mock<IFilteringService<PlannedItem, PlannedItemView>> FilteringPlanningServiceMock { get; } = new();
    private Mock<ITransactionManager> ITransactionManagerMock { get; } = new();


    [Fact]
    public void GetItem_ValidInput_ReturnsItem() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var planningService = new PlanningService(EnumerablePlanningServiceMock.Object, PostingPlanningServiceMock.Object, DeletingPlanningServiceMock.Object, FilteringPlanningServiceMock.Object);
      var incomeController = new IncomeController(incomeService, IAccountsServiceMock.Object, planningService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(itemView);

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      incomeController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var item = incomeController.GetItem(id.ToString());

      var okResult = item.Result as OkObjectResult;
      ItemView? httpResponse = okResult?.Value != null ? (ItemView)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(itemView.Comment, httpResponse.Comment);
    }

    [Fact]
    public void GetItem_AccessViolation_ReturnsApiException() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var planningService = new PlanningService(EnumerablePlanningServiceMock.Object, PostingPlanningServiceMock.Object, DeletingPlanningServiceMock.Object, FilteringPlanningServiceMock.Object);
      var expensesController = new IncomeController(incomeService, IAccountsServiceMock.Object, planningService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var itemView = new ItemView(new Item() { OwnerUserId = "ANOTHER", Comment = "TestComment" });
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(itemView);

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);


      expensesController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = () => expensesController.GetItem(id.ToString());

      // Assert      
      Assert.Throws<ApiException>(result);
    }

    [Fact]
    public void GetItems_ValidInput_ReturnsItem() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var planningService = new PlanningService(EnumerablePlanningServiceMock.Object, PostingPlanningServiceMock.Object, DeletingPlanningServiceMock.Object, FilteringPlanningServiceMock.Object);
      var incomeController = new IncomeController(incomeService, IAccountsServiceMock.Object, planningService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>()))
                            .Returns((
                              new List<ItemView>() {
                                new ItemView(new Item()),
                                new ItemView(new Item()),
                                new ItemView(new Item())
                              }, 3
                            ));


      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);


      incomeController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var item = incomeController.GetItems(new TableLazyLoadEvent());

      var okResult = item.Result as OkObjectResult;
      var httpResponse = okResult?.Value as ItemsResponse;

      // Assert
      Assert.NotNull(okResult?.Value);
      Assert.Equal(3, httpResponse?.Items.Count());
      Assert.Equal(3, httpResponse?.TotalCount);
    }

    [Fact]
    public void GetItems_AccessViolation_ReturnsBadRequest() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var planningService = new PlanningService(EnumerablePlanningServiceMock.Object, PostingPlanningServiceMock.Object, DeletingPlanningServiceMock.Object, FilteringPlanningServiceMock.Object);
      var incomeController = new IncomeController(incomeService, IAccountsServiceMock.Object, planningService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var itemView = new ItemView(new Item() { OwnerUserId = "ANOTHER", Comment = "TestComment" });
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(itemView);

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);


      incomeController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = incomeController.GetItems(new TableLazyLoadEvent());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task DeleteItem_ValidInput_ReturnsTrue() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var planningService = new PlanningService(EnumerablePlanningServiceMock.Object, PostingPlanningServiceMock.Object, DeletingPlanningServiceMock.Object, FilteringPlanningServiceMock.Object);
      var incomeController = new IncomeController(incomeService, IAccountsServiceMock.Object, planningService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(itemView);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), It.IsAny<IClientSessionHandle>())).Returns(true);

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);


      incomeController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var call = await incomeController.DeleteItem(id.ToString());
      var okResult = call.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);

    }

    [Fact]
    public async Task DeleteItem_AccessViolation_ReturnsBadRequest() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var planningService = new PlanningService(EnumerablePlanningServiceMock.Object, PostingPlanningServiceMock.Object, DeletingPlanningServiceMock.Object, FilteringPlanningServiceMock.Object);
      var incomeController = new IncomeController(incomeService, IAccountsServiceMock.Object, planningService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<ClaimsPrincipal>((appUser) => null);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var itemView = new ItemView(new Item() { OwnerUserId = "ANOTHER", Comment = "TestComment" });
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(itemView);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), It.IsAny<IClientSessionHandle>())).Returns(true);

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      // Act
      var result = await incomeController.DeleteItem(id.ToString());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }


    [Fact]
    public async Task Upsert_ValidInput_ReturnsTrue() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var planningService = new PlanningService(EnumerablePlanningServiceMock.Object, PostingPlanningServiceMock.Object, DeletingPlanningServiceMock.Object, FilteringPlanningServiceMock.Object);
      var incomeController = new IncomeController(incomeService, IAccountsServiceMock.Object, planningService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(itemView);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), It.IsAny<IClientSessionHandle>())).Returns(true);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), out isAdded, It.IsAny<IClientSessionHandle>())).Returns(true);
      IAccountsServiceMock.Setup(x => x.ChangeQuantity(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<ITransactionManager>())).Returns(true);

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      incomeController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var call = await incomeController.Upsert(itemView, id.ToString());
      var okResult = call.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);
    }

    [Fact]
    public async Task Upsert_AccessViolation_ReturnsBadRequest() {
      // Arrange
      var incomeService = new IncomeService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var planningService = new PlanningService(EnumerablePlanningServiceMock.Object, PostingPlanningServiceMock.Object, DeletingPlanningServiceMock.Object, FilteringPlanningServiceMock.Object);
      var incomeController = new IncomeController(incomeService, IAccountsServiceMock.Object, planningService, IUserFactoryMock.Object, ITransactionManagerMock.Object);

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var itemView = new ItemView(new Item() { OwnerUserId = "ANOTHER", Comment = "TestComment" });
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(itemView);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<ItemView>(), It.IsAny<string>(), null)).Returns(true);
      IAccountsServiceMock.Setup(x => x.ChangeQuantity(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<ITransactionManager>())).Returns(true);

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);


      incomeController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = await incomeController.Upsert(itemView, id.ToString());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }
  }
}