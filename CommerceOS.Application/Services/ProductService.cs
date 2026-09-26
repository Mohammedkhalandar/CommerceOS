using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;

    private static readonly TimeSpan CacheExpiration =
        TimeSpan.FromMinutes(10);

    private static string GetCacheKey(Guid id)
    {
        return $"product:{id}";
    }

    public ProductService(
        IProductRepository productRepository,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var product = new Product(
            request.Name,
            request.Description,
            request.Brand);

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        var dto = MapToDto(product);

        await _cacheService.SetAsync(
            GetCacheKey(product.Id),
            dto,
            CacheExpiration,
            cancellationToken);

        return dto;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(
            cancellationToken);

        return products
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(id);

        // 1. Check Redis first
        var cachedProduct =
            await _cacheService.GetAsync<ProductDto>(
                cacheKey,
                cancellationToken);

        if (cachedProduct is not null)
        {
            return cachedProduct;
        }

        // 2. Cache miss → query MySQL
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
            return null;

        var dto = MapToDto(product);

        // 3. Store result in Redis
        await _cacheService.SetAsync(
            cacheKey,
            dto,
            CacheExpiration,
            cancellationToken);

        return dto;
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateProductDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
            return false;

        product.UpdateDetails(
            request.Name,
            request.Description,
            request.Brand);

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        // Remove old cached version
        await _cacheService.RemoveAsync(
            GetCacheKey(id),
            cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
            return false;

        product.Deactivate();

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        // Remove old cached version
        await _cacheService.RemoveAsync(
            GetCacheKey(id),
            cancellationToken);

        return true;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            IsActive = product.IsActive,
            CreatedAtUtc = product.CreatedAtUtc,
            UpdatedAtUtc = product.UpdatedAtUtc
        };
    }
}