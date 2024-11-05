using budget.core.Entities;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services;
using budget.core.Services.Interfaces;
using MongoDB.Driver;
using Moq;

namespace budget.test.core {
  public class CreditsServiceTest {
    private Mock<IEnumerableService<UserSettings, UserSettingsView>> EnumerableServiceUserSettingsMock { get; } = new Mock<IEnumerableService<UserSettings, UserSettingsView>>();
    private Mock<IDeletingService<UserSettings, UserSettingsView>> DeletingServiceUserSettingsMock { get; } = new Mock<IDeletingService<UserSettings, UserSettingsView>>();
    private Mock<IPostingService<UserSettings, UserSettingsView>> PostingServiceUserSettingsMock { get; } = new Mock<IPostingService<UserSettings, UserSettingsView>>();
    private Mock<IFilteringService<UserSettings, UserSettingsView>> FilteringServiceUserSettingsMock { get; } = new Mock<IFilteringService<UserSettings, UserSettingsView>>();
    private Mock<IEnumerableService<CreditItem, CreditItemView>> EnumerableServiceMock { get; } = new Mock<IEnumerableService<CreditItem, CreditItemView>>();
    private Mock<IDeletingService<CreditItem, CreditItemView>> DeletingServiceMock { get; } = new Mock<IDeletingService<CreditItem, CreditItemView>>();
    private Mock<IPostingService<CreditItem, CreditItemView>> PostingServiceMock { get; } = new Mock<IPostingService<CreditItem, CreditItemView>>();
    private Mock<IFilteringService<CreditItem, CreditItemView>> FilteringServiceMock { get; } = new Mock<IFilteringService<CreditItem, CreditItemView>>();

