//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Sites.Render.Components;

namespace Kooboo.Sites.Relation
{
    /// <summary>
    /// compute the relation of items that has dom document. like page, view, layout, text content. 
    /// NOTE: condition to use this method is that all links to internal object is start with / and all reference url to external object is start with http or https. This is the case after any website import into kooboo.
    /// </summary>
    public class DomRelation
    {
        public static void Compute(SiteDb SiteDb, Document dom, Guid OwnerObjectId, byte OwnerConstType, string baseUrl = "", HtmlHeader header = null)
        {
            List<string> ResourceGroupUrl = ComputeReferenceStyle(dom, SiteDb, OwnerObjectId, OwnerConstType, baseUrl, header);

            var scriptgroup = ComputeReferenceScript(dom, SiteDb, OwnerObjectId, OwnerConstType, baseUrl, header);

            ResourceGroupUrl.AddRange(scriptgroup);

            ComputeUrlRelation(SiteDb, OwnerObjectId, OwnerConstType, ResourceGroupUrl, ConstObjectType.ResourceGroup);

            if (dom != null)
            {
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    dom.URL = baseUrl;
                }
                ComputeImage(dom, SiteDb, OwnerObjectId, OwnerConstType, baseUrl);

                ComputeEmbeddedStyle(dom, SiteDb, OwnerObjectId, OwnerConstType);

                ComputeEmbeddedForms(dom, SiteDb, OwnerObjectId, OwnerConstType);

                ComputeEmbeddedScript(dom, SiteDb, OwnerObjectId, OwnerConstType);

                ComputeEmbeddedKKScript(dom, SiteDb, OwnerObjectId, OwnerConstType);

                ComputeInlineCss(dom, SiteDb, OwnerObjectId, OwnerConstType, baseUrl);

                ComputeEmbededImage(dom, SiteDb, OwnerObjectId, OwnerConstType);

                ComputeLinks(dom, SiteDb, OwnerObjectId, OwnerConstType);

                computeLabel(dom, SiteDb, OwnerObjectId, OwnerConstType);

                computekConfig(dom, SiteDb, OwnerObjectId, OwnerConstType);

                ComputeComponent(dom, SiteDb, OwnerObjectId, OwnerConstType);

                computeLayout(dom, SiteDb, OwnerObjectId, OwnerConstType);      
            }
        }

        public static void ComputeComponent(Document dom, SiteDb sitedb, Guid objectId, byte constType)
        {
            foreach (var item in Kooboo.Sites.Render.Components.Container.List)
            {
                if (item.Value.StoreConstType == 0)
                {
                    continue;
                }
                var com = item.Value;
                var els = dom.getElementsByTagName(com.TagName).item;
                if (com.IsRegularHtmlTag)
                {
                    els.RemoveAll(o => !IsComponent(o, com));
                }

                List<Guid> ids = new List<Guid>();
                foreach (var el in els)
                {
                    if (!string.IsNullOrEmpty(el.id))
                    {
                        Guid id = Data.IDGenerator.GetOrGenerate(el.id, com.StoreConstType);
                        if (id != default(Guid))
                        {
                            ids.Add(id);
                        }
                    }
                }

                var currentRelations = sitedb.Relations.GetRelations(objectId, com.StoreConstType);
                foreach (var relation in currentRelations)
                {
                    if (!ids.Contains(relation.objectYId))
                    {
                        sitedb.Relations.Delete(relation.Id);
                    }
                }

                foreach (var id in ids)
                {
                    sitedb.Relations.AddOrUpdate(objectId, id, constType, com.StoreConstType);
                }
            }

        }

        public static bool IsComponent(Element element, IComponent com)
        {
            if (com.IsRegularHtmlTag)
            {
                if (Render.Components.Manager.ElementHasEngine(element))
                {
                    return true;
                }

                return false;
                // return  Kooboo.Sites.Render.Components.Manager.IsComponentElement(element);     
            }

            return true;
        }


