namespace CareerPulse.Application.Exceptions;

/// <summary>
/// Represents a conflict that prevents the requested operation from being completed.
/// Mapped to HTTP 409 Conflict by the global exception middleware.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}