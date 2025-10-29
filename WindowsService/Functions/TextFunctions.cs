using System.Text.RegularExpressions;

namespace WindowsService.Functions
{
    internal class TextFunctions
    {
        public static Regex WildcardMatch(string text)
        {
            string patternString = Regex.Replace(text, ".",
            x =>
            {
                string y = x.Value;
                if (y.Equals("?")) { return "."; }
                else if (y.Equals("*")) { return ".*"; }
                else { return Regex.Escape(y); }
            });
            if (!patternString.StartsWith("*")) { patternString = "^" + patternString; }
            if (!patternString.EndsWith("*")) { patternString = patternString + "$"; }
            return new Regex(patternString, RegexOptions.IgnoreCase);
        }

        public static string FormatFileSize(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}
