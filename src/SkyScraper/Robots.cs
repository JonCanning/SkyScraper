using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SkyScraper
{
    public static class Robots
    {
        const string DisallowRegex = @"^Disallow:\s";
        const string AllowRegex = @"^Allow:\s";
        static string[] disallow;
        static string[] allow;

        public static void Load(string robotsTxt, string userAgent = "*")
        {
            var addRule = false;
            robotsTxt = Regex.Replace(robotsTxt, @"\r\n|\n\r|\n|\r", "\r\n");
            var stringReader = new StringReader(robotsTxt);
            var rules = new StringBuilder();
            while (stringReader.Peek() > -1)
            {
                var line = stringReader.ReadLine() ?? string.Empty;
                var agentName = Regex.Match(line, @"(?<=^User-agent:\s).*").Value;
                addRule = string.IsNullOrEmpty(agentName) ? addRule : agentName == "*" || agentName == userAgent;
                if (addRule && line.IsRule())
                    rules.AppendLine(line);
            }
            var allRules = rules.ToString();
            disallow = allRules.CreateRegexRules(DisallowRegex);
            allow = allRules.CreateRegexRules(AllowRegex);
        }

        public static bool PathIsAllowed(string pathAndQuery)
        {
            var disallowed = disallow.Any(x => Regex.IsMatch(pathAndQuery, x));
            return !disallowed && allow.Any(x => Regex.IsMatch(pathAndQuery, x));
            //return allow.Any(x => Regex.IsMatch(pathAndQuery, x)) || disallow.All(x => !Regex.IsMatch(pathAndQuery, x));
        }

        static bool IsRule(this string input)
        {
            var rules = string.Format("{0}|{1}", DisallowRegex, AllowRegex);
            return Regex.IsMatch(input, rules);
        }

        static string[] CreateRegexRules(this string allRules, string regex)
        {
            return Regex.Matches(allRules, string.Format(@"(?<={0})[^\s]*", regex), RegexOptions.Multiline).Cast<Match>().Select(x => x.Groups[0].Value.RegexRule()).ToArray();
        }

        static string RegexRule(this string input)
        {
            input = input.Replace("*", ".*");
            return string.Format("^{0}.*$", input);
        }
    }
}