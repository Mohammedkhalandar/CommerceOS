using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence.Repositories;

public class EfCartRepository : ICartRepository
{
    private readonly CommerceDbContext _context;

    public EfCartRepository(CommerceDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .FirstOrDefaultAsync(
                c => c.CustomerId == customerId,
                cancellationToken);
    }

    public async Task<Cart?> GetByIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .FirstOrDefaultAsync(
                c => c.Id == cartId,
                cancellationToken);
    }

    public async Task<CartItem?> GetCartItemAsync(
        Guid cartId,
        Guid productVariantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(
                x =>
                    x.CartId == cartId &&
                    x.ProductVariantId == productVariantId,
                cancellationToken);
    }

    public async Task<ProductVariant?> GetProductVariantAsync(
        Guid productVariantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProductVariants
            .Include(x => x.Product)
            .FirstOrDefaultAsync(
                x => x.Id == productVariantId,
                cancellationToken);
    }

    public async Task AddCartAsync(
        Cart cart,
        CancellationToken cancellationToken = default)
    {
        await _context.Carts.AddAsync(
            cart,
            cancellationToken);
    }

    public async Task AddCartItemAsync(
        CartItem cartItem,
        CancellationToken cancellationToken = default)
    {
        await _context.CartItems.AddAsync(
            cartItem,
            cancellationToken);
    }

    public void RemoveCartItem(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
    }

    public void RemoveCart(Cart cart)
    {
        _context.Carts.Remove(cart);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}