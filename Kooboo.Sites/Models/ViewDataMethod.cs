//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;


namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable]
    public class ViewDataMethod : CoreObject
    {
        public ViewDataMethod()
        {
            ConstType = ConstObjectType.ViewDataMethod; 
        }

        private Guid _id;
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.ViewId.ToString() + this.MethodId.ToString() + this.AliasName;
                    _id = Data.IDGenerator.GetId(unique);
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        } 

        /// <summary>
        /// Alias for data binding or other purpose. Can be same as 
        /// </summary>
        public string AliasName { get; set; }

        public Guid MethodId { get; set; }
  
        public Guid ViewId { get; set; }

        private List<ViewDataMethod> _children; 
        public List<ViewDataMethod> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<ViewDataMethod>();
                }
                return _children; 
            }
            set
            { _children = value;  }
        }
         
        public bool HasChildren
        {
            get
            {
                return (_children != null &&  _children.Count() > 0); 
            }
        }
         
        public override int GetHashCode()
        {
            string unique = this.ViewId.ToString()+ this.MethodId.ToString() +this.AliasName;

            if (_children !=null && _children.Count()>0)
            {
                foreach (var item in _children)
                {
                    unique += item.GetHashCode().ToString(); 
                }
            }
             
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

    }
}
