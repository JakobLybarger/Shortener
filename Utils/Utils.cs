using System.Text;

namespace Shortener.Utils;

public class Utils
{
    public static string GenerateUrl()
    {
        string options = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

        Random rand = new();
        Span<char> result = stackalloc char[6];
        for (var i = 0; i < 6; i++)
        {
            result[i] = options[rand.Next(options.Length)];
        }

        return new string(result);
    }
}