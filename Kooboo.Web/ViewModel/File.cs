//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Lib;
using System;
using System.Collections.Generic;
using Kooboo.Lib.Helper;

namespace Kooboo.Web.ViewModel
{
    public class FileOverViewModel
    {
        public List<FileFolderViewModel> Folders { get; set; }

        public List<FileItemViewModel> Files { get; set; }

        public List<CrumbPath> CrumbPath { get; set; }
    }

    public class FileFolderViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string FullPath { get; set; }

        public int Count { get; set; }
        public string NavigateUrl { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class  FileItemViewModel
    { 
        public Guid Id { get; set; }

        public string Name { get; set; }
          
        public string Url { get; set; }

        public string PreviewUrl { get; set;  }
          
        public long Size { get; set; }
         
        public string Type { get; set; }

        public Dictionary<string, int> Relations { get; set; }

        public DateTime LastModified { get; set; }
         
        public string MimeType
        {
            get
            {
                return IOHelper.MimeType(this.Url);
            }
        }
    }

    public class FileDeleteViewModel
    {
        public List<string> Folders { get; set; } = new List<string>();

        public List<Guid> Images { get; set; } = new List<Guid>();
    } 
   
}