        public static void computeLabel(Document dom, SiteDb sitedb, Guid objectId, byte constType)
        {
            List<Guid> labelid = new List<Guid>();

            List<Element> labelitems = new List<Element>();
            var oldlabels = dom.getElementByAttribute(ConstTALAttributes.label).item;
            labelitems.AddRange(oldlabels);
            var newlabels = dom.getElementByAttribute("k-label").item;
            labelitems.AddRange(newlabels);


            foreach (var item in labelitems)
            {
                string labelkey = item.getAttribute(ConstTALAttributes.label);

                if (string.IsNullOrEmpty(labelkey))
                {
                    labelkey = item.getAttribute("k-label");
                }

                if (string.IsNullOrEmpty(labelkey))
                {
                    continue;
                }

                string value = item.InnerHtml;
                if (string.IsNullOrEmpty(value))
                {
                    value = labelkey;
                }
                string defaultculture = sitedb.WebSite.DefaultCulture;

                var label = sitedb.Labels.GetOrAdd(labelkey, value, defaultculture);

                labelid.Add(label.Id);
            }

            var currentblockrelation = sitedb.Relations.GetRelations(objectId, ConstObjectType.Label);

            foreach (var item in currentblockrelation)
            {
                if (!labelid.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }
            foreach (var item in labelid)
            {
                sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.Label);
            }   
        }


        public static void computekConfig(Document dom, SiteDb sitedb, Guid objectId, byte constType)
        {
            List<Guid> configids = new List<Guid>();

            List<Element> configitems = new List<Element>();
         
            var configtags = dom.getElementByAttribute("k-config").item;

            configitems.AddRange(configtags);
                                               
            foreach (var item in configitems)
            {
                string key = item.getAttribute("k-config");
                
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                //var config = sitedb.KConfig.GetOrAdd(key, item.tagName, item.OuterHtml);
                var config = sitedb.KConfig.GetOrAdd(key, item);

                configids.Add(config.Id);
            }

            var currentblockrelation = sitedb.Relations.GetRelations(objectId, ConstObjectType.Kconfig);

            foreach (var item in currentblockrelation)
            {
                if (!configids.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }
            foreach (var item in configids)
            {
                sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.Kconfig);
            }
        }



        public static void computeLayout(Document dom, SiteDb sitedb, Guid objectId, byte constType)
        {
            List<Guid> layoutids = new List<Guid>();
            foreach (var item in dom.getElementsByTagName("layout").item)
            {
                if (!string.IsNullOrEmpty(item.id))
                {
                    Guid layoutid = default(Guid);
                    if (!System.Guid.TryParse(item.id, out layoutid))
                    {
                        layoutid = Data.IDGenerator.Generate(item.id, ConstObjectType.Layout);
                    }

                    if (layoutid != default(Guid))
                    {
                        layoutids.Add(layoutid);
                    }
                }
            }

            var currentLayoutRelation = sitedb.Relations.GetRelations(objectId, ConstObjectType.Layout);
            foreach (var item in currentLayoutRelation)
            {
                if (!layoutids.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }

            foreach (var item in layoutids)
            {
                sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.Layout);
            }
        }


        #region Embedded Relation

        private static List<Style> GetEmbeddedStyles(Document dom, Guid objectId, byte constType)
        {
            List<Style> stylelist = new List<Style>();

            HTMLCollection embedStyle = dom.getElementsByTagName("style");
            int itemindexcounter = 0;

            foreach (var item in embedStyle.item)
            {
                Style style = new Style();
                style.OwnerObjectId = objectId;
                style.OwnerConstType = constType;
                style.ItemIndex = itemindexcounter;
                style.Body = item.InnerHtml;
                style.IsEmbedded = true;
                style.media = item.getAttribute("media");
                style.Engine = item.getAttribute("engine");
                if (!string.IsNullOrWhiteSpace(style.Engine))
                {
                    style.Extension = style.Engine;
                }
                stylelist.Add(style);

                itemindexcounter += 1;
            }
            return stylelist;
        }

