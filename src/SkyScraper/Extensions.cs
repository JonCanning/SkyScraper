using System;

namespace SkyScraper
{
    public static class Extensions
    {
        public static bool LinkIsLocal(this string link, string baseUri)
        {
            if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
            {
                if (!link.StartsWith(baseUri) || link.StartsWith("//"))
                    return false;
            }
            return Uri.IsWellFormedUriString(link, UriKind.Relative) || link.StartsWith(baseUri);
        }

        public static bool LinkDoesNotContainAnchor(this string str)
        {
            return !str.Contains("#");
        }

        public static void Try<T>(this T obj, Action<T> action)
        {
            try
            {
                action(obj);
            }
            catch { }
        }
    }
}