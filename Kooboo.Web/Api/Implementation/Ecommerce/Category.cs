//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel.Ecommerce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation.Ecommerce
{
    public class CategoryApi : SiteObjectApi<Category>
    {

        public override List<object> List(ApiCall call)
        {
            var db = call.Context.WebSite.SiteDb();
            var all = db.Category.All();

            return buildTree(all, call.WebSite.DefaultCulture).ToList<object>();
        }

        [Kooboo.Attributes.RequireModel(typeof(List<CategoryViewModel>))]
        public override Guid Post(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            List<CategoryViewModel> viewmodel = call.Context.Request.Model as List<CategoryViewModel>;

            foreach (var item in viewmodel)
            {
                savecat(sitedb, item, default(Guid));
            }

            return default(Guid);

        }

        public override bool Delete(ApiCall call)
        {
            var id = call.ObjectId;

            if (id !=null)
            {   
                call.WebSite.SiteDb().Category.Delete(id);
                return true; 
            }

            return false;    
        }

        public object CheckProuctCount(Guid Id, ApiCall call)
        {
            return call.Context.WebSite.SiteDb().Product.GetByCategory(Id).Count();    
        }


        //[Kooboo.Attributes.RequireParameters("id", "values")]
        //public override Guid PostSample(ApiCall apiCall)
        //{
        //    Guid id = apiCall.ObjectId;
        //    var strvalues = apiCall.GetValue("values");
        //    if (string.IsNullOrEmpty(strvalues))
        //    {
        //        return default(Guid);
        //    }

        //    Dictionary<string, object> values = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, object>>(strvalues);

        //    if (id != default(Guid))
        //    {
        //        var current = apiCall.WebSite.SiteDb().HtmlBlocks.Get(id);
        //        if (current != null)
        //        {
        //            foreach (var item in values)
        //            {
        //                current.SetValue(item.Key, item.Value);
        //            }
        //            apiCall.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(current, apiCall.Context.User.Id);
        //            return id;
        //        }
        //    }
        //    else
        //    {
        //        string name = apiCall.GetValue("name");
        //        HtmlBlock newblock = new HtmlBlock();
        //        newblock.Name = name;
        //        newblock.Values = values;

        //        apiCall.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(newblock, apiCall.Context.User.Id);

        //        return newblock.Id;
        //    }

        //    return default(Guid);
        //}

        private void savecat(SiteDb sitedb, CategoryViewModel model, Guid ParentId)
        {
            Category cat = new Category();
            cat.ParentId = ParentId;

            Dictionary<string, string> values = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, string>>(model.Values);

            foreach (var item in values)
            {
                cat.SetValue(item.Key, item.Value);
            }

            if (model.Id == default(Guid))
            {
                string defaultculture = sitedb.WebSite.DefaultCulture;
                if (values.ContainsKey(defaultculture))
                {
                    string name = values[defaultculture];
                    cat.Name = Kooboo.Sites.Contents.UserKeyHelper.ToSafeUserKey(name);
                    cat.Id = default(Guid); 
                 }
            }

            sitedb.Category.AddOrUpdate(cat);

            foreach (var item in model.SubCats)
            {
                savecat(sitedb, item, cat.Id);
            }

        }


        public List<CategoryViewModel> buildTree(List<Category> all, string culture)
        {
            List<CategoryViewModel> result = new List<CategoryViewModel>();

            foreach (var item in all.Where(o => o.ParentId == default(Guid)))
            {
                CategoryViewModel model = new CategoryViewModel();
                model.Id = item.Id;       
                model.Values = Lib.Helper.JsonHelper.Serialize(item.Values);

                model.Name = item.GetValue(culture).ToString(); 

                appendsub(model, all, culture);

                result.Add(model);
            }

            return result;

        }

        public void appendsub(CategoryViewModel parent, List<Category> all, string culture)
        {
            var subs = all.Where(o => o.ParentId == parent.Id);
            if (subs != null && subs.Any())
            {
                foreach (var item in subs)
                {
                    CategoryViewModel sub = new CategoryViewModel();
                    sub.Id = item.Id; 
                    sub.Values = Lib.Helper.JsonHelper.Serialize(item.Values);

                    sub.Name = item.GetValue(culture).ToString();

                    appendsub(sub, all, culture);
                    parent.SubCats.Add(sub);
                }
            }
        }


    }
}
