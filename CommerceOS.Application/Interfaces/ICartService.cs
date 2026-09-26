using CommerceOS.Application.DTOs;

namespace CommerceOS.Application.Interfaces;

public interface ICartService
{
    Task<CartDto?> GetCartAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<CartDto> AddToCartAsync(
        Guid customerId,
        AddToCartDto request,
        CancellationToken cancellationToken = default);

    Task<CartDto?> UpdateCartItemAsync(
        Guid customerId,
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveCartItemAsync(
        Guid customerId,
        Guid productVariantId,
        CancellationToken cancellationToken = default);

    Task<bool> ClearCartAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
}