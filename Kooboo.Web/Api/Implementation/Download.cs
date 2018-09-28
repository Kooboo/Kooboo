using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using System;

namespace Kooboo.Web.Api.Implementation
{
    public class DownloadApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "download"; 
            }
        }

        public bool RequireSite
        {
            get
            {
                return false; 
            }
        }

        public bool RequireUser
        {
            get
            {
                return false; 
            }
        }

        public BinaryResponse Package(ApiCall call)
        {
            var package = Kooboo.Data.GlobalDb.Template.Get(call.ObjectId);

            var userid = call.GetGuidValue("UserId");
            BinaryResponse response = new BinaryResponse();
            response.ContentType = "application/zip";

            if (package != null)
            {
                var allbytes = System.IO.File.ReadAllBytes(package.FilePath); 
                response.Headers.Add("Content-Disposition", $"attachment;filename={package.Name}.zip");
                response.BinaryBytes = allbytes; 
                 Data.Cache.ObjectCache.AddDownloadCounter(call.ObjectId);  
                Data.GlobalDb.TemplateDownload.AddOrUpdate(new Data.Template.TemplateDownload { PackageId = call.ObjectId, ClientIp = call.Context.Request.IP, UserId = userid });   
            }
            else
            {
                response.BinaryBytes = new byte[0];  
            }
            return response; 
        }
           
        public BinaryResponse ThemeImg(ApiCall call)
        {
            BinaryResponse response = new BinaryResponse();
            response.ContentType = "image";
            response.BinaryBytes = new byte[0];

            string imgfolder = System.IO.Path.Combine(Data.AppSettings.ThemeFolder, "img");
            Lib.Helper.IOHelper.EnsureDirectoryExists(imgfolder);  

            string name = call.NameOrId; 
            if (!string.IsNullOrEmpty(name))
            {
                var path = System.IO.Path.Combine(imgfolder,  name); 
                if (System.IO.File.Exists(path))
                { 
                    int width = call.GetValue<int>("width");
                    int height = call.GetValue<int>("height"); 
                    if (width >0 || height >0)
                    {
                        response.BinaryBytes = ThumbNail(name, width, height); 
                    } 
                    else
                    { 
                        response.BinaryBytes = System.IO.File.ReadAllBytes(path);
                    }
                     
                    var info = new System.IO.FileInfo(path);
                    var extension = info.Extension; 
                    if (!string.IsNullOrEmpty(extension) && extension.StartsWith("."))
                    {
                        extension = extension.Substring(1); 
                    }
                    response.ContentType = response.ContentType + "/" + extension;  
                }
            }
            return response;
        }

        public byte[] ThumbNail(string name, int width, int height)
        { 
            var path = System.IO.Path.Combine(Data.AppSettings.ThemeFolder, "img", name);

            string thumbnailname = width.ToString() + "_" + height.ToString() + "_" + name;

            string thumbfolder = System.IO.Path.Combine(Data.AppSettings.ThemeFolder, "thumbnail");
            Lib.Helper.IOHelper.EnsureDirectoryExists(thumbfolder);  
            string thumbnailpath = System.IO.Path.Combine(thumbfolder, thumbnailname);

            if (!System.IO.File.Exists(thumbnailpath))
            { 
                if (System.IO.File.Exists(path))
                {
                    var allbytes = System.IO.File.ReadAllBytes(path);

                    System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(allbytes));

                    if (image.Height < height && image.Width < width)
                    {
                        return allbytes;
                    }

                    if (height <= 0 && image.Width > 0)
                    {
                        height = (int)width * image.Height / image.Width;
                    }

                    if (width <= 0 && image.Height > 0)
                    {
                        width = (int)height * image.Width / image.Height;
                    }

                    var thumbnail = image.GetThumbnailImage(width, height, null, new IntPtr());

                    thumbnail.Save(thumbnailpath);

                } 
            }
         
            if (System.IO.File.Exists(thumbnailpath))
            {
                return System.IO.File.ReadAllBytes(thumbnailpath); 
            }
            return new byte[0]; 
        }

    }
}
