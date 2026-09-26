using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // GET: api/Carts/{customerId}
    [HttpGet("{customerId:guid}")]
    public async Task<ActionResult<CartDto>> GetCart(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var cart = await _cartService.GetCartAsync(
            customerId,
            cancellationToken);

        if (cart == null)
        {
            return NotFound(new
            {
                message = "Cart was not found."
            });
        }

        return Ok(cart);
    }

    // POST: api/Carts/{customerId}/items
    [HttpPost("{customerId:guid}/items")]
    public async Task<ActionResult<CartDto>> AddToCart(
        Guid customerId,
        [FromBody] AddToCartDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartService.AddToCartAsync(
                customerId,
                request,
                cancellationToken);

            return Ok(cart);
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
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: api/Carts/{customerId}/items/{productVariantId}
    [HttpPut("{customerId:guid}/items/{productVariantId:guid}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(
        Guid customerId,
        Guid productVariantId,
        [FromQuery] int quantity,
        CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartService.UpdateCartItemAsync(
                customerId,
                productVariantId,
                quantity,
                cancellationToken);

            if (cart == null)
            {
                return NotFound(new
                {
                    message = "Cart or cart item was not found."
                });
            }

            return Ok(cart);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // DELETE: api/Carts/{customerId}/items/{productVariantId}
    [HttpDelete("{customerId:guid}/items/{productVariantId:guid}")]
    public async Task<IActionResult> RemoveCartItem(
        Guid customerId,
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        var removed = await _cartService.RemoveCartItemAsync(
            customerId,
            productVariantId,
            cancellationToken);

        if (!removed)
        {
            return NotFound(new
            {
                message = "Cart or cart item was not found."
            });
        }

        return NoContent();
    }

    // DELETE: api/Carts/{customerId}
    [HttpDelete("{customerId:guid}")]
    public async Task<IActionResult> ClearCart(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var cleared = await _cartService.ClearCartAsync(
            customerId,
            cancellationToken);

        if (!cleared)
        {
            return NotFound(new
            {
                message = "Cart was not found."
            });
        }

        return NoContent();
    }
}