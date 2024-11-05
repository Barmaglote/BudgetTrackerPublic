using budget.core.Services.Interfaces;
using budget.core.Services;
using Moq;
using webapi.Controllers;
using budget.core.Factories.Interfaces;
using budget.core.Models.Users;
using budget.core.Models;
using budget.core.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using utils.Exceptions;
using budget.webapi.Models;

namespace budget.test.webapi {
  public class CreditsControllerTest {
    private Mock<IAccountsService> IAccountsServiceMock { get; } = new Mock<IAccountsService>();
    private Mock<ITransferService> ITransferServiceMock { get; } = new Mock<ITransferService>();
    private Mock<IUserFactory> IUserFactoryMock { get; } = new Mock<IUserFactory>();
    private Mock<IEnumerableService<CreditItem, CreditItemView>> EnumerableServiceMock { get; } = new();
    private Mock<IDeletingService<CreditItem, CreditItemView>> DeletingServiceMock { get; } = new();
    private Mock<IPostingService<CreditItem, CreditItemView>> PostingServiceMock { get; } = new();
    private Mock<IFilteringService<CreditItem, CreditItemView>> FilteringServiceMock { get; } = new();
    private Mock<IEnumerableService<UserSettings, UserSettingsView>> EnumerableServiceUserSettingsMock { get; } = new();
    private Mock<IDeletingService<UserSettings, UserSettingsView>> DeletingServiceUserSettingsMock { get; } = new();
    private Mock<IPostingService<UserSettings, UserSettingsView>> PostingServiceUserSettingsMock { get; } = new();
    private Mock<IFilteringService<UserSettings, UserSettingsView>> FilteringServiceUserSettingsMock { get; } = new();

