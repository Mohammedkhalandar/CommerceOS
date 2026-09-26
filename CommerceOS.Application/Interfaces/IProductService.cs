using CommerceOS.Application.DTOs;

namespace CommerceOS.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> CreateAsync(
        CreateProductDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateProductDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}