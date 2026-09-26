using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto?> GetCartAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(
            customerId,
            cancellationToken);

        if (cart == null)
            return null;

        return MapToDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(
        Guid customerId,
        AddToCartDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID is required.");

        if (request.Quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        // --------------------------------------------------
        // 1. Find product variant
        // --------------------------------------------------

        var productVariant = await _cartRepository.GetProductVariantAsync(
            request.ProductVariantId,
            cancellationToken);

        if (productVariant == null)
            throw new KeyNotFoundException(
                "Product variant was not found.");

        if (!productVariant.IsActive)
            throw new InvalidOperationException(
                "This product variant is not active.");

        // --------------------------------------------------
        // 2. Find customer's cart
        // --------------------------------------------------

        var cart = await _cartRepository.GetByCustomerIdAsync(
            customerId,
            cancellationToken);

        // --------------------------------------------------
        // 3. Create cart if it doesn't exist
        // --------------------------------------------------

        if (cart == null)
        {
            cart = new Cart(customerId);

            await _cartRepository.AddCartAsync(
                cart,
                cancellationToken);
        }

        // --------------------------------------------------
        // 4. Add product to cart
        //
        // Cart.AddItem() automatically:
        // - creates a CartItem if it doesn't exist
        // - increases quantity if it already exists
        // --------------------------------------------------

        cart.AddItem(
            request.ProductVariantId,
            request.Quantity,
            productVariant.Price);

        // --------------------------------------------------
        // 5. Save
        // --------------------------------------------------

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        // --------------------------------------------------
        // 6. Reload cart
        // --------------------------------------------------

        var updatedCart =
            await _cartRepository.GetByCustomerIdAsync(
                customerId,
                cancellationToken);

        if (updatedCart == null)
            throw new InvalidOperationException(
                "Cart could not be loaded after saving.");

        return MapToDto(updatedCart);
    }

    public async Task<CartDto?> UpdateCartItemAsync(
        Guid customerId,
        Guid productVariantId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var cart = await _cartRepository.GetByCustomerIdAsync(
            customerId,
            cancellationToken);

        if (cart == null)
            return null;

        var cartItem = await _cartRepository.GetCartItemAsync(
            cart.Id,
            productVariantId,
            cancellationToken);

        if (cartItem == null)
            return null;

        // Use domain method instead of assigning Quantity directly.
        cartItem.UpdateQuantity(quantity);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        var updatedCart =
            await _cartRepository.GetByCustomerIdAsync(
                customerId,
                cancellationToken);

        if (updatedCart == null)
            return null;

        return MapToDto(updatedCart);
    }

    public async Task<bool> RemoveCartItemAsync(
        Guid customerId,
        Guid productVariantId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(
            customerId,
            cancellationToken);

        if (cart == null)
            return false;

        var cartItem = await _cartRepository.GetCartItemAsync(
            cart.Id,
            productVariantId,
            cancellationToken);

        if (cartItem == null)
            return false;

        _cartRepository.RemoveCartItem(cartItem);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ClearCartAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(
            customerId,
            cancellationToken);

        if (cart == null)
            return false;

        _cartRepository.RemoveCart(cart);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,

            Items = cart.Items
                .Select(item => new CartItemDto
                {
                    Id = item.Id,
                    ProductVariantId = item.ProductVariantId,

                    Sku = item.ProductVariant?.Sku
                          ?? string.Empty,

                    ProductName =
                        item.ProductVariant?.Product?.Name
                        ?? string.Empty,

                    // Use the price stored in CartItem.
                    // This preserves the price at the time
                    // the item was added to the cart.
                    UnitPrice = item.UnitPrice,

                    Quantity = item.Quantity
                })
                .ToList()
        };
    }
}