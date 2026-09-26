using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class Product : BaseEntity
{
    private readonly List<ProductVariant> _variants = [];
    private readonly List<Category> _categories = [];

    private Product()
    {
    }

    public Product(
        string name,
        string description,
        string brand)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Product brand is required.");

        Name = name;
        Description = description;
        Brand = brand;
    }

    public string Name { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public string Brand { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<ProductVariant> Variants =>
        _variants.AsReadOnly();

    public IReadOnlyCollection<Category> Categories =>
        _categories.AsReadOnly();

    public void UpdateDetails(
        string name,
        string description,
        string brand)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Product brand is required.");

        Name = name;
        Description = description;
        Brand = brand;

        MarkUpdated();
    }

    public void AddVariant(ProductVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        _variants.Add(variant);
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }
}