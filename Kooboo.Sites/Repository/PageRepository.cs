//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.Repository
{
    public class PageRepository : SiteRepositoryBase<Page>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<Page>(o => o.DefaultStart);
                paras.AddColumn<Page>(o => o.Online);
                paras.AddColumn<Page>(o => o.IsStatic);
                paras.AddColumn<Page>(o => o.Id);
                paras.AddColumn("LayoutName", 100);
                paras.AddColumn("Name", 100);
                paras.SetPrimaryKeyField<Page>(o => o.Id);
                return paras;
            }
        }
         /// <summary>
        /// Get the page, layout and view ids, those can be used for style owner object id. 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Guid> GetRelatedObjectIds(Page page)
        {
            List<Guid> allobjectids = new List<Guid>();
            allobjectids.Add(page.Id);
            if (page.HasLayout && !string.IsNullOrEmpty(page.LayoutName))
            {
                Guid layoutid = Data.IDGenerator.Generate(page.LayoutName, ConstObjectType.Layout);
            }
            var viewrelations = SiteDb.Relations.GetRelations(page.Id, ConstObjectType.View);

            foreach (var item in viewrelations)
            {
                allobjectids.Add(item.objectYId);
            }

            var alllayouts = SiteDb.Relations.GetRelations(page.Id, ConstObjectType.Layout);
            foreach (var item in alllayouts)
            {
                allobjectids.Add(item.objectYId);
            }
            return allobjectids;
        }

        /// <summary>
        /// Get all the style ids that being used by page. 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Guid> GetStyles(Page page)
        {
            HashSet<Guid> styleids = new HashSet<Guid>();

            List<Guid> allobjectids = GetRelatedObjectIds(page);

            /// embedded styles
            var embedded = SiteDb.Styles.Query.WhereIn<Guid>(o => o.OwnerObjectId, allobjectids).UseColumnData().SelectAll();

            if (embedded.Count() > 0)
            {
                var list = embedded.Select(o => o.Id).ToList();
                foreach (var item in list)
                {
                    styleids.Add(item);
                }
            }

            List<Guid> externalstyleids = new List<Guid>();

            var ExternalStyleRelations = SiteDb.Relations.Query.Where(o => o.ConstTypeY == ConstObjectType.Route && o.RouteDestinationType == ConstObjectType.Style).SelectAll().Where(o => allobjectids.Contains(o.objectXId)).ToList();

            foreach (var item in ExternalStyleRelations)
            {
                var routeid = item.objectYId;
                var route = SiteDb.Routes.Get(routeid);
                if (route != null && route.objectId != default(Guid))
                {
                    externalstyleids.Add(route.objectId);
                }
            }

            /// get all the imports. 
            List<Guid> importedids = new List<Guid>();

            foreach (var item in externalstyleids)
            {
                var importes = SiteDb.Styles.GetImports(item);
                if (importes != null && importes.Count() > 0)
                {
                    importedids.AddRange(importes);
                }
            }

            foreach (var item in externalstyleids)
            {
                styleids.Add(item);
            }

            foreach (var item in importedids)
            {
                styleids.Add(item);
            }

            return styleids.ToList();
        }

        /// <summary>
        /// Get all the style ids that is being used by this page. 
        /// </summary>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public List<Guid> GetRelatedOwnerObjectIds(Guid PageId)
        {
            var page = Get(PageId, true);
            return GetRelatedOwnerObjectIds(page);
        }
        /// <summary>
        /// Get all the page related owner object ids that can be used to querey css rule.. 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Guid> GetRelatedOwnerObjectIds(Page page)
        {
            var all = GetRelatedObjectIds(page);
            var styles = GetStyles(page);
            all.AddRange(styles);
            return all;
        }


        public List<Guid> GetAllMethodIds(Guid PageId)
        {
            List<Guid> MethodIds = new List<Guid>();

            var viewrelations = this.SiteDb.Relations.GetRelations(PageId, ConstObjectType.View);

            List<Guid> ViewIds = new List<Guid>();

            foreach (var item in viewrelations)
            {
                ViewIds.Add(item.objectYId);
            }

            foreach (var ViewId in ViewIds)
            {
                var viewmethods = this.SiteDb.ViewDataMethods.Query.Where(o => o.ViewId == ViewId).SelectAll();

                foreach (var viewmethod in viewmethods)
                {
                    MethodIds.Add(viewmethod.MethodId);
                }
            }
            return MethodIds;
        }

  
        public Dictionary<byte, HashSet<Guid>> GetRelatedObject(Guid ObjectId, params byte[] DestinationConstTypes)
        {
            var result = new Dictionary<byte, HashSet<Guid>>();
            HashSet<Guid> ids = new HashSet<Guid>(); 
            foreach (var item in DestinationConstTypes)
            {
                var relations = SiteDb.Relations.GetRelations(ObjectId, item);

                foreach (var onerelation in relations)
                {
                    addRelatedResult(result, item, onerelation.objectYId);
                    ids.Add(onerelation.objectYId);  
                }  
            }

            foreach (var item in ids)
            {
                var subresult = GetRelatedObject(item, DestinationConstTypes);
                foreach (var subitem in subresult)
                {
                    foreach (var subitemitem in subitem.Value)
                    {
                        addRelatedResult(result, subitem.Key, subitemitem); 
                    }
                }
            }
            return result; 
        }


        private void addRelatedResult(Dictionary<byte, HashSet<Guid>> CurrentResult, byte consttype, Guid newId)
        {
            if (CurrentResult.ContainsKey(consttype))
            {
                var list = CurrentResult[consttype]; 
                list.Add(newId); 
            }
            else
            {
                var List = new HashSet<Guid>();
                List.Add(newId);
                CurrentResult[consttype] = List;
            }
        }


        public override bool AddOrUpdate(Page value)
        {
            return this.AddOrUpdate(value, default(Guid));
        }

        public override bool AddOrUpdate(Page value, Guid UserId)
        {
            return base.AddOrUpdate(value, UserId);
        }

    }
}