        private static List<Form> GetEmbeddedForms(Document dom, Guid objectId, byte constType)
        {
            List<Form> formlist = new List<Form>();
            int itemindexcounter = 0;
            foreach (var item in dom.forms.item)
            {
                if (!Kooboo.Sites.Render.Components.Manager.IsComponentElement(item))
                {
                    if (!Service.DomService.IsAspNetWebForm(item))
                    {
                        Form form = new Form
                        {
                            OwnerObjectId = objectId,
                            OwnerConstType = constType,
                            Body = item.InnerHtml,
                            KoobooId = DomService.GetKoobooId(item),
                            Method = item.getAttribute("method") ?? "get",
                            IsEmbedded = true
                        };

                        form.Engine = item.getAttribute("engine");

                        form.ItemIndex = itemindexcounter;

                        foreach (var att in item.attributes)
                        {
                            form.Attributes.Add(att.name, att.value);
                        }
                        formlist.Add(form);
                    }
                }

            }
            return formlist;
        }

        private static List<Script> GetEmbeddedScripts(Document dom, Guid objectId, byte constType)
        {
            List<Script> ScriptList = new List<Script>();

            HTMLCollection embedScript = dom.getElementsByTagName("script");
            int itemindexcounter = 0;

            foreach (var item in embedScript.item)
            {
                if (!item.hasAttribute("src"))
                {
                    if (!Kooboo.Sites.Render.Components.Manager.IsKScript(item))
                    {
                        string scripttext = item.InnerHtml;
                        if (string.IsNullOrEmpty(scripttext))
                        {
                            continue;
                        }
                        Script script = new Script();
                        script.OwnerObjectId = objectId;
                        script.OwnerConstType = constType;
                        script.ItemIndex = itemindexcounter;
                        script.IsEmbedded = true;
                        script.Body = scripttext;
                        script.Engine = item.getAttribute("engine");

                        if (!string.IsNullOrWhiteSpace(script.Engine))
                        {
                            script.Extension = script.Engine;
                        }

                        ScriptList.Add(script);
                    }
                }
                itemindexcounter += 1;
            }
            return ScriptList;
        }

        private static List<Code> GetEmbeddedKKScripts(Document dom, Guid objectId, byte constType)
        {
            List<Code> ScriptList = new List<Code>();

            HTMLCollection embedScript = dom.getElementsByTagName("script");
            int itemindexcounter = 0;

            foreach (var item in embedScript.item)
            {
                if (!item.hasAttribute("src"))
                {
                    if (Kooboo.Sites.Render.Components.Manager.IsKScript(item))
                    {
                        string scripttext = item.InnerHtml;
                        if (string.IsNullOrEmpty(scripttext))
                        {
                            continue;
                        }
                        Code script = new Code();
                        script.OwnerObjectId = objectId;
                        script.OwnerConstType = constType;
                        script.ItemIndex = itemindexcounter;
                        script.IsEmbedded = true;
                        script.CodeType = CodeType.PageScript;
                        script.Body = scripttext;
                        script.Engine = item.getAttribute("engine");
                        ScriptList.Add(script);
                    }
                }
                itemindexcounter += 1;
            }
            return ScriptList;
        }


        public static Element GetEmbeddedByItemIndex(Document dom, int itemindex, string tagName)
        {
            HTMLCollection embedStyle = dom.getElementsByTagName(tagName);
            int itemindexcounter = 0;

            foreach (var item in embedStyle.item)
            {
                if (itemindexcounter == itemindex)
                {
                    return item;
                }
                itemindexcounter += 1;
            }
            return null;
        }

