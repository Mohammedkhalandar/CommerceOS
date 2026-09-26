namespace CommerceOS.Application.DTOs;

public class CartDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public List<CartItemDto> Items { get; set; } = new();

    public int TotalItems => Items.Sum(x => x.Quantity);

    public decimal SubTotal => Items.Sum(x => x.TotalPrice);
}