namespace CommerceOS.Domain.Exceptions;

public class InventoryConcurrencyException : Exception
{
    public InventoryConcurrencyException()
        : base("Inventory was changed by another request. Please try again.")
    {
    }

    public InventoryConcurrencyException(string message)
        : base(message)
    {
    }

    public InventoryConcurrencyException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}