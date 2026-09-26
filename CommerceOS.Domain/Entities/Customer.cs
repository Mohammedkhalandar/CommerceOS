using CommerceOS.Domain.Common;
using System.Net;

namespace CommerceOS.Domain.Entities;

public class Customer : BaseEntity
{
    private readonly List<Address> _addresses = [];

    private Customer()
    {
    }

    public Customer(
        string firstName,
        string lastName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<Address> Addresses =>
        _addresses.AsReadOnly();

    public void UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        FirstName = firstName;
        LastName = lastName;

        MarkUpdated();
    }

    public void AddAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        _addresses.Add(address);

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