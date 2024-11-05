using budget.core.Entities;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Services;
using budget.core.Services.Interfaces;
using budget.utils.ApiFilters;
using budget.webapi.Controllers;
using budget.webapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace webapi.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
[ValidateAppUser]
[Route("[controller]")]
public class StatisticsController : BaseController {
  private readonly IStatisticsService<Item, ItemView> _incomeStatisticsService;
  private readonly CreditsService _creditsService;
  private readonly IncomeService _incomeService;
  private readonly ExpensesService _expensesService;

  public StatisticsController(
    IStatisticsService<Item, ItemView> incomeStatisticsService,
    CreditsService creditsService,
    IncomeService incomeService,
    ExpensesService expensesService,
    IUserFactory _userFactory) : base(_userFactory) {
    _incomeStatisticsService = incomeStatisticsService;
    _creditsService = creditsService;
    _incomeService = incomeService;
    _expensesService = expensesService;
  }

  [HttpPost("{area}/category")]
  public ActionResult<List<StatsByCategoryView>> GetStatisticsByCategory([FromRoute] [Required] string area, [FromBody] TableLazyLoadEvent tableLazyLoadEvent) {
    if (CurrentAppUser == null) { return BadRequest(); }
    return Ok(_incomeStatisticsService.GetStatsByCategory(CurrentAppUser, area, tableLazyLoadEvent));
  }

  [HttpPost("{area}/date")]
  public ActionResult<List<StatsByCategoryView>> GetStatisticsByDate([FromRoute][Required] string area, [FromBody] TableLazyLoadEvent tableLazyLoadEvent) {
    if (CurrentAppUser == null) { return BadRequest(); }
    return Ok(_incomeStatisticsService.GetStatsByDate(CurrentAppUser, area, tableLazyLoadEvent));
  }

  [HttpGet("credits")]
  public ActionResult<GeneralCreditsStatistics> GetGeneralCreditsStatistics() {
    if (CurrentAppUser == null) { return BadRequest(); }
    return Ok(_creditsService.GetGeneralCreditsStatistics(CurrentAppUser));
  }

  [HttpGet("brief")]
  public ActionResult<BriefStatistics> GetBriefStatistics() {
    if (CurrentAppUser == null) { return BadRequest(); }
    var lastIncome = _incomeService.GetLastItem(CurrentAppUser);
    var lastExpense = _expensesService.GetLastItem(CurrentAppUser);

    var briefStatistics = new BriefStatistics() {
      LastIncome = lastIncome,
      LastExpense = lastExpense
    };

    return Ok(briefStatistics);
  }

  [HttpGet("regular/{year}")]
  public ActionResult<RegularStatistics> GetRegualarPaymnets([FromRoute] int year) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var incomes = _incomeService.GetRegularPayments(CurrentAppUser, year);
    var expenses = _expensesService.GetRegularPayments(CurrentAppUser, year);

    var briefStatistics = new RegularStatistics() {
      Incomes = incomes,
      Expenses = expenses
    };

    return Ok(briefStatistics);
  }
}
