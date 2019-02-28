//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Dom;
using Newtonsoft.Json;
using System.Text;

namespace Kooboo.Sites.Contents.Models
{
    public class MultilingualContent
    {
        private string _lang;
        public string Lang
        {
            get
            {
                if (_lang == null)
                {
                    return string.Empty;
                }
                return _lang;
            }
            set { _lang = value; }
        }


        private Dictionary<string, string> _fieldvalues; 

        [Kooboo.IndexedDB.CustomAttributes.KoobooKeyIgnoreCase]
        public Dictionary<string, string> FieldValues {

            get
            {
                if (_fieldvalues == null)
                {
                    _fieldvalues =  new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                } 
                return _fieldvalues; 
            }
            set
            {
                _fieldvalues = value; 
            }
        } 
    }

    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    public class TextContent : CoreObject, IDomObject
    {
        public TextContent()
        {
            this.ConstType = ConstObjectType.TextContent;
        }

        private Guid _id;

        private string _name;
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    foreach (var item in this.Contents)
                    {
                        if (item.FieldValues != null)
                        {
                            foreach (var field in item.FieldValues)
                            {
                                if (string.IsNullOrEmpty(field.Value))
                                {
                                    return field.Value;
                                }
                            }
                        }
                    }
                }
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.UserKey);
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private string _userkey;
        public string UserKey
        {
            get { return _userkey; }
            set
            {
                _userkey = value;
                _id = default(Guid);
            }
        }

        public Guid FolderId { get; set; }

        public Guid ParentId { get; set; }

        public Guid ContentTypeId { get; set; }

        public int Order { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string SummaryText
        {
            get
            {
                var content = this.GetContentStore(null);

                if (content != null && content.FieldValues.Count() > 0)
                {
                    return content.FieldValues.First().Value;
                }
                return null;
            }
        }

        private Dictionary<Guid, List<Guid>> _embedded;
        public Dictionary<Guid, List<Guid>> Embedded
        {
            get
            {
                if (_embedded == null)
                {
                    _embedded = new Dictionary<Guid, List<Guid>>();
                }
                return _embedded;
            }
            set
            {
                _embedded = value;
            }
        }

        public List<MultilingualContent> Contents { get; set; } = new List<MultilingualContent>();

        private Document _dom;

        [JsonIgnore]
        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        [Kooboo.Attributes.SummaryIgnore]
        public Document Dom
        {
            get
            {
                if (_dom == null)
                {
                    _dom = DomParser.CreateDom(this.Body);
                }
                return _dom;
            }
            set
            {
                _dom = value;
            }
        }

        [JsonIgnore]
        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        [Kooboo.Attributes.SummaryIgnore]
        public string Body
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in this.Contents)
                {
                    sb.Append($"<KooobooLanguage name=\"{item.Lang}\">");

                    if (item.FieldValues != null)
                    {
                        foreach (var field in item.FieldValues)
                        {
                            sb.Append($"<KoobooField name=\"{field.Key}\">{field.Value}</KoobooField>");
                        }
                    }
                    sb.Append("</KooobooLanguage>");
                }
                return sb.ToString();
            }

            set
            {
                string DomHtml = value;

                if (!string.IsNullOrEmpty(DomHtml))
                {
                    var dom = Kooboo.Dom.DomParser.CreateDom(DomHtml);
                    var langcontents = dom.getElementsByTagName("KooobooLanguage");

                    foreach (var item in langcontents.item)
                    {
                        string lang = item.getAttribute("name");
                        if (lang == null)
                        {
                            lang = string.Empty;
                        }
                        foreach (var field in item.getElementsByTagName("KoobooField").item)
                        {
                            var key = field.getAttribute("name");
                            var fieldvalue = field.InnerHtml;
                            this.SetValue(key, fieldvalue, lang);
                        }
                    }
                }
            }
        }

        internal MultilingualContent GetContentStore(string Lang, bool createNew = false)
        {
            MultilingualContent content = null;
            if (!string.IsNullOrEmpty(Lang))
            {
                content = this.Contents.Find(o => o.Lang == Lang);
            }
            if (content == null && this.Contents.Count() > 0)
            {
                content = this.Contents.Find(o => string.IsNullOrEmpty(o.Lang));
            }

            if (content == null && this.Contents.Count() > 0 && !string.IsNullOrEmpty(Lang))
            {
                string lower = Lang.ToLower();
                if (lower.Length > 2)
                {
                    lower = lower.Substring(0, 2);
                }
                foreach (var item in this.Contents)
                {
                    if (item.Lang.ToLower().StartsWith(lower))
                    {
                        return item;
                    }
                }
            }

            if (content == null)
            {
                if (createNew)
                {
                    content = new MultilingualContent();
                    content.Lang = Lang;
                    this.Contents.Add(content);
                }
                else if (this.Contents.Count() > 0)
                {
                    return this.Contents.First();
                }
            }
            return content;
        }

        public object GetValue(string FieldName, string Lang = null)
        {
            if (FieldName == null)
            {
                return null;
            }

            string lower = FieldName.ToLower();
            if (lower == "userkey")
            {
                return this.UserKey;
            }
            else if (lower == "id")
            {
                return this.Id;
            }

            else if (lower == "order" || lower == "sequence")
            {
                return this.Order;
            }

            MultilingualContent content = GetContentStore(Lang);
            if (content !=null &&  content.FieldValues.ContainsKey(FieldName))
            {
                return content.FieldValues[FieldName];
            }
             
            if (lower == "folderid")
            {
                return this.FolderId;
            }

            else if (lower == "parentid")
            {
                return this.ParentId;
            }
            else if (lower == "contenttypeid")
            {
                return this.ContentTypeId;
            }
  
            else if (lower == "online")
            {
                return this.Online;
            }
            else if (lower == "lastmodify" || lower == "lastmodified")
            {
                return this.LastModified;
            }


            foreach (var item in this.Contents)
            {
                if (item.FieldValues.ContainsKey(FieldName))
                {
                    return item.FieldValues[FieldName];
                }
            }

            return null;
        }

        public void SetValue(string FieldName, string Value, string Lang = null)
        {
            string lower = FieldName.ToLower();
            if (lower == "userkey")
            {
                this.UserKey = Value;
                return;
            }
            else if (lower == "folderid")
            {
                Guid folderid;
                if (System.Guid.TryParse(Value, out folderid))
                {
                    this.FolderId = folderid;
                    return;
                }
            }
            else if (lower == "id")
            {
                Guid id;
                if (Guid.TryParse(Value, out id))
                {
                    this.Id = id;
                    return;
                }
            }
            else if (lower == "parentid")
            {
                Guid parentid;
                if (Guid.TryParse(Value, out parentid))
                {
                    this.ParentId = parentid;
                    return;
                }
            }
            else if (lower == "contenttypeid")
            {
                Guid contenttypeid;
                if (Guid.TryParse(Value, out contenttypeid))
                {
                    this.ContentTypeId = contenttypeid;
                    return;
                }
            }
            else if (lower == "order")
            {
                int order = 0;
                if (int.TryParse(Value, out order))
                {
                    this.Order = order;
                    return;
                }
            }
            else if (lower == "online")
            {
                bool online = false;
                if (bool.TryParse(Value, out online))
                {
                    this.Online = online;
                    return;
                }
            }
            else if (lower == "lastmodify" || lower == "lastmodified")
            {
                DateTime date;
                if (DateTime.TryParse(Value, out date))
                {
                    this.LastModified = date;
                    return;
                }
            }
            var content = GetContentStore(Lang, true);
            content.FieldValues[FieldName] = Value;
        }


        public override int GetHashCode()
        {
            string unique = this.Name + this.UserKey + this.FolderId.ToString();
            unique += this.ParentId.ToString() + this.ContentTypeId.ToString() + this.Order.ToString() + this.Online.ToString();
            if (_embedded != null)
            {
                foreach (var item in Embedded)
                {
                    unique += item.Key.ToString();
                    if (item.Value != null)
                    {
                        foreach (var value in item.Value)
                        {
                            unique += value.ToString();
                        }
                    }
                }
            }

            foreach (var item in this.Contents)
            {
                unique += item.Lang;
                if (item.FieldValues != null)
                {
                    foreach (var subitem in item.FieldValues)
                    {
                        unique += subitem.Key + subitem.Value;
                    }
                }
            }

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}
