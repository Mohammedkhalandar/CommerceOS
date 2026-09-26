namespace CommerceOS.Application.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Brand { get; set; } = null!;
}