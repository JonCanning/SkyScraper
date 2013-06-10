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
        static IEnumerable<Rule> aggregatedRules = new Rule[0];

        public static void Load(string robotsTxt, string userAgent = "*")
        {
            if (string.IsNullOrEmpty(robotsTxt))
                return;
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
            aggregatedRules = botRulesList.Concat(allRulesList).Select(x => new Rule(x.AsRegexRule(), Regex.IsMatch(x, AllowRegex))).ToArray();
        }

        public static bool PathIsAllowed(string path)
        {
            foreach (var rule in aggregatedRules.Where(x => x.Regex.IsMatch(path)))
                return rule.IsAllowed;
            return true;
        }

        static bool IsRule(this string input)
        {
            var rules = string.Format("{0}|{1}", DisallowRegex, AllowRegex);
            return Regex.IsMatch(input, rules);
        }

        static Regex AsRegexRule(this string input)
        {
            input = input.Substring(input.IndexOf(' ') + 1);
            input = Regex.Match(input, "[^\\s]*").Value;
            input = input.Replace("*", ".*");
            input = string.Format("^{0}.*$", input);
            return new Regex(input);
        }

        class Rule
        {
            public Rule(Regex regex, bool isAllowed)
            {
                Regex = regex;
                IsAllowed = isAllowed;
            }

            public Regex Regex { get; private set; }
            public bool IsAllowed { get; private set; }
        }
    }
}