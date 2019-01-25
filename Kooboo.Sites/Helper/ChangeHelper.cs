//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Helper
{
    public static class ChangeHelper
    {
        public static void ChangeUrl(SiteDb sitedb, IRepository repo, Guid ObjectId, string OldUrl, string NewUrl)
        {
            var UseObject = repo.Get(ObjectId);
            if (UseObject != null)
            {
                if (UseObject is IDomObject)
                {
                    var domobject = UseObject as IDomObject;
                    if (domobject != null)
                    {
                        string body = domobject.Body;
                        string newbody = Service.DomService.UpdateUrl(body, OldUrl, NewUrl);
                        domobject.Body = newbody;
                        repo.AddOrUpdate(domobject);
                    }
                }
                else if (UseObject is Style)
                {
                    var style = UseObject as Style;
                    style.Body = Lib.Helper.StringHelper.ReplaceIgnoreCase(style.Body, OldUrl, NewUrl);
                    sitedb.Styles.AddOrUpdate(style);
                }
                else if (UseObject is CmsCssRule)
                {
                    var rule = UseObject as CmsCssRule;
                    if (rule.IsInline)
                    {
                        var rulerepo = sitedb.GetRepository(rule.OwnerObjectConstType);
                        if (rulerepo != null)
                        {
                            var ownerobject = rulerepo.Get(rule.OwnerObjectId);
                            if (ownerobject != null && ownerobject is IDomObject)
                            {
                                var domobject = ownerobject as IDomObject;
                                var node = Service.DomService.GetElementByKoobooId(domobject.Dom, rule.KoobooId);
                                if (node != null && node.nodeType == Dom.enumNodeType.ELEMENT)
                                {
                                    var el = node as Kooboo.Dom.Element;
                                    string style = el.getAttribute("style");

                                    if (!string.IsNullOrEmpty(style))
                                    {
                                        style = Lib.Helper.StringHelper.ReplaceIgnoreCase(style, OldUrl, NewUrl);
                                    }
                                    el.setAttribute("style", style);

                                    string newhtml = Service.DomService.ReSerializeElement(el);

                                    List<SourceUpdate> updates = new List<SourceUpdate>();
                                    updates.Add(new SourceUpdate() { StartIndex = el.location.openTokenStartIndex, EndIndex = el.location.endTokenEndIndex, NewValue = newhtml });

                                    domobject.Body = Service.DomService.UpdateSource(domobject.Body, updates);
                                    rulerepo.AddOrUpdate(domobject);
                                }
                            }
                        }
                    }
                    else if (rule.ruleType== RuleType.ImportRule)
                    {

                        string newdecltext = Lib.Helper.StringHelper.ReplaceIgnoreCase(rule.RuleText, OldUrl, NewUrl);
                         
                        List<CmsCssRuleChanges> changelist = new List<CmsCssRuleChanges>();
                        CmsCssRuleChanges changes = new CmsCssRuleChanges();
                        changes.CssRuleId = rule.Id;
                        changes.selectorText = newdecltext; // rule.SelectorText;
                        changes.DeclarationText = newdecltext;
                        changes.ChangeType = ChangeType.Update;
                        changelist.Add(changes);

                        Guid parentstyle = rule.ParentStyleId;
                        if (parentstyle == default(Guid))
                        {
                            parentstyle = rule.OwnerObjectId;
                        }
                        sitedb.CssRules.UpdateStyle(changelist, parentstyle);

                    }
                    else
                    {
                        var declaration = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(rule.CssText);
                        foreach (var item in declaration.item)
                        {
                            if (item.value.IndexOf(OldUrl, StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                item.value = Lib.Helper.StringHelper.ReplaceIgnoreCase(item.value, OldUrl, NewUrl);
                            }
                        }
                        string newdecltext = Kooboo.Dom.CSS.CSSSerializer.serializeDeclarationBlock(declaration);

                        List<CmsCssRuleChanges> changelist = new List<CmsCssRuleChanges>();
                        CmsCssRuleChanges changes = new CmsCssRuleChanges();
                        changes.CssRuleId = rule.Id;
                        changes.selectorText = rule.SelectorText;
                        changes.DeclarationText = newdecltext;
                        changes.ChangeType = ChangeType.Update;
                        changelist.Add(changes);
                        Guid parentstyle = rule.ParentStyleId;
                        if (parentstyle == default(Guid))
                        {
                            parentstyle = rule.OwnerObjectId;
                        }
                        sitedb.CssRules.UpdateStyle(changelist, parentstyle);
                    }
                }

                else if (UseObject is ResourceGroup)
                {
                    var oldid = Data.IDGenerator.GetRouteId(OldUrl);

                    var group = UseObject as ResourceGroup;

                    int neworder = 0;

                    if (group.Children.ContainsKey(oldid))
                    {
                        neworder = group.Children[oldid];
                    }

                    group.Children.Remove(oldid);

                    if (NewUrl != null && !NewUrl.ToLower().StartsWith("http://") && !NewUrl.ToLower().StartsWith("https://"))
                    {
                        var newid = Data.IDGenerator.GetRouteId(NewUrl);
                        group.Children[newid] = neworder;
                        sitedb.ResourceGroups.AddOrUpdate(group);
                    }
                }
            }
        }

        public static void DeleteUrl(SiteDb sitedb, IRepository repo, Guid ObjectId, string OldUrl)
        {
            var UseObject = repo.Get(ObjectId);
            if (UseObject != null)
            {
                if (UseObject is IDomObject)
                {
                    var domobject = UseObject as IDomObject;
                    if (domobject != null)
                    {
                        string body = domobject.Body;
                        string newbody = Service.DomService.DeleteUrl(body, OldUrl);
                        domobject.Body = newbody;
                        repo.AddOrUpdate(domobject);
                    }
                }
                else if (UseObject is Style)
                {
                    var style = UseObject as Style;
                    style.Body = Lib.Helper.StringHelper.ReplaceIgnoreCase(style.Body, OldUrl, "");
                    sitedb.Styles.AddOrUpdate(style);
                }
                else if (UseObject is CmsCssRule)
                {
                    var rule = UseObject as CmsCssRule;
                    if (rule.IsInline)
                    {
                        var rulerepo = sitedb.GetRepository(rule.OwnerObjectConstType);
                        if (rulerepo != null)
                        {
                            var ownerobject = rulerepo.Get(rule.OwnerObjectId);
                            if (ownerobject != null && ownerobject is IDomObject)
                            {
                                var domobject = ownerobject as IDomObject;
                                var node = Service.DomService.GetElementByKoobooId(domobject.Dom, rule.KoobooId);
                                if (node != null && node.nodeType == Dom.enumNodeType.ELEMENT)
                                {
                                    var el = node as Kooboo.Dom.Element;
                                    string style = el.getAttribute("style");
                                    if (!string.IsNullOrEmpty(style))
                                    {
                                        string[] seperators = new string[1];
                                        seperators[0] = ";";
                                        string[] parts = style.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                                        int count = parts.Length;
                                        for (int i = 0; i < count; i++)
                                        {
                                            if (parts[i].IndexOf(OldUrl, StringComparison.OrdinalIgnoreCase) > -1)
                                            {
                                                parts[i] = "";
                                            }
                                        }
                                        style = string.Join(";", parts);
                                    }

                                    if (string.IsNullOrWhiteSpace(style))
                                    {
                                        el.removeAttribute("style");
                                    }
                                    else
                                    {
                                        el.setAttribute("style", style);
                                    }

                                    string newhtml = Service.DomService.ReSerializeElement(el);

                                    List<SourceUpdate> updates = new List<SourceUpdate>();
                                    updates.Add(new SourceUpdate() { StartIndex = el.location.openTokenStartIndex, EndIndex = el.location.endTokenEndIndex, NewValue = newhtml });

                                    domobject.Body = Service.DomService.UpdateSource(domobject.Body, updates);
                                    rulerepo.AddOrUpdate(domobject);
                                }
                            }
                        }
                    }
                    else if (rule.ruleType == RuleType.ImportRule)
                    {
                         
                        List<CmsCssRuleChanges> changelist = new List<CmsCssRuleChanges>();
                        CmsCssRuleChanges changes = new CmsCssRuleChanges();
                        changes.CssRuleId = rule.Id;
                        changes.selectorText = rule.SelectorText;
                        changes.DeclarationText = "";
                        changes.ChangeType = ChangeType.Delete;
                        changelist.Add(changes);
                        Guid parentstyle = rule.ParentStyleId;
                        if (parentstyle == default(Guid))
                        {
                            parentstyle = rule.OwnerObjectId;
                        }

                        sitedb.CssRules.UpdateStyle(changelist, parentstyle);

                    }
                    else
                    {
                        var declaration = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(rule.CssText);

                        var found = declaration.item.FindAll(o => !string.IsNullOrEmpty(o.value) && o.value.IndexOf(OldUrl, StringComparison.OrdinalIgnoreCase) > -1);
                        if (found != null && found.Count > 0)
                        {
                            foreach (var founditem in found)
                            {
                                declaration.item.Remove(founditem);
                            }
                        }

                        string newdecltext = Kooboo.Dom.CSS.CSSSerializer.serializeDeclarationBlock(declaration);

                        List<CmsCssRuleChanges> changelist = new List<CmsCssRuleChanges>();
                        CmsCssRuleChanges changes = new CmsCssRuleChanges();
                        changes.CssRuleId = rule.Id;
                        changes.selectorText = rule.SelectorText;
                        changes.DeclarationText = newdecltext;
                        changes.ChangeType = ChangeType.Update;
                        changelist.Add(changes);
                        Guid parentstyle = rule.ParentStyleId;
                        if (parentstyle == default(Guid))
                        {
                            parentstyle = rule.OwnerObjectId;
                        }
                        sitedb.CssRules.UpdateStyle(changelist, parentstyle);
                    }


                }

                else if (UseObject is ResourceGroup)
                {
                    var oldid = Data.IDGenerator.GetRouteId(OldUrl);

                    var group = UseObject as ResourceGroup;

                    group.Children.Remove(oldid);

                    repo.AddOrUpdate(group);

                }

            }
        }

        public static void DeleteRoutableObject(SiteDb sitedb, IRepository repo, ISiteObject value)
        {
            // delete route.. 
            //TODO: 
            // Sites.Helper.ChangeHelper.DeleteUrl(this.SiteDb,)   
            var objectroutes = sitedb.Routes.Query.Where(o => o.objectId == value.Id).SelectAll();
            foreach (var item in objectroutes)
            {
                var referredby = sitedb.Relations.GetReferredByRelations(item.Id);

                foreach (var by in referredby)
                {
                    var repofrom = sitedb.GetRepository(by.ConstTypeX);
                    if (repofrom != null)
                    {
                        Sites.Helper.ChangeHelper.DeleteUrl(sitedb, repofrom, by.objectXId, item.Name);
                    }
                }

                var stillusedby = sitedb.Relations.Query.Where(o => o.objectYId == item.Id).SelectAll();
                if ((stillusedby == null || stillusedby.Count == 0 || (stillusedby.Count == 1 && stillusedby[0].objectXId == value.Id)))
                {
                    sitedb.Routes.Delete(item.Id);
                }
                else
                {
                    item.objectId = default(Guid);
                    sitedb.Routes.AddOrUpdate(item);
                }
            }
        }

    
        public static void DeleteComponentFromSource(SiteDb sitedb, ISiteObject value)
        {
            if (value is IEmbeddable)
            {
                var embed = value as IEmbeddable; 
                if (embed.IsEmbedded)
                {
                    // embedded handled by update source. 
                    return; 
                }
            }

            var com = Kooboo.Sites.Render.Components.Container.GetByConstType(value.ConstType);
            if (com != null)
            {
                var relations = sitedb.Relations.GetReferredByRelations(value.Id);

                if (relations != null && relations.Count > 0)
                {
                    foreach (var relation in relations)
                    {
                        var repo = sitedb.GetRepository(relation.ConstTypeX);

                        var UseObject = repo.Get(relation.objectXId);

                        if (UseObject != null)
                        {
                            if (UseObject is IDomObject)
                            {
                                var domobject = UseObject as IDomObject;
                                if (domobject != null)
                                {
                                    List<SourceUpdate> updates = new List<SourceUpdate>();

                                    var el = domobject.Dom.getElementsByTagName(com.TagName);

                                    foreach (var item in el.item)
                                    {
                                        if (!string.IsNullOrEmpty(com.StoreEngineName))
                                        {
                                            var engine = item.getAttribute("engine");
                                            if (!Lib.Helper.StringHelper.IsSameValue(engine, com.StoreEngineName))
                                            {
                                                continue;
                                            }
                                        }

                                        if (Lib.Helper.StringHelper.IsSameValue(item.id, value.Name))
                                        {
                                            updates.Add(new SourceUpdate() { StartIndex = item.location.openTokenStartIndex, EndIndex = item.location.endTokenEndIndex });
                                        }
                                    }

                                    string newbody = Service.DomService.UpdateSource(domobject.Body, updates);
                                    domobject.Body = newbody;
                                    repo.AddOrUpdate(domobject);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
