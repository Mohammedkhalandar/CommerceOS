using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductVariantRepository _variantRepository;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IProductVariantRepository variantRepository)
    {
        _inventoryRepository = inventoryRepository;
        _variantRepository = variantRepository;
    }

    public async Task<InventoryDto?> GetByVariantIdAsync(
        Guid productVariantId,
        CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByVariantIdAsync(
            productVariantId,
            cancellationToken);

        return inventory is null
            ? null
            : MapToDto(inventory);
    }

    public async Task<InventoryDto> CreateAsync(
        Guid productVariantId,
        int initialQuantity,
        CancellationToken cancellationToken = default)
    {
        if (productVariantId == Guid.Empty)
            throw new ArgumentException(
                "Product variant ID is required.");

        if (initialQuantity < 0)
            throw new ArgumentException(
                "Initial quantity cannot be negative.");

        var variant = await _variantRepository.GetByIdAsync(
            productVariantId,
            cancellationToken);

        if (variant is null)
            throw new KeyNotFoundException(
                "Product variant was not found.");

        var existingInventory =
            await _inventoryRepository.GetByVariantIdAsync(
                productVariantId,
                cancellationToken);

        if (existingInventory is not null)
            throw new InvalidOperationException(
                "Inventory already exists for this product variant.");

        var inventory = new Inventory(
            productVariantId,
            initialQuantity);

        await _inventoryRepository.AddAsync(
            inventory,
            cancellationToken);

        await _inventoryRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(inventory);
    }

    public async Task<bool> AddStockAsync(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var inventory = await GetInventoryAsync(
            productVariantId,
            cancellationToken);

        if (inventory is null)
            return false;

        inventory.AddStock(quantity);

        await _inventoryRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ReserveStockAsync(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var inventory = await GetInventoryAsync(
            productVariantId,
            cancellationToken);

        if (inventory is null)
            return false;

        inventory.ReserveStock(quantity);

        await _inventoryRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ReleaseStockAsync(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var inventory = await GetInventoryAsync(
            productVariantId,
            cancellationToken);

        if (inventory is null)
            return false;

        inventory.ReleaseReservation(quantity);

        await _inventoryRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ConfirmStockAsync(
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var inventory = await GetInventoryAsync(
            productVariantId,
            cancellationToken);

        if (inventory is null)
            return false;

        inventory.ConfirmReservation(quantity);

        await _inventoryRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private async Task<Inventory?> GetInventoryAsync(
        Guid productVariantId,
        CancellationToken cancellationToken)
    {
        return await _inventoryRepository.GetByVariantIdAsync(
            productVariantId,
            cancellationToken);
    }

    private static InventoryDto MapToDto(
        Inventory inventory)
    {
        return new InventoryDto
        {
            Id = inventory.Id,
            ProductVariantId = inventory.ProductVariantId,
            AvailableQuantity = inventory.AvailableQuantity,
            ReservedQuantity = inventory.ReservedQuantity,
            TotalQuantity = inventory.TotalQuantity,
            Version = inventory.Version,
            CreatedAtUtc = inventory.CreatedAtUtc,
            UpdatedAtUtc = inventory.UpdatedAtUtc
        };
    }
}