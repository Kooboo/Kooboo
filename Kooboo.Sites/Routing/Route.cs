//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Extensions; 

namespace Kooboo.Sites.Routing
{
    public class Route : Models.CoreObject
    {
        public Route()
        { 
            this.ConstType = ConstObjectType.Route;
        }

        private string _name;

        public override string Name
        {
            get
            {
                return _name; 
            }

            set
            {
               _name = value;
                if (!string.IsNullOrEmpty(_name))
                {
                   //_name = Lib.Helper.StringHelper.ToValidFileName(_name); 
                    if (_name.StartsWith("\\"))
                    {
                        _name = "/" + _name.Substring(1); 
                    }
                    if (!_name.StartsWith("/"))
                    {
                        _name = "/" + _name;
                    } 
                    _name = System.Net.WebUtility.UrlDecode(_name); 
                } 
            }
        }

        public Guid objectId { get; set; }

        /// <summary>
        /// the destination object type. it can also be another route.
        /// this is not the consttype of this site object. 
        /// </summary>
        public byte DestinationConstType { get; set; }

        /// <summary>
        /// The parsed parameters of dynamic routes also the parameters that is not in the route itself but defined by ViewDataMethod.
        /// The format key: input.Field, value: {input.Field}
        /// </summary>
        public Dictionary<string, string> Parameters {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new Dictionary<string, string>(); 
                }
                return _parameters; 
            }
            set
            {
                _parameters = value; 
            }
        }
         
        private Dictionary<string, string> _parameters; 

        public override int GetHashCode()
        {
            string unique = this.Name + this.objectId.ToString() + this.DestinationConstType.ToString();
            if (_parameters != null)
            {
                foreach (var item in _parameters)
                {
                    unique += item.Key + item.Value;
                }
            }
        
            return Lib.Security.Hash.ComputeInt(unique); 
        }
         
    }

}
