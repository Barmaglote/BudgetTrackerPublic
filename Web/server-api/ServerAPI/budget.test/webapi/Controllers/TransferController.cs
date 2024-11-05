using budget.core.DB.Interfaces;
using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services;
using budget.core.Services.Interfaces;
using budget.webapi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System.Security.Claims;
using webapi.Controllers;

namespace budget.test.webapi {
  public class TransferControllerTest{
    private Mock<IEnumerableService<CreditItem, CreditItemView>> EnumerableCreditServiceMock { get; } = new();
    private Mock<IPostingService<CreditItem, CreditItemView>> PostingCreditServiceMock { get; } = new();
    private Mock<IDeletingService<CreditItem, CreditItemView>> DeletingCreditServiceMock { get; } = new();
    private Mock<IFilteringService<CreditItem, CreditItemView>> FilteringCreditServiceMock { get; } = new();
    private Mock<IUserFactory> IUserFactoryMock { get; } = new();
    private Mock<ITransferService> ITransferServiceMock { get; } = new();
    private Mock<IEnumerableService<UserSettings, UserSettingsView>> EnumerableUserSettingsServiceMock { get; } = new();
    private Mock<IPostingService<UserSettings, UserSettingsView>> PostingUserSettingsServiceMock { get; } = new();
    private Mock<IDeletingService<UserSettings, UserSettingsView>> DeletingUserSettingsServiceMock { get; } = new();
    private Mock<IFilteringService<UserSettings, UserSettingsView>> FilteringUserSettingsServiceMock { get; } = new();
    private Mock<ITransactionManager> ITransactionManagerMock { get; } = new();

