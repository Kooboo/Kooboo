//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.InlineEditor.Model;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kooboo.Dom;

namespace Kooboo.Sites.InlineEditor.Executor
{
    public class StyleExecutor : IInlineExecutor
    {
        public string EditorType
        {
            get
            {
                return "style";
            }
        }

        public void ExecuteObject(RenderContext context, IRepository repo, string nameOrId, List<IInlineModel> updates)
        {
            throw new NotImplementedException();
        }

        public void EnsureStyleId(List<Model.StyleModel> items, RenderContext context)
        {
            var page = context.GetItem<Page>();

            if (items.Count() == 0)
            {
                return;
            }
            Dictionary<string, Guid> UrlIdMaps = new Dictionary<string, Guid>();

            foreach (var item in items)
            {
                if (item.StyleId == default(Guid))
                {
                    if (!string.IsNullOrEmpty(item.StyleTagKoobooId))
                    {
                        // embedded style.
                        item.StyleId = GetStyleIdByKoobooTag(context, item.StyleTagKoobooId, item.ObjectType, item.NameOrId);
                    }
                    else
                    {
                        string cssUrl = item.StyleSheetUrl;

                        if (string.IsNullOrEmpty(cssUrl))
                        {
                            var allPageCssUrl = Relation.DomRelation.GetReferenceStyleUrl(page.Dom, page.Headers);
                            allPageCssUrl.Reverse();
                            foreach (var cssitem in allPageCssUrl)
                            {
                                if (!string.IsNullOrEmpty(cssitem) && !Service.DomUrlService.IsExternalLink(cssitem))
                                {
                                    cssUrl = cssitem;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(cssUrl))
                        {
                            Guid styleid = default(Guid);
                            string relativeurl = Kooboo.Lib.Helper.UrlHelper.RelativePath(cssUrl);

                            if (UrlIdMaps.ContainsKey(relativeurl))
                            {
                                styleid = UrlIdMaps[relativeurl];
                            }
                            else
                            {
                                var route = context.WebSite.SiteDb().Routes.GetByUrl(relativeurl);
                                if (route != null && route.objectId != default(Guid) && route.DestinationConstType == ConstObjectType.Style)
                                {
                                    styleid = route.objectId;
                                    UrlIdMaps[relativeurl] = styleid;
                                }
                            }

                            item.StyleId = styleid;
                        }
                        else
                        {
                            var allstyle = context.WebSite.SiteDb().Styles.Query.Where(o => o.OwnerObjectId == page.Id).SelectAll().OrderByDescending(o => o.ItemIndex);

                            if (allstyle.Any())
                            {
                                item.StyleId = allstyle.First().Id;
                            }
                        }
                    }
                }
            }
        }

        public Guid GetStyleIdByKoobooTag(RenderContext context, string styleTagKoobooId, string objectType = null, string nameOrId = null)
        {
            IDomObject domobject = null;
            Guid objectid = default(Guid);

            if (!string.IsNullOrEmpty(objectType) && !string.IsNullOrEmpty(nameOrId))
            {
                var repo = context.WebSite.SiteDb().GetRepository(objectType);
                var siteobject = repo?.GetByNameOrId(nameOrId);
                if (siteobject != null && siteobject is IDomObject domObject)
                {
                    domobject = domObject;
                    objectid = domobject.Id;
                }
            }

            if (objectid == default(Guid))
            {
                var page = context.GetItem<Page>();
                domobject = page;
                objectid = page.Id;
            }

            var element = Service.DomService.GetElementByKoobooId(domobject?.Dom, styleTagKoobooId);
            if (element != null)
            {
                string inner = element.InnerHtml;
                int bodyhash = Lib.Security.Hash.ComputeIntCaseSensitive(inner);

                var style = context.WebSite.SiteDb().Styles.Query.Where(o => o.OwnerObjectId == objectid && o.BodyHash == bodyhash).FirstOrDefault();
                if (style != null)
                {
                    return style.Id;
                }
            }
            return default(Guid);
        }

        public void EnsureCssRuleId(List<Model.StyleModel> items, RenderContext context)
        {
            var addtional = new List<Model.StyleModel>();

            Func<string, string> removeSpace = (s) => Regex.Replace(s, @"\s", "", RegexOptions.IgnoreCase);

            foreach (var item in items.GroupBy(o => o.StyleId))
            {
                if (item.Key != default(Guid))
                {
                    var allrules = context.WebSite.SiteDb().CssRules.Query.Where(o => o.ParentStyleId == item.Key).SelectAll();

                    foreach (var ruleitem in item)
                    {
                        if (ruleitem.RuleId == default(Guid) && !string.IsNullOrEmpty(ruleitem.Selector))
                        {
                            var foundrules = allrules.FindAll(o => CssSelectorComparer.IsEqual(o.SelectorText, ruleitem.Selector));

                            if (foundrules != null && foundrules.Any())
                            {
                                if (!string.IsNullOrWhiteSpace(ruleitem.MediaRuleList))
                                {
                                    foundrules = foundrules.Where(w =>
                                    {
                                        var parent = allrules.FirstOrDefault(f => f.Id == w.ParentCssRuleId);
                                        return parent != null && removeSpace(parent.SelectorText) == removeSpace(ruleitem.MediaRuleList);
                                    }).ToList();
                                }
                                else
                                {
                                    foundrules = foundrules.Where(e => e.ParentCssRuleId == default(Guid)).ToList();
                                }

                                var result = AssignRuleId(ruleitem, foundrules);
                                addtional.AddRange(result);
                            }
                        }
                    }
                }
            }

            if (addtional.Count() > 0)
            {
                items.AddRange(addtional);
            }
        }

        public List<Model.StyleModel> AssignRuleId(Model.StyleModel item, List<CmsCssRule> foundrules)
        {
            List<Model.StyleModel> additionRules = new List<Model.StyleModel>();

            var matchrule = foundrules.OrderByDescending(o => o.DuplicateIndex).First();

            item.RuleId = matchrule.Id;

            bool isWithinMedia = matchrule.ParentCssRuleId != default(Guid);
            foundrules.Remove(matchrule);

            var othermediaruls = foundrules.FindAll(o => o.ParentCssRuleId != default(Guid));
            foreach (var rule in othermediaruls)
            {
                var copyitem = Lib.Serializer.Copy.DeepCopy<Model.StyleModel>(item);
                copyitem.RuleId = rule.Id;
                additionRules.Add(copyitem);
            }

            if (isWithinMedia)
            {
                //outside should change the last index.
                var otherNonMediaRules = foundrules.Where(o => o.ParentCssRuleId == default(Guid)).ToList();
                if (otherNonMediaRules != null && otherNonMediaRules.Any())
                {
                    var lastNonMedia = otherNonMediaRules.OrderByDescending(o => o.DuplicateIndex).First();
                    var copyitem = Lib.Serializer.Copy.DeepCopy<Model.StyleModel>(item);
                    copyitem.RuleId = lastNonMedia.Id;
                    additionRules.Add(copyitem);
                }
            }

            return additionRules;
        }

        public void Execute(RenderContext context, List<IInlineModel> updatelist)
        {
            var updates = updatelist.Cast<Model.StyleModel>().ToList();
            List<Model.StyleModel> inlinechanges = new List<Model.StyleModel>();
            List<Model.StyleModel> stylechanges = new List<Model.StyleModel>();

            foreach (var item in updates)
            {
                if (item == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(item.KoobooId))
                {
                    inlinechanges.Add(item);
                }
                else
                {
                    stylechanges.Add(item);
                }
            }
            ProcessInlineCss(context, inlinechanges);

            EnsureStyleId(stylechanges, context);
            EnsureCssRuleId(stylechanges, context);
            ProcessStyleRules(context, stylechanges);
        }

        public void ProcessInlineCss(RenderContext context, List<Model.StyleModel> changes)
        {
            List<IInlineModel> domUpdates = ConvertToDomUpdate(context, changes);

            new DomExecutor().Execute(context, domUpdates);
        }

        public List<IInlineModel> ConvertToDomUpdate(RenderContext context, List<StyleModel> changes)
        {
            List<IInlineModel> domUpdates = new List<IInlineModel>();
            foreach (var item in changes.GroupBy(o => o.ObjectType))
            {
                var objecttype = item.Key;
                // ProcessInline(context, changes, objecttype, nameorid);
                var sitedb = context.WebSite.SiteDb();
                var repo = sitedb.GetRepository(objecttype);
                if (repo == null) { continue; }

                var list = item.ToList();
                foreach (var subitem in list.GroupBy(o => o.NameOrId))
                {
                    string nameorid = subitem.Key;
                    var siteobject = repo.GetByNameOrId(nameorid);
                    if (siteobject != null && siteobject is IDomObject domobject)
                    {
                        foreach (var updateitem in subitem.GroupBy(o => o.KoobooId))
                        {
                            string koobooid = updateitem.Key;

                            if (Sites.Service.DomService.GetElementByKoobooId(domobject.Dom, koobooid) is Element el)
                            {
                                var style = el.getAttribute("style");

                                List<Kooboo.Dom.CSS.CSSDeclaration> propertychanges = new List<Dom.CSS.CSSDeclaration>();
                                foreach (var propertyitem in updateitem.ToList())
                                {
                                    propertychanges.Add(new Dom.CSS.CSSDeclaration() { propertyname = propertyitem.Property, value = propertyitem.Value, important = propertyitem.Important });
                                }
                                var newstylevalue = UpdateHelper.UpdateOrAppendInlineCss(style, propertychanges);

                                DomModel model = new DomModel
                                {
                                    KoobooId = koobooid,
                                    NameOrId = nameorid,
                                    ObjectType = objecttype,
                                    AttributeName = "style",
                                    Value = newstylevalue
                                };
                                domUpdates.Add(model);
                            }
                        }
                    }
                }
            }

            return domUpdates;
        }

        private void ProcessInline(RenderContext context, List<Model.StyleModel> changes, string objectType, string nameOrId)
        {
            if (changes.Count == 0 || string.IsNullOrEmpty(objectType) || string.IsNullOrEmpty(nameOrId))
            {
                return;
            }
            var inlinechanges = new List<InlineStyleChange>();

            foreach (var item in changes)
            {
                if (!string.IsNullOrEmpty(item.Property))
                {
                    InlineStyleChange current = inlinechanges.Find(o => o.KoobooId == item.KoobooId);

                    if (current == null)
                    {
                        current = new InlineStyleChange();
                        current.KoobooId = item.KoobooId;
                        current.PropertyValues.Add(item.Property, item.Value);
                        inlinechanges.Add(current);
                    }
                    else
                    {
                        if (current.PropertyValues.ContainsKey(item.Property))
                        {
                            current.PropertyValues[item.Property] = item.Value;
                        }
                        else
                        {
                            current.PropertyValues.Add(item.Property, item.Value);
                        }
                    }
                }
            }

            var repo = context.WebSite.SiteDb().GetRepository(objectType);

            var koobooobject = repo.GetByNameOrId(nameOrId);

            if (koobooobject is IDomObject domobject && inlinechanges.Any())
            {
                domobject.Body = Service.DomService.UpdateInlineStyle(domobject.Body, inlinechanges);
                repo.AddOrUpdate(domobject);
            }
        }

        public void ProcessStyleRules(RenderContext context, List<Model.StyleModel> changes)
        {
            var page = context.GetItem<Page>();

            var groupby = changes.GroupBy(o => o.StyleId);
            foreach (var item in groupby)
            {
                if (item.Key == default(Guid))
                {
                    var newStyle = new Style
                    {
                        IsEmbedded = true,
                        OwnerObjectId = page.Id,
                        OwnerConstType = ConstObjectType.Page,
                        Body = GetCssText(item.ToList()),
                        ItemIndex = -1
                    };
                    context.WebSite.SiteDb().Styles.AddOrUpdate(newStyle, true, true, context.User.Id);
                }
                else
                {
                    ProcessStyleRules(context, item.ToList(), item.Key);
                }
            }
        }

        private static string GetCssText(List<Model.StyleModel> changes)
        {
            string csstext = string.Empty;

            foreach (var item in changes.GroupBy(o => o.Selector))
            {
                string selector = item.Key;
                string oneselector = string.Empty;
                foreach (var citem in item.ToList())
                {
                    string decl = citem.Property + ":" + citem.Value;
                    if (citem.Important)
                    {
                        decl += "!important";
                    }
                    decl += ";";
                    oneselector += decl + "\r\n";
                }
                if (!string.IsNullOrEmpty(oneselector))
                {
                    oneselector = "\r\n" + selector + "\r\n{\r\n" + oneselector + "\r\n}";
                }
                csstext += oneselector;
            }

            return csstext;
        }

        public void ProcessStyleRules(RenderContext context, List<Model.StyleModel> changes, Guid StyleId)
        {
            if (!changes.Any()) return;

            var prechanges = new List<RuleChange>();
            RuleChange current = null;
            foreach (var item in changes)
            {
                current = null;

                if (item.RuleId != default(Guid))
                {
                    current = prechanges.Find(o => o.RuleId == item.RuleId);

                    if (current == null)
                    {
                        current = new RuleChange();
                        var cmscssrule = context.WebSite.SiteDb().CssRules.Get(item.RuleId);
                        if (cmscssrule == null)
                        {
                            current.RuleId = default(Guid);
                            current.Declarations = new Dom.CSS.CSSDeclarationBlock();
                        }
                        else
                        {
                            current.RuleId = item.RuleId;
                            current.Declarations = Service.CssService.ParseStyleDeclaration(cmscssrule);
                            if (current.Declarations == null)
                                current.Declarations = new Dom.CSS.CSSDeclarationBlock();
                        }
                        if (current != null)
                        {
                            prechanges.Add(current);
                        }
                    }

                    if (current != null)
                    {
                        current.Selector = item.Selector;
                    }
                }
                else
                {
                    current = prechanges.Find(o => o.RuleId == default(Guid) && o.Selector == item.Selector);
                    if (current == null)
                    {
                        current = new RuleChange
                        {
                            Selector = item.Selector, Declarations = new Dom.CSS.CSSDeclarationBlock()
                        };
                        prechanges.Add(current);
                    }
                }

                if (current != null)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        current.Declarations.setProperty(item.Property, item.Value, item.Important);
                        var prop = current.Declarations.item.FirstOrDefault(f => f != null && f.propertyname == item.Property);
                        if (prop != null)
                        {
                            current.Declarations.item.Remove(prop);
                            current.Declarations.item.Add(prop);
                        }
                    }
                    else
                    {
                        current.Declarations.removeProperty(item.Property);
                    }
                }
            }

            var changelist = new List<CmsCssRuleChanges>();

            foreach (var item in prechanges)
            {
                var onechange = new CmsCssRuleChanges
                {
                    CssRuleId = item.RuleId,
                    selectorText = item.Selector,
                    DeclarationText = item.Declarations.GenerateCssText()
                };

                if (item.Declarations.item.Count == 0)
                {
                    onechange.ChangeType = ChangeType.Delete;
                }
                else
                {
                    onechange.ChangeType = item.RuleId == default(Guid) ? ChangeType.Add : ChangeType.Update;
                }

                changelist.Add(onechange);
            }

            context.WebSite.SiteDb().CssRules.UpdateStyle(changelist, StyleId);
        }
    }

    public class RuleChange
    {
        public Guid RuleId { get; set; }
        public string Selector { get; set; }

        public Dom.CSS.CSSDeclarationBlock Declarations { get; set; }
    }
}