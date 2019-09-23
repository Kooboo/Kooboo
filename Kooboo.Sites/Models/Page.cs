//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Extensions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Kooboo.Sites.Cache;

namespace Kooboo.Sites.Models
{ 
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Text)]
    [Kooboo.Attributes.Routable]
    public class Page : DomObject, Kooboo.Data.Interface.IScriptable
    {
        public Page()
        {
            ConstType = ConstObjectType.Page;
            this.Online = true;   
        }
         
        private HtmlHeader _Headers; 
        public HtmlHeader Headers {

            get {
                if (_Headers == null)
                {
                    _Headers = new HtmlHeader();
                }
                return _Headers; 
            }
            set { _Headers = value;  }
        } 
                 
        private Guid _id; 
        public override Guid Id
        {
            get
            {
              if (_id == default(Guid))
              {
                  _id = System.Guid.NewGuid();
              }
              return _id; 
            }
            set
            {
                _id = value; 
            }
        }
          
        /// <summary>
        /// if page does not contain html header tag, the start layout of the page. 
        /// To be checked when adding layout to a page. 
        /// This store the name of the layout, but it can be converted to Guid key using hash function.
        /// </summary>
        public string LayoutName { get; set; }
         
        public bool HasLayout
        {
            get
            {
                return !string.IsNullOrEmpty(this.LayoutName);
            }
        }

        /// <summary>
        /// static or dynamic page. dynamic page uses layout, View or TAL elements. static page pure html. 
        /// </summary>
        public bool IsStatic { get; set; }


        public bool IsSecure { get; set; }
           

        [JsonConverter(typeof(StringEnumConverter))]
        public PageType Type { get; set; }

        /// <summary>
        /// the default start page of current site.
        /// </summary>
        public bool DefaultStart { get; set; }

        private Dictionary<string, string> _parameters; 
        public Dictionary<string, string> Parameters {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); 
                }
                return _parameters; 
            }
            set 
            {
                _parameters = value; 
            }
        }

        public List<string> RequestParas { get; set; }
         
        public override int GetHashCode()
        {
            string un = this.Name;
            un += this.Body;
            un += this.Online.ToString();
            un += this.Headers.GetHashCode().ToString();
            un += this.LayoutName;
            un += this.IsStatic.ToString(); 
             
            if (this._parameters != null)
            {
                foreach (var item in this._parameters)
                {
                    un += item.Key + item.Value;
                }
            } 
            un += this.DefaultStart.ToString();  

            return Lib.Security.Hash.ComputeIntCaseSensitive(un); 
        }

    }

    public enum PageType
    {
        Normal =0,
        Layout = 1,
        RichText  =2
    }
}
