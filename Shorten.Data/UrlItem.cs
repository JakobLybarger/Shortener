namespace Shorten.Data;

public class UrlItem
{
    public string Id { get; set; }
    public string ShortenedUrl { get; set; }
    public string MappedUrl { get; set; }
    public int RedirectCount { get; set; }
}
