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
public class PlanningController : BaseController {
  private readonly PlanningService _planningService;

  public PlanningController(PlanningService planningService, IUserFactory _userFactory) : base(_userFactory) {
    _planningService = planningService;
  }

  [HttpGet("{year}/{month}")]
  public ActionResult<PlannedItemView[]> GetPlannedPayments([FromRoute][Required] int year, [FromRoute][Required] int month) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var result = _planningService.GetItemByMonth(CurrentAppUser, year, month);

    return Ok(result);
  }

  [HttpPost()]
  public ActionResult<bool> Upsert([FromBody][Required] PlannedItemView plannedItemView) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var result = _planningService.UpsertItem(CurrentAppUser, plannedItemView, out bool isAdded);

    return Ok(result);
  }

  [HttpDelete("{id}")]
  public ActionResult<bool> DeleteItem([FromRoute][Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var planningItemView = _planningService.GetViewById(CurrentAppUser, id);
    if (planningItemView == null) {
      return BadRequest(false);
    }

    var result = _planningService.DeleteItem(CurrentAppUser, planningItemView);
    return Ok(result);
  }
}

