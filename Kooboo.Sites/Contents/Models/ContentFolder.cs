//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Extensions;

namespace Kooboo.Sites.Contents.Models
{
    /// <summary>
    /// The text content folder. 
    /// </summary>
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    [Kooboo.Attributes.NameAsID]
    public class ContentFolder : Kooboo.Sites.Models.CoreObject
    {
        public ContentFolder()
        {
            this.ConstType = ConstObjectType.ContentFolder;
        }
        public Guid ParentFolderId { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_displayName))
                {
                    _displayName = Name;
                }
                return _displayName;
            }
            set { _displayName = value; }
        }

        public Guid ContentTypeId { get; set; }
   
        private List<EmbeddedFolder> _embedded; 
        public List<EmbeddedFolder> Embedded {
            get
            {
                if (_embedded == null)
                {
                    _embedded = new List<EmbeddedFolder>(); 
                }
                return _embedded; 
            }
            set { _embedded = value;  }
        }

        private List<CategoryFolder> _category; 

        public List<CategoryFolder> Category {
            get
            {
                if (_category == null)
                {
                    _category = new List<CategoryFolder>(); 
                }
                return _category; 
            }
            set { _category = value;  }
        }

        public override int GetHashCode()
        {
            string unique = this.Name;
            unique += this.ContentTypeId.ToString();
            unique += this.DisplayName;
            unique += this.Sortable.ToString();             
             
            foreach (var item in this.Category)
            {
                unique += item.Alias + item.FolderId.ToString() + item.Multiple.ToString(); 
            }

            foreach (var item in this.Embedded)
            {
                unique += item.Alias + item.FolderId.ToString(); 
            }
              
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }

        public bool Sortable { get; set; }

    }

    public class EmbeddedFolder
    {
        public string Alias { get; set; }

        public Guid FolderId { get; set; }
    }

    public class CategoryFolder
    {
        public string Alias { get; set; }

        public Guid  FolderId { get; set; }

        public bool Multiple { get; set;  }
    }
 
}
