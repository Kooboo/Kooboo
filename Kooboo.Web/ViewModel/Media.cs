//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;
using Kooboo.Data.Models;

namespace Kooboo.Web.ViewModel
{ 
    public class MediaLibraryViewModel
    {
        public List<ImageFolderViewModel> Folders { get; set; }

        public List<MediaFileViewModel> Files { get; set; }

        public List<CrumbPath> CrumbPath { get; set; }
    }

    public class MediaPagedViewModel
    {
        public List<ImageFolderViewModel> Folders { get; set; }

        public PagedListViewModel<MediaFileViewModel>  Files { get; set; }

        public List<CrumbPath> CrumbPath { get; set; }
    }

    public class ImageFolderViewModel
    { 
        public string Id { get; set; }
         
        public string Name { get; set; }
         
        public string FullPath { get; set; }
 
        public int Count { get; set; } 
        public string NavigateUrl { get; set; } 
        public DateTime LastModified { get; set; }
    }
     
    public class MediaFileViewModel
    {
        /// <summary>
        /// The image Id, can be used to retrieve the image and edit it. 
        /// </summary> 
        public Guid Id { get; set; }
         
        public string Name { get; set; } 
   
        public string Thumbnail { get; set; }
         
        public string Url { get; set; }
         
        public int Height { get; set; }
         
        public int Width { get; set; }
         
        public long Size { get; set; }
         
        public bool IsImage { get; set; }
         
        public string Type { get; set; }
        
        public Dictionary<string, int> References { get; set; }
        
        public DateTime LastModified { get; set; }
        
        public string Alt { get; set; }

        public string PreviewUrl { get; set; }

   
        public string MimeType
        {
            get
            {
                return IOHelper.MimeType(this.Url);
            }
        }
    }

    public class ImageDeleteViewModel
    {
        public List<string> Folders { get; set; } = new List<string>();

        public List<Guid> Images { get; set; } = new List<Guid>();
    }

    public class ImageViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set;}

        public string SiteUrl { get; set; } 

        public string Alt { get; set; }

        public string FullUrl { get; set; }


    }

}
