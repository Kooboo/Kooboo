//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using Kooboo.Extensions;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.InlineEditor.Model;
using Kooboo.Mail.Imap.Commands;

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

        public void ExecuteObject(RenderContext context, IRepository repo, string NameOrId, List<IInlineModel> updates)
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
                        string CssUrl = item.StyleSheetUrl;

                        if (string.IsNullOrEmpty(CssUrl))
                        {
                            var allPageCssUrl = Relation.DomRelation.GetReferenceStyleUrl(page.Dom, page.Headers);
                            allPageCssUrl.Reverse();
                            foreach (var cssitem in allPageCssUrl)
                            {
                                if (!string.IsNullOrEmpty(cssitem) && !Service.DomUrlService.IsExternalLink(cssitem))
                                {
                                    CssUrl = cssitem;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(CssUrl))
                        {
                            Guid styleid = default(Guid);
                            string relativeurl = Kooboo.Lib.Helper.UrlHelper.RelativePath(CssUrl);

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

                            if (allstyle.Count() > 0)
                            {
                                item.StyleId = allstyle.First().Id;
                            }
                        }
                    }
                }
            }
        }

        public Guid GetStyleIdByKoobooTag(RenderContext Context, string StyleTagKoobooId, string ObjectType = null, string NameOrId = null)
        {
            IDomObject domobject = null;
            Guid objectid = default(Guid);

            if (!string.IsNullOrEmpty(ObjectType) && !string.IsNullOrEmpty(NameOrId))
            {
                var repo = Context.WebSite.SiteDb().GetRepository(ObjectType);
                if (repo != null)
                {
                    var siteobject = repo.GetByNameOrId(NameOrId);
                    if (siteobject != null && siteobject is IDomObject)
                    {
                        domobject = siteobject as IDomObject;
                        objectid = domobject.Id;
                    }
                }
            }

            if (objectid == default(Guid))
            {
                var page = Context.GetItem<Page>();
                domobject = page as IDomObject;
                objectid = page.Id;
            }


            var element = Service.DomService.GetElementByKoobooId(domobject.Dom, StyleTagKoobooId);
            if (element != null)
            {
                string inner = element.InnerHtml;
                int bodyhash = Lib.Security.Hash.ComputeIntCaseSensitive(inner);

                var style = Context.WebSite.SiteDb().Styles.Query.Where(o => o.OwnerObjectId == objectid && o.BodyHash == bodyhash).FirstOrDefault();
                if (style != null)
                {
                    return style.Id;
                }
            }
            return default(Guid);
        }

        public void EnsureCssRuleId(List<Model.StyleModel> items, RenderContext context)
        {
            List<Model.StyleModel> addtional = new List<Model.StyleModel>();

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

                            if (foundrules != null && foundrules.Count() > 0)
                            {
                                // 如果媒体查询不为空
                                if (!string.IsNullOrWhiteSpace(ruleitem.MediaRuleList))
                                {
                                    foreach (var foundruleItem in foundrules.ToList()) {
                                        // 如果没有父id（不在媒体查询中）
                                        if (foundruleItem.ParentCssRuleId == default(Guid)) {
                                            foundrules.Remove(foundruleItem);
                                        }

                                        var media = allrules.FirstOrDefault(o => o.Id == foundruleItem.ParentCssRuleId);

                                        if (media == null || !CssSelectorComparer.IsEqual(media.SelectorText, ruleitem.MediaRuleList))
                                        {
                                            foundrules.Remove(foundruleItem);
                                        }
                                    }
                                }
                                else {
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
            List<Model.StyleModel> AdditionRules = new List<Model.StyleModel>();

            var matchrule = foundrules.OrderByDescending(o => o.DuplicateIndex).First();

            item.RuleId = matchrule.Id;

            bool IsWithinMedia = matchrule.ParentCssRuleId != default(Guid);
            foundrules.Remove(matchrule);

            var othermediaruls = foundrules.FindAll(o => o.ParentCssRuleId != default(Guid));
            foreach (var rule in othermediaruls)
            {
                var copyitem = Lib.Serializer.Copy.DeepCopy<Model.StyleModel>(item);
                copyitem.RuleId = rule.Id;
                AdditionRules.Add(copyitem); 
            }

            if (IsWithinMedia)
            {
                //outside should change the last index.
                var otherNonMediaRules = foundrules.Where(o => o.ParentCssRuleId == default(Guid)).ToList(); 
                if (otherNonMediaRules !=null && otherNonMediaRules.Count()>0)
                {
                    var lastNonMedia = otherNonMediaRules.OrderByDescending(o => o.DuplicateIndex).First();
                    var copyitem = Lib.Serializer.Copy.DeepCopy<Model.StyleModel>(item);
                    copyitem.RuleId = lastNonMedia.Id;
                    AdditionRules.Add(copyitem);

                } 
            } 

            return AdditionRules; 


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
        public   void ProcessInlineCss(RenderContext context, List<Model.StyleModel> changes)
        {
            List<IInlineModel> DomUpdates = ConvertToDomUpdate(context, changes);

            new DomExecutor().Execute(context, DomUpdates);
        }

        public   List<IInlineModel> ConvertToDomUpdate(RenderContext context, List<StyleModel> changes)
        {
            List<IInlineModel> DomUpdates = new List<IInlineModel>();
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
                    if (siteobject != null && siteobject is IDomObject)
                    {
                        var domobject = siteobject as IDomObject;
                        foreach (var updateitem in subitem.GroupBy(o => o.KoobooId))
                        {
                            string koobooid = updateitem.Key;

                            var el = Sites.Service.DomService.GetElementByKoobooId(domobject.Dom, koobooid) as Kooboo.Dom.Element;

                            if (el != null)
                            {
                                var style = el.getAttribute("style");

                                List<Kooboo.Dom.CSS.CSSDeclaration> propertychanges = new List<Dom.CSS.CSSDeclaration>();
                                foreach (var propertyitem in updateitem.ToList())
                                {
                                    propertychanges.Add(new Dom.CSS.CSSDeclaration() { propertyname = propertyitem.Property, value = propertyitem.Value, important = propertyitem.Important });
                                }
                                var newstylevalue = UpdateHelper.UpdateOrAppendInlineCss(style, propertychanges);

                                DomModel model = new DomModel();
                                model.KoobooId = koobooid;
                                model.NameOrId = nameorid;
                                model.ObjectType = objecttype;
                                model.AttributeName = "style";
                                model.Value = newstylevalue;
                                DomUpdates.Add(model);

                            }

                        }
                    }
                }
            }

            return DomUpdates;
        }

        private   void ProcessInline(RenderContext context, List<Model.StyleModel> changes, string ObjectType, string NameOrId)
        {
            if (changes.Count() == 0 || string.IsNullOrEmpty(ObjectType) || string.IsNullOrEmpty(NameOrId))
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
            
            var repo = context.WebSite.SiteDb().GetRepository(ObjectType);

            var koobooobject = repo.GetByNameOrId(NameOrId);
             
            if (koobooobject is IDomObject && inlinechanges.Count() > 0)
            {
                var domobject = koobooobject as IDomObject;
                domobject.Body = Service.DomService.UpdateInlineStyle(domobject.Body, inlinechanges);
                repo.AddOrUpdate(domobject);
            }
 
        }

        public  void ProcessStyleRules(RenderContext context, List<Model.StyleModel> changes)
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
                    context.WebSite.SiteDb().Styles.AddOrUpdate(newStyle,  true, true, context.User.Id);
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

        public  void ProcessStyleRules(RenderContext context, List<Model.StyleModel> changes, Guid StyleId)
        {
            if (!changes.Any())
            { return; }

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
                            if(current.Declarations==null)
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
                        current = new RuleChange();
                        current.Selector = item.Selector;
                        current.Declarations = new Dom.CSS.CSSDeclarationBlock();
                        prechanges.Add(current);
                    }

                }

                if (current != null)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        current.Declarations.setProperty(item.Property, item.Value, item.Important);
                        var prop= current.Declarations.item.FirstOrDefault(f => f != null && f.propertyname == item.Property);
                        if (prop != null) {
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
                var onechange = new CmsCssRuleChanges();
                onechange.CssRuleId = item.RuleId;
                onechange.selectorText = item.Selector;
                onechange.DeclarationText = item.Declarations.GenerateCssText();

                if (item.Declarations.item.Count() == 0)
                {
                    onechange.ChangeType = ChangeType.Delete;
                }
                else
                {
                    if (item.RuleId == default(Guid))
                    {
                        onechange.ChangeType = ChangeType.Add;
                    }
                    else
                    {
                        onechange.ChangeType = ChangeType.Update;
                    }
                }

                changelist.Add(onechange);

            }

            context.WebSite.SiteDb().CssRules.UpdateStyle(changelist, StyleId);
        }

    }
    
  public  class RuleChange
    {
        public Guid RuleId { get; set; }
        public string Selector { get; set; }

        public Dom.CSS.CSSDeclarationBlock Declarations { get; set; }
    }
}
