namespace CommerceOS.Application.DTOs;

public class CreateProductVariantDto
{
    public Guid ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }
}