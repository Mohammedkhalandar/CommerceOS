using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // ============================================
    // ADMIN ONLY — CREATE
    // ============================================

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductDto request,
        CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    // ============================================
    // AUTHENTICATED USERS — READ
    // ============================================

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    // ============================================
    // ADMIN ONLY — UPDATE
    // ============================================

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductDto request,
        CancellationToken cancellationToken)
    {
        var updated = await _productService.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    // ============================================
    // ADMIN ONLY — DELETE
    // ============================================

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _productService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}