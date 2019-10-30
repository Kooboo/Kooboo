//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            List<Guid> allobjectids = new List<Guid> {page.Id};
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

            // embedded styles
            var embedded = SiteDb.Styles.Query.WhereIn<Guid>(o => o.OwnerObjectId, allobjectids).UseColumnData().SelectAll();

            if (embedded.Any())
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

            // get all the imports.
            List<Guid> importedids = new List<Guid>();

            foreach (var item in externalstyleids)
            {
                var importes = SiteDb.Styles.GetImports(item);
                if (importes != null && importes.Any())
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
        /// <param name="pageId"></param>
        /// <returns></returns>
        public List<Guid> GetRelatedOwnerObjectIds(Guid pageId)
        {
            var page = Get(pageId, true);
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

        public List<Guid> GetAllMethodIds(Guid pageId)
        {
            List<Guid> methodIds = new List<Guid>();

            var viewrelations = this.SiteDb.Relations.GetRelations(pageId, ConstObjectType.View);

            List<Guid> viewIds = new List<Guid>();

            foreach (var item in viewrelations)
            {
                viewIds.Add(item.objectYId);
            }

            foreach (var viewId in viewIds)
            {
                var viewmethods = this.SiteDb.ViewDataMethods.Query.Where(o => o.ViewId == viewId).SelectAll();

                foreach (var viewmethod in viewmethods)
                {
                    methodIds.Add(viewmethod.MethodId);
                }
            }
            return methodIds;
        }

        public Dictionary<byte, HashSet<Guid>> GetRelatedObject(Guid objectId, params byte[] destinationConstTypes)
        {
            var result = new Dictionary<byte, HashSet<Guid>>();
            HashSet<Guid> ids = new HashSet<Guid>();
            foreach (var item in destinationConstTypes)
            {
                var relations = SiteDb.Relations.GetRelations(objectId, item);

                foreach (var onerelation in relations)
                {
                    addRelatedResult(result, item, onerelation.objectYId);
                    ids.Add(onerelation.objectYId);
                }
            }

            foreach (var item in ids)
            {
                var subresult = GetRelatedObject(item, destinationConstTypes);
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

        private void addRelatedResult(Dictionary<byte, HashSet<Guid>> currentResult, byte consttype, Guid newId)
        {
            if (currentResult.ContainsKey(consttype))
            {
                var list = currentResult[consttype];
                list.Add(newId);
            }
            else
            {
                var list = new HashSet<Guid> {newId};
                currentResult[consttype] = list;
            }
        }

        public override bool AddOrUpdate(Page value)
        {
            return this.AddOrUpdate(value, default(Guid));
        }

        public override bool AddOrUpdate(Page value, Guid userId)
        {
            return base.AddOrUpdate(value, userId);
        }
    }
}