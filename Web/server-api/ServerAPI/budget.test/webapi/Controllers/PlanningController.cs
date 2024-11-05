using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services;
using budget.core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using System.Security.Claims;
using webapi.Controllers;

namespace budget.test.webapi {
  public class PlanningControllerTest {
    private Mock<IUserFactory> IUserFactoryMock { get; } = new();
    private Mock<IEnumerableService<PlannedItem, PlannedItemView>> IEnumerableServiceMock = new();
    private Mock<IPostingService<PlannedItem, PlannedItemView>> IPostingServiceMock = new();
    private Mock<IDeletingService<PlannedItem, PlannedItemView>> IDeletingServiceMock = new();
    private Mock<IFilteringService<PlannedItem, PlannedItemView>> IFilteringServiceMock = new();

    [Fact]
    public void GetPlannedPayments_ValidInput_ReturnsItems() {
      // Arrange
      int year = 2024;
      int month = 5;
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var planningService = new PlanningService(IEnumerableServiceMock.Object, IPostingServiceMock.Object, IDeletingServiceMock.Object, IFilteringServiceMock.Object);
      var planningController = new PlanningController(planningService, IUserFactoryMock.Object);
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      IFilteringServiceMock.Setup(x => x.GetItemByMonth(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
        new[] {
          new PlannedItemView(new PlannedItem()),
          new PlannedItemView(new PlannedItem()),
          new PlannedItemView(new PlannedItem())
        }
      );

      planningController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var call = planningController.GetPlannedPayments(year, month);
      var actionResult = (OkObjectResult)call.Result;
      var httpResponse = actionResult.Value != null ? (PlannedItemView[])actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(3, httpResponse.Length);
    }

    [Fact]
    public void GetPlannedPayments_AccessViolation_ReturnsBarRequest() {
      // Arrange
      int year = 2024;
      int month = 5;
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
          new Claim(ClaimTypes.Name, "TestUser"),
          new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var planningService = new PlanningService(IEnumerableServiceMock.Object, IPostingServiceMock.Object, IDeletingServiceMock.Object, IFilteringServiceMock.Object);
      var planningController = new PlanningController(planningService, IUserFactoryMock.Object);
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>(null);

      IFilteringServiceMock.Setup(x => x.GetItemByMonth(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
        new[] {
            new PlannedItemView(new PlannedItem()),
            new PlannedItemView(new PlannedItem()),
            new PlannedItemView(new PlannedItem())
        }
      );

      // Act
      var call = planningController.GetPlannedPayments(year, month);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(call.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void Upsert_ValidInput_ReturnsTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var planningService = new PlanningService(IEnumerableServiceMock.Object, IPostingServiceMock.Object, IDeletingServiceMock.Object, IFilteringServiceMock.Object);
      var planningController = new PlanningController(planningService, IUserFactoryMock.Object);
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var isAdded = true;
      IPostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<PlannedItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      planningController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var plannedItemView = new PlannedItemView(new PlannedItem());

      // Act
      var call = planningController.Upsert(plannedItemView);
      var okResult = call.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);

    }

    [Fact]
    public void Upsert_AccessViolation_ReturnsBadRequest() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
          new Claim(ClaimTypes.Name, "TestUser"),
          new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var planningService = new PlanningService(IEnumerableServiceMock.Object, IPostingServiceMock.Object, IDeletingServiceMock.Object, IFilteringServiceMock.Object);
      var planningController = new PlanningController(planningService, IUserFactoryMock.Object);
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>(null);

      var isAdded = true;
      IPostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<PlannedItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      var plannedItemView = new PlannedItemView(new PlannedItem());

      // Act
      var call = planningController.Upsert(plannedItemView);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(call.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void DeleteItem_ValidInput_ReturnsTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var planningService = new PlanningService(IEnumerableServiceMock.Object, IPostingServiceMock.Object, IDeletingServiceMock.Object, IFilteringServiceMock.Object);
      var planningController = new PlanningController(planningService, IUserFactoryMock.Object);
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var isAdded = true;
      IPostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<PlannedItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var plannedItemView = new PlannedItemView(new PlannedItem());
      IEnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(plannedItemView);
      IDeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<PlannedItemView>(), It.IsAny<string>(), null)).Returns(true);

      planningController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };
    
      // Act
      var call = planningController.DeleteItem(id.ToString());
      var okResult = call.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);
    }

    [Fact]
    public void DeleteItem_AccessViolation_ReturnsBarRequest() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
          new Claim(ClaimTypes.Name, "TestUser"),
          new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      var planningService = new PlanningService(IEnumerableServiceMock.Object, IPostingServiceMock.Object, IDeletingServiceMock.Object, IFilteringServiceMock.Object);
      var planningController = new PlanningController(planningService, IUserFactoryMock.Object);
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns<AppUser>(null);

      IDeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<PlannedItemView>(), It.IsAny<string>(), null)).Returns(true);

      planningController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      var plannedItemView = new PlannedItemView(new PlannedItem());

      // Act
      var call = planningController.Upsert(plannedItemView);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(call.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

  }
}