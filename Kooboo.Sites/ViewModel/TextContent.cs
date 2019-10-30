//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.ViewModel
{
    public class TextContentViewModel : IDynamic
    {
        public Guid Id { get; set; }

        public string UserKey { get; set; }

        public Guid FolderId { get; set; }

        public Guid ParentId { get; set; }

        public Guid ContentTypeId { get; set; }

        public int Order { get; set; }

        public Dictionary<Guid, List<Guid>> Embedded = new Dictionary<Guid, List<Guid>>();

        public Dictionary<string, string> TextValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Object GetValue(string fieldName)
        {
            string lower = fieldName.ToLower();

            switch (lower)
            {
                case "userkey":
                    return this.UserKey;
                case "folderid":
                    return this.FolderId;
                case "id":
                    return this.Id;
                case "sequence":
                    return this.Order;
            }

            if (TextValues.ContainsKey(fieldName))
            {
                return TextValues[fieldName];
            }

            switch (lower)
            {
                case "parentid":
                    return this.ParentId;
                case "contenttypeid":
                    return this.ContentTypeId;
                case "order":
                    return this.Order;
                case "online":
                    return this.Online;
                case "lastmodify":
                case "lastmodified":
                    return this.LastModified;
                case "creationdate":
                    return this.CreationDate;
                default:
                    return null;
            }
        }

        public void SetValue(string fieldName, object value)
        {
            this.TextValues[fieldName] = value.ToString();
        }

        public Object GetValue(string fieldName, RenderContext context)
        {
            string culture = context.Culture;

            var result = GetValue(fieldName);
            if (result == null && context != null)
            {
                // check category and embedded.
                var sitedb = context.WebSite.SiteDb();
                var folder = sitedb.ContentFolders.Get(this.FolderId);
                if (folder != null)
                {
                    //check category.
                    var category = folder.Category.Find(o => o.Alias == fieldName);
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

                        return mulresult.Any() ? mulresult : null;
                    }
                    //check embedded.

                    var embed = folder.Embedded.Find(o => o.Alias == fieldName);
                    if (embed != null)
                    {
                        List<TextContentViewModel> emresult = new List<TextContentViewModel>();

                        if (this.Embedded.ContainsKey(embed.FolderId))
                        {
                            var ids = this.Embedded[embed.FolderId];

                            if (ids != null && ids.Any())
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

                        if (byParentIds != null && byParentIds.Any())
                        {
                            foreach (var subitem in byParentIds)
                            {
                                if (emresult.Find(o => o.Id == subitem.Id) == null)
                                {
                                    emresult.Add(sitedb.TextContent.GetView(subitem, context.Culture));
                                }
                            }
                        }
                        return emresult.OrderByDescending(o => o.LastModified).ToList();
                    }
                }
            }

            return result;
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