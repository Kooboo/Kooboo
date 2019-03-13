//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Repository
{
    public class ThumbnailRepository : SiteRepositoryBase<Thumbnail>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var para = new ObjectStoreParameters();
                para.AddColumn<Thumbnail>(o => o.ImageId);
                para.SetPrimaryKeyField<Thumbnail>(o => o.Id); 
                return para;
            }
        }

        private object _object = new object();

        public Thumbnail GetThumbnail(Guid imageid, int width, int height)
        {
            Guid thumbnailid = Kooboo.Data.IDGenerator.GetImageThumbNailId(imageid, height, width);

            Thumbnail thumbnail = Get(thumbnailid);

            if (thumbnail == null)
            {
                thumbnail = new Thumbnail();

                //Image koobooimage = SiteDb.ImagePool.Get(imageid);
                Kooboo.Sites.Models.Image koobooimage = SiteDb.Images.Get(imageid);

                if (koobooimage == null)
                {
                    return null;
                }

                // calculate the width or height. 
                if (height <= 0 && koobooimage.Width > 0)
                {
                    height = (int)width * koobooimage.Height / koobooimage.Width;
                }

                if (width <= 0 && koobooimage.Height > 0)
                {
                    width = (int)height * koobooimage.Width / koobooimage.Height;
                }

                if (height <=0) { height = 90;  }
                if (width <=0) { width = 90; }


                thumbnail.Width = width;
                thumbnail.Height = height;
                thumbnail.ImageId = imageid;
                thumbnail.Extension = koobooimage.Extension;

                thumbnail.Id = thumbnailid; 
                thumbnail.ContentType = "image"; 
                string imagetype = thumbnail.Extension; 
                if (!string.IsNullOrEmpty(imagetype) && imagetype.StartsWith("."))
                {
                    imagetype = imagetype.Substring(1);
                }
                thumbnail.ContentType = thumbnail.ContentType + "/" + imagetype;

                //"image/svg+xml"
                if (!string.IsNullOrEmpty(imagetype) && imagetype.ToLower() == "svg")
                {
                    thumbnail.ContentType = thumbnail.ContentType + "+xml";
                    thumbnail.ContentBytes = koobooimage.ContentBytes; 
                }
                else
                {

                    if (koobooimage == null|| koobooimage.ContentBytes==null)
                    {
                        return null; 
                    }
                    var contentBytes = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetThumbnailImage(koobooimage.ContentBytes, width, height);
                    if (contentBytes == null) return null;

                    thumbnail.ContentBytes = contentBytes; 
                }
              
                AddOrUpdate(thumbnail); 
            }
            return thumbnail;
        }

        public void DeleteByImageId(Guid ImageId)
        {
            var all = this.Query.Where(o => o.ImageId == ImageId).SelectAll();
            foreach (var item in all)
            {
                this.Delete(item.Id);
            }

        }

    }
}
