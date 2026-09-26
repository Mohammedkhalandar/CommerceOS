using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(
        IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("variant/{productVariantId:guid}")]
    public async Task<ActionResult<InventoryDto>> GetByVariantId(
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        var inventory =
            await _inventoryService.GetByVariantIdAsync(
                productVariantId,
                cancellationToken);

        if (inventory is null)
        {
            return NotFound(new
            {
                message = "Inventory was not found."
            });
        }

        return Ok(inventory);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryDto>> Create(
        Guid productVariantId,
        int initialQuantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var inventory =
                await _inventoryService.CreateAsync(
                    productVariantId,
                    initialQuantity,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetByVariantId),
                new
                {
                    productVariantId =
                        inventory.ProductVariantId
                },
                inventory);
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
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("variant/{productVariantId:guid}/stock")]
    public async Task<IActionResult> AddStock(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated =
                await _inventoryService.AddStockAsync(
                    productVariantId,
                    quantity,
                    cancellationToken);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Inventory was not found."
                });
            }

            return NoContent();
        }
        catch (InventoryConcurrencyException ex)
        {
            return Conflict(new
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
    }

    [HttpPost("variant/{productVariantId:guid}/reserve")]
    public async Task<IActionResult> ReserveStock(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated =
                await _inventoryService.ReserveStockAsync(
                    productVariantId,
                    quantity,
                    cancellationToken);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Inventory was not found."
                });
            }

            return NoContent();
        }
        catch (InventoryConcurrencyException ex)
        {
            return Conflict(new
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

    [HttpPost("variant/{productVariantId:guid}/release")]
    public async Task<IActionResult> ReleaseStock(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated =
                await _inventoryService.ReleaseStockAsync(
                    productVariantId,
                    quantity,
                    cancellationToken);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Inventory was not found."
                });
            }

            return NoContent();
        }
        catch (InventoryConcurrencyException ex)
        {
            return Conflict(new
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

    [HttpPost("variant/{productVariantId:guid}/confirm")]
    public async Task<IActionResult> ConfirmStock(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated =
                await _inventoryService.ConfirmStockAsync(
                    productVariantId,
                    quantity,
                    cancellationToken);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Inventory was not found."
                });
            }

            return NoContent();
        }
        catch (InventoryConcurrencyException ex)
        {
            return Conflict(new
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
}