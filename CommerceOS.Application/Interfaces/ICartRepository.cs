using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetByIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default);

    Task<CartItem?> GetCartItemAsync(
        Guid cartId,
        Guid productVariantId,
        CancellationToken cancellationToken = default);

    Task<ProductVariant?> GetProductVariantAsync(
        Guid productVariantId,
        CancellationToken cancellationToken = default);

    Task AddCartAsync(
        Cart cart,
        CancellationToken cancellationToken = default);

    Task AddCartItemAsync(
        CartItem cartItem,
        CancellationToken cancellationToken = default);

    void RemoveCartItem(CartItem cartItem);

    void RemoveCart(Cart cart);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}