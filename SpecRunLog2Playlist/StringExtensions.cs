using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool IsRegExMatch(this string text, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline);
            return regex.IsMatch(text);
        }

        public static string GetRegExMatch(this string text, string pattern)
        {
            var result = "";
            var regex = new Regex(pattern, RegexOptions.Singleline);
            var match = regex.Match(text);
            if (match.Success)
                result = match.Value;
            return result;
        }

        public static string GetRegExCaptureGroup(this string text, string pattern, int groupIndex = 1)
        {
            var result = "";
            var regex = new Regex(pattern, RegexOptions.Singleline);
            var match = regex.Match(text);
            if (match.Success)
                result = match.Groups[groupIndex].Value;
            return result;
        }

        public static string ReplaceRegExMatch(this string text, string pattern, string replacementText)
        {
            return Regex.Replace(text, pattern, replacementText, RegexOptions.Singleline);
        }

        public static string ReplaceRegExMatch(this string text, string pattern, MatchEvaluator replacementMethod)
        {
            return Regex.Replace(text, pattern, replacementMethod);
        }

    }
}