        public static void ComputeEmbeddedStyle(Document dom, SiteDb sitedb, Guid objectId, byte constType)
        {
            var NewStyleList = GetEmbeddedStyles(dom, objectId, constType);

            var oldStyles = sitedb.Styles.Query.Where(o => o.OwnerObjectId == objectId && o.IsEmbedded == true).SelectAll();

            foreach (var item in oldStyles)
            {
                if (NewStyleList.Find(o => o.Id == item.Id) == null)
                {
                    sitedb.Styles.Delete(item.Id, false);
                }
            }

            foreach (var item in NewStyleList)
            {
                sitedb.Styles.AddOrUpdate(item);
            }
        }

        public static void ComputeEmbeddedForms(Document dom, SiteDb sitedb, Guid objectId, byte constType)
        {
            var newforms = GetEmbeddedForms(dom, objectId, constType);

            var oldforms = sitedb.Forms.Query.Where(o => o.OwnerObjectId == objectId).SelectAll();

            foreach (var item in oldforms)
            {
                if (newforms.Find(o => o.Id == item.Id) == null)
                {
                    sitedb.Forms.Delete(item.Id, false);
                }
            }

            foreach (var item in newforms)
            {
                // form, do not overwritte existing setting. 
                var existing = sitedb.Forms.Get(item.Id);

                if (existing != null)
                {
                    item.FormSubmitter = existing.FormSubmitter;
                    item.FailedCallBack = existing.FailedCallBack;
                    item.Method = existing.Method;
                    item.RedirectUrl = existing.RedirectUrl;
                    item.Setting = existing.Setting;
                    item.SuccessCallBack = existing.SuccessCallBack;
                    item.IsEmbedded = existing.IsEmbedded;
                }
                sitedb.Forms.AddOrUpdate(item);
            }
        }

        public static void ComputeEmbeddedScript(Document dom, SiteDb sitedb, Guid objectId, byte constType)
        {
            var newScriptList = GetEmbeddedScripts(dom, objectId, constType);

            var oldscript = sitedb.Scripts.Query.Where(o => o.OwnerObjectId == objectId && o.IsEmbedded == true).SelectAll();

            foreach (var item in oldscript)
            {
                if (!newScriptList.Where(o => o.Id == item.Id).Any())
                {
                    sitedb.Scripts.Delete(item.Id, false);
                }
            }

            foreach (var item in newScriptList)
            {
                sitedb.Scripts.AddOrUpdate(item);
            }
        }

        public static void ComputeEmbeddedKKScript(Document dom, SiteDb sitedb, Guid objectId, byte constType)
        {
            var newScriptList = GetEmbeddedKKScripts(dom, objectId, constType);

            var oldscript = sitedb.Code.Query.Where(o => o.OwnerObjectId == objectId && o.IsEmbedded == true).SelectAll();

            foreach (var item in oldscript)
            {
                if (!newScriptList.Where(o => o.Id == item.Id).Any())
                {
                    sitedb.Code.Delete(item.Id, false);
                }
            }

            foreach (var item in newScriptList)
            {
                sitedb.Code.AddOrUpdate(item);
            }
        }

        #endregion

        /// <summary>
        /// compute the styles of
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sitedb"></param>
        public static List<string> ComputeReferenceStyle(Document doc, SiteDb sitedb, Guid objectId, byte constType, string baseUrl = "", HtmlHeader header = null)
        {
            List<string> UrlList = GetReferenceStyleUrl(doc, header);
            DomUrlService.MakeAllUrlRelative(UrlList, baseUrl);

            List<string> styleurls = new List<string>();
            List<string> groupurls = new List<string>();

            foreach (var item in UrlList)
            {
                if (Kooboo.Sites.Service.GroupService.IsGroupUrl(item))
                {
                    groupurls.Add(item);
                }
                else
                {
                    styleurls.Add(item);
                }
            }

            ComputeUrlRelation(sitedb, objectId, constType, styleurls, ConstObjectType.Style);

            return groupurls;
        }

