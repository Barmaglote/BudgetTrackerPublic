using budget.core.Configurations;
using budget.core.DB.Interfaces;
using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using Moq;

namespace budget.test.core {
  public class DataValidatorTest {

    private Mock<IDBClient> IDBClientMock { get; } = new Mock<IDBClient>();
    private Mock<IOptions<ApiSettings>> IOptionsAppSettingsMock { get; } = new Mock<IOptions<ApiSettings>>();
    
    [Theory]
    [InlineData(15, 10, true, "income")]
    [InlineData(5, 10, false, "income")]
    [InlineData(15, 10, true, "expenses")]
    [InlineData(5, 10, false, "expenses")]
    [InlineData(15, 10, true, "credits")]
    [InlineData(5, 10, false, "credits")]
    [InlineData(15, 10, true, "planning")]
    [InlineData(5, 10, false, "planning")]
    [InlineData(15, 10, false, "wrong")]
    [InlineData(5, 10, false, "wrong")]
    public void IsValidToAdd_ValidInput_ReturnTrue(int maxItems, int exists, bool expectedResult, string collectionName) {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var mockMongoDatabase = new Mock<IMongoDatabase>();
      var mockMongoCollection = new Mock<IMongoCollection<Item>>();
      var mockApiSettings = new ApiSettings() { MaxTransaction = maxItems, MaxCredits = maxItems, MaxPlannings = maxItems };
      IOptionsAppSettingsMock.Setup(x => x.Value).Returns(mockApiSettings);
      var item = new Item();

      IDBClientMock.Setup(x => x.GetMongoDatabase()).Returns(mockMongoDatabase.Object);
      mockMongoDatabase.Setup(x => x.GetCollection<Item>(It.IsAny<string>(), null)).Returns(mockMongoCollection.Object);
      mockMongoCollection.Setup(x => x.CountDocuments(It.IsAny<FilterDefinition<Item>>(), null, default)).Returns(exists);

      var dataValidator = new DataValidator(IOptionsAppSettingsMock.Object, IDBClientMock.Object);

      // Act
      var result = dataValidator.IsValidToAdd(appUser, collectionName, item);
      // Assert
      Assert.Equal(result, expectedResult);
    }

    [Theory]
    [InlineData(15, 10, true, "income")] // exists > maxItems = false
    [InlineData(15, 15, true, "income")]
    [InlineData(15, 16, false, "income")]
    [InlineData(16, 15, true, "income")]
    [InlineData(5, 10, false, "income")]
    [InlineData(15, 10, true, "expenses")]
    [InlineData(15, 15, true, "expenses")]
    [InlineData(15, 16, false, "expenses")]
    [InlineData(16, 15, true, "expenses")]
    [InlineData(5, 10, false, "expenses")]
    [InlineData(15, 10, true, "credits")]
    [InlineData(15, 15, true, "credits")]
    [InlineData(15, 16, false, "credits")]
    [InlineData(16, 15, true, "credits")]
    [InlineData(5, 10, false, "credits")]
    [InlineData(15, 10, true, "planning")]
    [InlineData(15, 15, true, "planning")]
    [InlineData(15, 16, false, "planning")]
    [InlineData(16, 15, true, "planning")]
    [InlineData(5, 10, false, "planning")]
    [InlineData(15, 10, false, "wrong")]
    [InlineData(5, 10, false, "wrong")]
    public void IsValidToUpdate_ValidInput_ReturnTrue(int maxItems, int exists, bool expectedResult, string collectionName) {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var mockMongoDatabase = new Mock<IMongoDatabase>();
      var mockMongoCollection = new Mock<IMongoCollection<Item>>();
      var mockApiSettings = new ApiSettings() { MaxTransaction = maxItems, MaxCredits = maxItems, MaxPlannings = maxItems };
      IOptionsAppSettingsMock.Setup(x => x.Value).Returns(mockApiSettings);
      var item = new Item();

      IDBClientMock.Setup(x => x.GetMongoDatabase()).Returns(mockMongoDatabase.Object);
      mockMongoDatabase.Setup(x => x.GetCollection<Item>(It.IsAny<string>(), null)).Returns(mockMongoCollection.Object);
      mockMongoCollection.Setup(x => x.CountDocuments(It.IsAny<FilterDefinition<Item>>(), null, default)).Returns(exists);

      var dataValidator = new DataValidator(IOptionsAppSettingsMock.Object, IDBClientMock.Object);

      // Act
      var result = dataValidator.IsValidToUpdate(appUser, collectionName, item);
      // Assert
      Assert.Equal(result, expectedResult);
    }

    [Theory]
    [InlineData(2, 5, 5, false)]
    [InlineData(2, 3, 5, false)]
    [InlineData(4, 5, 5, true)]
    [InlineData(4, 3, 5, false)]
    [InlineData(4, 6, 5, true)]
    [InlineData(4, 5, 3, false)]
    [InlineData(4, 3, 3, false)]
    [InlineData(4, 4, 6, false)]
    [InlineData(4, 5, 1, false)]
    [InlineData(4, 5, 4, false)]
    [InlineData(4, 5, 9, true)]
    [InlineData(4, 5, 10, true)]

    public void IsValidToAdd_ValidInputForUserSettings_ReturnTrue(int maxAccounts, int maxTemplates, int maxCategories, bool expectedResult) {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var mockMongoDatabase = new Mock<IMongoDatabase>();
      var mockMongoCollection = new Mock<IMongoCollection<Item>>();

      var userSetting = new UserSettings() {
          Accounts = new List<AccountItem>() { 
            new AccountItem() { Title = "1111", AccountType = AccountType.Deposit, Currency = "rub" }, 
            new AccountItem() { Title = "2222", AccountType = AccountType.Deposit, Currency = "rub" },
            new AccountItem() { Title = "3333", AccountType = AccountType.Deposit, Currency = "rub" },
          },
          Templates = new Dictionary<string, TemplateItem[]> {
             { 
                "income", 
                  new TemplateItem[] { 
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "2" },
                  new TemplateItem() { AccountId = "3" },
                  new TemplateItem() { AccountId = "4" },
                } 
             },
             {
                "expenses",
                  new TemplateItem[] {
                  new TemplateItem() { AccountId = "1" },
                  new TemplateItem() { AccountId = "2" },
                  new TemplateItem() { AccountId = "3" },
                  new TemplateItem() { AccountId = "4" },
                  new TemplateItem() { AccountId = "5" },
                }
             },            
          },
          Categories = new Dictionary<string, string[]> { 
             {
                "income", new string[] { "111", "222", "333", "444", "555" }
             } 
          }
      };

      var mockApiSettings = new ApiSettings() { MaxAccounts = maxAccounts, MaxTemplates = maxTemplates, MaxCategories = maxCategories };
      IOptionsAppSettingsMock.Setup(x => x.Value).Returns(mockApiSettings);

      var dataValidator = new DataValidator(IOptionsAppSettingsMock.Object, IDBClientMock.Object);

      // Act
      var result = dataValidator.IsValidToAdd(appUser, "usersettings", userSetting);
      // Assert
      Assert.Equal(result, expectedResult);
    }
  }
}