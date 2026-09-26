using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly IProductVariantRepository _variantRepository;
    private readonly IProductRepository _productRepository;

    public ProductVariantService(
        IProductVariantRepository variantRepository,
        IProductRepository productRepository)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
    }

    public async Task<ProductVariantDto> CreateAsync(
        CreateProductVariantDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
            throw new KeyNotFoundException(
                "Product was not found.");

        var existingVariant = await _variantRepository.GetBySkuAsync(
            request.Sku,
            cancellationToken);

        if (existingVariant is not null)
            throw new InvalidOperationException(
                $"A product variant with SKU '{request.Sku}' already exists.");

        var variant = new ProductVariant(
            request.ProductId,
            request.Sku,
            request.Price,
            request.Color,
            request.Size);

        await _variantRepository.AddAsync(
            variant,
            cancellationToken);

        await _variantRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(variant);
    }

    public async Task<IReadOnlyList<ProductVariantDto>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var variants = await _variantRepository.GetByProductIdAsync(
            productId,
            cancellationToken);

        return variants
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProductVariantDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var variant = await _variantRepository.GetByIdAsync(
            id,
            cancellationToken);

        return variant is null
            ? null
            : MapToDto(variant);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateProductVariantDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var variant = await _variantRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (variant is null)
            return false;

        var existingVariant = await _variantRepository.GetBySkuAsync(
            request.Sku,
            cancellationToken);

        if (existingVariant is not null &&
            existingVariant.Id != id)
        {
            throw new InvalidOperationException(
                $"A product variant with SKU '{request.Sku}' already exists.");
        }

        variant.UpdateDetails(
            request.Sku,
            request.Price,
            request.Color,
            request.Size);

        await _variantRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var variant = await _variantRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (variant is null)
            return false;

        variant.Deactivate();

        await _variantRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static ProductVariantDto MapToDto(
        ProductVariant variant)
    {
        return new ProductVariantDto
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            Sku = variant.Sku,
            Price = variant.Price,
            Color = variant.Color,
            Size = variant.Size,
            IsActive = variant.IsActive,
            CreatedAtUtc = variant.CreatedAtUtc,
            UpdatedAtUtc = variant.UpdatedAtUtc
        };
    }
}