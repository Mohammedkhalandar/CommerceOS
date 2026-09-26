using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create(
        Guid orderId,
        decimal amount,
        string provider,
        CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentService.CreateAsync(
                orderId,
                amount,
                provider,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = payment.Id },
                payment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentService.GetByIdAsync(
            id,
            cancellationToken);

        if (payment is null)
        {
            return NotFound(new
            {
                message = "Payment was not found."
            });
        }

        return Ok(payment);
    }

    [HttpPost("{id:guid}/process")]
    public async Task<IActionResult> Process(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var processed = await _paymentService.ProcessAsync(
                id,
                cancellationToken);

            if (!processed)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("{id:guid}/succeed")]
    public async Task<IActionResult> Succeed(
        Guid id,
        string transactionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var succeeded = await _paymentService.SucceedAsync(
                id,
                transactionId,
                cancellationToken);

            if (!succeeded)
                return NotFound();

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("{id:guid}/fail")]
    public async Task<IActionResult> Fail(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var failed = await _paymentService.FailAsync(
                id,
                cancellationToken);

            if (!failed)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> Refund(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var refunded = await _paymentService.RefundAsync(
                id,
                cancellationToken);

            if (!refunded)
                return NotFound();

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}