    [Fact]
    public void GetItem_ValidInput_ReturnsItemView() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),        
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() { OwnerUserId = "0", Category = "loan" });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);

      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var call = creditController.GetItem(id.ToString());
      var okResult = call.Result as OkObjectResult;
      var itemView = okResult?.Value as CreditItemView;

      // Assert
      Assert.NotNull(okResult?.Value);
      Assert.NotNull(itemView);
      Assert.Equal(200, okResult.StatusCode);
      Assert.Equal("loan", itemView.Category);
    }

    [Fact]
    public void GetItem_InvalidUser_ReturnsBadRequest() {
      // Arrange
      var creditItemView = new CreditItemView(new CreditItem { OwnerUserId = "0", Category = "loan" });
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);

      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
   
      // Act
      var result = creditController.GetItem(id.ToString());

      // Assert
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void GetItem_WrongUser_ReturnsApiException() {
      // Arrange
      var creditItemView = new CreditItemView(new CreditItem { OwnerUserId = "10", Category = "loan" });
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);

      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        },
      };

      // Act
      var result = () => creditController.GetItem(id.ToString());

      // Assert
      Assert.Throws<ApiException>(result);
    }

    [Fact]
    public void GetItems_ValidInput_ReturnsItemViews() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() { OwnerUserId = "0", Category = "loan" });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>()))
                            .Returns(( 
                              new List<CreditItemView>() {
                                new CreditItemView(new CreditItem()),
                                new CreditItemView(new CreditItem()),
                                new CreditItemView(new CreditItem())
                              }, 3 
                            ));

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var tableLazyLoadEvent = new TableLazyLoadEvent();

      // Act
      var call = creditController.GetItems(tableLazyLoadEvent);
      var okResult = call.Result as OkObjectResult;
      var creditsResponse = okResult?.Value as CreditsResponse;

      // Assert
      Assert.NotNull(okResult?.Value);
      Assert.Equal(3, creditsResponse?.Items.Count());
      Assert.Equal(3, creditsResponse?.TotalCount);
    }

    [Fact]
    public void GetItems_InvalidUser_ReturnsBadRequest() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() { OwnerUserId = "0", Category = "loan" });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>()))
                            .Returns((
                              new List<CreditItemView>() {
                                new CreditItemView(new CreditItem()),
                                new CreditItemView(new CreditItem()),
                                new CreditItemView(new CreditItem())
                              }, 3
                            ));

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var tableLazyLoadEvent = new TableLazyLoadEvent();

      // Act
      var call = creditController.GetItems(tableLazyLoadEvent);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestResult>(call.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void DeleteItem_ValidInput_ReturnsItemViews() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() { 
        OwnerUserId = "0", 
        Category = "loan", 
        IsActive = false,
        Plan = new List<Payment>() { 
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var call = creditController.DeleteItem(id.ToString());
      var okResult = call.Result as OkObjectResult;
      bool? deleteResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(deleteResponse);
      Assert.True(deleteResponse.HasValue);
      Assert.True(deleteResponse.Value);
    }

    [Fact]
    public void DeleteItem_InvalidUser_ReturnsBadRequest() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() { OwnerUserId = "0", Category = "loan" });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
      DeletingServiceMock.Setup(x => x.DeleteItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), null)).Returns(true);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var call = creditController.DeleteItem(id.ToString());      

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
      var creditItemView = new CreditItemView(new CreditItem() {
        OwnerUserId = "0",
        Category = "loan",
        IsActive = false,
        Plan = new List<Payment>() {
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var call = creditController.Upsert(creditItemView);
      var okResult = call.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);
    }

    [Fact]
    public void Upsert_InvalidUser_ReturnsBadRequest() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() {
        OwnerUserId = "0",
        Category = "loan",
        IsActive = false,
        Plan = new List<Payment>() {
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var call = creditController.Upsert(creditItemView);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(call.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void Activate_ValidInput_ReturnsTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() {
        OwnerUserId = "0",
        Category = "loan",
        IsActive = false,
        Plan = new List<Payment>() {
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var activationData = new ActivationData();

      // Act
      var call = creditController.Activate(id.ToString(), activationData);
      var okResult = call.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);
    }

    [Fact]
    public void Activate_InvalidInput_ReturnsBadRequest() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() {
        OwnerUserId = "0",
        Category = "loan",
        IsActive = false,
        Plan = new List<Payment>() {
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var activationData = new ActivationData();

      // Act
      var call = creditController.Activate("wrong", activationData);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(call.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void Activate_AlreadyActivatedInput_ReturnsBadRequest() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() {
        OwnerUserId = "0",
        Category = "loan",
        IsActive = true,
        Plan = new List<Payment>() {
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var activationData = new ActivationData();

      // Act
      var call = creditController.Activate(id.ToString(), activationData);

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(call.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void GetNextPayments_ValidInput_ReturnsTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() {
        OwnerUserId = "0",
        Category = "loan",
        IsActive = false,
        Plan = new List<Payment>() {
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>()))
                            .Returns((
                              new List<CreditItemView>() {
                                new CreditItemView(new CreditItem() { AccountId = "01", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "02", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "03", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "01", IsActive = false, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "02", IsActive = false, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "03", IsActive = false, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "04", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "05", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "06", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                              }, 9
                            ));

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var activationData = new ActivationData();

      // Act
      var call = creditController.GetNextPayments();
      var actionResult = (OkObjectResult)call.Result;
      var httpResponse = actionResult.Value != null ? (List<PaymentInfo>)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(5, httpResponse.Count);
      Assert.Equal(5, httpResponse.Capacity);
    }

    [Fact]
    public void GetNextPayments_InvalidInput_BadRequestResult() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() {
        OwnerUserId = "0",
        Category = "loan",
        IsActive = false,
        Plan = new List<Payment>() {
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(creditItemView);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var activationData = new ActivationData();

      // Act
      var call = creditController.GetNextPayments();

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(call.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }


    [Fact]
    public void GetNextPayments_ShortList_ReturnsItems() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);
      var creditItemView = new CreditItemView(new CreditItem() {
        OwnerUserId = "0",
        Category = "loan",
        IsActive = false,
        Plan = new List<Payment>() {
          new Payment() { isPaid = false }
        }
      });

      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      var creditService = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);
      var creditController = new CreditsController(creditService, IAccountsServiceMock.Object, ITransferServiceMock.Object, IUserFactoryMock.Object);
      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>()))
                            .Returns((
                              new List<CreditItemView>() {
                                new CreditItemView(new CreditItem() { AccountId = "01", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "02", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "03", IsActive = true, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "01", IsActive = false, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "02", IsActive = false, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                                new CreditItemView(new CreditItem() { AccountId = "03", IsActive = false, Plan = new List<Payment>() { new Payment() { isPaid = false }, new Payment() { isPaid = true } } }),
                              }, 6
                            ));

      creditController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var activationData = new ActivationData();

      // Act
      var call = creditController.GetNextPayments();
      var actionResult = (OkObjectResult)call.Result;
      var httpResponse = actionResult.Value != null ? (List<PaymentInfo>)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(3, httpResponse.Count);
      Assert.Equal(3, httpResponse.Capacity);
    }
  }
}