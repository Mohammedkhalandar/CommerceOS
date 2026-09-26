using CommerceOS.Application.DTOs.Orders;
using CommerceOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        CreateOrderDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                order);
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
    public async Task<ActionResult<OrderDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return NotFound(new
            {
                message = "Order was not found."
            });
        }

        return Ok(order);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetByCustomerId(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetByCustomerIdAsync(
            customerId,
            cancellationToken);

        return Ok(orders);
    }

    [HttpPost("{id:guid}/payment-pending")]
    public async Task<IActionResult> MarkPaymentPending(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _orderService.MarkPaymentPendingAsync(
                id,
                cancellationToken);

            if (!updated)
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

    [HttpPost("{id:guid}/paid")]
    public async Task<IActionResult> MarkPaid(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _orderService.MarkPaidAsync(
                id,
                cancellationToken);

            if (!updated)
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

    [HttpPost("{id:guid}/processing")]
    public async Task<IActionResult> StartProcessing(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _orderService.StartProcessingAsync(
                id,
                cancellationToken);

            if (!updated)
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

    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> Ship(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _orderService.ShipAsync(
                id,
                cancellationToken);

            if (!updated)
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

    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> Deliver(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _orderService.DeliverAsync(
                id,
                cancellationToken);

            if (!updated)
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

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _orderService.CancelAsync(
                id,
                cancellationToken);

            if (!updated)
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
}