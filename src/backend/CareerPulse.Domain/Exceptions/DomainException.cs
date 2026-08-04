namespace CareerPulse.Domain.Exceptions;

/// <summary>
/// Represents a violation of a domain invariant or business rule.
/// Mapped to HTTP 409 Conflict by the global exception middleware.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
