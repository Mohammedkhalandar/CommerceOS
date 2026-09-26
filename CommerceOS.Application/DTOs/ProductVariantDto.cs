namespace CommerceOS.Application.DTOs;

public class ProductVariantDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}