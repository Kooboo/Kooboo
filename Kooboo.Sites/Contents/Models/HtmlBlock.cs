//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Dom;
using Kooboo.Extensions;
using Newtonsoft.Json;
using System.Text;

namespace Kooboo.Sites.Contents.Models
{
    [Kooboo.Attributes.NameAsID]
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    public class HtmlBlock : MultipleLanguageObject, IDomObject
    {
        public HtmlBlock()
        {
            this.ConstType = ConstObjectType.HtmlBlock;
        }

        [JsonIgnore]
        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        [Kooboo.Attributes.SummaryIgnore]
        public string Body
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in this.Values)
                {
                    sb.Append($"<KoobooField name=\"{item.Key}\">{item.Value}</KoobooField>");
                }
                return sb.ToString();
            }
            set
            {
                string DomHtml = value;
                if (!string.IsNullOrEmpty(DomHtml))
                {
                    var dom = Kooboo.Dom.DomParser.CreateDom(DomHtml);
                    var kooboos = dom.getElementsByTagName("KoobooField");

                    foreach (var item in kooboos.item)
                    {
                        var langkey = item.getAttribute("name");
                        var langvalue = item.InnerHtml;
                        this.SetValue(langkey, langvalue);
                    }
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
        }

        public override int GetHashCode()
        {
            string unique = string.Empty;
            foreach (var item in this.Values)
            {
                unique += item;
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  
        }

    }
}
