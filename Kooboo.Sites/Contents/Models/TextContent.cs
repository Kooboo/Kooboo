//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                return _lang ?? string.Empty;
            }
            set { _lang = value; }
        }

        private Dictionary<string, string> _fieldvalues;

        [Kooboo.IndexedDB.CustomAttributes.KoobooKeyIgnoreCase]
        public Dictionary<string, string> FieldValues
        {
            get
            {
                return _fieldvalues ??
                       (_fieldvalues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
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

                if (content != null && content.FieldValues.Any())
                {
                    return content.FieldValues.First().Value;
                }
                return null;
            }
        }

        private Dictionary<Guid, List<Guid>> _embedded;

        public Dictionary<Guid, List<Guid>> Embedded
        {
            get { return _embedded ?? (_embedded = new Dictionary<Guid, List<Guid>>()); }
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
            get { return _dom ?? (_dom = DomParser.CreateDom(this.Body)); }
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
                string domHtml = value;

                if (!string.IsNullOrEmpty(domHtml))
                {
                    var dom = Kooboo.Dom.DomParser.CreateDom(domHtml);
                    var langcontents = dom.getElementsByTagName("KooobooLanguage");

                    foreach (var item in langcontents.item)
                    {
                        string lang = item.getAttribute("name") ?? string.Empty;
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

        internal MultilingualContent GetContentStore(string lang, bool createNew = false)
        {
            MultilingualContent content = null;
            if (!string.IsNullOrEmpty(lang))
            {
                content = this.Contents.Find(o => o.Lang == lang);
            }
            if (content == null && this.Contents.Any())
            {
                content = this.Contents.Find(o => string.IsNullOrEmpty(o.Lang));
            }

            if (content == null && this.Contents.Any() && !string.IsNullOrEmpty(lang))
            {
                string lower = lang.ToLower();
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
                    content.Lang = lang;
                    this.Contents.Add(content);
                }
                else if (this.Contents.Any())
                {
                    return this.Contents.First();
                }
            }
            return content;
        }

        public object GetValue(string fieldName, string lang = null)
        {
            if (fieldName == null)
            {
                return null;
            }

            string lower = fieldName.ToLower();
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

            MultilingualContent content = GetContentStore(lang);
            if (content != null && content.FieldValues.ContainsKey(fieldName))
            {
                return content.FieldValues[fieldName];
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
                if (item.FieldValues.ContainsKey(fieldName))
                {
                    return item.FieldValues[fieldName];
                }
            }

            return null;
        }

        public void SetValue(string fieldName, string value, string lang = null)
        {
            string lower = fieldName.ToLower();
            if (lower == "userkey")
            {
                this.UserKey = value;
                return;
            }
            else if (lower == "folderid")
            {
                if (System.Guid.TryParse(value, out var folderid))
                {
                    this.FolderId = folderid;
                    return;
                }
            }
            else if (lower == "id")
            {
                if (Guid.TryParse(value, out var id))
                {
                    this.Id = id;
                    return;
                }
            }
            else if (lower == "parentid")
            {
                if (Guid.TryParse(value, out var parentid))
                {
                    this.ParentId = parentid;
                    return;
                }
            }
            else if (lower == "contenttypeid")
            {
                if (Guid.TryParse(value, out var contenttypeid))
                {
                    this.ContentTypeId = contenttypeid;
                    return;
                }
            }
            else if (lower == "order")
            {
                if (int.TryParse(value, out var order))
                {
                    this.Order = order;
                    return;
                }
            }
            else if (lower == "online")
            {
                if (bool.TryParse(value, out var online))
                {
                    this.Online = online;
                    return;
                }
            }
            else if (lower == "lastmodify" || lower == "lastmodified")
            {
                if (DateTime.TryParse(value, out var date))
                {
                    this.LastModified = date;
                    return;
                }
            }
            var content = GetContentStore(lang, true);
            content.FieldValues[fieldName] = value;
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