//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation.Ecommerce
{
 
    public class ProductTypeApi : SiteObjectApi<ProductType>
    { 
        public override List<object> List(ApiCall call)
        {
            var all = call.WebSite.SiteDb().ProductType.All();
            List<ContentTypeItemViewModel> result = new List<ContentTypeItemViewModel>();
            foreach (var item in all)
            {
                ContentTypeItemViewModel model = new ContentTypeItemViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.PropertyCount = item.Properties.Count();
                model.LastModified = item.LastModified;
                result.Add(model);
            }
            return result.ToList<object>();
        }

        [Attributes.RequireModel(typeof(ProductType))]
        public override Guid Post(ApiCall call)
        {
            ProductType value = (ProductType)call.Context.Request.Model;

           // EnsureSystemFields(value); 
          //  ValidateReservedFields(value, call);

            call.WebSite.SiteDb().ProductType.AddOrUpdate(value, call.Context.User.Id);
            return value.Id;
        }

        //private void EnsureSystemFields(ContentType contenttype)
        //{
        //    foreach (var item in contenttype.Properties)
        //    {
        //        if (item.Name.ToLower() == SystemFields.UserKey.Name.ToLower())
        //        {
        //            item.MultipleLanguage = false;
        //            item.DataType = Data.Definition.DataTypes.String;
        //            item.ControlType = ControlTypes.TextBox;
        //            item.IsSystemField = true;
        //        }
        //        else if (item.Name.ToLower() == SystemFields.Sequence.Name.ToLower())
        //        {
        //            item.MultipleLanguage = SystemFields.Sequence.MultipleLanguage;
        //            item.DataType = SystemFields.Sequence.DataType;
        //            item.ControlType = SystemFields.Sequence.ControlType;
        //            item.IsSystemField = true;
        //        }
        //        else if (item.Name.ToLower() == SystemFields.Online.Name.ToLower())
        //        {
        //            item.MultipleLanguage = SystemFields.Online.MultipleLanguage;
        //            item.DataType = SystemFields.Online.DataType;
        //            item.ControlType = SystemFields.Online.ControlType;
        //            item.IsSystemField = true;
        //        }
        //    }

        //    // remove duplicate system fields... 
        //    List<ContentProperty> removeProp = new List<ContentProperty>();

        //    bool hasuserkey = false;
        //    bool hasseq = false;
        //    bool hasonline = false;

        //    foreach (var item in contenttype.Properties)
        //    {
        //        if (item.Name.ToLower() == SystemFields.UserKey.Name.ToLower())
        //        {
        //            if (hasuserkey)
        //            {
        //                removeProp.Add(item);
        //            }
        //            else { hasuserkey = true; }
        //        }
        //        else if (item.Name.ToLower() == SystemFields.Sequence.Name.ToLower())
        //        {
        //            if (hasseq)
        //            {
        //                removeProp.Add(item);
        //            }
        //            else { hasseq = true; }
        //        }

        //        else if (item.Name.ToLower() == SystemFields.Online.Name.ToLower())
        //        {
        //            if (hasonline)
        //            {
        //                removeProp.Add(item);
        //            }
        //            else { hasonline = true; }
        //        }

        //    }

        //}

        //private void ValidateReservedFields(ContentType contentType, ApiCall call)
        //{
        //    foreach (var item in contentType.Properties)
        //    {
        //        if (SystemFields.ReservedFields.Contains(item.Name.ToLower()))
        //        {
        //            throw new Exception(item.Name + " " + Data.Language.Hardcoded.GetValue("is system reserved field", call.Context));
        //        }
        //    }
        //}

        public override object Get(ApiCall call)
        {
            if (call.ObjectId == default(Guid))
            {
                return new ProductType(); 

                // new... 
                //ContentType contentType = new ContentType();
                //contentType.Properties = new List<ContentProperty>{
                //            SystemFields.UserKey,
                //            SystemFields.Online,
                //            SystemFields.Sequence,
                //        };
                //return contentType;
            }
            else
            {
                var type = call.WebSite.SiteDb().ProductType.Get(call.ObjectId);
               // removeSystemField(type);
                return type;
            }
        }

        private void removeSystemField(ContentType type)
        {
            foreach (var item in SystemFields.ReservedFields)
            {
                var find = type.Properties.Find(o => o.Name.ToLower() == item.ToLower());
                if (find != null)
                {
                    type.Properties.Remove(find);
                }
            }

            // SET online field to boolean. 
            var online = type.Properties.Find(o => o.Name == SystemFields.Online.Name);

            online.ControlType = ControlTypes.Boolean;
        }

           
     
        public List<ColumnViewModel> Columns(Guid Id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
                                  
            var columns = sitedb.ProductType.GetColumns(Id); 
            List<ColumnViewModel> result = new List<ColumnViewModel>();

            foreach (var item in columns)
            {
                if (item.Name.ToLower() != "online")
                {
                    ColumnViewModel model = new ColumnViewModel();
                    model.Name = item.Name;
                    model.DisplayName = item.DisplayName;
                    model.DataType = item.DataType;
                    result.Add(model);
                }
            }

            var lastmodified = result.Find(o => o.Name == SystemFields.LastModified.Name);

            if (lastmodified == null)
            {
                ColumnViewModel model = new ColumnViewModel();
                model.Name = SystemFields.LastModified.Name;
                model.DisplayName = SystemFields.LastModified.DisplayName;
                model.DataType = SystemFields.LastModified.DataType;
                result.Insert(0, model);
            }

            return result;
        }


    }












}
