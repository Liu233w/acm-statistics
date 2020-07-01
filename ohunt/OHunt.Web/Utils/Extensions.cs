using System.Collections.Generic;

namespace OHunt.Web.Utils
{
    public static class Extensions
    {
        public static string JoinToString(
            this IEnumerable<string> enumerable)
        {
            return enumerable.JoinToString("");
        }

        public static string JoinToString(
            this IEnumerable<string> enumerable,
            string separator)
        {
            return string.Join(separator, enumerable);
        }
    }
}
