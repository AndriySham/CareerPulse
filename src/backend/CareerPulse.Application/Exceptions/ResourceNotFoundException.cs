namespace CareerPulse.Application.Exceptions;

/// <summary>
/// Represents an attempt to access a resouse that does not exist.
/// Mapped to HTTP 404 NotFound by the global exception middleware.
/// </summary>
public sealed class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message) : base(message)
    {
    }
}