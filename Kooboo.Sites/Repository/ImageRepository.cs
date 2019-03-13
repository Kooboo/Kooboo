//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Extensions;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Repository
{
    public class ImageRepository : SiteRepositoryBase<Image>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.SetPrimaryKeyField<Image>(o => o.Id);
                paras.AddColumn<Image>(o => o.Id);
                paras.AddColumn<Image>(o => o.Height); 
                paras.AddColumn<Image>(o => o.Width);
                paras.AddColumn<Image>(o => o.Size);
                paras.AddColumn("Name", 100);
                paras.AddColumn<Image>(o => o.LastModified);
                paras.SetPrimaryKeyField<Image>(o => o.Id);
                return paras;
            }
        }
         
         
        public List<Image> ListUsedBy(byte ConstType)
        {
            // For style. it is actually must be css declaration. 
            if (ConstType == ConstObjectType.Style)
            {
                ConstType = ConstObjectType.CssRule;
            }

            List<Image> list = new List<Image>();

            foreach (var item in Query.UseColumnData().SelectAll())
            {
                var usedrelation = ListUsedByRelation(item, ConstType);

                if (usedrelation != null && usedrelation.Any())
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public override bool AddOrUpdate(Image value, Guid UserId)
        { 
             if (value !=null)
            {
                if (!value.IsSvg && value.Width <=0 || value.Height <=0)
                {
                    var size = Lib.Utilities.CalculateUtility.GetImageSize(value.ContentBytes);
                    value.Height = size.Height;
                    value.Width = size.Width; 
                }

                return base.AddOrUpdate(value, UserId);
            }

            return false;               
          
        }

        public List<ObjectRelation> ListUsedByRelation(Image image, byte constType = 0)
        {
            if (image == null)
            {
                return null;
            }
            if (constType == ConstObjectType.Style || constType == ConstObjectType.CssRule)
            {
                constType = ConstObjectType.CssRule;
            }
            return this.SiteDb.Relations.GetReferredBy(image, constType);
        }
        

        public Image UploadImage(byte[] contentBytes, string fullName, Guid UserId)
        { 
            string relativeUrl = UrlHelper.RelativePath(fullName);
            bool found = true; 

            Image koobooimage = this.GetByUrl(relativeUrl); 

            if (koobooimage==null)
            {
                koobooimage = new Image();
                found = false; 
            }   
            koobooimage.Name = UrlHelper.FileName(fullName);
            koobooimage.Extension = UrlHelper.FileExtension(fullName);

            koobooimage.ContentBytes = contentBytes;
               
            if (!found)
            {
                SiteDb.Routes.AddOrUpdate(relativeUrl, ConstObjectType.Image, koobooimage.Id, UserId);
            }

         
        AddOrUpdate(koobooimage, UserId);
      

            return koobooimage;
        }

 
        public List<Image> ListUsedByPage(Guid PageId, bool UseColumnData = true)
        {
            var objectids = SiteDb.Pages.GetRelatedObjectIds(SiteDb.Pages.Get(PageId));

            return ListUsedByObjects(objectids.ToArray());
        }

        public List<Image> ListUsedByObjects(params Guid[] objectids)
        {
            List<Image> images = new List<Image>();

            var allrelations = SiteDb.Relations.Query.Where(o => o.RouteDestinationType == ConstObjectType.Image && o.ConstTypeY == ConstObjectType.Route).WhereIn<Guid>(o => o.objectXId, objectids.ToList()).SelectAll();

            foreach (var item in allrelations)
            {
                var route = SiteDb.Routes.Get(item.objectYId);
                if (route != null && route.objectId != default(Guid))
                {
                    var image = Get(route.objectId, true);
                    if (image != null)
                    {
                        images.Add(image);
                    }
                }
            }

            return images;
        }

        public List<Image> ListUsedByPageStyle(Guid PageId)
        {
            List<Image> images = new List<Image>();
            var pagedeclarations = Kooboo.Sites.Relation.CmsCssRuleRelation.ListUsedByPage(SiteDb, PageId);

            List<Guid> Ruleids = pagedeclarations.Select(o => o.Id).ToList();

            var allStyleImageRelations = SiteDb.Relations.Query.Where(o => o.ConstTypeY == ConstObjectType.Route && o.RouteDestinationType == ConstObjectType.Image && o.ConstTypeX == ConstObjectType.CssRule).WhereIn<Guid>(o => o.objectXId, Ruleids).SelectAll();

            foreach (var item in allStyleImageRelations)
            {
                var route = SiteDb.Routes.Get(item.objectYId);
                if (route != null && route.objectId != default(Guid))
                { 
                    var image = Get(route.objectId, true);
                    if (image != null)
                    {
                        images.Add(image);
                    }
                }
            } 
            return images;
        }

        public List<Image> Search(string keyword, int skip = 0, int count = 50)
        {  
            keyword = keyword.ToLower();

            Func<string, bool> HasKeyword = (input) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    return false; 
                }
                if (input.ToLower().Contains(keyword))
                {
                    return true; 
                }
                return false; 
            };
              
            var images = new List<Image>();
            var resultbyname = SiteDb.Images.TableScan.Where(o => HasKeyword(o.Name) || HasKeyword(o.Alt)).SelectAll();
            //images.AddRange(resultbyname); 
           // var resultbyname = SiteDb.Images.Query.Where(o=>HasKeyword(o.Name) || HasKeyword(o.Alt)).SelectAll();

            images.AddRange(resultbyname);

            var imagepathtree = Cache.RouteTreeCache.RouteTree(this.SiteDb, ConstObjectType.Image);

            if (imagepathtree != null)
            {
                HashSet<Guid> imageids = new HashSet<Guid>();
                SearchPathTree(imagepathtree.root, imageids, keyword);
                foreach (var item in imageids)
                {
                    if (images.Find(o => o.Id == item) == null)
                    {
                        var image = Get(item);
                        if (image != null)
                        {
                            images.Add(image);
                        }
                    }
                }
            }

            return images;
        }

        private void SearchPathTree(Models.Path tree, HashSet<Guid> ImageIdList, string keyword)
        { 
            if (tree == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(tree.segment) && tree.segment.ToLower().Contains(keyword))
            {
                var allrouteids = GetAllSubRouteId(tree);

                foreach (var item in allrouteids)
                {
                    var route = SiteDb.Routes.Get(item);
                    if (route != null && route.DestinationConstType == ConstObjectType.Image)
                    {
                        ImageIdList.Add(route.objectId);
                    }
                }
                return; 
            }

            foreach (var item in tree.Children)
            {
                SearchPathTree(item.Value, ImageIdList, keyword);
            }
        }

        private HashSet<Guid> GetAllSubRouteId(Models.Path tree)
        {
            HashSet<Guid> result = new HashSet<Guid>();
            if (tree.RouteId != default(Guid))
            {
                result.Add(tree.RouteId);
            } 
            foreach (var item in tree.Children)
            {
                var subset = GetAllSubRouteId(item.Value);
                foreach (var set in subset)
                {
                    result.Add(set);
                }
            }
            return result;
        }


    }
}
