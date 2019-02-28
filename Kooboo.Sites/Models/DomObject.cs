//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Dom;
using Kooboo.Sites.Models;
using Kooboo.Extensions;

namespace Kooboo.Sites.Models
{
    /// <summary>
    /// Objects that contains Dom body...
    /// </summary> 
    public class DomObject : CoreObject, IDomObject
    {
        private string _body;

        public string Body
        {
            get
            { return _body; }
            set
            { 
                _body = value; 
                if (_body!=null)
                {
                    _body = _body.Trim(new char[] { '\uFEFF', '\u200B' });
                }
                this._dom = null;
            }
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
                    _name = Lib.Helper.StringHelper.ToValidFileName(_name); 
                } 
            }
        }

        private Dom.Document _dom;

        [JsonIgnore] 
        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        [Kooboo.Attributes.SummaryIgnore]
        public  Document  Dom
        { 
            get
            {
                if (_dom == null && !string.IsNullOrEmpty(this.Body))
                {
                    _dom = Kooboo.Dom.DomParser.CreateDom(this.Body);
                }
                return _dom;
            }   
            set
            {
                _dom = value; 
            }
        }

        public void Dispose()
        {
            if (_dom != null)
            {
                _dom.Dispose();
                _dom = null;
            }
            this.Body = null;
        }

        internal void DisposeDom()
        {
            if (_dom != null)
            {
                _dom.Dispose();
                _dom = null; 
            } 
        }
    }
}
