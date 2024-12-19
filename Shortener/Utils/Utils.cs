namespace Shortener.Utils;

public static class Utils
{
    public static string GenerateUrl()
    {
        var options = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

        var rand = new Random();
        Span<char> result = stackalloc char[6];
        for (var i = 0; i < 6; i++)
        {
            result[i] = options[rand.Next(options.Length)];
        }

        return new string(result);
    }
}