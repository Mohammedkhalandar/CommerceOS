using System.ComponentModel.DataAnnotations;

namespace CommerceOS.Application.DTOs;

public class AddToCartDto
{
    [Required]
    public Guid ProductVariantId { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }
}