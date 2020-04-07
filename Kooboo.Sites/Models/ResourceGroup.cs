//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Models
{
    [Serializable]
    [Kooboo.Attributes.Routable]
    public class ResourceGroup : CoreObject
    {  
        public ResourceGroup()
        {
            ConstType = ConstObjectType.ResourceGroup;
        }

        private Guid _id;
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {

                    _id = Data.IDGenerator.GetResourceGroupId(this.Name, this.Type); 
                    
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }
          
        /// <summary>
        /// Target source type, the constobjecttype. 
        /// </summary>
        public byte Type { get; set; }

        private Dictionary<Guid, int> _children; 
        
        // route id, and the order...
        public Dictionary<Guid, int> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new Dictionary<Guid, int>();
                }
                return _children; 
            }
            set { _children = value; }
        }
          
        public override int GetHashCode()
        {
            string unique = this.Type.ToString();
            foreach (var item in this.Children)
            {
                unique += item.Key.ToString() + item.Value.ToString();
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  
        }

        public string Extension {
            get
            {
                if (this.Type == ConstObjectType.Style)
                {
                    return "css"; 
                }
                else
                {
                    return "js"; 
                }
            } 
        }
    }
}
