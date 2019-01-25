//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{ 
    public class ObjectInfo
    {  
        public Guid ObjectId { get; set; }

        public byte ConstType { get; set; }

        public Type ModelType { get; set; }

        private string _name; 
        public string Name {
            get {
                if (string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_displayname))
                {
                    return this._displayname; 
                }
                return _name; 
            }
            set { _name = value;  }
        }

        public string Url { get; set; }

        private string _displayname; 
        public string DisplayName
        {
           get
            {
                if (!string.IsNullOrEmpty(_displayname))
                {
                    return _displayname; 
                }
                if (!string.IsNullOrEmpty(this.Name))
                { return this.Name; }

                if (!string.IsNullOrEmpty(this.Url))
                { return this.Url;  }
                 
                return null;    
            }
            set
            {
                _displayname = value; 
            }
        }

        public int Size { get; set; }
    }
      
}
