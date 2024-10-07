using System.Text;

namespace UrlShortener.Helper;

public static class Encurtador
{
    private static readonly string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private static readonly int BASE = alfabeto.Length;

    public static String Encode(int num)
    {

        if (num == 0)
            return alfabeto[0].ToString();

        StringBuilder stringBuilder = new StringBuilder();

        while(num > 0)
        {
            stringBuilder.Append(alfabeto[num % BASE]);
            num /= BASE;
        }
        return stringBuilder.ToString();

    }

}