    [Fact]
    public void GetGeneralCreditsStatistics_ValidInput_ReturnStatistics() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accounts = new List<AccountItem> {
        new AccountItem { ID = "123", Quantity = 100.0m, Title = "My Salary Card 1", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = "456", Quantity = 100.0m, Title = "My Salary Card 2", AccountType = AccountType.DebitCard, Currency = "usd" },
        new AccountItem { ID = "789", Quantity = 100.0m, Title = "My Salary Card 3", AccountType = AccountType.DebitCard, Currency = "eur" }
      };
      var userSettings = new UserSettings { Accounts = accounts };

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      FilteringServiceUserSettingsMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { new UserSettingsView(userSettings) };
      });

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns<AppUser, string, TableLazyLoadEvent>((appUser, collectionName, tableLazyLoadEvent) => {
        return new ( new List<CreditItemView>() { 
          new CreditItemView(new CreditItem() { 
            IsActive = true, 
            AccountId = "123", 
            Category = "loans", 
            Months = 5, 
            Plan = new List<Payment>() { 
              new Payment() { Month = 1, Quantity = 100, isPaid = true },
              new Payment() { Month = 2, Quantity = 100, isPaid = true },
              new Payment() { Month = 3, Quantity = 100, isPaid = false },
            } 
          }),
          new CreditItemView(new CreditItem() {
            IsActive = true,
            AccountId = "456",
            Category = "loans",
            Months = 5,
            Plan = new List<Payment>() {
              new Payment() { Month = 1, Quantity = 100, isPaid = true },
              new Payment() { Month = 2, Quantity = 100, isPaid = false },
              new Payment() { Month = 3, Quantity = 100, isPaid = false },
            }
          }),
          new CreditItemView(new CreditItem() {
            IsActive = true,
            AccountId = "789",
            Category = "loans",
            Months = 5,
            Plan = new List<Payment>() {
              new Payment() { Month = 1, Quantity = 100, isPaid = false },
              new Payment() { Month = 2, Quantity = 100, isPaid = false },
              new Payment() { Month = 3, Quantity = 100, isPaid = false },
            }
          }),
          new CreditItemView(new CreditItem() {
            IsActive = false,
            AccountId = "789",
            Category = "loans",
            Months = 5,
            Plan = new List<Payment>() {
              new Payment() { Month = 1, Quantity = 100, isPaid = false },
              new Payment() { Month = 2, Quantity = 100, isPaid = false },
              new Payment() { Month = 3, Quantity = 100, isPaid = false },
            }
          }),
          new CreditItemView(new CreditItem() {
            IsActive = true,
            AccountId = "789",
            Category = "loans",
            Months = 5,
            Plan = null
          })
        }, 3);
      });

      var service = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);

      // Action
      var statistics = service.GetGeneralCreditsStatistics(appUser);

      // Assert
      Assert.NotNull(statistics);
      Assert.Equal(100, statistics?.Debt["rub"]);
      Assert.Equal(200, statistics?.Debt["usd"]);
      Assert.Equal(300, statistics?.Debt["eur"]);
    }

    [Fact]
    public void GetTopPaymentsForActiveCredits_ValidInput_ReturnStatistics() {
      // Arrange
      var appUser = BudgetTrackerUser.CreateEmpty();
      var accounts = new List<AccountItem> {
        new AccountItem { ID = "123", Quantity = 100.0m, Title = "My Salary Card 1", AccountType = AccountType.DebitCard, Currency = "rub" },
        new AccountItem { ID = "456", Quantity = 100.0m, Title = "My Salary Card 2", AccountType = AccountType.DebitCard, Currency = "usd" },
        new AccountItem { ID = "789", Quantity = 100.0m, Title = "My Salary Card 3", AccountType = AccountType.DebitCard, Currency = "eur" }
      };
      var userSettings = new UserSettings { Accounts = accounts };

      var userSettingsServiceMock = new UserSettingsService(EnumerableServiceUserSettingsMock.Object, PostingServiceUserSettingsMock.Object, DeletingServiceUserSettingsMock.Object, FilteringServiceUserSettingsMock.Object);
      FilteringServiceUserSettingsMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<FilterDefinition<UserSettings>>(), It.IsAny<string>())).Returns<AppUser, FilterDefinition<UserSettings>, string>((appUser, filterField, collectionName) => {
        return new List<UserSettingsView>() { new UserSettingsView(userSettings) };
      });

      EnumerableServiceMock.Setup(x => x.GetItemViews(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<TableLazyLoadEvent>())).Returns<AppUser, string, TableLazyLoadEvent>((appUser, collectionName, tableLazyLoadEvent) => {
        return new(new List<CreditItemView>() {
          new CreditItemView(new CreditItem() {
            IsActive = true,
            AccountId = "123",
            Category = "loans",
            Months = 5,
            Plan = new List<Payment>() {
              new Payment() { Month = 1, Quantity = 100, isPaid = true, Date = new DateTime(2020, 03, 12) },
              new Payment() { Month = 2, Quantity = 100, isPaid = true, Date = new DateTime(2020, 03, 12) },
              new Payment() { Month = 3, Quantity = 100, isPaid = false, Date = new DateTime(2020, 03, 12) },
            }
          }),
          new CreditItemView(new CreditItem() {
            IsActive = true,
            AccountId = "456",
            Category = "loans",
            Months = 5,
            Plan = new List<Payment>() {
              new Payment() { Month = 1, Quantity = 100, isPaid = false, Date = new DateTime(2019, 03, 12) },
              new Payment() { Month = 2, Quantity = 100, isPaid = false, Date = new DateTime(2020, 03, 12) },
              new Payment() { Month = 3, Quantity = 100, isPaid = false, Date = new DateTime(2020, 03, 12) },
            }
          }),
          new CreditItemView(new CreditItem() {
            IsActive = true,
            AccountId = "789",
            Category = "loans",
            Months = 5,
            Plan = new List<Payment>() {
              new Payment() { Month = 1, Quantity = 100, isPaid = false, Date = new DateTime(2020, 03, 12) },
              new Payment() { Month = 2, Quantity = 100, isPaid = false, Date = new DateTime(2020, 03, 12) },
              new Payment() { Month = 3, Quantity = 100, isPaid = false, Date = new DateTime(2020, 03, 12) },
            }
          }),
          new CreditItemView(new CreditItem() {
            IsActive = false,
            AccountId = "789",
            Category = "loans",
            Months = 5,
            Plan = new List<Payment>() {
              new Payment() { Month = 1, Quantity = 100, isPaid = false, Date = new DateTime(2010, 03, 12) },
              new Payment() { Month = 2, Quantity = 100, isPaid = false, Date = new DateTime(2010, 03, 12) },
              new Payment() { Month = 3, Quantity = 100, isPaid = false, Date = new DateTime(2010, 03, 12) },
            }
          }),
          new CreditItemView(new CreditItem() {
            IsActive = true,
            AccountId = "789",
            Category = "loans",
            Months = 5,
            Plan = null
          })
        }, 3);
      });

      var service = new CreditsService(EnumerableServiceMock.Object, PostingServiceMock.Object, DeletingServiceMock.Object, FilteringServiceMock.Object, userSettingsServiceMock);

      // Action
      var payments = service.GetTopPaymentsForActiveCredits(appUser);

      // Assert
      Assert.NotNull(payments);
      Assert.Equal(5, payments.Count());
      Assert.DoesNotContain(payments, x => x.Payment == null);
      Assert.DoesNotContain(payments, x => x.Payment.Date < new DateTime(2015, 01, 01));
      Assert.Equal(2019, payments.First().Payment.Date.Year);
    }
  }
}