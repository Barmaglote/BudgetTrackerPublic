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
using utils.Exceptions;

namespace webapi.Controllers;

[Authorize]
[Route("[controller]")]
public class CreditsController : BaseController {
  private readonly CreditsService _creditsService;
  private readonly IAccountsService _accountsService;
  private readonly ITransferService _transferService;

  public CreditsController(CreditsService creditsService, IAccountsService accountsService, ITransferService transferService, IUserFactory _userFactory) : base(_userFactory) {
    _creditsService = creditsService;
    _accountsService = accountsService;
    _transferService = transferService;
  }

  [HttpGet("{id}")]
  [ValidateAppUser]
  public ActionResult<ItemView> GetItem([FromRoute][Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var itemView = _creditsService.GetViewById(CurrentAppUser, id);

    if (itemView == null) {
      throw new ApiException("Unable to find object");
    }

    if (itemView.OwnerUserId != CurrentAppUser.Id) {
      throw new ApiException("Access violation");
    }

    return Ok(itemView);
  }

  [HttpPost("items")]
  public ActionResult<CreditsResponse> GetItems([FromBody] TableLazyLoadEvent tableLazyLoadEvent) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var (Items, TotalCount) = _creditsService.GetItems(CurrentAppUser, tableLazyLoadEvent);

    return Ok(new CreditsResponse() { Items = Items, TotalCount = TotalCount });
  }

  [HttpDelete("{id}")]
  public ActionResult<bool> DeleteItem([FromRoute][Required] string id) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var creditItemView = _creditsService.GetViewById(CurrentAppUser, id);
    if (creditItemView == null || (creditItemView.IsActive && creditItemView.Plan != null && creditItemView.Plan.Any() && creditItemView.Plan.Any(x => !x.isPaid))) {
      return BadRequest(false);
    }

    var result = _creditsService.DeleteItem(CurrentAppUser, creditItemView);
    return Ok(result);
  }

  [HttpPost()]
  public ActionResult<bool> Upsert([FromBody][Required] CreditItemView creditItemView) {
    //  TODO: добавить ограничение на количество кредитов тот - выводить предупреждение также на UI - придется разделить добавление и обновление POST/PUT
    if (CurrentAppUser == null) { return BadRequest(); }
    var result = _creditsService.UpsertItem(CurrentAppUser, creditItemView, out bool isAdded);
    return Ok(result);
  }

  [HttpPost("activate/{id}")]
  public ActionResult<bool> Activate([FromRoute][Required] string id, [FromBody][Required] ActivationData activationData) {
    if (CurrentAppUser == null) { return BadRequest(); }
    var item = _creditsService.GetViewById(CurrentAppUser, id);

    if (item == null) {
      return BadRequest(false);
    }

    if (item.IsActive) {
      return BadRequest(false);
    }

    item.IsActive = true;

    var result = _creditsService.UpsertItem(CurrentAppUser, item, out bool isAdded);

    if (!isAdded && !string.IsNullOrEmpty(activationData.AccountId)) {

      if (item.AccountId == null) {
        return BadRequest(false);
      }

      var fromAccount = _accountsService.GetItemById(CurrentAppUser, item.AccountId);
      var toAccount = _accountsService.GetItemById(CurrentAppUser, activationData.AccountId);

      if (fromAccount == null || toAccount == null || fromAccount.Currency != toAccount.Currency) {
        BadRequest(false);
      }

      if (fromAccount != null && toAccount != null) {
        var transfer = new TransferItem() {
          FromAccount = fromAccount,
          ToAccount = toAccount,
          Date = DateTime.UtcNow,
          FromQuantity = item.Quantity,
          ToQuantity = item.Quantity
        };

        _transferService.AddTransfer(CurrentAppUser, transfer);
      }
    }

    return Ok(result);
  }

  [HttpGet("payments/next")]
  public ActionResult<List<PaymentInfo>> GetNextPayments() {
    if (CurrentAppUser == null) { return BadRequest(); }
    var result = _creditsService.GetTopPaymentsForActiveCredits(CurrentAppUser);

    return result == null ? Ok(new List<PaymentInfo>()) : Ok(result);
  }
}
