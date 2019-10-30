//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using Newtonsoft.Json;

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
                _body = _body?.Trim('\uFEFF', '\u200B');
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
        public Document Dom
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