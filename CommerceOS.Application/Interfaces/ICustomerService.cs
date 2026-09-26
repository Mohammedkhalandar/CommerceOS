using CommerceOS.Application.DTOs.Customers;

namespace CommerceOS.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(
        CreateCustomerDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<CustomerDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}