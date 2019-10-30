//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class ProductViewModel : IDynamic
    {
        public Guid Id { get; set; }

        public string UserKey { get; set; }

        public Guid ProductTypeId { get; set; }

        public int Order { get; set; }

        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Object GetValue(string fieldName)
        {
            string lower = fieldName.ToLower();

            switch (lower)
            {
                case "userkey":
                    return this.UserKey;
                case "id":
                    return this.Id;
                case "sequence":
                    return this.Order;
            }

            if (Values.ContainsKey(fieldName))
            {
                return Values[fieldName];
            }
            else if (lower == "contenttypeid")
            {
                return this.ProductTypeId;
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

        public void SetValue(string fieldName, object value)
        {
            this.Values[fieldName] = value.ToString();
        }

        public Object GetValue(string fieldName, RenderContext context)
        {
            string culture = context.Culture;

            var result = GetValue(fieldName);
            if (result == null && context != null)
            {
                //// check category and embedded.
                //var sitedb = Context.WebSite.SiteDb();
                //var folder = sitedb.ContentFolders.Get(this.FolderId);
                //if (folder != null)
                //{
                //    //check category.
                //    var category = folder.Category.Find(o => o.Alias == FieldName);
                //    if (category != null)
                //    {
                //        List<TextContentViewModel> mulresult = new List<TextContentViewModel>();
                //        var ids = sitedb.ContentCategories.Query.Where(o => o.ContentId == this.Id && o.CategoryFolder == category.FolderId).SelectAll().Select(o => o.CategoryId).ToList();

                //        foreach (var item in ids)
                //        {
                //            var contentview = sitedb.TextContent.GetView(item, culture);

                //            if (contentview != null)
                //            {
                //                if (category.Multiple)
                //                {
                //                    mulresult.Add(contentview);
                //                }
                //                else
                //                {
                //                    return contentview;
                //                }
                //            }
                //        }

                //        if (mulresult.Count() > 0)
                //        {
                //            return mulresult;
                //        }
                //        else
                //        { return null; }
                //    }
                //    //check embedded.

                //    var embed = folder.Embedded.Find(o => o.Alias == FieldName);
                //    if (embed != null)
                //    {
                //        List<TextContentViewModel> emresult = new List<TextContentViewModel>();

                //        if (this.Embedded.ContainsKey(embed.FolderId))
                //        {
                //            var ids = this.Embedded[embed.FolderId];

                //            if (ids != null && ids.Count() > 0)
                //            {
                //                foreach (var item in ids)
                //                {
                //                    var view = sitedb.TextContent.GetView(item, culture);
                //                    if (view != null)
                //                    {
                //                        emresult.Add(view);
                //                    }
                //                }

                //            }
                //        }

                //        var byParentIds = sitedb.TextContent.Query.Where(o => o.FolderId == embed.FolderId && o.ParentId == this.Id).SelectAll();

                //        if (byParentIds != null && byParentIds.Count() > 0)
                //        {
                //            foreach (var subitem in byParentIds)
                //            {
                //                if (emresult.Find(o => o.Id == subitem.Id) == null)
                //                {
                //                    emresult.Add(sitedb.TextContent.GetView(subitem, Context.Request.Culture));
                //                }
                //            }
                //        }
                //        return emresult.OrderByDescending(o => o.LastModified).ToList();
                //    }
                //}
            }

            return result;
        }

        public DateTime LastModified { get; set; }

        public DateTime CreationDate { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public bool Online { get; set; }

        Dictionary<string, object> IDynamic.Values
        {
            get
            {
                return this.Values.ToDictionary(o => o.Key, o => (object)o.Value);
            }
        }
    }
}