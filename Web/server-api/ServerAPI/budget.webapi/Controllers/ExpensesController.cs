using budget.core.DB.Interfaces;
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
using System.Diagnostics.CodeAnalysis;
using utils.Exceptions;

namespace webapi.Controllers;

[Authorize]
[ValidateAppUser]
[Route("[controller]")]
public class ExpensesController : BaseController {
  private readonly ExpensesService _expensesService;
  private readonly IAccountsService _accountsService;
  private readonly PlanningService _planningService;
  private readonly ITransactionManager _transactionManager;

  public ExpensesController(
    ExpensesService expsensesService,
    IAccountsService accountsService,
    PlanningService planningService,
    IUserFactory _userFactory,
    ITransactionManager transactionManager) : base(_userFactory) {
    _expensesService = expsensesService;
    _accountsService = accountsService;
    _planningService = planningService;
    _transactionManager = transactionManager;
  }

  [HttpGet("{id}")]
  public ActionResult<ItemView> GetItem([FromRoute] [Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var itemView = _expensesService.GetViewById(CurrentAppUser, id);

    if (itemView == null) {
      throw new ApiException("Unable to find object");
    }

    if (itemView.OwnerUserId != CurrentAppUser.Id) {
      throw new ApiException("Access violation");
    }

    return Ok(itemView);
  }

  [HttpPost("items")]
  public ActionResult<ItemsResponse> GetItems([FromBody] TableLazyLoadEvent tableLazyLoadEvent) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var (Items, TotalCount) = _expensesService.GetItems(CurrentAppUser, tableLazyLoadEvent);

    return Ok(new ItemsResponse() { Items = Items, TotalCount = TotalCount }); ;
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<bool>> DeleteItem([FromRoute][Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var itemView = _expensesService.GetViewById(CurrentAppUser, id);

    if (itemView == null || itemView.AccountId == null) {
      return BadRequest(false);
    }

    bool result;

    _transactionManager.StartTransaction();
    try {
      result = _expensesService.DeleteItem(CurrentAppUser, itemView, _transactionManager);

      if (result) {
        result &= _accountsService.ChangeQuantity(CurrentAppUser, itemView.AccountId, itemView.Quantity, _transactionManager);
      }

      if (!result) {
        await _transactionManager.AbortTransactionAsync();
      }

      await _transactionManager.CommitTransactionAsync();
    } catch (Exception) {
      await _transactionManager.AbortTransactionAsync();
      throw;
    }

    return Ok(result);
  }

  [HttpPost()]
  public async Task<ActionResult<bool>> Upsert([FromBody][NotNull] ItemView itemView, [FromQuery] string? plannedId) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var currentValue = 0;

    if (!string.IsNullOrWhiteSpace(itemView?.IdString)) {
      var item = _expensesService.GetViewById(CurrentAppUser, itemView.IdString);

      if (item != null) {
        currentValue = (int)item.Quantity;
      }
    }

    _transactionManager.StartTransaction();
    bool result;
    try {
      result = _expensesService.UpsertItem(CurrentAppUser, itemView, out bool isAdded, _transactionManager);
      if (!result) { return BadRequest(false); }

      if (itemView?.AccountId != null) {
        if (isAdded) {
          result &= _accountsService.ChangeQuantity(CurrentAppUser, itemView.AccountId, -itemView.Quantity, _transactionManager);
        } else {
          result &= _accountsService.ChangeQuantity(CurrentAppUser, itemView.AccountId, currentValue - itemView.Quantity, _transactionManager);
        }
      }

      if (!string.IsNullOrEmpty(plannedId)) {
        var plannedItem = _planningService.GetViewById(CurrentAppUser, plannedId);
        if (plannedItem != null) {
          plannedItem.IsPaid = true;
          result &= _planningService.UpsertItem(CurrentAppUser, plannedItem, out bool isItemAdded, _transactionManager);
        }
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

    return Ok(result);
  }
}
