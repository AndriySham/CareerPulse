namespace CareerPulse.Domain.ValueObjects;

/// <summary>
/// Value Object: Website URL with basic format validation.
/// Embedded in Company aggregate per DOMAIN_MODEL.md §4.
/// </summary>
public sealed record WebsiteUrl
{
    public string Value { get; init; } = string.Empty;

    private WebsiteUrl() { }

    public static WebsiteUrl Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty.", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException($"'{url}' is not a valid HTTP/HTTPS URL.", nameof(url));

        return new WebsiteUrl { Value = uri.ToString() };
    }

    public override string ToString() => Value;
}
