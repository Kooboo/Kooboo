//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System.Linq;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.DataTraceAndModify;

namespace Kooboo.Sites.ViewModel
{
    public class TextContentViewModel : IDynamic, ITraceability
    {
        public Guid Id { get; set; }

        public string UserKey { get; set; }

        public Guid FolderId { get; set; }

        public Guid ParentId { get; set; }

        public Guid ContentTypeId { get; set; }

        public int Order { get; set; }

        public Dictionary<Guid, List<Guid>> Embedded = new Dictionary<Guid, List<Guid>>();

        public Dictionary<string, string> TextValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Object GetValue(string FieldName)
        {
            string lower = FieldName.ToLower();

            if (lower == "userkey")
            {
                return this.UserKey;
            }
            else if (lower == "folderid")
            {
                return this.FolderId;
            }
            else if (lower == "id")
            {
                return this.Id;
            }
            else if (lower == "sequence")
            {
                return this.Order;
            }

            if (TextValues.ContainsKey(FieldName))
            {
                return TextValues[FieldName];
            }

            if (lower == "parentid")
            {
                return this.ParentId;
            }
            else if (lower == "contenttypeid")
            {
                return this.ContentTypeId;
            }
            else if (lower == "order")
            {
                return this.Order;
            }
            else if (lower == "online")
            {
                return this.Online;
            }
            else if (lower == "lastmodify" || lower == "lastmodified")
            {
                return this.LastModified;
            }
            else if (lower == "creationdate")
            {
                return this.CreationDate;
            }
            return null;
        }

        public void SetValue(string FieldName, object Value)
        {
            this.TextValues[FieldName] = Value.ToString();
        }

        public Object GetValue(string FieldName, RenderContext Context)
        {
            string culture = Context.Culture;

            var result = GetValue(FieldName);
            if (result == null && Context != null)
            {
                // check category and embedded. 
                var sitedb = Context.WebSite.SiteDb();
                var folder = sitedb.ContentFolders.Get(this.FolderId);
                if (folder != null)
                {
                    //check category.
                    var category = folder.Category.Find(o => o.Alias == FieldName);
                    if (category != null)
                    {
                        List<TextContentViewModel> mulresult = new List<TextContentViewModel>();
                        var ids = sitedb.ContentCategories.Query.Where(o => o.ContentId == this.Id && o.CategoryFolder == category.FolderId).SelectAll().Select(o => o.CategoryId).ToList();

                        foreach (var item in ids)
                        {
                            var contentview = sitedb.TextContent.GetView(item, culture);

                            if (contentview != null)
                            {
                                if (category.Multiple)
                                {
                                    mulresult.Add(contentview);
                                }
                                else
                                {
                                    return contentview;
                                }
                            }
                        }

                        if (mulresult.Count() > 0)
                        {
                            return mulresult;
                        }
                        else
                        { return null; }
                    }
                    //check embedded.

                    var embed = folder.Embedded.Find(o => o.Alias == FieldName);
                    if (embed != null)
                    {
                        List<TextContentViewModel> emresult = new List<TextContentViewModel>();

                        if (this.Embedded.ContainsKey(embed.FolderId))
                        {
                            var ids = this.Embedded[embed.FolderId];

                            if (ids != null && ids.Count() > 0)
                            {
                                foreach (var item in ids)
                                {
                                    var view = sitedb.TextContent.GetView(item, culture);
                                    if (view != null)
                                    {
                                        emresult.Add(view);
                                    }
                                }

                            }
                        }

                        var byParentIds = sitedb.TextContent.Query.Where(o => o.FolderId == embed.FolderId && o.ParentId == this.Id).SelectAll();

                        if (byParentIds != null && byParentIds.Count() > 0)
                        {
                            foreach (var subitem in byParentIds)
                            {
                                if (emresult.Find(o => o.Id == subitem.Id) == null)
                                {
                                    emresult.Add(sitedb.TextContent.GetView(subitem, Context.Culture));
                                }
                            }
                        }
                        return emresult.OrderByDescending(o => o.LastModified).ToList();
                    }
                }
            }

            return result;
        }

        public IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>
            {
                { "id",Id.ToString()}
            };
        }

        public DateTime LastModified { get; set; }

        public DateTime CreationDate { get; set; }

        public bool Online { get; set; }

        public Dictionary<string, object> Values
        {
            get
            {
                var result = this.TextValues.ToDictionary(o => o.Key, o => (object)o.Value);
                result["Id"] = this.Id.ToString();
                result["ParentId"] = this.ParentId.ToString();
                result["ContentTypeId"] = this.ContentTypeId.ToString();
                result["UserKey"] = this.UserKey;
                result["LastModified"] = this.LastModified.ToString();
                result["Online"] = this.Online.ToString();

                return result;
            }
        }

        public string Source => "textcontent";
    }

    public class EmbeddedContentViewModel
    {
        public ContentFolder EmbeddedFolder { get; set; }
        public List<TextContentViewModel> Contents { get; set; } = new List<TextContentViewModel>();
        public string Alias { get; set; }

    }


    public class EmbeddedBy
    {
        public string FolderName { get; set; }

        public Guid FolderId { get; set; }
    }


}
