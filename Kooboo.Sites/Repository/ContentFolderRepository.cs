//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class ContentFolderRepository : SiteRepositoryBase<ContentFolder>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<ContentFolder>(it => it.Id);
                paras.AddColumn<ContentFolder>(it => it.Name);
                paras.AddColumn<ContentFolder>(it => it.ParentFolderId);
                paras.AddColumn<ContentFolder>(it => it.ContentTypeId);
                paras.AddColumn<ContentFolder>(it => it.LastModified);
                paras.SetPrimaryKeyField<ContentFolder>(o => o.Id);
                return paras;
            }
        }

        public List<ContentProperty> GetColumns(ContentFolder folder)
        {
            var contentType = SiteDb.ContentTypes.Get(folder.ContentTypeId);
            if (contentType == null)
            {
                return new List<ContentProperty>();
            }
            return contentType.Properties;
        }

        public ContentProperty GetColumn(ContentFolder folder, string columnName)
        {
            var contentType = SiteDb.ContentTypes.Get(folder.ContentTypeId);

            return contentType?.GetProperty(columnName);
        }

        public ContentFolder GetByName(string folderName)
        {
            return this.Query.Where(o => o.Name == folderName).FirstOrDefault();
        }

        public bool IsFolderNameExists(string folderName)
        {
            Guid key = Kooboo.Data.IDGenerator.Generate(folderName, ConstObjectType.ContentFolder);

            var folder = this.Get(key);
            return folder != null;
        }

        public bool IsTreeStyleFolder(ContentFolder folder)
        {
            if (folder == null || folder.ContentTypeId == Guid.Empty)
            {
                return false;
            }
            var contentType = SiteDb.ContentTypes.Get(folder.ContentTypeId);

            return contentType != null && contentType.IsNested;
        }

        public override List<UsedByRelation> GetUsedBy(Guid objectId)
        {
            var objectrelations = this.SiteDb.Relations.GetReferredBy(this.SiteObjectType, objectId);

            // content folder only used by datamethodsetting.
            var result = Helper.RelationHelper.ShowUsedBy(this.SiteDb, objectrelations);

            foreach (var item in result)
            {
                var viewmethod = GetViewDataMethods(item.ObjectId);

                if (viewmethod != null && viewmethod.Any())
                {
                    foreach (var viewm in viewmethod)
                    {
                        var view = this.SiteDb.Views.Get(viewm.ViewId);
                        item.Remark += view.Name;
                    }
                }
                else
                {
                }
            }

            return result;
        }

        public List<ViewDataMethod> GetViewDataMethods(Guid methodId)
        {
            List<ViewDataMethod> result = new List<ViewDataMethod>();

            var all = this.SiteDb.ViewDataMethods.List();
            foreach (var item in all)
            {
                if (HasMethod(item, methodId))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private bool HasMethod(ViewDataMethod datamethod, Guid methodid)
        {
            if (datamethod.MethodId == methodid)
            {
                return true;
            }

            if (datamethod.HasChildren)
            {
                foreach (var item in datamethod.Children)
                {
                    if (HasMethod(item, methodid))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<Relation.ObjectRelation> CleanRelation(List<Relation.ObjectRelation> input)
        {
            List<Relation.ObjectRelation> result = new List<Relation.ObjectRelation>();

            var allviewmethods = this.SiteDb.ViewDataMethods.All();

            return input.Where(o => allviewmethods.Find(v => v.MethodId == o.objectXId) != null).ToList();
        }

        public List<EmbeddedBy> GetEmbeddedBy(Guid folderId)
        {
            List<EmbeddedBy> result = new List<EmbeddedBy>();

            var allfolder = List();

            foreach (var item in allfolder)
            {
                var find = item.Embedded.Find(o => o.FolderId == folderId);
                if (find != null)
                {
                    EmbeddedBy model = new EmbeddedBy {FolderId = item.Id, FolderName = item.Name};
                    result.Add(model);
                }
            }

            return result;
        }
    }
}