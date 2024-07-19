//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class StyleApi : SiteObjectApi<Style>
    {
        [Kooboo.Attributes.RequireParameters("id")]
        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        public StyleViewModel GetEdit(ApiCall call)
        {
            var style = call.WebSite.SiteDb().Styles.Get(call.ObjectId);
            if (style != null)
            {
                var model = new StyleViewModel();
                model.Id = style.Id;
                model.Name = style.Name;
                model.Body = style.Body;
                model.DisplayName = style.DisplayName;
                model.FullUrl = ObjectService.GetObjectRelativeUrl(call.WebSite.SiteDb(), style);

                model.Extension = style.Extension;

                if (model.Extension != null && model.Extension != "css" && model.Extension != ".css")
                {
                    model.SourceChange = style.SourceChange;
                    if (!string.IsNullOrEmpty(style.Source))
                    {
                        model.Body = style.Source;
                    }
                }
                return model;
            }
            else
            {
                return new StyleViewModel();
            }

        }

        [Kooboo.Web.Menus.SiteObjectMenu]
        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<IEmbeddableItemListViewModel> External(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();

            int storenameHash = Lib.Security.Hash.ComputeInt(sitedb.Styles.StoreName);

            return sitedb
                .Styles
                .GetExternals()
                .SortByNameOrLastModified(apiCall)
                .Select(item => new IEmbeddableItemListViewModel(sitedb, item)
                {
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = storenameHash
                });
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<IEmbeddableItemListViewModel> Embedded(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();
            return sitedb.Styles
                .GetEmbeddeds()
                .SortByBodyOrLastModified(apiCall)
                .Select(o => new IEmbeddableItemListViewModel(sitedb, o));
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        public List<InlineItemViewModel> Inline(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();
            List<InlineItemViewModel> result = new List<InlineItemViewModel>();

            foreach (var item in sitedb.CssRules.GetInLineRules())
            {
                var info = ObjectService.GetObjectInfo(sitedb, item);

                InlineItemViewModel newitem = new InlineItemViewModel();
                newitem.Id = item.Id;
                newitem.Name = string.IsNullOrEmpty(item.KoobooOpenTag) ? item.RuleText : item.KoobooOpenTag;
                newitem.OwnerName = info.DisplayName;
                newitem.OwnerType = info.ModelType.Name;
                newitem.LastModified = item.LastModified;
                newitem.Source = item.RuleText;
                result.Add(newitem);
            }

            return result;
        }

        [Kooboo.Attributes.RequireModel(typeof(StyleEditViewModel))]
        [Permission(Feature.STYLE, Action = Data.Permission.Action.EDIT)]
        public Guid Update(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var model = call.Context.Request.Model as StyleEditViewModel;
            if (string.IsNullOrEmpty(model.Extension))
            {
                model.Extension = "css";
            }
            string source = null;

            if (model.Extension != "css" && model.Extension != ".css")
            {
                source = model.Body;
                model.Body = Kooboo.Sites.Engine.Manager.Execute(model.Extension, call.Context, model.Body);
            }


            if (model.Id != default(Guid))
            {
                var style = sitedb.Styles.Get(model.Id);
                if (style != null)
                {
                    (model as IDiffChecker).CheckDiff(style);

                    style.Body = model.Body;
                    if (style.Extension == null)
                    {
                        style.Extension = model.Extension;
                    }

                    if (source != null)
                    {
                        style.Source = source;
                        style.SourceChange = false;
                    }
                    sitedb.Styles.AddOrUpdate(style, true, true, call.Context.User.Id);
                    return style.Id;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    return default(Guid);
                }

                if (!model.Name.EndsWith("." + model.Extension))
                {
                    model.Name = model.Name + "." + model.Extension;
                }

                string url = model.Name;
                if (url.StartsWith("\\"))
                {
                    url = "/" + url.Substring(1);
                }
                if (!url.StartsWith("/"))
                {
                    url = "/" + url;
                }
                var route = sitedb.Routes.GetByUrl(url);
                if (route != null)
                {
                    var style = sitedb.Styles.Get(route.objectId);
                    if (style != null)
                    {
                        style.Body = model.Body;
                        if (style.Extension == null)
                        {
                            style.Extension = model.Extension;
                        }

                        if (source != null)
                        {
                            style.Source = source;
                            style.SourceChange = false;
                        }
                        sitedb.Styles.AddOrUpdate(style, false, false, call.Context.User.Id);
                        return style.Id;
                    }
                }

                Style newstyle = new Style();
                newstyle.Name = model.Name;
                newstyle.Body = model.Body;
                newstyle.Extension = model.Extension;

                if (source != null)
                {
                    newstyle.Source = source;
                    newstyle.SourceChange = false;
                }

                sitedb.Routes.AddOrUpdate(url, newstyle, call.Context.User.Id);
                sitedb.Styles.AddOrUpdate(newstyle, call.Context.User.Id);
                return newstyle.Id;
            }
            return default(Guid);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        public List<UsedByRelation> Relation(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string type = call.GetValue("type", "by");
            if (string.IsNullOrEmpty(type))
            {
                return null;
            }
            byte consttype = ConstTypeContainer.GetConstType(type);

            if (call.ObjectId != default(Guid))
            {
                return sitedb.Styles.GetUsedBy(call.ObjectId)
                       .Where(it => it.ConstType == consttype)
                       .Select(it =>
                       {
                           it.Url = sitedb.WebSite.BaseUrl(it.Url);
                           return it;
                       }).ToList();
            }

            return null;
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        public List<CssRuleViewModel> GetRules(ApiCall call)
        {
            return GetCssRuleViews(call.WebSite.SiteDb(), call.ObjectId);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        public List<CssRuleViewModel> GetCssRuleViews(SiteDb SiteDb, Guid ParentStyleId)
        {
            var allrules = SiteDb.CssRules.Query.Where(o => o.ParentStyleId == ParentStyleId).SelectAll();

            var current = allrules.Where(o => o.ParentCssRuleId == default(Guid)).ToList();

            return SetGetCssRule(allrules, current);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.EDIT)]
        private List<CssRuleViewModel> SetGetCssRule(List<CmsCssRule> allrules, List<CmsCssRule> currentrules)
        {
            List<CssRuleViewModel> result = new List<CssRuleViewModel>();

            foreach (var item in currentrules)
            {
                var ruleType = item.ruleType;
                if (item.ruleType == RuleType.ImportRule)
                {
                    CssRuleViewModel rule = new CssRuleViewModel();
                    rule.Selector = item.CssText;
                    rule.RuleType = ruleType;
                    rule.Id = item.Id;
                    result.Add(rule);
                }
                else if (item.ruleType == RuleType.MediaRule)
                {
                    CssRuleViewModel rule = new CssRuleViewModel();
                    rule.Selector = item.SelectorText;
                    rule.RuleType = ruleType;
                    List<CmsCssRule> subrules = allrules.Where(o => o.ParentCssRuleId == item.Id).ToList();
                    rule.Rules = SetGetCssRule(allrules, subrules);
                    rule.Id = item.Id;
                    result.Add(rule);
                }
                else
                {
                    // style rule or font face rule. 
                    CssRuleViewModel rule = new CssRuleViewModel();
                    rule.RuleType = ruleType;
                    List<DeclarationViewModel> subdeclarations = new List<DeclarationViewModel>();

                    var cssdecls = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(item.RuleText);

                    foreach (var jitem in cssdecls.item)
                    {
                        subdeclarations.Add(new DeclarationViewModel() { Name = jitem.propertyname, Value = jitem.value, Important = jitem.important });
                    }

                    rule.Declarations = subdeclarations;
                    rule.Selector = item.SelectorText;
                    rule.Id = item.Id;
                    result.Add(rule);
                }
            }

            return result;
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        public List<CssRuleViewModel> GetCssRuleList(List<CmsCssRule> cssRules, Data.Models.WebSite website)
        {
            List<CssRuleViewModel> rules = new List<CssRuleViewModel>();

            foreach (var item in cssRules)
            {
                var ruleType = item.ruleType;
                if (item.ruleType == RuleType.ImportRule)
                {
                    CssRuleViewModel rule = new CssRuleViewModel();
                    rule.Selector = item.CssText;
                    rule.RuleType = ruleType;
                    rule.Id = item.Id;
                    rules.Add(rule);
                }
                else if (item.ruleType == RuleType.MediaRule)
                {
                    CssRuleViewModel rule = new CssRuleViewModel();
                    rule.Selector = item.SelectorText;
                    rule.RuleType = ruleType;
                    List<CmsCssRule> subrules = website.SiteDb().CssRules.Query.Where(o => o.ParentCssRuleId == item.Id).SelectAll();

                    rule.Rules = this.GetCssRuleList(subrules, website);
                    rule.Id = item.Id;
                    rules.Add(rule);
                }
                else
                {
                    // style rule or font face rule. 

                    CssRuleViewModel rule = new CssRuleViewModel();
                    rule.RuleType = ruleType;
                    List<DeclarationViewModel> subdeclarations = new List<DeclarationViewModel>();

                    var cssdecls = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(item.RuleText);

                    foreach (var jitem in cssdecls.item)
                    {
                        subdeclarations.Add(new DeclarationViewModel() { Name = jitem.propertyname, Value = jitem.value, Important = jitem.important });
                    }

                    rule.Declarations = subdeclarations;
                    rule.Selector = item.SelectorText;
                    rule.Id = item.Id;
                    rules.Add(rule);
                }
            }

            return rules;

        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.EDIT)]
        public void UpdateRules(ApiCall call)
        {
            Guid StyleId = call.ObjectId;
            var json = call.Context.Request.Body;
            if (string.IsNullOrEmpty(json) || StyleId == default(Guid))
            {
                return;
            }

            UpdateStyleRuleViewModel model;

            try
            {
                model = Lib.Helper.JsonHelper.Deserialize<UpdateStyleRuleViewModel>(json);
            }
            catch (Exception)
            {
                throw;
            }

            List<CmsCssRuleChanges> changes = new List<CmsCssRuleChanges>();

            if (model.Added != null && model.Added.Count > 0)
            {
                foreach (var rule in model.Added.OrderBy(o => o.Key))
                {
                    if (rule.Value.RuleType == RuleType.StyleRule)
                    {
                        CmsCssRuleChanges changeitem = GetStyleRuleChangeItem(rule.Value);
                        changes.Add(changeitem);
                    }
                    else if (rule.Value.RuleType == RuleType.MediaRule)
                    {
                        CmsCssRuleChanges changeitem = GetMediaRuleAdded(rule.Value);
                        changes.Add(changeitem);
                    }
                }
            }
            if (model.Modified != null && model.Modified.Count > 0)
            {

                foreach (var rule in model.Modified)
                {
                    List<CmsCssDeclaration> declarations = new List<CmsCssDeclaration>();
                    if (rule.Value.Declarations != null)
                    {
                        foreach (var item in rule.Value.Declarations)
                        {
                            declarations.Add(new CmsCssDeclaration() { PropertyName = item.Name, Value = item.Value, Important = item.Important });
                        }
                    }
                    changes.Add(new CmsCssRuleChanges() { ChangeType = ChangeType.Update, CssRuleId = rule.Value.Id, selectorText = rule.Value.Selector, Declarations = declarations });
                }
            }
            if (model.Removed != null && model.Removed.Count > 0)
            {
                foreach (var guid in model.Removed)
                {
                    changes.Add(new CmsCssRuleChanges() { ChangeType = ChangeType.Delete, CssRuleId = guid });
                }
            }

            if (changes.Count > 0)
            {
                call.WebSite.SiteDb().CssRules.UpdateStyle(changes, StyleId);
            }
        }

        private static CmsCssRuleChanges GetStyleRuleChangeItem(CssRuleViewModel rule)
        {
            List<CmsCssDeclaration> declarations = new List<CmsCssDeclaration>();
            if (rule.Declarations != null)
            {
                foreach (var item in rule.Declarations)
                {
                    declarations.Add(new CmsCssDeclaration() { PropertyName = item.Name, Value = item.Value, Important = item.Important });
                }
            }
            var changeitem = new CmsCssRuleChanges() { ChangeType = ChangeType.Add, CssRuleId = rule.Id, selectorText = rule.Selector, Declarations = declarations };

            return changeitem;
        }

        private static CmsCssRuleChanges GetMediaRuleAdded(CssRuleViewModel rule)
        {
            CmsCssRuleChanges change = new CmsCssRuleChanges();
            if (rule.Selector.IndexOf("@media", StringComparison.OrdinalIgnoreCase) == -1)
            {
                change.selectorText = "@media " + rule.Selector;
            }
            else
            {
                change.selectorText = rule.Selector;
            }

            string ruletext = string.Empty;

            foreach (var item in rule.Rules)
            {
                ruletext += "\r\n" + item.Selector + "\r\n{\r\n";
                List<CmsCssDeclaration> declarations = new List<CmsCssDeclaration>();
                if (item.Declarations != null)
                {
                    foreach (var decl in item.Declarations)
                    {
                        declarations.Add(new CmsCssDeclaration() { PropertyName = decl.Name, Value = decl.Value, Important = decl.Important });
                    }
                    ruletext += CssService.SerializeCmsCssDeclaration(declarations);
                }
                ruletext += "\r\n}";
            }

            change.DeclarationText = ruletext;

            return change;
        }

        public override bool IsUniqueName(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            string name = call.NameOrId;

            if (!string.IsNullOrEmpty(name))
            {
                var value = sitedb.Styles.GetByNameOrId(name);
                if (value != null)
                {
                    return false;
                }

                List<string> dotextension = new List<string>();
                foreach (var item in GetExtensions(call))
                {
                    string dotitem = item;
                    if (!dotitem.StartsWith("."))
                    {
                        dotitem = "." + dotitem;
                    }
                    dotextension.Add(dotitem);
                }

                name = name.ToLower();

                var find = sitedb.Styles.Store.FullScan(o => samename(o.Name, name, dotextension)).FirstOrDefault();

                if (find != null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool samename(string dbname, string name, List<string> extensionsWithDot)
        {
            if (dbname == null || name == null)
            {
                return false;
            }

            dbname = dbname.ToLower();
            if (dbname == name)
            {
                return true;
            }

            foreach (var item in extensionsWithDot)
            {

                if (dbname.EndsWith(item))
                {
                    dbname = dbname.Substring(0, dbname.Length - item.Length);
                    if (dbname == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<string> GetExtensions(ApiCall call)
        {
            HashSet<string> result = new HashSet<string>();
            result.Add("css");

            var list = Kooboo.Sites.Engine.Manager.GetStyle();

            foreach (var item in list)
            {
                result.Add(item.Extension);
            }
            return result.ToList();
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            return base.List(call);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.STYLE, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }
    }
}
