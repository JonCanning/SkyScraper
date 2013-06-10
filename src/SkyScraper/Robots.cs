using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SkyScraper
{
    public static class Robots
    {
        const string DisallowRegex = @"^Disallow:\s";
        const string AllowRegex = @"^Allow:\s";
        static IEnumerable<Rule> aggregatedRules = new Rule[0];

        public static void Load(string robotsTxt, string userAgent = null)
        {
            if (string.IsNullOrEmpty(robotsTxt))
                return;
            var allRulesList = new List<string>();
            var botRulesList = new List<string>();
            var currentAgents = new string[0];
            robotsTxt = Regex.Replace(robotsTxt, @"\r\n|\n\r|\n|\r", "\r\n");
            var lines = new Queue<string>(robotsTxt.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            while (lines.Any())
            {
                var readAgents = lines.ReadAgents().ToArray();
                currentAgents = readAgents.Any() ? readAgents : currentAgents;
                var line = lines.Dequeue();
                if (line.IsRule() && currentAgents.Any() && (currentAgents.First() == "*" || currentAgents.Contains(userAgent)))
                    if (currentAgents.First() == "*")
                        allRulesList.Add(line);
                    else
                        botRulesList.Add(line);
            }
            aggregatedRules = botRulesList.Concat(allRulesList).Select(x => new Rule(x.AsRegexRule(), Regex.IsMatch(x, AllowRegex))).ToArray();
        }

        static IEnumerable<string> ReadAgents(this Queue<string> lines)
        {
            while (lines.Peek().StartsWith("User-agent: "))
            {
                var line = lines.Dequeue();
                yield return line.Substring(line.IndexOf(' ') + 1);
            }
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