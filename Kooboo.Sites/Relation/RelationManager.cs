//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Events.Cms;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Relation
{
    public static class RelationManager
    {
        public static void Compute(SiteObjectEvent SiteEvent)
        {
            if (SiteEvent == null || SiteEvent.Value == null)
            {
                return;
            }

            if (SiteEvent.Value is Style)
            {
                ComputeStyle(SiteEvent);
                return;
            }

            if (SiteEvent.Value is IDataMethodSetting)
            {
                if (SiteEvent.ChangeType == ChangeType.Delete)
                {
                    DataMethodRelation.Clean(SiteEvent.SiteDb, SiteEvent.Value.Id); 
                }
                else
                {
                    DataMethodRelation.Compute(SiteEvent.Value as IDataMethodSetting, SiteEvent.SiteDb);
                }   
            }

            if (SiteEvent.Value is ResourceGroup)
            {
                if (SiteEvent.ChangeType == ChangeType.Delete)
                {
                    SiteEvent.SiteDb.Relations.CleanObjectRelation(SiteEvent.Value.Id); 
                }
                else
                {
                    GroupRelation.Compute(SiteEvent.Value as ResourceGroup, SiteEvent.SiteDb);
                } 
            }

            Type ModelType = SiteEvent.Value.GetType();

            //only below needs to compute relation. 
            if (SiteEvent.Value is IDomObject || ModelType == typeof(CmsCssRule))
            {
                string basurl = null;
                HtmlHeader header = null;
                Document Dom = null;


                if (Attributes.AttributeHelper.IsRoutable(ModelType))
                {
                    basurl = Service.ObjectService.GetObjectRelativeUrl(SiteEvent.SiteDb, SiteEvent.Value.Id, SiteEvent.Value.ConstType);
                }

                if (ModelType == typeof(Page))
                {
                    var page = SiteEvent.Value as Page;
                    header = page.Headers;
                }

                if (!ShouldCheck(ModelType, SiteEvent))
                {
                    return;
                }

                Dom = GetDom(ModelType, SiteEvent);

                if (SiteEvent.ChangeType == ChangeType.Delete)
                {
                    var siteobject = SiteEvent.Value as SiteObject;
                    if (siteobject != null)
                    {
                        RelationManager.Clean(SiteEvent.SiteDb, siteobject);
                    }
                }
                else
                {
                    if (Dom != null || SiteEvent.Value is IDomObject)
                    {
                        DomRelation.Compute(SiteEvent.SiteDb, Dom, SiteEvent.Value.Id, SiteEvent.Value.ConstType, basurl, header);
                    }
                }
            }
        }

        public static Document GetDom(Type ModelType, SiteObjectEvent SiteEvent)
        {
            string domhtml = string.Empty;
            if (SiteEvent.ChangeType == ChangeType.Update || SiteEvent.ChangeType == ChangeType.Add)
            {
                if (ModelType == typeof(TextContent))
                {
                    domhtml = GetTextContentBody(SiteEvent.Value as TextContent, SiteEvent.SiteDb); 
                }
                else
                {
                    var newvalue = SiteEvent.Value as IDomObject;
                    if (newvalue != null)
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
                                            if (Lib.Helper.IOHelper.MimeType(value).ToLower().Contains("image"))
                                            {
                                                sb.Append($"<KoobooField name=\"{field.Key}\"><img src=\"{value}\" /></KoobooField>");
                                            }
                                            else
                                            {
                                                sb.Append($"<KoobooField name=\"{field.Key}\">{field.Value}</KoobooField>");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Lib.Helper.IOHelper.MimeType(field.Value).ToLower().Contains("image"))
                                    {
                                        sb.Append($"<KoobooField name=\"{field.Key}\"><img src=\"{field.Value}\" /></KoobooField>");
                                    }
                                    else
                                    {
                                        sb.Append($"<KoobooField name=\"{field.Key}\">{field.Value}</KoobooField>");
                                    }
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
       
        public static bool ShouldCheck(Type ModelType, SiteObjectEvent SiteEvent)
        { 
            if (SiteEvent.ChangeType == ChangeType.Update)
            { 
                // this can only be idomobject now because of early check.
                var old = SiteEvent.OldValue as IDomObject;
                var newvalue = SiteEvent.Value as IDomObject;

                if (old != null && newvalue != null)
                {
                    if (Lib.Helper.StringHelper.IsSameValue(old.Body, newvalue.Body))
                    {
                        if (ModelType == typeof(Page))
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

            if ((ModelType == typeof(CmsCssRule) ) && SiteEvent.ChangeType != ChangeType.Delete)
            {
                return false;
            }
            return true;

        }
         
        public static void Clean(SiteDb SiteDb, SiteObject SiteObject)
        {
            if (SiteObject == null)
            {
                return;
            }
            //embedded css rules.
            if (SiteObject.ConstType == ConstObjectType.CssRule)
            {
                List<CmsCssRule> rules = SiteDb.CssRules.Query.Where(o => o.ParentCssRuleId == SiteObject.Id).SelectAll();

                foreach (var item in rules)
                {
                    SiteDb.CssRules.Delete(item.Id);
                }
                
            }

            // remove embedded style and scripts. 
            var embeddedstyles = SiteDb.Styles.Query.Where(o => o.OwnerObjectId == SiteObject.Id).SelectAll();
            foreach (var item in embeddedstyles)
            {
                SiteDb.Styles.Delete(item.Id, false, false);
            }

            var embeddedkscript = SiteDb.Code.Query.Where(o => o.OwnerObjectId == SiteObject.Id).SelectAll();
            foreach (var item in embeddedkscript)
            {
                SiteDb.Code.Delete(item.Id, false, false);
            }


            // embedded scripts.
            var embeddedscripts = SiteDb.Scripts.Query.Where(o => o.OwnerObjectId == SiteObject.Id).SelectAll();
            foreach (var item in embeddedscripts)
            {
                SiteDb.Scripts.Delete(item.Id, false, false);
            }

            var inlinecss = SiteDb.CssRules.Query.Where(o => o.OwnerObjectId == SiteObject.Id && o.ParentStyleId == default(Guid)).SelectAll();
            foreach (var item in inlinecss)
            {
                SiteDb.CssRules.Delete(item.Id);
            }

            var forms = SiteDb.Forms.Query.Where(o => o.OwnerObjectId == SiteObject.Id).SelectAll();

            foreach (var item in forms)
            {
                SiteDb.Forms.Delete(item.Id);
            }

            SiteDb.Relations.CleanObjectRelation(SiteObject.Id);
        }
         
        public static void ComputeStyle(SiteObjectEvent StyleEvent)
        {
            var sitedb = StyleEvent.SiteDb; 

            var style = StyleEvent.Value as Style;
            if (style == null)
            {
               return;
            }

            if (StyleEvent.ChangeType == ChangeType.Add)
            {
                StyleRelation.Compute(style, sitedb);
            }
            else if (StyleEvent.ChangeType == ChangeType.Update)
            {
                var oldvalue = StyleEvent.OldValue as Style;
                if (oldvalue == null || !Lib.Helper.StringHelper.IsSameValue(oldvalue.Body, style.Body))
                {
                  StyleRelation.Compute(style, sitedb);
                }
            }
            else
            {
                // this is to delete style sheet... 
                List<Guid> ownerguid = new List<Guid>();
                ownerguid.Add(style.Id);
                 
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
         
                StyleEvent.SiteDb.Relations.CleanObjectRelation(StyleEvent.Value.Id);

                var relations = sitedb.Relations.Query.WhereIn<Guid>(o => o.objectXId, ownerguid).SelectAll();

                foreach (var item in relations)
                {
                    sitedb.Relations.Store.delete(item.Id);
                }

                var ReferByRelation = sitedb.Relations.Query.WhereIn<Guid>(o => o.objectYId, ownerguid).SelectAll();

                foreach (var item in ReferByRelation)
                {
                    sitedb.Relations.Store.delete(item.Id);
                } 
            } 
        }
         
    }
}
