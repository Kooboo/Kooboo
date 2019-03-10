//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Contents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{ 
    public class CreateContentFolderViewModel
    {
        public CreateContentFolderViewModel()
        { 
            Embedded = new List<EmbeddedFolder>();
            Category = new List<CategoryFolder>(); 
        }
 
        public Guid Id { get; set; }
         
        public string Name { get; set; }
         
        public string DisplayName { get; set; }
 
        public Guid ContentTypeId { get; set; }
 
        public List<Guid> EmbeddedFolders { get; set; }
 
        public Dictionary<Guid, bool> CategoryFolders { get; set; }
        public List<EmbeddedFolder> Embedded { get; set; }

        public List<CategoryFolder> Category { get; set; }


    }

    public class ContentFolderViewModel : Kooboo.Sites.Contents.Models.ContentFolder
    {
        
        public Dictionary<string, int> Relations { get; set; }

        public static ContentFolderViewModel Create(ContentFolder folder)
        {
            var json = Lib.Helper.JsonHelper.Serialize(folder);
            return Lib.Helper.JsonHelper.Deserialize<ContentFolderViewModel>(json); 
        }

    }

}
