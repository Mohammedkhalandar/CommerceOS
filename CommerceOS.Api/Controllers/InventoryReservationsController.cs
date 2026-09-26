using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryReservationsController : ControllerBase
{
    private readonly IInventoryReservationService
        _reservationService;

    public InventoryReservationsController(
        IInventoryReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<InventoryReservationDto>>
        GetByOrderId(
            Guid orderId,
            CancellationToken cancellationToken)
    {
        var reservation =
            await _reservationService.GetByOrderIdAsync(
                orderId,
                cancellationToken);

        if (reservation is null)
        {
            return NotFound(new
            {
                message = "Inventory reservation was not found."
            });
        }

        return Ok(reservation);
    }

    [HttpGet("inventory/{inventoryId:guid}")]
    public async Task<
        ActionResult<IReadOnlyList<InventoryReservationDto>>>
        GetByInventoryId(
            Guid inventoryId,
            CancellationToken cancellationToken)
    {
        var reservations =
            await _reservationService.GetByInventoryIdAsync(
                inventoryId,
                cancellationToken);

        return Ok(reservations);
    }
}