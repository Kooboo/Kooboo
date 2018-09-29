using System;
using System.Collections.Generic;
using Kooboo.Sites.Models;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Extensions;
using Kooboo.Sites.NodeTree;
using Kooboo.Dom;
using Kooboo.Controls;

namespace Kooboo.Sites.Automation.Layout
{
    public class LayoutAutomation : ISiteAutomation
    {

        private WebSite website;

        public LayoutAutomation(WebSite website)
            : this()
        {
            this.website = website;
        }

        public LayoutAutomation()
        {

        }

        public string StartView()
        {
            return "~/Areas/Admin/Views/Automation/LayoutAutomation.cshtml";
        }

        /// <summary>
        /// analyze and return the 
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public object Analyze(WebSite site)
        {
            LayoutNodeTree tree = new LayoutNodeTree(Sites.Tag.ElementFilter.LayoutFilter);

            tree.DoMerge = true; 
            tree.StartFromBody = true;

            var pages = site.SiteDb().Pages.All();
            foreach (var page in pages)
            {
                Kooboo.Sites.Service.PageService.ApplySiteStyle(page, site.SiteDb()); 
                tree.AddPage(page);
            }

            ///// apply layout score rules. 
            var layoutScoreRules = Ranking.BuiltInRankingRules.LayoutRules();

            foreach (var group in tree.RootNode.NodeGroups)
            {
                foreach (var item in group.ChildNodes)
                {
                    applyLayoutScore(item, layoutScoreRules);
                }
            }


            /// calculate the highest score and make selection. 

            /// start rendering. 

            return tree.ToTreeView();

        }

        private void applyLayoutScore(SiteNode node, List<Ranking.RankingRule> rules)
        {
            Ranking.RankingRuleEvaluaor.ApplyScore(node, rules);

            foreach (var group in node.NodeGroups)
            {
                foreach (var item in group.ChildNodes)
                {
                    if (item.DomNode != null && item.DomNode.nodeType == Dom.enumNodeType.ELEMENT)
                    {
                        Element e = item.DomNode as Element;

                        if (Tag.ElementFilter.LayoutFilter(e))
                        {
                            applyLayoutScore(item, rules);
                        }
                    }

                }
            }

        }


        public void Confirm(Dictionary<string, string> FormValues)
        {
            throw new NotImplementedException();
        }

        public void Apply(Dictionary<string, string> FormVales)
        {
            throw new NotImplementedException();
        }
    }
}
