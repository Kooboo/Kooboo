//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    public class Product : CoreObject
    {
        public Product()
        {
            this.ConstType = ConstObjectType.Product;
        }

        private Guid _id;

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

        // can be used for url..
        public string UserKey { get; set; }

        public Guid ProductTypeId { get; set; }

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
            if (content == null && this.Contents.Any())
            {
                content = this.Contents.Find(o => string.IsNullOrEmpty(o.Lang));
            }

            if (content == null && this.Contents.Any() && !string.IsNullOrEmpty(Lang))
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
            if (lower == "key")
            {
                return this.UserKey;
            }
            else if (lower == "id")
            {
                return this.Id;
            }

            MultilingualContent content = GetContentStore(lang);
            if (content != null && content.FieldValues.ContainsKey(fieldName))
            {
                return content.FieldValues[fieldName];
            }
            else if (lower == "ProductTypeId")
            {
                return this.ProductTypeId;
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
            if (lower == "key")
            {
                this.UserKey = value;
                return;
            }
            else if (lower == "id")
            {
                if (Guid.TryParse(value, out var id))
                {
                    this.Id = id;
                    return;
                }
            }
            else if (lower == "ProductTypeId")
            {
                if (Guid.TryParse(value, out var contenttypeid))
                {
                    this.ProductTypeId = contenttypeid;
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

        public int Order { get; set; }

        public override int GetHashCode()
        {
            string unique = this.Name + this.UserKey;
            unique += this.ProductTypeId.ToString() + this.Online.ToString() + this.Order.ToString();

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