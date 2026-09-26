using CommerceOS.Application.DTOs;

namespace CommerceOS.Application.Interfaces;

public interface IProductVariantService
{
    Task<ProductVariantDto> CreateAsync(
        CreateProductVariantDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductVariantDto>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<ProductVariantDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateProductVariantDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}