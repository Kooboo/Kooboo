//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.IndexedDB;

namespace Kooboo.Sites.Repository
{
    public class ThumbnailRepository : SiteRepositoryBase<Thumbnail>
    {
        internal override ObjectStoreParameters StoreParameters
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
                Image koobooimage = SiteDb.Images.Get(imageid);

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

                    var systhumbnail = GenerateThumbnail(koobooimage, width, height);
                    if (systhumbnail == null)
                    {
                        return null; 
                    } 
                       System.IO.MemoryStream memstream = new System.IO.MemoryStream();
                        systhumbnail.Save(memstream, System.Drawing.Imaging.ImageFormat.Png);
                        thumbnail.ContentBytes = memstream.ToArray(); 
                }
              
                AddOrUpdate(thumbnail); 
            }
            return thumbnail;
        }

        public System.Drawing.Image GenerateThumbnail(Image koobooimage, int width, int height)
        {
            if (koobooimage == null || koobooimage.ContentBytes==null)
            {
                return null;
            }

            System.IO.MemoryStream stream = new System.IO.MemoryStream(koobooimage.ContentBytes);

            System.Drawing.Image image = null;
            System.Drawing.Image systhumbnail = null;

            image = System.Drawing.Image.FromStream(stream);
            if (image.Width < width && image.Height < height)
            {
                return image;
            }
            systhumbnail = image.GetThumbnailImage(width, height, null, new IntPtr());

            return systhumbnail;
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
