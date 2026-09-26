using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class Category : BaseEntity
{
    private readonly List<Product> _products = [];

    private Category()
    {
    }

    public Category(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Category slug is required.");

        Name = name;
        Slug = slug;
    }

    public string Name { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        Name = name;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}