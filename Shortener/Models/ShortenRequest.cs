namespace Shortener.Models;

public class ShortenRequest
{
    public string Url { get; set; } = string.Empty;
    public string? CustomShortenedUrl { get; set; }
}