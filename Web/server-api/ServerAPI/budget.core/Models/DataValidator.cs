using budget.core.Configurations;
using budget.core.DB.Interfaces;
using budget.core.Entities;
using budget.core.Models.Interfaces;
using budget.core.Models.Users;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace budget.core.Models {
  public class DataValidator : IDataValidator {
    private readonly IOptions<ApiSettings> _appSettings;
    private IMongoDatabase _database;
    public DataValidator(IOptions<ApiSettings> appSettings, IDBClient dbClient) {
      _appSettings = appSettings;
      _database = dbClient.GetMongoDatabase();
    }

    public bool IsValidToAdd(AppUser appUser, string collectionName, BaseItem item) {
      if (string.IsNullOrWhiteSpace(collectionName) || appUser == null || item == null) { return false; }
      return IsValidToAdd(appUser.Id, collectionName, item);
    }

    public bool IsValidToAdd(string userId, string collectionName, BaseItem item) {
      if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(userId) || item == null) { return false; }

      switch (collectionName.ToLower()) {
        case "income":
          var incomesCurrentMonth = CountRecordsForCurrentMonth<Item>(userId, collectionName);
          if (incomesCurrentMonth >= _appSettings.Value.MaxTransaction) { return false; }
          return true;
        case "expenses":
          var expensesCurrentMonth = CountRecordsForCurrentMonth<Item>(userId, collectionName);
          if (expensesCurrentMonth >= _appSettings.Value.MaxTransaction) { return false; }
          return true;
        case "credits":
          var credits = CountRecordsForCurrentMonth<Item>(userId, collectionName);
          if (credits >= _appSettings.Value.MaxCredits) { return false; }
          return true;
        case "planning":
          var plans = CountRecordsForCurrentMonth<Item>(userId, collectionName);
          if (plans >= _appSettings.Value.MaxPlannings) { return false; }
          return true;
        case "payment":
          return true; // Add check
        case "usersettings":
          var settings = (UserSettings)item;

          if (settings.Accounts != null && settings.Accounts.Count() > _appSettings.Value.MaxAccounts) { return false; }

          if (settings.Templates != null && settings.Templates.TryGetValue("income", out TemplateItem[]? incomeTemplates)) {
            if (incomeTemplates != null && incomeTemplates.Count() > _appSettings.Value.MaxTemplates) { return false; }
          }

          if (settings.Templates != null && settings.Templates.TryGetValue("expenses", out TemplateItem[]? expensesTemplates)) {
            if (expensesTemplates != null && expensesTemplates.Count() > _appSettings.Value.MaxTemplates) { return false; }
          }

          if (settings.Categories != null && settings.Categories.TryGetValue("income", out string[]? incomeCategories)) {
            if (incomeCategories != null && incomeCategories.Count() > _appSettings.Value.MaxCategories) { return false; }
          }

          if (settings.Categories != null && settings.Categories.TryGetValue("expenses", out string[]? expensesCategories)) {
            if (expensesCategories != null && expensesCategories.Count() > _appSettings.Value.MaxCategories) { return false; }
          }

          if (settings.Categories != null && settings.Categories.TryGetValue("credits", out string[]? creditsCategories)) {
            if (creditsCategories != null && creditsCategories.Count() > _appSettings.Value.MaxCategories) { return false; }
          }

          return true;
        default:
          return false;
      }
    }

    public bool IsValidToUpdate(AppUser appUser, string collectionName, BaseItem item) {
      if (string.IsNullOrWhiteSpace(collectionName) || appUser == null || item == null) { return false; }

      return IsValidToUpdate(appUser.Id, collectionName, item);
    }

    public bool IsValidToUpdate(string userId, string collectionName, BaseItem item) {
      if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(userId) || item == null) { return false; }

      switch (collectionName.ToLower()) {
        case "income":
          var incomesCurrentMonth = CountRecordsForCurrentMonth<Item>(userId, collectionName);
          if (incomesCurrentMonth > _appSettings.Value.MaxTransaction) { return false; }
          return true;
        case "expenses":
          var expensesCurrentMonth = CountRecordsForCurrentMonth<Item>(userId, collectionName);
          if (expensesCurrentMonth > _appSettings.Value.MaxTransaction) { return false; }
          return true;
        case "credits":
          var credits = CountRecordsForCurrentMonth<Item>(userId, collectionName);
          if (credits > _appSettings.Value.MaxCredits) { return false; }
          return true;
        case "planning":
          var plans = CountRecordsForCurrentMonth<Item>(userId, collectionName);
          if (plans > _appSettings.Value.MaxPlannings) { return false; }
          return true;
        case "usersettings":
          var settings = (UserSettings)item;

          if (settings.Accounts != null && settings.Accounts.Count() > _appSettings.Value.MaxAccounts) { return false; }

          if (settings.Templates != null && settings.Templates.TryGetValue("income", out TemplateItem[]? incomeTemplates)) {
            if (incomeTemplates != null && incomeTemplates.Count() > _appSettings.Value.MaxTemplates) { return false; }
          }

          if (settings.Templates != null && settings.Templates.TryGetValue("expenses", out TemplateItem[]? expensesTemplates)) {
            if (expensesTemplates != null && expensesTemplates.Count() > _appSettings.Value.MaxTemplates) { return false; }
          }

          if (settings.Categories != null && settings.Categories.TryGetValue("income", out string[]? incomeCategories)) {
            if (incomeCategories != null && incomeCategories.Count() > _appSettings.Value.MaxCategories) { return false; }
          }

          if (settings.Categories != null && settings.Categories.TryGetValue("expenses", out string[]? expensesCategories)) {
            if (expensesCategories != null && expensesCategories.Count() > _appSettings.Value.MaxCategories) { return false; }
          }

          if (settings.Categories != null && settings.Categories.TryGetValue("credits", out string[]? creditsCategories)) {
            if (creditsCategories != null && creditsCategories.Count() > _appSettings.Value.MaxCategories) { return false; }
          }

          return true;
        default:
          return false;
      }
    }

    public long CountRecordsForCurrentMonth<T>(AppUser appUser, string collectionName) {
      return CountRecordsForCurrentMonth<T>(appUser.Id, collectionName);
    }

    public long CountRecordsForCurrentMonth<T>(string userId, string collectionName) {
      var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

      var filter = Builders<T>.Filter.And(
          Builders<T>.Filter.Eq("OwnerUserId", userId),
          Builders<T>.Filter.Gte("CreateDate", firstDayOfMonth)
      );

      var _collection = _database.GetCollection<T>(collectionName);

      var count = _collection.CountDocuments(filter);
      return count;
    }

  }
}
