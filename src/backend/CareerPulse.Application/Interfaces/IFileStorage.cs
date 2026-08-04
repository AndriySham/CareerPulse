namespace CareerPulse.Application.Interfaces;

/// <summary>
/// Storage abstraction for binary file operations.
/// ADR 003: All file I/O goes through this interface.
/// Phase 1 Implementation: LocalFileStorage (storage/resumes/)
/// Phase 2 Implementation: GoogleDriveStorage (Google Drive API)
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Saves a file stream and returns a storage reference key.
    /// </summary>
    Task<string> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a file stream by its storage reference key.
    /// </summary>
    Task<Stream> GetFileAsync(string fileReference, CancellationToken ct = default);

    /// <summary>
    /// Deletes a file by its storage reference key.
    /// </summary>
    Task DeleteFileAsync(string fileReference, CancellationToken ct = default);

    /// <summary>
    /// Returns a temporary download URL for the file.
    /// Returns null for providers that do not support direct URLs (e.g., local storage).
    /// </summary>
    Task<string?> GetDownloadUrlAsync(string fileReference, TimeSpan expiry, CancellationToken ct = default);
}
