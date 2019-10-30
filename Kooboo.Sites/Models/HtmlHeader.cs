//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Models
{
    public class HtmlHeader : SiteObject
    {
        private Dictionary<string, string> _title;

        public bool HasValue()
        {
            if (_title != null && _title.Count > 0)
            {
                return true;
            }

            if (_metas != null && _metas.Count > 0)
            {
                return true;
            }

            if (_script != null && _script.Count > 0)
            {
                return true;
            }

            if (_styles != null && _styles.Count > 0)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(this.CustomHeader))
            {
                return true;
            }
            return false;
        }

        public Dictionary<string, string> Titles
        {
            get { return _title ?? (_title = new Dictionary<string, string>()); }
            set
            {
                _title = value;
            }
        }

        public string CustomHeader { get; set; }

        public string GetTitle(string culture = null)
        {
            if (_title == null || !_title.Any())
            { return null; }

            string value = null;
            if (!string.IsNullOrEmpty(culture))
            {
                if (this.Titles.TryGetValue(culture, out value))
                {
                    return value;
                }
            }

            if (this.Titles.TryGetValue("", out value))
            {
                return value;
            }

            return this.Titles.First().Value;
        }

        public void SetTitle(string value, string culture = null)
        {
            if (string.IsNullOrEmpty(culture))
            {
                this.Titles[""] = value;
            }
            else
            {
                this.Titles[culture] = value;
            }
        }

        private List<HtmlMeta> _metas;

        public List<HtmlMeta> Metas
        {
            get { return _metas ?? (_metas = new List<HtmlMeta>()); }
            set
            {
                _metas = value;
            }
        }

        private HashSet<string> _styles;

        public HashSet<string> Styles
        {
            get { return _styles ?? (_styles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)); }
            set { _styles = value; }
        }

        private HashSet<string> _script;

        public HashSet<string> Scripts
        {
            get { return _script ?? (_script = new HashSet<string>(StringComparer.OrdinalIgnoreCase)); }
            set { _script = value; }
        }

        //[Kooboo.Attributes.SummaryIgnore]
        //public string PreSetCulture { get; set; }

        public override int GetHashCode()
        {
            string unique = string.Empty;

            foreach (var item in this.Titles)
            {
                unique += item.Key;
                if (!string.IsNullOrEmpty(item.Value))
                {
                    unique += item.Value;
                }
            }

            foreach (var item in this.Metas)
            {
                unique += item.name + item.httpequiv + item.charset;
                foreach (var content in item.content)
                {
                    unique += content.Key;
                    if (!string.IsNullOrEmpty(content.Value))
                    {
                        unique += content.Value;
                    }
                }
            }

            foreach (var item in this.Scripts)
            {
                unique += item;
            }

            foreach (var item in this.Styles)
            {
                unique += item;
            }

            unique += this.CustomHeader;

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}