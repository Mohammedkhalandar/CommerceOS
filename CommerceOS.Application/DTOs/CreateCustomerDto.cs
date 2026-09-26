namespace CommerceOS.Application.DTOs.Customers;

public class CreateCustomerDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;
}