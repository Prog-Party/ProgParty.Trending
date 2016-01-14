using System.Text.RegularExpressions;

namespace ProgParty.Core.Extension
{
    public static class StringExtension
    {
        public static string RemoveDoubleSpaces(this string value) => Regex.Replace(value, @"\s+", " ");
    }
}