        public static List<string> GetReferenceStyleUrl(Document doc, HtmlHeader header)
        {
            List<string> UrlList = Service.DomUrlService.GetReferenceStyles(doc);
            if (header != null)
            {
                UrlList.AddRange(header.Styles);
            }
            return UrlList;
        }

        /// <summary>
        /// compute html tag inline css. 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="sitedb"></param>
        /// <param name="objectId"></param>
        /// <param name="constType"></param>
        public static void ComputeInlineCss(Document doc, SiteDb sitedb, Guid objectId, byte constType, string baseurl)
        {
            HTMLCollection elements = doc.getElementByAttribute("style");

            List<CmsCssRule> cmsrules = new List<CmsCssRule>();

            foreach (var item in elements.item)
            {
                string style = item.getAttribute("style");

                if (!string.IsNullOrEmpty(style))
                {
                    CmsCssRule rule = new CmsCssRule();
                    rule.OwnerObjectId = objectId;
                    rule.OwnerObjectConstType = constType;
                    rule.IsInline = true;
                    rule.CssText = style;
                    rule.KoobooId = Service.DomService.GetKoobooId(item);
                    rule.KoobooOpenTag = Service.DomService.GetOpenTag(item);
                    rule.DisplayName = Service.DomService.GetElementDisplayName(item);
                    var decls = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(style);
                    rule.Properties = decls.item.Select(o => o.propertyname).ToList();
                    cmsrules.Add(rule);
                }
            }

            /// get old relation. 
            List<CmsCssRule> oldrules = sitedb.CssRules.Query.Where(o => o.OwnerObjectId == objectId && o.IsInline).SelectAll();

            foreach (var item in oldrules)
            {
                if (!cmsrules.Where(o => o.Id == item.Id).Any())
                {
                    sitedb.CssRules.Delete(item.Id, false);
                }
            }

            foreach (var item in cmsrules)
            {
                sitedb.CssRules.AddOrUpdate(item, default(Guid), false);
                Relation.CmsCssRuleRelation.ComputeUrl(item, baseurl, sitedb);
            }
        }

        /// <summary>
        /// Compute the image references, includes internal and external image references. 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sitedb"></param>
        public static void ComputeImage(Document doc, SiteDb sitedb, Guid objectId, byte constType, string baseUrl = "")
        {
            var imagelinks = Service.DomUrlService.GetImages(doc);

            DomUrlService.MakeAllUrlRelative(imagelinks, baseUrl);

            ComputeUrlRelation(sitedb, objectId, constType, imagelinks, ConstObjectType.Image);

        }

        /// <summary>
        /// Compute the embedded image that use base64 encoded image. 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sitedb"></param>
        public static void ComputeEmbededImage(Document doc, SiteDb sitedb, Guid objectId, byte constType)
        {
            return;
        }

        public static List<string> ComputeReferenceScript(Document doc, SiteDb sitedb, Guid objectId, byte constType, string baseUrl = "", HtmlHeader header = null)
        {
            List<string> UrlList = Service.DomUrlService.GetReferenceScripts(doc);
            if (header != null)
            {
                UrlList.AddRange(header.Scripts);
            }
            DomUrlService.MakeAllUrlRelative(UrlList, baseUrl);

            List<string> scripturls = new List<string>();
            List<string> groupurls = new List<string>();

            foreach (var item in UrlList)
            {
                if (Kooboo.Sites.Service.GroupService.IsGroupUrl(item))
                {
                    groupurls.Add(item);
                }
                else
                {
                    scripturls.Add(item);
                }
            }

            ComputeUrlRelation(sitedb, objectId, constType, scripturls, ConstObjectType.Script);

            return groupurls;
        }

        public static void ComputeLinks(Document doc, SiteDb sitedb, Guid objectId, byte constType, string baseUrl = "")
        {
            List<string> UrlList = DomUrlService.GetLinks(doc);
            DomUrlService.MakeAllUrlRelative(UrlList, baseUrl);

            ComputeUrlRelation(sitedb, objectId, constType, UrlList, ConstObjectType.Link);
        }

