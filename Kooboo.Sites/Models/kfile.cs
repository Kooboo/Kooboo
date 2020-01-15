using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Models
{
    public class kfile : ISiteObject
    {
       
        public kfile()
        {
            this.ConstType = ConstObjectType.kfile; 
        }

        public byte ConstType { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }

        private Guid _id; 
        public Guid Id {
            get {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid(); 
                }
                return _id; 
            }
            set { _id = value;  }
        }

        public string Name { get; set; }
    }
}
