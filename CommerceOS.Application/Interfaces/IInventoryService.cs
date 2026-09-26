using CommerceOS.Application.DTOs;

namespace CommerceOS.Application.Interfaces;

public interface IInventoryService
{
    Task<InventoryDto?> GetByVariantIdAsync(
        Guid productVariantId,
        CancellationToken cancellationToken = default);

    Task<InventoryDto> CreateAsync(
        Guid productVariantId,
        int initialQuantity,
        CancellationToken cancellationToken = default);

    Task<bool> AddStockAsync(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default);

    Task<bool> ReserveStockAsync(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default);

    Task<bool> ReleaseStockAsync(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default);

    Task<bool> ConfirmStockAsync(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default);
}