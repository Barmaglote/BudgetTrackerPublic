using budget.core.DB.Interfaces;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Services;
using budget.utils.ApiFilters;
using budget.webapi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace webapi.Controllers;

[Authorize]
[ValidateAppUser]
public class UserSettingsController : BaseController {

  private readonly UserSettingsService _userSettingsService;
  private readonly IncomeService _incomeService;
  private readonly ExpensesService _expensesService;
  private readonly CreditsService _creditsService;
  private readonly ITransactionManager _transactionManager;
  public UserSettingsController(
    UserSettingsService userSettingsService,
    IncomeService incomeService,
    ExpensesService expensesService,
    CreditsService creditsService,
    IUserFactory _userFactory,
    ITransactionManager transactionManager) : base(_userFactory) {
    _userSettingsService = userSettingsService;
    _incomeService = incomeService;
    _expensesService = expensesService;
    _creditsService = creditsService;
    _transactionManager = transactionManager;
  }

  [HttpGet]
  public ActionResult<UserSettingsView> GetUserSettings() {
    if (CurrentAppUser == null) { return BadRequest(); }
    return Ok(_userSettingsService.GetUserSettings(CurrentAppUser));
  }

  [HttpPost]
  public ActionResult<bool> SaveUserSettings([FromBody] UserSettingsView userSettings) {
    if (CurrentAppUser == null) { return false; }
    var result = _userSettingsService.UpsertUserSettings(CurrentAppUser, userSettings);
    return result ? Ok(result) : BadRequest(result);
  }

  [HttpPost("account")]
  public ActionResult<bool> UpsertAccountItem([FromBody][Required] AccountItem accountItem) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var result = _userSettingsService.Upsert(CurrentAppUser, accountItem);
    return result ? Ok(result) : BadRequest(result);
  }

  [HttpDelete("account/{id}")]
  public async Task<ActionResult<bool>> DeleteAccountItem([FromRoute][Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }

    _transactionManager.StartTransaction();
    bool result;
    try {
      var creditItemViews = _creditsService.GetViewByAccountId(CurrentAppUser, id);
      if (creditItemViews != null && creditItemViews.Any(
          item => item.IsActive 
          && item.Plan != null 
          && item.Plan.Any() 
          && item.Plan.Any(x => !x.isPaid))) {
        await _transactionManager.AbortTransactionAsync();
      }

      result = _userSettingsService.DeleteAccountItem(CurrentAppUser, id, _transactionManager);
      if (result) {
        // Do not check result, because it can get 0 records to delete
        _incomeService.DeleteItemByAccountId(id, _transactionManager);
        _expensesService.DeleteItemByAccountId(id, _transactionManager);
        _creditsService.DeleteItemByAccountId(id, _transactionManager);
      }

      if (!result) {
        await _transactionManager.AbortTransactionAsync();
      } else {
        await _transactionManager.CommitTransactionAsync();
      }
    } catch (Exception) {
      await _transactionManager.AbortTransactionAsync();
      throw;
    }

    return result ? Ok(result) : BadRequest(result);
  }

  [HttpGet("account/{id}")]
  public ActionResult<AccountItem> GetAccountItem([FromRoute][Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var result = _userSettingsService.GetAccountItem(CurrentAppUser, id);
    return result != null ? Ok(result) : BadRequest(result);
  }

  [HttpPost("language")]
  public ActionResult<UserSettingsView> UpdateLanguage([FromBody][Required] string language) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var result = _userSettingsService.UpdateField(CurrentAppUser, "Language", language);
    if (!result) { return BadRequest(false); }

    return Ok(_userSettingsService.GetUserSettings(CurrentAppUser));
  }

  [HttpPost("locale")]
  public ActionResult<UserSettingsView> UpdateLocale([FromBody][Required] string locale) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var result = _userSettingsService.UpdateField(CurrentAppUser, "Locale", locale);
    if (!result) { return BadRequest(false); }

    return Ok(_userSettingsService.GetUserSettings(CurrentAppUser));
  }
}

