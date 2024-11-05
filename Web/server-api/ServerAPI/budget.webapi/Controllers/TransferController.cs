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
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace webapi.Controllers;

[Authorize]
[ValidateAppUser]
[Route("[controller]")]
public class TransferController : BaseController {
  private readonly ITransferService _transferService;
  private readonly CreditsService _creditsService;
  private readonly ITransactionManager _transactionManager;

  public TransferController(ITransferService transferService, CreditsService creditsService, IUserFactory _userFactory, ITransactionManager transactionManager) : base(_userFactory) {
    _transferService = transferService;
    _creditsService = creditsService;
    _transactionManager = transactionManager;
  }

  [HttpGet("{id}")]
  public ActionResult<TransferItem> GetItem([FromRoute][Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var itemView = _transferService.GetItemById(CurrentAppUser, id);

    return Ok(itemView);
  }

  [HttpPost("items")]
  public ActionResult<TransfersResponse> GetItems([FromBody] TableLazyLoadEvent tableLazyLoadEvent) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var (Items, TotalCount) = _transferService.GetItems(CurrentAppUser, tableLazyLoadEvent);

    return Ok(new TransfersResponse() { Items = Items == null ? new List<TransferItem>() : Items, TotalCount = TotalCount });
  }

  [HttpPost()]
  public async Task<ActionResult<bool>> AddTransfer([FromBody][Required] TransferItem transferItem, [FromQuery] string? creditId) {
    if (CurrentAppUser == null) { return BadRequest(); }

    _transactionManager.StartTransaction();
    bool result;
    try {

      result = _transferService.AddTransfer(CurrentAppUser, transferItem, _transactionManager);
      if (!result) {
        await _transactionManager.AbortTransactionAsync();
        return BadRequest(false);
      }

      if (!string.IsNullOrWhiteSpace(creditId)) {
        var creditView = string.IsNullOrWhiteSpace(creditId) ? null : _creditsService.GetViewById(CurrentAppUser, creditId);
        if (creditView == null || creditView.Plan == null) {
          await _transactionManager.AbortTransactionAsync();
          return BadRequest(false);
        }

        var paymentIndex = creditView.Plan.FindIndex(x => !x.isPaid && x.Quantity == transferItem.ToQuantity && transferItem.Date == x.Date);
        if (paymentIndex > -1 && creditView.Plan[paymentIndex] != null) {
          creditView.Plan[paymentIndex].isPaid = true;
          result &= _creditsService.UpsertItem(CurrentAppUser, creditView, out var isAdded, _transactionManager);
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

    return result ? Ok(true) : BadRequest(false);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<bool>> DeleteTransfer([FromRoute][Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }
    bool result;
    _transactionManager.StartTransaction();

    try {
      result = _transferService.DeleteItemById(CurrentAppUser, id, _transactionManager);
      await _transactionManager.CommitTransactionAsync();
    } catch (Exception ex) {
      await _transactionManager.AbortTransactionAsync();
      Log.Error(ex.Message);
      result = false;
    }

    return result ? Ok(true) : BadRequest(false);
  }
}
