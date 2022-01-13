using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FallenMoon.Library.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxChars)
        {
            if (value == null) return string.Empty;

            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "&hellip;";
        }
    }

    public static class HTMLParser
    {
        public static string Link(this string s, string url)
        {
            return string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, s);
        }
        public static string ParseURL(this string s)
        {
            return Regex.Replace(s, @"(http(s)?://)([\w-]+\.)+[\w-]+(/\S\w[\w- ;,./?%&=]\S*)?", new MatchEvaluator(HTMLParser.URL));
        }
        public static string ParseUsername(this string s)
        {
            return Regex.Replace(s, "(@)((?:[A-Za-z0-9-_]*))", new MatchEvaluator(HTMLParser.Username));
        }
        public static string ParseHashtag(this string s)
        {
            return Regex.Replace(s, "(#)((?:[A-Za-z0-9-_]*))", new MatchEvaluator(HTMLParser.Hashtag));
        }
        private static string Hashtag(Match m)
        {
            string x = m.ToString();
            string tag = x.Replace("#", "");
            return x.Link("https://www.twitter.com/hashtag/" + tag);
        }
        private static string Username(Match m)
        {
            string x = m.ToString();
            string username = x.Replace("@", "");
            return x.Link("https://www.twitter.com/" + username);
        }
        private static string URL(Match m)
        {
            string x = m.ToString();
            return x.Link(x);
        }
    }
}
