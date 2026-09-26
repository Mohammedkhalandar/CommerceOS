using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class Address : BaseEntity
{
    private Address()
    {
    }

    public Address(
        string fullName,
        string addressLine1,
        string city,
        string state,
        string postalCode,
        string country)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");

        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentException("Address is required.");

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.");

        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State is required.");

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code is required.");

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country is required.");

        FullName = fullName;
        AddressLine1 = addressLine1;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public Guid CustomerId { get; private set; }

    public string FullName { get; private set; } = null!;

    public string AddressLine1 { get; private set; } = null!;

    public string? AddressLine2 { get; private set; }

    public string City { get; private set; } = null!;

    public string State { get; private set; } = null!;

    public string PostalCode { get; private set; } = null!;

    public string Country { get; private set; } = null!;

    public bool IsDefault { get; private set; }

    public void SetDefault()
    {
        IsDefault = true;
        MarkUpdated();
    }

    public void RemoveAsDefault()
    {
        IsDefault = false;
        MarkUpdated();
    }
}