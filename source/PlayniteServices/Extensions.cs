namespace System;

public static class Extensions
{
    public static string UriCombine(this string baseUri, params string[] segments)
    {
        if (baseUri.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        if (segments.Length == 0)
        {
            return baseUri;
        }

        return segments.Aggregate(baseUri, (c, s) => $"{c.TrimEnd('/')}/{s.TrimStart('/')}");
    }
}
