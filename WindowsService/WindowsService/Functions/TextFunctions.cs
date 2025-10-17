using System.Text.RegularExpressions;

namespace WindowsService.WindowsService.Functions
{
    public class TextFunctions
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
    }
}
