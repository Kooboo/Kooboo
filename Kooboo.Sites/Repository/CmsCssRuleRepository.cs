//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Data.Interface;
using Kooboo.Sites.Service;
using Kooboo.Data.Models;

namespace Kooboo.Sites.Repository
{
    public class CmsCssRuleRepository : SiteRepositoryBase<CmsCssRule>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<CmsCssRule>(o => o.ParentStyleId);
                paras.AddColumn<CmsCssRule>(o => o.ParentCssRuleId);
                paras.AddColumn<CmsCssRule>(o => o.OwnerObjectId);
                paras.AddColumn<CmsCssRule>(o => o.OwnerObjectConstType);
                paras.AddColumn<CmsCssRule>(o => o.ruleType);
                paras.AddColumn<CmsCssRule>(o => o.Id);
                paras.AddColumn<CmsCssRule>(o => o.IsInline);
                paras.SetPrimaryKeyField<CmsCssRule>(o => o.Id);
                return paras;
            }
        }

        public override bool AddOrUpdate(CmsCssRule value, Guid UserId = default(Guid))
        {
            return AddOrUpdate(value, UserId, false);
        }

        public bool AddOrUpdate(Models.CmsCssRule value, Guid UserId = default(Guid), bool updateSource = false)
        {
            var old = Get(value.Id);
            if (old == null)
            {
                RaiseBeforeEvent(value, ChangeType.Add);
                bool ok = Store.add(value.Id, value, false);
                if (ok)
                {
                    if (updateSource)
                    {
                        if (value.IsInline)
                        {
                            AddInlineCss(value.OwnerObjectId, value.OwnerObjectConstType, value.KoobooId, value.RuleText);
                        }
                        else
                        {
                            AddNewCssRule(value.ParentStyleId, value.CssText);
                        }
                    }

                    RaiseEvent(value, ChangeType.Add);
                }

            }

            else
            {
                if (!IsEqual(value, old))
                {
                    RaiseBeforeEvent(value, ChangeType.Update, old);

                    Store.update(value.Id, value, false);

                    if (updateSource)
                    {
                        if (value.IsInline)
                        {
                            UpdateInlineCss(value.Id, value.CssText);
                        }
                        else
                        {
                            CmsCssRuleChanges change = new CmsCssRuleChanges();
                            change.ChangeType = ChangeType.Update;
                            change.CssText = value.CssText;
                            change.CssRuleId = value.Id;

                            List<CmsCssRuleChanges> list = new List<CmsCssRuleChanges>();
                            list.Add(change);
                            UpdateStyle(list, value.ParentStyleId);

                        }
                    }

                    RaiseEvent(value, ChangeType.Update, old);
                }
            }

            return true;
        }

        public override void Delete(Guid id, Guid Userid = default(Guid))
        {
            Delete(id, true);
        }

        public void Delete(Guid id, bool updatesource = true)
        {
            var cssrule = Get(id);

            if (cssrule == null)
            { return; }


            if (updatesource)
            {
                if (cssrule.IsInline)
                { _RemoveInlineCss(cssrule); }
                else
                {
                    ///remove the css rule...
                    CmsCssRuleChanges change = new CmsCssRuleChanges();
                    change.ChangeType = ChangeType.Delete;
                    change.CssText = cssrule.CssText;
                    change.CssRuleId = cssrule.Id;

                    List<CmsCssRuleChanges> list = new List<CmsCssRuleChanges>();
                    list.Add(change);
                    UpdateStyle(list, cssrule.ParentStyleId); 
                }
            } 
            RaiseBeforeEvent(cssrule, ChangeType.Delete);
            Store.delete(id, false);
            RaiseEvent(cssrule, ChangeType.Delete); 
        }

        /// <summary>
        /// Update the inline css style, this cmscssrule must be verified to be inline
        /// </summary>
        /// <param name="CmsCssRuleId"></param>
        /// <param name="RuleText"></param>
        public void UpdateInlineCss(Guid CmsCssRuleId, string RuleText)
        {
            CmsCssRule cssrule = Get(CmsCssRuleId);
            if (cssrule == null)
            {
                return;
            }
            UpdateInlineCss(cssrule, RuleText);
        }

        public void UpdateInlineCss(CmsCssRule cssrule, string RuleText)
        {
            if (cssrule.IsInline)
            {
                cssrule.CssText = RuleText;
                var ownertype = Service.ConstTypeService.GetModelType(cssrule.OwnerObjectConstType);

                var repo = this.SiteDb.GetRepository(ownertype);
                if (repo == null)
                {
                    return;
                }
                var siteobject = repo.Get(cssrule.OwnerObjectId);

                if (siteobject != null && siteobject is IDomObject)
                {
                    var domobject = siteobject as IDomObject;
                    string newhtml = string.Empty;

                    Node node = Service.DomService.GetElementByKoobooId(domobject.Dom, cssrule.KoobooId);
                    if (node.nodeType == enumNodeType.ELEMENT)
                    {
                        Element element = node as Element;
                        element.removeAttribute("style");
                        element.attributes.Add(new Attr() { name = "style", value = RuleText });
                        newhtml = domobject.Dom.HtmlSource.Substring(0, element.location.openTokenStartIndex);
                        newhtml += Service.DomService.ReSerializeOpenTag(element);
                        newhtml += domobject.Dom.HtmlSource.Substring(element.location.openTokenEndIndex + 1);
                    }
                    domobject.Body = newhtml;
                    repo.AddOrUpdate(domobject);
                }
            }
        }

        public void AddInlineCss(Guid ObjectId, byte ObjectType, string KoobooId, string RuleText)
        {

            var ownertype = Service.ConstTypeService.GetModelType(ObjectType);

            var repo = this.SiteDb.GetRepository(ownertype);
            if (repo == null)
            {
                return;
            }
            var siteobject = repo.Get(ObjectId);

            if (siteobject != null && siteobject is IDomObject)
            {
                var domobject = siteobject as IDomObject;
                string newhtml = string.Empty;

                Node node = Service.DomService.GetElementByKoobooId(domobject.Dom, KoobooId);
                if (node.nodeType == enumNodeType.ELEMENT)
                {
                    Element element = node as Element;
                    element.removeAttribute("style");
                    element.attributes.Add(new Attr() { name = "style", value = RuleText });
                    newhtml = domobject.Dom.HtmlSource.Substring(0, element.location.openTokenStartIndex);
                    newhtml += Service.DomService.ReSerializeOpenTag(element);
                    newhtml += domobject.Dom.HtmlSource.Substring(element.location.openTokenEndIndex + 1);
                }
                domobject.Body = newhtml;
                repo.AddOrUpdate(domobject);

            }

        }

        /// <summary>
        /// remove inline css from the source and also from the relations. 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="CmsCssRuleId"></param>
        public void RemoveInlineCss(Guid CmsCssRuleId)
        {
            CmsCssRule cssrule = Get(CmsCssRuleId);
            _RemoveInlineCss(cssrule);
        }

        private void _RemoveInlineCss(CmsCssRule cssrule)
        {
            if (cssrule != null && cssrule.IsInline)
            {
                var ownertype = Service.ConstTypeService.GetModelType(cssrule.OwnerObjectConstType);

                var repo = this.SiteDb.GetRepository(ownertype);
                if (repo == null)
                {
                    return;
                }
                var siteobject = repo.Get(cssrule.OwnerObjectId);

                if (siteobject != null && siteobject is IDomObject)
                {
                    var domobject = siteobject as IDomObject;
                    string newhtml = string.Empty;

                    Node node = Service.DomService.GetElementByKoobooId(domobject.Dom, cssrule.KoobooId);
                    if (node.nodeType == enumNodeType.ELEMENT)
                    {
                        Element element = node as Element;
                        element.removeAttribute("style");
                        newhtml = domobject.Dom.HtmlSource.Substring(0, element.location.openTokenStartIndex);
                        newhtml += Service.DomService.ReSerializeOpenTag(element);
                        newhtml += domobject.Dom.HtmlSource.Substring(element.location.openTokenEndIndex + 1);
                    }
                    domobject.Body = newhtml;
                    repo.AddOrUpdate(domobject);

                }
            }

        }

        public void AddNewCssRule(Guid ParentStyleId, string selector, string declarationText)
        {
            CmsCssRuleChanges change = new CmsCssRuleChanges();
            change.selectorText = selector;
            change.ChangeType = ChangeType.Add;
            change.DeclarationText = declarationText;

            List<CmsCssRuleChanges> list = new List<CmsCssRuleChanges>();
            list.Add(change);
            UpdateStyle(list, ParentStyleId);
        }

        public void AddNewCssRule(Guid ParentStyleId, string cssText)
        {
            CmsCssRuleChanges change = new CmsCssRuleChanges();
            change.CssText = cssText;
            change.ChangeType = ChangeType.Add;

            List<CmsCssRuleChanges> list = new List<CmsCssRuleChanges>();
            list.Add(change);
            UpdateStyle(list, ParentStyleId);
        }

        public void AddOrUpdateDeclaration(Guid CmsCssRuleId, string PropertyName, string Value, bool Importance = false)
        { 
            var currentRule = Get(CmsCssRuleId);
            if (currentRule !=null)
            {
                string newvalue = GetRuleTextAddOrUpdateDeclaration(CmsCssRuleId, PropertyName, Value, Importance);
                currentRule.CssText = newvalue;
                AddOrUpdate(currentRule, default(Guid), true);  
            } 
        }
         
        // only for style rule. 
        public string GetRuleTextAddOrUpdateDeclaration(Guid CmsCssRuleId, string PropertyName, string Value, bool Importance = false)
        {
            var cmsrule = SiteDb.CssRules.Get(CmsCssRuleId);
            if (cmsrule != null)
            {
                var declarations = CSSSerializer.deserializeDeclarationBlock(cmsrule.CssText);
                declarations.updateDeclaration(new CSSDeclaration() { propertyname = PropertyName, value = Value, important = Importance });
                string ruletext = CSSSerializer.serializeDeclarationBlock(declarations);

                string newruletext = cmsrule.SelectorText + "\r\n{\r\n" + ruletext + "\r\n}\r\n";
                return newruletext;
            }
            return null;
        }

        /// <summary>
        /// make a list of changes to cssrules. 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="changelist"></param>
        public void UpdateStyle(List<CmsCssRuleChanges> changelist, Guid ParentStyleId)
        {
            Style style = SiteDb.Styles.Get(ParentStyleId);
            if (style == null)
            {
                return;
            }

            var ConvertedCssRules = Kooboo.Sites.Service.CssService.ConvertCss(style.Body, style.Id);

            List<ChangePlan> changeplans = new List<ChangePlan>();

            string cssText = style.Body;

            foreach (var item in changelist)
            {
                if (item.ChangeType == ChangeType.Update || item.ChangeType == ChangeType.Delete)
                {
                    var converted = ConvertedCssRules.Find(o => o.RuleId == item.CssRuleId);

                    CmsCssRule oldrule = converted != null ? converted.CmsRule : null;

                    if (oldrule != null)
                    {
                        CSSRule cssrule = converted.CssRule;

                        if (oldrule.ruleType == RuleType.StyleRule)
                        {
                            if (item.ChangeType == ChangeType.Delete)
                            {
                                changeplans.Add(new ChangePlan() { StartIndex = cssrule.StartIndex, EndIndex = cssrule.EndIndex, ChangeInto = "" });
                            }
                            else
                            {
                                changeplans.Add(new ChangePlan() { StartIndex = cssrule.StartIndex, EndIndex = cssrule.EndIndex, ChangeInto = item.CssText });
                            }
                        }

                        else if (oldrule.ruleType == RuleType.MediaRule)
                        {
                            // media rule can only change selector text. 
                            if (item.ChangeType == ChangeType.Update)
                            {
                                string newselector = item.selectorText;
                                changeplans.Add(new ChangePlan() { StartIndex = cssrule.StartIndex, EndIndex = cssrule.EndSelectorIndex, ChangeInto = newselector });
                            }
                            else
                            {
                                /// this is del this media rule. 
                                changeplans.Add(new ChangePlan() { StartIndex = cssrule.StartIndex, EndIndex = cssrule.EndIndex, ChangeInto = "" });
                            }
                        }
                        else if (oldrule.ruleType == RuleType.FontFaceRule)
                        {
                            if (item.ChangeType == ChangeType.Update)
                            {
                                string newcsstext = item.DeclarationText;
                                changeplans.Add(new ChangePlan() { StartIndex = cssrule.EndSelectorIndex + 1, EndIndex = cssrule.EndIndex, ChangeInto = newcsstext });
                            }
                            else
                            {
                                // to del one rule. 
                                changeplans.Add(new ChangePlan() { StartIndex = cssrule.StartIndex, EndIndex = cssrule.EndIndex, ChangeInto = "" });
                            }

                        }
                        else if (oldrule.ruleType == RuleType.ImportRule)
                        {
                            if (item.ChangeType == ChangeType.Update)
                            {
                                string newimportext = item.selectorText;
                                changeplans.Add(new ChangePlan() { StartIndex = cssrule.EndSelectorIndex + 1, EndIndex = cssrule.EndIndex, ChangeInto = newimportext });
                            }
                            else
                            {
                                changeplans.Add(new ChangePlan() { StartIndex = cssrule.StartIndex, EndIndex = cssrule.EndIndex, ChangeInto = "" });
                            }
                        }
                    }
                }

                else
                {
                    //this is a new add.
                    string newstring = "\r\n" + item.CssText;  
                    int insertposition = GetAppendIndex(ConvertedCssRules);
                    
                    if (item.CssText.IndexOf("@import", StringComparison.OrdinalIgnoreCase)!=-1)
                    {
                        changeplans.Add(new ChangePlan() { StartIndex = 0, EndIndex = -1, ChangeInto = newstring });
                    }

                  else  if (insertposition > -1)
                    {
                        changeplans.Add(new ChangePlan() { StartIndex = insertposition, EndIndex = insertposition - 1, ChangeInto = newstring });
                    }
                    else
                    {
                        changeplans.Add(new ChangePlan() { StartIndex = -1, EndIndex = -1, ChangeInto = newstring });
                    }
                }
            }

            string newCssText = ParseChange(cssText, changeplans);

            style.Body = newCssText;

            SiteDb.Styles.AddOrUpdate(style, true, true);
        }


        private int GetAppendIndex(List<CssConvertResult> rules)
        {
            int endindex = -1;
            if (rules != null)
            {
                foreach (var item in rules)
                {
                    if (item.CssRule != null && item.CssRule.EndIndex > endindex)
                    {
                        endindex = item.CssRule.EndIndex;
                    }
                }
            }
            if (endindex > -1)
            {
                endindex = endindex + 1;
            }
            return endindex;
        }

        public void UpdateDom(List<CmsCssRuleChanges> changes, Guid OwnerObjectId, byte OwnerObjectType)
        {
            IRepository repo;
            List<SourceUpdate> sourceupdates = new List<SourceUpdate>();

            var ownertype = Service.ConstTypeService.GetModelType(OwnerObjectType);

            repo = this.SiteDb.GetRepository(ownertype);
            if (repo == null)
            {
                return;
            }
            var siteobject = repo.Get(OwnerObjectId);

            if (siteobject != null && siteobject is DomObject)
            {
                var domobject = siteobject as DomObject;

                foreach (var item in changes)
                {
                    var cmsrule = SiteDb.CssRules.Get(item.CssRuleId);

                    Node node = Service.DomService.GetElementByKoobooId(domobject.Dom, cmsrule.KoobooId);
                    if (node != null && node.nodeType == enumNodeType.ELEMENT)
                    {
                        Element element = node as Element;
                        element.removeAttribute("style");
                        if (item.ChangeType != ChangeType.Delete)
                        {
                            element.attributes.Add(new Attr() { name = "style", value = item.CssText });
                        }
                        string newopentag = Service.DomService.ReSerializeOpenTag(element);

                        sourceupdates.Add(new SourceUpdate() { StartIndex = element.location.openTokenStartIndex, EndIndex = element.location.openTokenEndIndex, NewValue = newopentag });


                    }
                }

                if (sourceupdates.Count() > 0)
                {
                    domobject.Body = Sites.Service.DomService.UpdateSource(domobject.Dom.HtmlSource, sourceupdates);

                    repo.AddOrUpdate(domobject);
                }
            }
        }
         

        public static string ParseChange(string cssText, List<ChangePlan> changeplans)
        {
            string appendAddedCss = string.Empty;
            cssText = cssText ?? string.Empty;
            int currentindex = 0;
            int length = cssText.Length;
            StringBuilder sb = new StringBuilder();

            changeplans.Sort(new changecompare());

            foreach (var item in changeplans)
            {
                if (item.StartIndex <= -1 && item.EndIndex <= -1)
                {
                    appendAddedCss += "\r\n" + item.ChangeInto;
                }
                else
                { 
                    sb.Append(cssText.Substring(currentindex, item.StartIndex - currentindex));
                    sb.Append(item.ChangeInto);

                    currentindex = item.EndIndex + 1;
                }
            }

            if (currentindex < length - 1)
            {
                sb.Append(cssText.Substring(currentindex, length - currentindex));
            }
            return sb.ToString() + appendAddedCss;
        }


        /// <summary>
        /// get the sub cssrule of this style id.
        /// </summary>
        /// <param name="StyleId"></param>
        /// <returns></returns>
        public List<CmsCssRule> GetByStyleId(Guid StyleId)
        {
            return Query.Where(o => o.ParentStyleId == StyleId && o.ParentCssRuleId == default(Guid)).SelectAll();
        }

        /// <summary>
        /// get the sub rule of current cmscssrule.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="CssRuleId"></param>
        /// <returns></returns>
        public List<CmsCssRule> GetByRuleId(Guid CssRuleId)
        {
            return Query.Where(o => o.ParentCssRuleId == CssRuleId).SelectAll();
        }

        public List<CmsCssRule> GetInLineRules()
        {
            return Query.Where(o => o.IsInline).SelectAll().OrderBy(o => o.LastModified).Take(9999).ToList();
        }
        /// <summary>
        /// Used to show whethere this rule is being used or not.
        /// This determined based Dom Css Selector match.
        /// </summary>
        /// <param name="SiteDb"></param>
        /// <param name="CssRuleId"></param>
        /// <returns></returns>
        public List<UsedByRelation> ShowRelations(Guid CssRuleId)
        {
            var cssrule = SiteDb.CssRules.Get(CssRuleId);
            return ShowRelations(cssrule);
        }

        public List<UsedByRelation> ShowRelations(CmsCssRule cssrule)
        {
            List<UsedByRelation> result = new List<UsedByRelation>();
            if (cssrule == null || cssrule.ruleType != RuleType.StyleRule)
            { return result; }

            if (cssrule.IsInline)
            {
                var objectinfo = Service.ObjectService.GetObjectInfo(SiteDb, cssrule.OwnerObjectId, cssrule.OwnerObjectConstType);
                UsedByRelation usedby = new UsedByRelation();
                usedby.Name = objectinfo.Name;
                usedby.Remark = cssrule.KoobooOpenTag;
                usedby.ObjectId = objectinfo.ObjectId;
                usedby.ConstType = objectinfo.ConstType;
                usedby.Url = objectinfo.Url;
                result.Add(usedby);
                return result;
            }

            Style style = SiteDb.Styles.Get(cssrule.ParentStyleId);

            var relations = SiteDb.Styles.GetUsedBy(style);

            foreach (var item in relations)
            {
                var siteobject = Service.ObjectService.GetSiteObject(SiteDb, item.ObjectId, item.ConstType);
                Document dom = null;
                if (siteobject is IDomObject)
                {
                    var domobject = siteobject as IDomObject;
                    dom = domobject.Dom;
                }

                if (dom != null)
                {
                    var elements = dom.getElementsByCSSSelector(cssrule.SelectorText);

                    string name = null;
                    byte consttype = 0;
                    Guid ObjectId = default(Guid);
                    string url = null;

                    if (elements != null && elements.length > 0)
                    {
                        var objectinfo = Service.ObjectService.GetObjectInfo(SiteDb, siteobject);
                        name = objectinfo.Name;
                        consttype = objectinfo.ConstType;
                        ObjectId = objectinfo.ObjectId;
                        url = objectinfo.Url;
                    }

                    foreach (var eitem in elements.item)
                    {
                        var objectinfo = Service.ObjectService.GetObjectInfo(SiteDb, cssrule);
                        UsedByRelation usedby = new UsedByRelation();
                        usedby.Name = name;
                        usedby.ObjectId = ObjectId;
                        usedby.ConstType = consttype;
                        usedby.Remark = Service.DomService.GetOpenTag(eitem);
                        usedby.Url = url;
                        result.Add(usedby);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// List the rules that can be clean, that is not being used any more. 
        /// </summary>
        /// <returns></returns>
        public List<CmsCssRule> CleanUp()
        {
            Dictionary<Guid, Document> domlist = new Dictionary<Guid, Document>();

            List<CmsCssRule> results = new List<CmsCssRule>();

            foreach (var item in SiteDb.CssRules.All())
            {
                if (item.ruleType == RuleType.StyleRule && !item.IsInline)
                {
                    Style style = SiteDb.Styles.Get(item.ParentStyleId);
                    var relations = SiteDb.Styles.GetUsedBy(style);

                    foreach (var objItem in relations)
                    {
                        Document dom;
                        dom = null;

                        if (!domlist.ContainsKey(objItem.ObjectId))
                        {
                            Document newdoc = null;
                            var siteobject = Service.ObjectService.GetSiteObject(SiteDb, objItem.ObjectId, objItem.ConstType);

                            if (siteobject is IDomObject)
                            {
                                var domobject = siteobject as IDomObject;
                                newdoc = domobject.Dom;
                            }

                            domlist[objItem.ObjectId] = newdoc;
                        }

                        if (domlist.ContainsKey(objItem.ObjectId))
                        {
                            dom = domlist[objItem.ObjectId];
                        }

                        if (dom != null)
                        {

                            var elements = dom.getElementsByCSSSelector(item.SelectorText);

                            if (elements == null || elements.length == 0)
                            {
                                results.Add(item);
                                break;
                            }

                        }
                    }
                }
            }
            return results;
        }
         
        public List<CmsCssRule> ListUsedByPage(Guid PageId)
        {
            var relativeobjectids = this.SiteDb.Pages.GetRelatedOwnerObjectIds(PageId);
            return this.Query.WhereIn<Guid>(o => o.OwnerObjectId, relativeobjectids).SelectAll();
        } 
    }
}
