using System.Linq;

namespace LightTrace.Extensions
{
    internal static class StringExtensions
    {
        public static string TakeFirst(this string str, int count) => 
            string.Join("", str.Take(count));
    }
}
