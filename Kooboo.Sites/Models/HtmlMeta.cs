//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class HtmlMeta
    {
        public string name { get; set; }

        public string httpequiv { get; set; }

        private Dictionary<string, string> _content;
        /// <summary>
        /// multi lingual content. 
        /// </summary>
        public Dictionary<string, string> content
        {
            get
            {
                if (_content == null)
                {
                    _content = new Dictionary<string, string>();
                }
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        public string charset { get; set; }

        public string GetContent(string culture = null)
        {
            string value = null;
            if (!string.IsNullOrEmpty(culture))
            {
                if (this.content.TryGetValue(culture, out value))
                {
                    return value;
                }
            }

            if (this.content.TryGetValue("", out value))
            {
                return value;
            }
            return null;
        }

        public void SetContent(string value, string culture = null)
        {
            if (string.IsNullOrEmpty(culture))
            {
                this.content[""] = value;
            }
            else
            {
                this.content[culture] = value;
            }
        }

    }

    public class SingleMeta
    {
        public string name { get; set; }

        public string httpequiv { get; set; }

        public string content { get; set; }
          
        public string charset { get; set; } 

    }
}