        public static List<Guid> GetRouteIds(SiteDb SiteDb, string Url)
        {
            List<Guid> result = new List<Guid>();
            if (Url.StartsWith(Sites.Systems.Routes.SystemRoutePrefix))
            {
                // this only has resoruce group...
                var dict = Sites.Systems.Routes.ParseSystemRoute(SiteDb, Url);
                if (dict.ContainsKey("nameorid"))
                {
                    var nameorid = dict["nameorid"];
                    var group = SiteDb.ResourceGroups.GetByNameOrId(nameorid);

                    if (group != null)
                    {
                        result.AddRange(group.Children.Keys);
                    }
                }
            }
            else
            {
                var route = Kooboo.Sites.Routing.ObjectRoute.GetRoute(SiteDb, Url);

                if (route != null)
                {
                    result.Add(route.Id);
                }
                else
                {
                    Guid routeid = Data.IDGenerator.GetRouteId(Url);
                    result.Add(routeid);
                }
            }

            return result;

        }

        public static void ComputeEmbed(Document doc, SiteDb sitedb, Guid objectId, byte constType)
        {
            HTMLCollection embedElement = doc.getElementsByTagName("embed");

            List<string> UrlList = new List<string>();
            foreach (var item in embedElement.item)
            {
                string fileurl = DomUrlService.GetLinkOrSrc(item);

                if (string.IsNullOrEmpty(fileurl))
                {
                    continue;
                }
                UrlList.Add(fileurl);
            }
            ComputeUrlRelation(sitedb, objectId, constType, UrlList, ConstObjectType.CmsFile);
        }

        public static void ComputeUrlRelation(SiteDb sitedb, Guid objectId, byte constType, List<string> urllist, byte DestConstType)
        {
            List<Guid> internalRoutes = new List<Guid>();
            List<Guid> ExternalResource = new List<Guid>();

            var FinalDestConstType = DestConstType;

            var oldRouteRelations = sitedb.Relations.GetRelationViaRoutes(objectId, DestConstType);

            var oldExternalResourceRelations = sitedb.Relations.GetExternalRelations(objectId, DestConstType);

            foreach (var item in urllist.Distinct())
            {
                if (DestConstType == ConstObjectType.Link)
                {
                    FinalDestConstType = Kooboo.Sites.Service.ConstTypeService.GetConstTypeByUrl(item);
                }

                if (Service.DomUrlService.IsExternalLink(item))
                {
                    Guid externalid = Kooboo.Data.IDGenerator.Generate(item, ConstObjectType.ExternalResource);
                    ExternalResource.Add(externalid);
                    if (oldExternalResourceRelations.Find(o => o.objectYId == externalid) == null)
                    {
                        sitedb.ExternalResource.AddOrUpdate(item, FinalDestConstType);
                    }
                }
                else
                {
                    var routeids = GetRouteIds(sitedb, item);
                    internalRoutes.AddRange(routeids);
                    if (routeids.Count == 1 && oldRouteRelations.Find(o => o.objectYId == routeids[0]) == null)
                    {
                        sitedb.Routes.EnsureExists(item, FinalDestConstType);
                    }
                }
            }

            foreach (var item in oldRouteRelations)
            {
                if (!internalRoutes.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }

            foreach (var item in internalRoutes)
            {
                if (oldRouteRelations.Find(o => o.objectYId == item) == null)
                {
                    sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.Route, DestConstType);
                }
            }

            foreach (var item in oldExternalResourceRelations)
            {
                if (!ExternalResource.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }

            foreach (var item in ExternalResource)
            {
                if (oldExternalResourceRelations.Find(o => o.objectYId == item) == null)
                {
                    sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.ExternalResource, DestConstType);
                }
            }
        }

    }
}
