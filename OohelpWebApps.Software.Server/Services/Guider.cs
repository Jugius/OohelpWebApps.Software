using System.Runtime.InteropServices;

namespace OohelpWebApps.Software.Server.Services;


/// <summary>
/// By Nick Chapsas, code is here : https://www.youtube.com/watch?v=B2yOjLyEZk0
/// </summary>
public class Guider
{
    private const char EqualsChar = '=';
    private const char NegationChar = '-';
    private const char UnderscoreChar = '_';
    private const char SlashChar = '/';
    private const byte SlashByte = (byte)'/';
    private const char PlusChar = '+';
    private const byte PlusByte = (byte)'+';

    public static Guid ToGuidFromString(ReadOnlySpan<char> str)
    {
        Span<char> base64Chars = stackalloc char[24];

        for (int i = 0; i < 22; i++)
        {
            base64Chars[i] = str[i] switch
            {
                NegationChar => SlashChar,
                UnderscoreChar => PlusChar,
                _ => str[i]
            };
        }
        base64Chars[22] = EqualsChar;
        base64Chars[23] = EqualsChar;

        Span<byte> valueBites = stackalloc byte[16];
        Convert.TryFromBase64Chars(base64Chars, valueBites, out _);
        return new Guid(valueBites);
    }

    public static string ToStringFromGuid(Guid guid)
    {
        Span<byte> guidBites = stackalloc byte[16];
        Span<byte> base64Bytes = stackalloc byte[24];

        MemoryMarshal.TryWrite(guidBites, ref guid);
        System.Buffers.Text.Base64.EncodeToUtf8(guidBites, base64Bytes, out _, out _);

        Span<char> resultChars = stackalloc char[22];

        for (int i = 0; i < 22; i++)
        {
            resultChars[i] = base64Bytes[i] switch
            {
                SlashByte => NegationChar,
                PlusByte => UnderscoreChar,
                _ => (char)base64Bytes[i]
            };
        }
        return new string(resultChars);
    }
}