    [Fact]
    public void GetItem_ValidInput_ReturnItemView() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var transferController = new TransferController(ITransferServiceMock.Object, creditsService, IUserFactoryMock.Object, ITransactionManagerMock.Object);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      ITransferServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>()))
        .Returns(new TransferItem() { 
          FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
          ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
          FromQuantity = 100,
          ToQuantity = 200
        });

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      transferController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = transferController.GetItem(id.ToString());
      var actionResult = (OkObjectResult)result.Result;
      var httpResponse = actionResult.Value != null ? (TransferItem)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(200, httpResponse.ToQuantity);
      Assert.Equal(100, httpResponse.FromQuantity);
    }

    [Fact]
    public void GetItem_AccessViolation_ReturnItemBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var transferController = new TransferController(ITransferServiceMock.Object, creditsService, IUserFactoryMock.Object, ITransactionManagerMock.Object);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      ITransferServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>()))
        .Returns(new TransferItem() {
          FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
          ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
          FromQuantity = 100,
          ToQuantity = 200
        });

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      transferController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = transferController.GetItem(id.ToString());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void GetItems_ValidInput_ReturnItemViews() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var transferController = new TransferController(ITransferServiceMock.Object, creditsService, IUserFactoryMock.Object, ITransactionManagerMock.Object);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });

      ITransferServiceMock.Setup(x => x.GetItems(It.IsAny<AppUser>(), It.IsAny<TableLazyLoadEvent>()))
        .Returns(() => 
          new(new[] {
            new TransferItem() {
              FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              FromQuantity = 100,
              ToQuantity = 200
            },
            new TransferItem() {
              FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              FromQuantity = 100,
              ToQuantity = 200
            },
            new TransferItem() {
              FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              FromQuantity = 100,
              ToQuantity = 200
            }
          }, 3)
      );

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      transferController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      // Act
      var result = transferController.GetItems(new TableLazyLoadEvent());
      var actionResult = (OkObjectResult)result.Result;
      var httpResponse = actionResult.Value != null ? (TransfersResponse)actionResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.Equal(3, httpResponse.Items.Count());
      Assert.Equal(3, httpResponse.TotalCount);
    }

    [Fact]
    public void GetItems_AccessViolation_ReturnItemBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var transferController = new TransferController(ITransferServiceMock.Object, creditsService, IUserFactoryMock.Object, ITransactionManagerMock.Object);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      ITransferServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>()))
        .Returns(new TransferItem() {
          FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
          ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
          FromQuantity = 100,
          ToQuantity = 200
        });

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      transferController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = transferController.GetItems(new TableLazyLoadEvent());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task AddTransfer_ValidInput_ReturnTrue() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var transferController = new TransferController(ITransferServiceMock.Object, creditsService, IUserFactoryMock.Object, ITransactionManagerMock.Object);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });

      ITransferServiceMock.Setup(x => x.GetItems(It.IsAny<AppUser>(), It.IsAny<TableLazyLoadEvent>()))
        .Returns(() =>
          new(new[] {
            new TransferItem() {
              FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              FromQuantity = 100,
              ToQuantity = 200
            },
            new TransferItem() {
              FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              FromQuantity = 100,
              ToQuantity = 200
            },
            new TransferItem() {
              FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
              FromQuantity = 100,
              ToQuantity = 200
            }
          }, 3)
      );

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      transferController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var creditId = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var transferItem = new TransferItem() {
        FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1", Quantity = 200 },
        ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1", Quantity = 200 },
        FromQuantity = 100,
        ToQuantity = 200,
        Date = new DateTime(2024, 05, 03)
      };

      ITransferServiceMock.Setup(x => x.AddTransfer(It.IsAny<AppUser>(), It.IsAny<TransferItem>(), It.IsAny<ITransactionManager>())).Returns(true);

      EnumerableCreditServiceMock.Setup(x => x.GetItemView(It.IsAny<ObjectId>(), It.IsAny<string>())).Returns(new CreditItemView(new CreditItem() { 
        Plan = new List<Payment>() { 
          new Payment() { isPaid = true, Quantity = 100, Date = new DateTime(2024, 05, 03) }, 
          new Payment() { isPaid = true, Quantity = 200, Date = new DateTime(2024, 05, 03) }, 
          new Payment() { isPaid = false, Quantity = 200, Date = new DateTime(2024, 05, 05) },
          new Payment() { isPaid = false, Quantity = 100, Date = new DateTime(2024, 05, 05) },
          new Payment() { isPaid = false, Quantity = 200, Date = new DateTime(2024, 05, 03) }
        } 
      }));

      var isAdded = true;
      PostingCreditServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<CreditItemView>(), It.IsAny<string>(), out isAdded, It.IsAny<IClientSessionHandle>())).Returns(true);

      // Act
      var result = await transferController.AddTransfer(transferItem, creditId.ToString());
      var okResult = result.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);
    }

    [Fact]
    public void AddTransfer_AccessViolation_ReturnItemBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var transferController = new TransferController(ITransferServiceMock.Object, creditsService, IUserFactoryMock.Object, ITransactionManagerMock.Object);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      ITransferServiceMock.Setup(x => x.GetItemById(It.IsAny<AppUser>(), It.IsAny<string>()))
        .Returns(new TransferItem() {
          FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
          ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1" },
          FromQuantity = 100,
          ToQuantity = 200
        });

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      transferController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      // Act
      var result = transferController.GetItems(new TableLazyLoadEvent());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }


    [Fact]
    public async Task DeleteTransfer_ValidInput_ReturnTrue() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var transferController = new TransferController(ITransferServiceMock.Object, creditsService, IUserFactoryMock.Object, ITransactionManagerMock.Object);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns(appUser);
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });

      ITransferServiceMock.Setup(x => x.DeleteItemById(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<ITransactionManager>())).Returns(true);
      
      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      transferController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = claimsPrincipal
        }
      };

      var creditId = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });
      var transferItem = new TransferItem() {
        FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1", Quantity = 200 },
        ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1", Quantity = 200 },
        FromQuantity = 100,
        ToQuantity = 200,
        Date = new DateTime(2024, 05, 03),
        TransactionId = id.ToString()
      };

      ITransferServiceMock.Setup(x => x.AddTransfer(It.IsAny<AppUser>(), It.IsAny<TransferItem>(), null)).Returns(true);

      // Act
      var result = await transferController.DeleteTransfer(transferItem.TransactionId.ToString());
      var okResult = result.Result as OkObjectResult;
      bool? httpResponse = okResult?.Value != null ? (bool)okResult.Value : null;

      // Assert      
      Assert.NotNull(httpResponse);
      Assert.True(httpResponse.HasValue);
      Assert.True(httpResponse.Value);
    }

    [Fact]
    public async Task DeleteTransfer_AccessViolation_ReturnItemBadRequest() {
      // Arrange
      var userSettingsService = new UserSettingsService(EnumerableUserSettingsServiceMock.Object, PostingUserSettingsServiceMock.Object, DeletingUserSettingsServiceMock.Object, FilteringUserSettingsServiceMock.Object);
      var creditsService = new CreditsService(EnumerableCreditServiceMock.Object, PostingCreditServiceMock.Object, DeletingCreditServiceMock.Object, FilteringCreditServiceMock.Object, userSettingsService);
      var transferController = new TransferController(ITransferServiceMock.Object, creditsService, IUserFactoryMock.Object, ITransactionManagerMock.Object);
      var id = new ObjectId(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11 });

      var appUser = BudgetTrackerUser.CreateEmpty();
      IUserFactoryMock.Setup(x => x.CreateUser(It.IsAny<ClaimsPrincipal>())).Returns((AppUser)null);
      var itemView = new ItemView(new Item() { OwnerUserId = "0", Comment = "TestComment" });
      ITransferServiceMock.Setup(x => x.DeleteItemById(It.IsAny<AppUser>(), It.IsAny<string>(), null)).Returns(true);

      var identity = new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Email, "test@example.com"),
      }, "TestAuthentication");

      var claimsPrincipal = new ClaimsPrincipal(identity);

      transferController.ControllerContext = new ControllerContext {
        HttpContext = new DefaultHttpContext {
          User = null
        }
      };

      var transferItem = new TransferItem() {
        FromAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1", Quantity = 200 },
        ToAccount = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "T1", Quantity = 200 },
        FromQuantity = 100,
        ToQuantity = 200,
        Date = new DateTime(2024, 05, 03),
        TransactionId = id.ToString()
      };

      // Act
      var result = await transferController.DeleteTransfer(transferItem.TransactionId.ToString());

      // Assert      
      var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
      Assert.Equal(400, badRequestResult.StatusCode);
    }
  }
}