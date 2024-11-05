using budget.core.DB.Interfaces;
using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services;
using budget.core.Services.Interfaces;
using MongoDB.Driver;
using Moq;
using utils.Exceptions;

namespace budget.test.core {
  public class UserSettingsServiceTest {
    private Mock<IEnumerableService<UserSettings, UserSettingsView>> EnumerableServiceMock { get; } = new();
    private Mock<IPostingService<UserSettings, UserSettingsView>> PostingServiceMock { get; } = new();
    private Mock<IDeletingService<UserSettings, UserSettingsView>> DeletingServiceMock { get; } = new();
    private Mock<IFilteringService<UserSettings, UserSettingsView>> FilteringServiceMock { get; } = new();
    private Mock<ITransactionManager> ITransactionManagerMock { get; } = new();

    [Fact]
    public void AreSettingsValid_ValidInput_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsView = new UserSettingsView(
          new UserSettings() { 
            Accounts = new List<AccountItem>() { 
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
            },
            Templates = new Dictionary<string, TemplateItem[]>() { 
              { "income", new [] { 
                  new TemplateItem() { AccountId = "1" }, 
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                } 
              }
            },
            Categories = new Dictionary<string, string[]>() {
              { "income", new [] { "1", "2", "3", "4" } },
              { "expenses", new [] { "1", "2", "3", "4" } }
            }
          }
      );
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);

