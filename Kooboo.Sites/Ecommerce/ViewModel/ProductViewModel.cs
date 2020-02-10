//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class ProductViewModel : IDynamic
    {

        public   ProductViewModel(Product product, string lang, List<Models.ProductProperty> Properties)
        {  
            this.Id = product.Id;
            this.ProductTypeId = product.ProductTypeId;
            this.UserKey = product.UserKey;
            this.LastModified = product.LastModified;
            this.Online = product.Online;
            this.CreationDate = product.CreationDate;

            var langcontent = product.GetContentStore(lang);
            if (langcontent != null)
            {
                this.Values = langcontent.FieldValues;
            }

            if (Properties != null)
            {
                foreach (var item in Properties.Where(o => !o.IsSystemField && !o.MultipleLanguage))
                {
                    if (!this.Values.ContainsKey(item.Name) || string.IsNullOrEmpty(this.Values[item.Name]))
                    {
                        bool found = false;
                        foreach (var citem in product.Contents)
                        {
                            foreach (var fielditem in citem.FieldValues)
                            {
                                if (fielditem.Key == item.Name)
                                {
                                    this.Values[item.Name] = fielditem.Value;
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                            { break; }
                        }
                    }
                }
            } 
        
        }


        public Guid Id { get; set; }

        public string UserKey { get; set; }
                                              
        public Guid ProductTypeId { get; set; }

        public int Order { get; set; }
                                             
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Object GetValue(string FieldName)
        {
            string lower = FieldName.ToLower();

            if (lower == "userkey")
            {
                return this.UserKey;
            }
        
            else if (lower == "id")
            {
                return this.Id;
            }
            else if (lower == "sequence")
            {
                return this.Order;
            }

            if (Values.ContainsKey(FieldName))
            {
                return Values[FieldName];
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

        public void SetValue(string FieldName, object Value)
        {
            this.Values[FieldName] = Value.ToString();
        }

        public Object GetValue(string FieldName, RenderContext Context)
        {
            string culture = Context.Culture;

            var result = GetValue(FieldName);
            if (result == null && Context != null)
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
                return this.Values.ToDictionary(o=>o.Key, o=>(object)o.Value);
            }
        }
    } 
}
