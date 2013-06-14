using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SkyScraper
{
    public static class Robots
    {
        const string Disallow = @"^Disallow:\s";
        const string Allow = @"^Allow:\s";
        static readonly Regex AllowRegex = new Regex(Allow);
        static readonly Regex Rules = new Regex(string.Format("{0}|{1}", Disallow, Allow));
        static ConcurrentQueue<Rule> aggregatedRules = new ConcurrentQueue<Rule>();

        public static string SiteMap { get; private set; }

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
                SetSiteMap(lines);
                var readAgents = lines.ReadAgents().ToArray();
                currentAgents = readAgents.Any() ? readAgents : currentAgents;
                if (!lines.Any())
                    continue;
                var line = lines.Dequeue();
                if (Rules.IsMatch(line) && currentAgents.Any() && (currentAgents.First() == "*" || currentAgents.Contains(userAgent)))
                    if (currentAgents.First() == "*")
                        allRulesList.Add(line);
                    else
                        botRulesList.Add(line);
            }
            aggregatedRules = new ConcurrentQueue<Rule>(botRulesList.AsRules().Concat(allRulesList.AsRules()));
        }

        static IEnumerable<Rule> AsRules(this IEnumerable<string> rules)
        {
            return rules.Select(x => new Rule(x.AsRegexRule(), AllowRegex.IsMatch(x)));
        } 

        static void SetSiteMap(this Queue<string> lines)
        {
            if (lines.Any() && lines.Peek().StartsWith("Sitemap: "))
                SiteMap = lines.Dequeue().Split(' ')[1];
        }

        static IEnumerable<string> ReadAgents(this Queue<string> lines)
        {
            while (lines.Any() && lines.Peek().StartsWith("User-agent: "))
                yield return lines.Dequeue().Split(' ')[1];
        }

        public static bool PathIsAllowed(string path)
        {
            foreach (var rule in aggregatedRules.Where(rule => rule.Regex.IsMatch(path)))
                return rule.IsAllowed;
            return true;
        }

        static Regex AsRegexRule(this string input)
        {
            input = input.Split(' ')[1];
            input = Regex.Escape(input);
            input = input.Replace("\\*", ".*");
            if (!input.EndsWith(".*"))
                input += ".*";
            input = string.Format("^{0}$", input);
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