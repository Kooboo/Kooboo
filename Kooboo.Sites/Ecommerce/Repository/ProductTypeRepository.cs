//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.Repository
{
  
    public class ProductTypeRepository : SiteRepositoryBase<ProductType>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<ProductType>(it => it.Id);
                paras.AddColumn<ProductType>(it => it.Name);
                paras.AddColumn<ProductType>(it => it.CreationDate);
                paras.AddColumn<ProductType>(it => it.LastModified);
                paras.SetPrimaryKeyField<ProductType>(o => o.Id);
                return paras;
            }
        }


        public List<ProductProperty> GetColumns(Guid ProductTypeId)
        {
            var value = this.Get(ProductTypeId);
            if (value == null)
            {
                return new List<ProductProperty>();
            }
            else
            {
                return value.Properties;
            }
        }

        //public List<ContentProperty> GetPropertiesByFolder(Guid FolderId)
        //{
        //    var folder = this.SiteDb.ContentFolders.Get(FolderId);
        //    if (folder != null)
        //    {
        //        return GetColumns(folder.ContentTypeId);
        //    }
        //    return null;
        //}


        //public override bool AddOrUpdate(ContentType value)
        //{
        //    EnsureSystemFields(value);
        //    return base.AddOrUpdate(value);
        //}

        //public override bool AddOrUpdate(ContentType value, Guid UserId)
        //{
        //    EnsureSystemFields(value);
        //    return base.AddOrUpdate(value, UserId);
        //}

        //public void EnsureSystemFields(ContentType contenttype)
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

        //    foreach (var item in removeProp)
        //    {
        //        contenttype.Properties.Remove(item);
        //    }

        //    if (!hasonline)
        //    {
        //        contenttype.Properties.Add(SystemFields.Online);
        //    }
        //}


        public bool IsNameExists(string contentTypeName)
        {
            var type = this.GetByNameOrId(contentTypeName);
            return (type != null);
        }

        public List<string> GetTitleColumns(Guid contentTypeId)
        {
            List<string> names = new List<string>();
            var contentType = this.Get(contentTypeId);

            foreach (var item in contentType.Properties.Where(o =>  !o.IsSystemField))
            {
                names.Add(item.Name);
            }

            if (names.Any())
            {
                return names;
            }

            names.Add(contentType.Properties.OrderBy(o => o.Order).First().Name);
            return names;
        }

     
    }
           

}
