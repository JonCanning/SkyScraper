using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SkyScraper
{
    public static class Robots
    {
        const string DisallowRegex = @"^Disallow:\s";
        const string AllowRegex = @"^Allow:\s";
        static Dictionary<string, string> aggregatedRules;

        public static void Load(string robotsTxt, string userAgent = "*")
        {
            aggregatedRules = new Dictionary<string, string>();
            var allRulesList = new List<string>();
            var botRulesList = new List<string>();
            string currentAgentName = null;
            robotsTxt = Regex.Replace(robotsTxt, @"\r\n|\n\r|\n|\r", "\r\n");
            var stringReader = new StringReader(robotsTxt);
            while (stringReader.Peek() > -1)
            {
                var line = stringReader.ReadLine() ?? string.Empty;
                var agentNameMatch = Regex.Match(line, @"(?<=^User-agent:\s)[^\s]*");
                if (agentNameMatch.Success)
                {
                    currentAgentName = agentNameMatch.Value;
                    continue;
                }
                if ((currentAgentName == "*" || currentAgentName == userAgent) && line.IsRule())
                    if (currentAgentName == "*")
                        allRulesList.Add(line);
                    else
                        botRulesList.Add(line);
            }
            aggregatedRules = botRulesList.Concat(allRulesList).ToDictionary(x => x, x => x.AsRegexRule());
        }

        public static bool PathIsAllowed(string path)
        {
            foreach (var rule in aggregatedRules.Where(x => Regex.IsMatch(path, x.Value)).Select(x => x.Key))
            {
                if (Regex.IsMatch(rule, DisallowRegex))
                    return false;
                if (Regex.IsMatch(rule, AllowRegex))
                    return true;
            }
            return true;
        }

        static bool IsRule(this string input)
        {
            var rules = string.Format("{0}|{1}", DisallowRegex, AllowRegex);
            return Regex.IsMatch(input, rules);
        }

        static string AsRegexRule(this string input)
        {
            var regex = Regex.IsMatch(input, DisallowRegex) ? DisallowRegex : AllowRegex;
            regex = string.Format(@"(?<={0})[^\s]*", regex);
            input = Regex.Match(input, regex).Value;
            input = input.Replace("*", ".*");
            return string.Format("^{0}.*$", input);
        }
    }
}