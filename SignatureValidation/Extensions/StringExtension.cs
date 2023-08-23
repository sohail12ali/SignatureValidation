namespace SignatureValidation.Extensions;

public static class StringExtension
{
    /// <summary>
    /// Custom Triming method to operate on string
    /// </summary>
    /// <param name="text"></param>
    /// <returns>string</returns>
    public static string Tm(this string text)
    {
        return text.TrimStart().TrimEnd();
    }

    /// <summary>
    /// Custom Triming method to operate on string and convert it to upper case
    /// </summary>
    /// <param name="text"></param>
    /// <returns>string</returns>
    public static string Up(this string text)
    {
        return text.TrimStart().TrimEnd().ToUpper();
    }

    /// <summary>
    /// Custom Triming method to operate on string and convert it to upper case and revmove all blank spaces from the text
    /// </summary>
    /// <param name="text"></param>
    /// <returns>string</returns>
    public static string UpTmW(this string text)
    {
        return text.TrimStart().TrimEnd().ToUpper().Replace(" ", string.Empty);
    }
}