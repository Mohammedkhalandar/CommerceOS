using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantsController : ControllerBase
{
    private readonly IProductVariantService _variantService;

    public ProductVariantsController(
        IProductVariantService variantService)
    {
        _variantService = variantService;
    }

    [HttpPost]
    public async Task<ActionResult<ProductVariantDto>> Create(
        CreateProductVariantDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var variant = await _variantService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = variant.Id },
                variant);
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

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ProductVariantDto>>> GetByProductId(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var variants = await _variantService.GetByProductIdAsync(
            productId,
            cancellationToken);

        return Ok(variants);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductVariantDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var variant = await _variantService.GetByIdAsync(
            id,
            cancellationToken);

        if (variant is null)
            return NotFound();

        return Ok(variant);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductVariantDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _variantService.UpdateAsync(
                id,
                request,
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _variantService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}