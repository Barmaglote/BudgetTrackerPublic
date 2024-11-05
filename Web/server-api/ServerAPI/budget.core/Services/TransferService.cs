using budget.core.DB.Interfaces;
using budget.core.Models;
using budget.core.Models.Users;
using budget.core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace budget.core.Services
{
  public class TransferService : ITransferService {

    private readonly IncomeService _incomeService;
    private readonly ExpensesService _expensesService;
    private readonly IAccountsService _accountService;
    private readonly UserSettingsService _userSettingsService;

    public TransferService(IncomeService incomeService, ExpensesService expensesService, IAccountsService accountService, UserSettingsService userSettingsService)
    {
      _incomeService = incomeService;
      _expensesService = expensesService;
      _accountService = accountService;
      _userSettingsService = userSettingsService;
    }

    public bool AddTransfer(AppUser appUser, TransferItem transferItem, ITransactionManager? transactionManager = null) {
      if (transferItem == null || transferItem.ToQuantity <= 0 || transferItem.FromQuantity <= 0) {
        return false;
      }

      if (transferItem.FromAccount == null || transferItem.ToAccount == null || string.IsNullOrEmpty(transferItem.ToAccount.ID) || string.IsNullOrEmpty(transferItem.FromAccount.ID)) {
        return false;
      }

      if (transferItem.FromAccount.ID == transferItem.ToAccount.ID) {
        return false;
      }

      /* TODO: Временно отключил. Возможно в будущем верну
      if (transferItem.FromAccount.Currency == transferItem.ToAccount.Currency && Math.Round(transferItem.ToQuantity) != Math.Round(transferItem.FromQuantity)) {
        return false;
      }
      */

      var date = DateTime.UtcNow;

      var transactionId = Guid.NewGuid().ToString();
      var incomeView = new ItemView("transfer", transferItem.ToQuantity, date, false, string.Format("Transfer: {0} -> {1}", transferItem.FromAccount.Title, transferItem.ToAccount.Title), appUser.Id, transferItem.ToAccount.ID, transactionId);
      var expensesView = new ItemView("transfer", transferItem.FromQuantity, date, false, string.Format("Transfer: {0} -> {1}", transferItem.FromAccount.Title, transferItem.ToAccount.Title), appUser.Id, transferItem.FromAccount.ID, transactionId);

      var isAdded = _incomeService.UpsertItem(appUser, incomeView, out var isIncomeAdded, transactionManager);
      if (!isAdded) {
        transactionManager?.AbortTransactionAsync();
        return false;
      }

      isAdded = _expensesService.UpsertItem(appUser, expensesView, out var isExpenseAdded, transactionManager);
      if (!isAdded) {
        transactionManager?.AbortTransactionAsync();
        return false;
      }

      var isUpdated = _accountService.ChangeQuantity(
        appUser,
        transferItem.ToAccount.ID,
        transferItem != null ? transferItem.ToQuantity : 0,
        transferItem.FromAccount.ID,
        -(transferItem != null ? transferItem.FromQuantity : 0),
        transactionManager
      );

      if (!isUpdated) {
        transactionManager?.AbortTransactionAsync();
        return false;
      }

      return true;
    }

    public TransferItem? GetItemById(AppUser appUser, string id) {
      if (appUser == null) { return null; }
      var incomes = _incomeService.FilterByField(appUser, "transactionId", id);
      var expenses = _expensesService.FilterByField(appUser, "transactionId", id);

      if (incomes == null || !incomes.Any() || expenses == null || !expenses.Any()) {
        return null;
      }
      var incomeItem = incomes.First();
      var expensesItem = expenses.First();

      if (expensesItem?.AccountId == null || incomeItem?.AccountId == null) { return null; }

      var fromAccount = _accountService.GetItemById(appUser, expensesItem.AccountId);
      var toAccount = _accountService.GetItemById(appUser, incomeItem.AccountId);

      if (fromAccount == null || toAccount == null) { return null; }

      return new TransferItem {
        TransactionId = incomeItem.TransactionId,
        FromAccount = fromAccount,
        ToAccount = toAccount,
        FromQuantity = expensesItem.Quantity,
        ToQuantity = incomeItem.Quantity,
        Date = incomeItem.Date
      };
    }

    public bool DeleteItemById(AppUser appUser, string id, ITransactionManager? transactionManager = null) {
      var incomes = _incomeService.FilterByField(appUser, "transactionId", id);
      var expenses = _expensesService.FilterByField(appUser, "transactionId", id);

      if (incomes == null || !incomes.Any() || expenses == null || !expenses.Any()) {
        return false;
      }

      if (incomes.Count() > 1 || expenses.Count() > 1) {
        return false;
      }

      var incomeItem = incomes.First();
      var expensesItem = expenses.First();

      if (expensesItem?.AccountId == null || incomeItem?.AccountId == null) { return false; }

      var accountIncome = _accountService.GetItemById(appUser, incomeItem.AccountId);
      var accountExpenses = _accountService.GetItemById(appUser, expensesItem.AccountId);

      if (accountIncome == null || incomeItem.Quantity > accountIncome?.Quantity || accountExpenses == null) {
        return false;
      }

      var isIncomeDeleted = _incomeService.DeleteItem(appUser, incomeItem, transactionManager);
      var isExpenseDeleted = _expensesService.DeleteItem(appUser, expensesItem, transactionManager);

      if (!isIncomeDeleted || !isExpenseDeleted) {
        transactionManager?.AbortTransactionAsync();
        return false;
      }

      var isChanged = _accountService.ChangeQuantity(appUser, incomeItem.AccountId, -incomeItem.Quantity, expensesItem.AccountId, expensesItem.Quantity, transactionManager);
      if (!isChanged) {
        transactionManager?.AbortTransactionAsync();
        return false;
      }

      return true;
    }

    public (IEnumerable<TransferItem>? Items, long TotalCount) GetItems(AppUser appUser, TableLazyLoadEvent tableLazyLoadEvent) {
      var (incomes, itemsTotalCount) = _incomeService.GetItems(appUser, tableLazyLoadEvent);
      if (incomes == null || !incomes.Any()) {
        return (null, 0);
      }

      var userSettings = _userSettingsService.GetUserSettings(appUser);

      var getAccountById = new Func<string?, AccountItem?>((string? id) => {
        if (userSettings == null) { return null; }
        return userSettings.Accounts?.FirstOrDefault(x => x.ID == id);
      });

      var result = (
          from income in incomes
          let itemView = income?.TransactionId == null ? null : _expensesService.FilterByField(appUser, "transactionId", income.TransactionId)
          where itemView != null && itemView.Any()
          let expense = itemView.First() // Берем первую запись из expenses
          select new TransferItem {
            TransactionId = income.TransactionId,
            ToAccount = getAccountById(income.AccountId) ?? new AccountItem() { AccountType = AccountType.Undefined, Currency = string.Empty, Title = string.Empty },
            FromAccount = getAccountById(expense.AccountId) ?? new AccountItem() { AccountType = AccountType.Undefined, Currency = string.Empty, Title = string.Empty },
            ToQuantity = income.Quantity,
            FromQuantity = expense.Quantity,
            Date = income.Date
          }
      ).ToList();

      return (result, result?.Count ?? 0);
    }
  }
}
