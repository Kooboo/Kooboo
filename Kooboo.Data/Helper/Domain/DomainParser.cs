//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nager.PublicSuffix
{
    public class DomainParser
    {
        private DomainDataStructure _domainDataStructure;
        private readonly ITldRuleProvider _ruleProvider;

        public DomainParser(IEnumerable<TldRule> rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            this.AddRules(rules);
        }

        public DomainParser(ITldRuleProvider ruleProvider)
        {
            if (ruleProvider == null)
            {
                throw new ArgumentNullException("ruleProvider");
            }

            this._ruleProvider = ruleProvider;
            this.AddRules(ruleProvider.BuildAsync().Result);
        }

        public DomainParser()
        {
            var reader = Kooboo.Data.Embedded.EmbeddedHelper.GetStreamReader("Kooboo.Data.tld.dat", typeof(Kooboo.Data.Models.WebSite));
            string data = reader.ReadToEnd(); 

            var ruleParser = new TldRuleParser();
            var rules = ruleParser.ParseRules(data);
            this.AddRules(rules); 
        }

        public void AddRules(IEnumerable<TldRule> tldRules)
        {
            this._domainDataStructure = new DomainDataStructure("*", new TldRule("*"));

            foreach (var tldRule in tldRules)
            {
                this.AddRule(tldRule);
            }
        }

        public void AddRule(TldRule tldRule)
        {
            var structure = this._domainDataStructure;
            var domainPart = string.Empty;

            var parts = tldRule.Name.Split('.').Reverse().ToList();
            for (var i = 0; i < parts.Count; i++)
            {
                domainPart = parts[i];

                if (parts.Count - 1 > i)
                {
                    //Check if domain exists
                    if (!structure.Nested.ContainsKey(domainPart))
                    {
                        structure.Nested.Add(domainPart, new DomainDataStructure(domainPart));
                    }

                    structure = structure.Nested[domainPart];
                    continue;
                }

                //Check if domain exists
                if (structure.Nested.ContainsKey(domainPart))
                {
                    structure.Nested[domainPart].TldRule = tldRule;
                    continue;
                }

                structure.Nested.Add(domainPart, new DomainDataStructure(domainPart, tldRule));
            }
        }

        public DomainName Get(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            string fullurl; 
            if (domain.ToLower().StartsWith("http://") || domain.ToLower().StartsWith("https://"))
            {
                fullurl = domain; 
            }
            else
            {
                fullurl = string.Concat("https://", domain); 
            }

            //We use Uri methods to normalize host (So Punycode is converted to UTF-8
            Uri uri;

            if (!Uri.TryCreate(fullurl, UriKind.RelativeOrAbsolute, out uri))
            {
                return null;
            }

            var normalizedDomain = uri.Host;
            var normalizedHost = uri.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

            var parts = normalizedHost
                .Split('.')
                .Reverse()
                .ToList();

            if (parts.Count == 0 || parts.Any(x => x.Equals(string.Empty)))
            {
                return null;
            }

            var structure = this._domainDataStructure;
            var matches = new List<TldRule>();
            this.FindMatches(parts, structure, matches);

            //Sort so exceptions are first, then by biggest label count (with wildcards at bottom) 
            var sortedMatches = matches.OrderByDescending(x => x.Type == TldRuleType.WildcardException ? 1 : 0)
                .ThenByDescending(x => x.LabelCount)
                .ThenByDescending(x => x.Name);

            var winningRule = sortedMatches.FirstOrDefault();
            if (winningRule == null)
            {
                winningRule = new TldRule("*");
            }

            //Domain is TLD
            if (parts.Count == winningRule.LabelCount)
            {
                return null;
            }

            var domainName = new DomainName(normalizedDomain, winningRule);
            return domainName;
        }

        private void FindMatches(IEnumerable<string> parts, DomainDataStructure structure, List<TldRule> matches)
        {
            if (structure.TldRule != null)
            {
                matches.Add(structure.TldRule);
            }

            var part = parts.FirstOrDefault();
            if (string.IsNullOrEmpty(part))
            {
                return;
            }

            DomainDataStructure foundStructure;
            if (structure.Nested.TryGetValue(part, out foundStructure))
            {
                FindMatches(parts.Skip(1), foundStructure, matches);
            }

            if (structure.Nested.TryGetValue("*", out foundStructure))
            {
                FindMatches(parts.Skip(1), foundStructure, matches);
            }
        }
    }
}
