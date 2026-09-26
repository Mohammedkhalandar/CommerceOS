using CommerceOS.Application.DTOs.Customers;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> CreateAsync(
        CreateCustomerDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException(
                "First name is required.");

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException(
                "Last name is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException(
                "Email is required.");

        var existingCustomer =
            await _customerRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

        if (existingCustomer is not null)
        {
            throw new InvalidOperationException(
                $"A customer with email '{request.Email}' already exists.");
        }

        var customer = new Customer(
            request.FirstName,
            request.LastName,
            request.Email);

        await _customerRepository.AddAsync(
            customer,
            cancellationToken);

        await _customerRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(customer);
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(
            cancellationToken);

        return customers
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(
            id,
            cancellationToken);

        return customer is null
            ? null
            : MapToDto(customer);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (customer is null)
            return false;

        customer.Deactivate();

        await _customerRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            IsActive = customer.IsActive,
            CreatedAtUtc = customer.CreatedAtUtc,
            UpdatedAtUtc = customer.UpdatedAtUtc
        };
    }
}