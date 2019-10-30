//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Relation
{
    public static class DataMethodRelation
    {
        public static void Compute(IDataMethodSetting methodsetting, SiteDb sitedb)
        {
            var values = methodsetting.ParameterBinding.Values;

            var allfolderids = GetFolderIds(methodsetting);

            var existings = sitedb.Relations.GetRelations(methodsetting.Id);

            foreach (var item in existings)
            {
                if (!allfolderids.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }
            foreach (var item in allfolderids)
            {
                sitedb.Relations.AddOrUpdate(methodsetting.Id, item, ConstObjectType.DataMethodSetting, ConstObjectType.Folder);
            }
        }

        public static void Clean(SiteDb siteDb, Guid dataMethodId)
        {
            var existings = siteDb.Relations.GetRelations(dataMethodId);
            foreach (var item in existings)
            {
                siteDb.Relations.Delete(item.Id);
            }
        }

        public static List<Guid> GetFolderIds(IDataMethodSetting methodsetting)
        {
            List<Guid> folderids = new List<Guid>();

            foreach (var item in methodsetting.ParameterBinding)
            {
                if (item.Key.ToLower() == "folderid")
                {
                    string strguid = item.Value.Binding;
                    if (System.Guid.TryParse(strguid, out var folderid))
                    {
                        folderids.Add(folderid);
                    }
                }
            }
            return folderids;
        }
    }
}