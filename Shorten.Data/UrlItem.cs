namespace Shorten.Data;

public class UrlItem
{
    public string Id { get; set; } = string.Empty;
    public string ShortenedUrl { get; set; } = string.Empty;
    public string MappedUrl { get; set; } = string.Empty;
    public int RedirectCount { get; set; } = 0;
}
