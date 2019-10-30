//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Dom;
using Kooboo.Events.Cms;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Relation
{
    public static class RelationManager
    {
        public static void Compute(SiteObjectEvent siteEvent)
        {
            if (siteEvent?.Value == null)
            {
                return;
            }

            if (siteEvent.Value is Style)
            {
                ComputeStyle(siteEvent);
                return;
            }

            if (siteEvent.Value is IDataMethodSetting dataMethodSetting)
            {
                if (siteEvent.ChangeType == ChangeType.Delete)
                {
                    DataMethodRelation.Clean(siteEvent.SiteDb, siteEvent.Value.Id);
                }
                else
                {
                    DataMethodRelation.Compute(dataMethodSetting, siteEvent.SiteDb);
                }
            }

            if (siteEvent.Value is ResourceGroup)
            {
                if (siteEvent.ChangeType == ChangeType.Delete)
                {
                    siteEvent.SiteDb.Relations.CleanObjectRelation(siteEvent.Value.Id);
                }
                else
                {
                    GroupRelation.Compute(siteEvent.Value as ResourceGroup, siteEvent.SiteDb);
                }
            }

            Type modelType = siteEvent.Value.GetType();

            //only below needs to compute relation.
            if (siteEvent.Value is IDomObject || modelType == typeof(CmsCssRule))
            {
                string basurl = null;
                HtmlHeader header = null;
                Document dom = null;

                if (Attributes.AttributeHelper.IsRoutable(modelType))
                {
                    basurl = Service.ObjectService.GetObjectRelativeUrl(siteEvent.SiteDb, siteEvent.Value.Id, siteEvent.Value.ConstType);
                }

                if (modelType == typeof(Page))
                {
                    var page = siteEvent.Value as Page;
                    header = page?.Headers;
                }

                if (!ShouldCheck(modelType, siteEvent))
                {
                    return;
                }

                dom = GetDom(modelType, siteEvent);

                if (siteEvent.ChangeType == ChangeType.Delete)
                {
                    if (siteEvent.Value is SiteObject siteobject)
                    {
                        RelationManager.Clean(siteEvent.SiteDb, siteobject);
                    }
                }
                else
                {
                    if (dom != null || siteEvent.Value is IDomObject)
                    {
                        DomRelation.Compute(siteEvent.SiteDb, dom, siteEvent.Value.Id, siteEvent.Value.ConstType, basurl, header);
                    }
                }
            }
        }

        public static Document GetDom(Type modelType, SiteObjectEvent siteEvent)
        {
            string domhtml = string.Empty;
            if (siteEvent.ChangeType == ChangeType.Update || siteEvent.ChangeType == ChangeType.Add)
            {
                if (modelType == typeof(TextContent))
                {
                    domhtml = GetTextContentBody(siteEvent.Value as TextContent, siteEvent.SiteDb);
                }
                else
                {
                    if (siteEvent.Value is IDomObject newvalue)
                    {
                        domhtml = newvalue.Body;
                    }
                }
            }
            return Kooboo.Dom.DomParser.CreateDom(domhtml);
        }

        public static string GetTextContentBody(TextContent content, SiteDb sitedb)
        {
            var contenttype = sitedb.ContentTypes.GetByFolder(content.FolderId);

            if (contenttype == null)
            {
                return content.Body;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in content.Contents)
                {
                    sb.Append($"<KooobooLanguage name=\"{item.Lang}\">");

                    if (item.FieldValues != null)
                    {
                        foreach (var field in item.FieldValues)
                        {
                            var property = contenttype.GetProperty(field.Key);
                            if (property != null && property.IsMedia())
                            {
                                if (property.MultipleValue)
                                {
                                    if (!string.IsNullOrEmpty(field.Value))
                                    {
                                        List<string> values = Lib.Helper.JsonHelper.Deserialize<List<string>>(field.Value);
                                        foreach (var value in values)
                                        {
                                            sb.Append(Lib.Helper.IOHelper.MimeType(value).ToLower().Contains("image")
                                                ? $"<KoobooField name=\"{field.Key}\"><img src=\"{value}\" /></KoobooField>"
                                                : $"<KoobooField name=\"{field.Key}\">{field.Value}</KoobooField>");
                                        }
                                    }
                                }
                                else
                                {
                                    sb.Append(Lib.Helper.IOHelper.MimeType(field.Value).ToLower().Contains("image")
                                        ? $"<KoobooField name=\"{field.Key}\"><img src=\"{field.Value}\" /></KoobooField>"
                                        : $"<KoobooField name=\"{field.Key}\">{field.Value}</KoobooField>");
                                }
                            }
                            else
                            {
                                sb.Append($"<KoobooField name=\"{field.Key}\">{field.Value}</KoobooField>");
                            }
                        }
                    }
                    sb.Append("</KooobooLanguage>");
                }
                return sb.ToString();
            }
        }

        public static bool ShouldCheck(Type modelType, SiteObjectEvent siteEvent)
        {
            if (siteEvent.ChangeType == ChangeType.Update)
            {
                // this can only be idomobject now because of early check.
                var old = siteEvent.OldValue as IDomObject;

                if (old != null && siteEvent.Value is IDomObject newvalue)
                {
                    if (Lib.Helper.StringHelper.IsSameValue(old.Body, newvalue.Body))
                    {
                        if (modelType == typeof(Page))
                        {
                            // if page and the header does not change..
                            var oldpage = old as Page;
                            var newpage = newvalue as Page;
                            if (oldpage.Headers.GetHashCode() == newpage.Headers.GetHashCode())
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if ((modelType == typeof(CmsCssRule)) && siteEvent.ChangeType != ChangeType.Delete)
            {
                return false;
            }
            return true;
        }

        public static void Clean(SiteDb siteDb, SiteObject siteObject)
        {
            if (siteObject == null)
            {
                return;
            }
            //embedded css rules.
            if (siteObject.ConstType == ConstObjectType.CssRule)
            {
                List<CmsCssRule> rules = siteDb.CssRules.Query.Where(o => o.ParentCssRuleId == siteObject.Id).SelectAll();

                foreach (var item in rules)
                {
                    siteDb.CssRules.Delete(item.Id);
                }
            }

            // remove embedded style and scripts.
            var embeddedstyles = siteDb.Styles.Query.Where(o => o.OwnerObjectId == siteObject.Id).SelectAll();
            foreach (var item in embeddedstyles)
            {
                siteDb.Styles.Delete(item.Id, false, false);
            }

            var embeddedkscript = siteDb.Code.Query.Where(o => o.OwnerObjectId == siteObject.Id).SelectAll();
            foreach (var item in embeddedkscript)
            {
                siteDb.Code.Delete(item.Id, false, false);
            }

            // embedded scripts.
            var embeddedscripts = siteDb.Scripts.Query.Where(o => o.OwnerObjectId == siteObject.Id).SelectAll();
            foreach (var item in embeddedscripts)
            {
                siteDb.Scripts.Delete(item.Id, false, false);
            }

            var inlinecss = siteDb.CssRules.Query.Where(o => o.OwnerObjectId == siteObject.Id && o.ParentStyleId == default(Guid)).SelectAll();
            foreach (var item in inlinecss)
            {
                siteDb.CssRules.Delete(item.Id);
            }

            var forms = siteDb.Forms.Query.Where(o => o.OwnerObjectId == siteObject.Id).SelectAll();

            foreach (var item in forms)
            {
                siteDb.Forms.Delete(item.Id);
            }

            siteDb.Relations.CleanObjectRelation(siteObject.Id);
        }

        public static void ComputeStyle(SiteObjectEvent styleEvent)
        {
            var sitedb = styleEvent.SiteDb;

            var style = styleEvent.Value as Style;
            if (style == null)
            {
                return;
            }

            if (styleEvent.ChangeType == ChangeType.Add)
            {
                StyleRelation.Compute(style, sitedb);
            }
            else if (styleEvent.ChangeType == ChangeType.Update)
            {
                if (!(styleEvent.OldValue is Style oldvalue) || !Lib.Helper.StringHelper.IsSameValue(oldvalue.Body, style.Body))
                {
                    StyleRelation.Compute(style, sitedb);
                }
            }
            else
            {
                // this is to delete style sheet...
                List<Guid> ownerguid = new List<Guid> {style.Id};

                var allownerrules = sitedb.CssRules.Query.Where(o => o.OwnerObjectId == style.Id).SelectAll();
                var allstylerules = sitedb.CssRules.Query.Where(o => o.ParentStyleId == style.Id).SelectAll();

                List<Guid> allruleid = new List<Guid>();
                allruleid.AddRange(allownerrules.Select(o => o.Id).ToList());
                allruleid.AddRange(allstylerules.Select(o => o.Id).ToList());

                foreach (var item in allruleid.Distinct())
                {
                    sitedb.CssRules.Store.delete(item);
                }

                ownerguid.AddRange(allruleid.Distinct());

                styleEvent.SiteDb.Relations.CleanObjectRelation(styleEvent.Value.Id);

                var relations = sitedb.Relations.Query.WhereIn<Guid>(o => o.objectXId, ownerguid).SelectAll();

                foreach (var item in relations)
                {
                    sitedb.Relations.Store.delete(item.Id);
                }

                var referByRelation = sitedb.Relations.Query.WhereIn<Guid>(o => o.objectYId, ownerguid).SelectAll();

                foreach (var item in referByRelation)
                {
                    sitedb.Relations.Store.delete(item.Id);
                }
            }
        }
    }
}