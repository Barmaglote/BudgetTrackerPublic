using budget.core.DB.Interfaces;
using budget.core.Factories.Interfaces;
using budget.core.Models;
using budget.core.Services;
using budget.utils.ApiFilters;
using budget.webapi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Stripe;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace webapi.Controllers;

[Authorize]
[ValidateAppUser]
[Route("[controller]")]
public class PaymentController : BaseController {
  private readonly PaymentService _paymentService;
  private readonly ITransactionManager _transactionManager;

  public PaymentController(PaymentService paymentService, IUserFactory _userFactory, ITransactionManager transactionManager) : base(_userFactory) {
    _paymentService = paymentService;
    _transactionManager = transactionManager;
  }

  [HttpPost("result")]
  public async Task<ActionResult<bool>> AddPayment([FromBody][Required][NotNull] PaymentIntentWrapper paymentIntent) {
    if (CurrentAppUser == null) { return BadRequest(); }

    _transactionManager.StartTransaction();
    bool result;
    try {
      result = _paymentService.UpsertItem(CurrentAppUser, new PaymentIntentResultView(ObjectId.Empty, paymentIntent.PaymentIntent), out bool isAdded, _transactionManager);
      if (!result) { return BadRequest(false); }

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

  public class PaymentIntentWrapper {
    public PaymentIntent PaymentIntent { get; set; }
  }
}