      // Act
      var result = userSettingsService.AreSettingsValid(appUser, userSettingsView);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void AreSettingsValid_InvalidInput1_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsView = new UserSettingsView(
          new UserSettings() {
            Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account06" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account07" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account08" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account09" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account10" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account11" }, // > MAX_TEMPLATES (10)
            },
            Templates = new Dictionary<string, TemplateItem[]>() {
              { "income", new [] {
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                }
              }
            },
            Categories = new Dictionary<string, string[]>() {
              { "income", new [] { "1", "2", "3", "4" } },
              { "expenses", new [] { "1", "2", "3", "4" } }
            }
          }
      );
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);

      // Act
      var result = userSettingsService.AreSettingsValid(appUser, userSettingsView);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void AreSettingsValid_InvalidInput2_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsView = new UserSettingsView(
          new UserSettings() {
            Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account06" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account07" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account08" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account09" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account10" },
            },
            Templates = new Dictionary<string, TemplateItem[]>() {
              { "income", new [] {
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                }
              }
            },
            Categories = new Dictionary<string, string[]>() {
              { "income", new [] { "1", "2", "3", "4" } },
              { "expenses", new [] { "1", "2", "3", "4" } }
            }
          }
      );
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);

      // Act
      var result = userSettingsService.AreSettingsValid(appUser, userSettingsView);

      // Assert
      Assert.True(userSettingsView.Templates["income"].Length > UserSettingsService.MAX_TEMPLATES);
      Assert.False(result);
    }

    [Fact]
    public void AreSettingsValid_InvalidInput3_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsView = new UserSettingsView(
          new UserSettings() {
            Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account06" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account07" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account08" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account09" },
            },
            Templates = new Dictionary<string, TemplateItem[]>() {
              { "income", new [] {
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" }, new TemplateItem() { AccountId = "1" },
                }
              }
            },
            Categories = new Dictionary<string, string[]>() {
              { "income", new [] { 
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
              } },
              { "expenses", new [] { "1", "2", "3", "4" } }
            }
          }
      );
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);

      // Act
      var result = userSettingsService.AreSettingsValid(appUser, userSettingsView);

      // Assert
      Assert.True(userSettingsView?.Accounts.Count() < UserSettingsService.MAX_ACCOUNTS);
      Assert.True(userSettingsView?.Templates["income"].Length < UserSettingsService.MAX_TEMPLATES);
      Assert.True(userSettingsView?.Categories["income"].Length > UserSettingsService.MAX_CATEGORIES);
      Assert.False(result);
    }

    [Fact]
    public void UpsertUserSettings_ValidInput_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsView = new UserSettingsView(
          new UserSettings() {
            Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
            },
            Templates = new Dictionary<string, TemplateItem[]>() {
              { "income", new [] {
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                }
              }
            },
            Categories = new Dictionary<string, string[]>() {
              { "income", new [] { "1", "2", "3", "4" } },
              { "expenses", new [] { "1", "2", "3", "4" } }
            }
          }
      );
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = userSettingsService.UpsertUserSettings(appUser, userSettingsView);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void UpsertUserSettings_InvalidInput1_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsView = new UserSettingsView(
          new UserSettings() {
            Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
            },
            Templates = new Dictionary<string, TemplateItem[]>() {
              { "income", new [] {
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                }
              }
            },
            Categories = new Dictionary<string, string[]>() {
              { "income", new [] { "1", "2", "3", "4" } },
              { "expenses", new [] { "1", "2", "3", "4" } }
            }
          }
      );
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = () => userSettingsService.UpsertUserSettings(appUser, userSettingsView);

      // Assert
      Assert.Throws<ApiException>(() => result());
    }

    [Fact]
    public void UpsertUserSettings_InvalidInput2_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsView = new UserSettingsView(
          new UserSettings() {
            Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05" },
            },
            Templates = new Dictionary<string, TemplateItem[]>() {
              { "income", new [] {
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                }
              }
            },
            Categories = new Dictionary<string, string[]>() {
              { "income", new [] {
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
              } },
              { "expenses", new [] { "1", "2", "3", "4" } }
            }
          }
      );
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var isAdded = true;
      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = () => userSettingsService.UpsertUserSettings(appUser, userSettingsView);

      // Assert
      Assert.Throws<ApiException>(() => result());
    }

    [Fact]
    public void DeleteAccountItem_ValidInput_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var id = "101";
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettings = new UserSettings() {
        Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01", ID = "100" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02", ID = "101" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03", ID = "102" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04", ID = "103" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05", ID = "104" },
            },
        Templates = new Dictionary<string, TemplateItem[]>() {
              { "income", new [] {
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "1" },
                }
              }
            },
        Categories = new Dictionary<string, string[]>() {
              { "income", new [] {
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
              } },
              { "expenses", new [] { "1", "2", "3", "4" } }
        }
      };

      var userSettingsView = new UserSettingsView(userSettings);
      var isAdded = false;

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { userSettingsView };
      });

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = userSettingsService.DeleteAccountItem(appUser, id, ITransactionManagerMock.Object);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void DeleteAccountItem_InvalidInput_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var id = "101";
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettings = new UserSettings() {
        Accounts = new List<AccountItem>(),
        Templates = new Dictionary<string, TemplateItem[]>(),
        Categories = new Dictionary<string, string[]>()
      };

      var userSettingsView = new UserSettingsView(userSettings);
      var isAdded = false;

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { userSettingsView };
      });

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = userSettingsService.DeleteAccountItem(appUser, id, ITransactionManagerMock.Object);

      // Assert
      Assert.False(result);
    }

    [Fact]
    public void GetAccountItem_ValidInput_ReturnAccountItem() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var id = "101";
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettings = new UserSettings() {
        Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01", ID = "100" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02", ID = "101" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03", ID = "102" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04", ID = "103" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05", ID = "104" },
            },
        Templates = new Dictionary<string, TemplateItem[]>(),
        Categories = new Dictionary<string, string[]>()
      };

      var userSettingsView = new UserSettingsView(userSettings);
      var isAdded = false;

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { userSettingsView };
      });

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var accountItem = userSettingsService.GetAccountItem(appUser, id);

      // Assert
      Assert.NotNull(accountItem);
      Assert.Equal("Account02", accountItem.Title);
    }

    [Fact]
    public void GetAccountItem_InvalidInput_ReturnNull() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var id = "151";
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettings = new UserSettings() {
        Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01", ID = "100" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02", ID = "101" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03", ID = "102" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04", ID = "103" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05", ID = "104" },
            },
        Templates = new Dictionary<string, TemplateItem[]>(),
        Categories = new Dictionary<string, string[]>()
      };

      var userSettingsView = new UserSettingsView(userSettings);
      var isAdded = false;

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { userSettingsView };
      });

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var accountItem = userSettingsService.GetAccountItem(appUser, id);

      // Assert
      Assert.Null(accountItem);
    }

    [Fact]
    public void Upsert_ValidInputExisting_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettings = new UserSettings() {
        Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01", ID = "100" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02", ID = "101" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03", ID = "102" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04", ID = "103" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05", ID = "104" },
            },
        Templates = new Dictionary<string, TemplateItem[]>(),
        Categories = new Dictionary<string, string[]>()
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account06", ID = "105" };

      var userSettingsView = new UserSettingsView(userSettings);
      var isAdded = false;

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { userSettingsView };
      });

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = userSettingsService.Upsert(appUser, accountItem);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void Upsert_ValidInputNew_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettings = new UserSettings() {
        Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01", ID = "100" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02", ID = "101" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03", ID = "102" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04", ID = "103" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05", ID = "104" },
            },
        Templates = new Dictionary<string, TemplateItem[]>(),
        Categories = new Dictionary<string, string[]>()
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account06" };

      var userSettingsView = new UserSettingsView(userSettings);
      var isAdded = false;

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { userSettingsView };
      });

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = userSettingsService.Upsert(appUser, accountItem);

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void UpdateField_ValidInput_ReturnTrue() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettings = new UserSettings() {
        Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01", ID = "100" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02", ID = "101" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03", ID = "102" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04", ID = "103" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05", ID = "104" },
            },
        Templates = new Dictionary<string, TemplateItem[]>(),
        Categories = new Dictionary<string, string[]>()
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account06" };

      var userSettingsView = new UserSettingsView(userSettings);
      var isAdded = false;

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { userSettingsView };
      });

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = userSettingsService.UpdateField(appUser, "Language", "ru");

      // Assert
      Assert.True(result);
    }

    [Fact]
    public void UpdateField_InvalidInput_ReturnFalse() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var userSettingsService = new UserSettingsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object);
      var userSettings = new UserSettings() {
        Accounts = new List<AccountItem>() {
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account01", ID = "100" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account02", ID = "101" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account03", ID = "102" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account04", ID = "103" },
              new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account05", ID = "104" },
            },
        Templates = new Dictionary<string, TemplateItem[]>(),
        Categories = new Dictionary<string, string[]>()
      };

      var accountItem = new AccountItem() { AccountType = AccountType.DebitCard, Currency = "rub", Title = "Account06" };

      var userSettingsView = new UserSettingsView(userSettings);
      var isAdded = false;

      FilteringServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { userSettingsView };
      });

      PostingServiceMock.Setup(x => x.UpsertItem(It.IsAny<AppUser>(), It.IsAny<UserSettingsView>(), It.IsAny<string>(), out isAdded, null)).Returns(true);

      // Act
      var result = userSettingsService.UpdateField(appUser, "WrongField", "ru");

      // Assert
      Assert.False(result);
    }
  }
}