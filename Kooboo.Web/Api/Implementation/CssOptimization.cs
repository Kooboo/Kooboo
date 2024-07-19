using System.Linq;
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.Implementation
{
    public class CssOptimization : IApi
    {
        public string ModelName => "CssOptimization";

        public bool RequireSite => true;

        public bool RequireUser => true;

        public List<OptimizationViewModel> List(ApiCall call)
        {
            var site = call.WebSite;
            var sitedb = call.WebSite.SiteDb();

            List<OptimizationViewModel> result = new List<OptimizationViewModel>();

            var unusedList = Kooboo.Sites.Service.CssCleanerService.GetUnusedRules(call.Context).Result;

            foreach (var item in unusedList)
            {
                var model = new OptimizationViewModel();
                model.Id = item.Id;
                model.Content = item.CssText;
                model.ParentId = item.ParentStyleId;
                result.Add(model);
            }

            foreach (var item in result.GroupBy(o => o.ParentId))
            {
                var list = item.ToList();

                var styleid = item.Key;

                var style = sitedb.Styles.Get(styleid);
                if (style != null)
                {
                    try
                    {
                        var url = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(site.SiteDb(), style);

                        foreach (var rule in list)
                        {
                            rule.StyleSheet = url;
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
            }

            return result.OrderBy(o => o.StyleSheet).ToList();
        }

        public void Delete(ApiCall call, Guid[] ids)
        {
            var sitedb = call.WebSite.SiteDb();
            List<CmsCssRule> rules = new List<CmsCssRule>();

            foreach (var item in ids)
            {
                var rule = sitedb.CssRules.Get(item);
                if (rule != null)
                {
                    rules.Add(rule);
                }
            }

            foreach (var item in rules.GroupBy(o => o.ParentStyleId))
            {
                var rulelist = item.ToList();

                List<CmsCssRuleChanges> changes = new List<CmsCssRuleChanges>();

                foreach (var delRule in rulelist)
                {
                    CmsCssRuleChanges change = new CmsCssRuleChanges();
                    change.CssRuleId = delRule.Id;
                    change.ChangeType = ChangeType.Delete;
                    changes.Add(change);
                }

                sitedb.CssRules.UpdateStyle(changes, item.Key);
            }

        }
    }

    public class OptimizationViewModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string StyleSheet { get; set; }

        public Guid ParentId { get; set; }

    }
}
