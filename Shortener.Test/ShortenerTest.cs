using Shortener.Utils;

namespace Shortener.Test;

public class ShortenerTest
{
    [Fact]
    public void ShortenedUrl_IsSixDigits()
    {
        var shortenedUrl = Utils.Utils.GenerateUrl();
        Assert.True(shortenedUrl.Length == 6);
    }
}