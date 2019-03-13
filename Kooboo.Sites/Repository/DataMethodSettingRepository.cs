//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class DataMethodSettingRepository : SiteRepositoryBase<DataMethodSetting>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddColumn<DataMethodSetting>(o => o.DeclareTypeHash);
                para.AddColumn<DataMethodSetting>(o => o.MethodSignatureHash);
                para.AddColumn<DataMethodSetting>(o => o.IsPublic);
                para.SetPrimaryKeyField<DataMethodSetting>(o => o.Id); 
                return para; 
            }
        }

        /// <summary>
        /// Get all the methods that have the folderid parameters..
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public List<DataMethodSetting> GetByFolder(Guid FolderId)
        {
            List<DataMethodSetting> result = new List<DataMethodSetting>();

            var all = this.All();
            foreach (var item in all)
            {
                if (item.DeclareType.Contains(typeof(Kooboo.Sites.DataSources.ContentItem).Name) || item.DeclareType.Contains(typeof(Kooboo.Sites.DataSources.ContentList).Name))
                { 
                    var keys = item.ParameterBinding.Keys;
                    foreach (var key in keys)
                    {
                        if (key.ToLower() == "folderid" || key.ToLower() == "folder.id" || key.ToLower() == "folder" || key.ToLower().Contains(".folderid"))
                        {
                            var value = item.ParameterBinding[key];

                            var bindingguid = default(Guid);

                            if (Guid.TryParse(value.Binding, out bindingguid))
                            {
                                if (bindingguid == FolderId)
                                {
                                    result.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            return result; 
        }

        public override  List<UsedByRelation> GetUsedBy(Guid ObjectId)
        {
            var viewmethods = this.SiteDb.ViewDataMethods.Query.Where(o => o.MethodId == ObjectId).SelectAll();
             
            List<UsedByRelation> relations = new List<UsedByRelation>();

            foreach (var item in viewmethods)
            {
                UsedByRelation relation = new UsedByRelation();
                  
                var objectinfo = Sites.Service.ObjectService.GetObjectInfo(this.SiteDb, item.ViewId, ConstObjectType.View);
                relation.Name = objectinfo.DisplayName;
                relation.Url = objectinfo.Url;
                relation.ModelType = objectinfo.ModelType;
                relation.ObjectId = objectinfo.ObjectId;
                relation.ConstType = objectinfo.ConstType;

                relations.Add(relation);
            }

            return relations;


        }

    }
}
