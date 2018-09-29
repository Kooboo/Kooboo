//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Nager.PublicSuffix
{
    public class TldRuleParser
    {
        public IEnumerable<TldRule> ParseRules(string data)
        {
            var lines = data.Split(new char[] { '\n', '\r' });
            return this.ParseRules(lines);
        }

        public IEnumerable<TldRule> ParseRules(IEnumerable<string> lines)
        {
            var items = new List<TldRule>();
            var division = TldRuleDivision.Unknown;

            foreach (var line in lines)
            {
                //Ignore empty lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                 
                //Ignore comments (and set Division)
                if (line.StartsWith("//"))
                {
                    //Detect Division
                    if (line.StartsWith("// ===BEGIN ICANN DOMAINS==="))
                    {
                        division = TldRuleDivision.ICANN;
                    }
                    else if (line.StartsWith("// ===END ICANN DOMAINS==="))
                    {
                        division = TldRuleDivision.Unknown;
                    }
                    else if (line.StartsWith("// ===BEGIN PRIVATE DOMAINS==="))
                    {
                        division = TldRuleDivision.Private;
                    }
                    else if (line.StartsWith("// ===END PRIVATE DOMAINS==="))
                    {
                        division = TldRuleDivision.Unknown;
                    }

                    continue;
                }

                var tldRule = new TldRule(line.Trim(), division);

                items.Add(tldRule);
            }

            return items;
        }
    }
}
