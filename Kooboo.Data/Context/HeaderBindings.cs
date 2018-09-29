//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Context
{
    public class HeaderBindings
    {
        public string MetaName { get; set; }
        public bool IsTitle { get; set; }

        public bool IsCustomHeader { get; set; }

        private string _content;

        /// <summary>
        ///  The content value. 
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;

                if (!string.IsNullOrEmpty(_content) && !this.IsCustomHeader)
                {                       
                    this.ValueQuery = new HeaderValueQuery(_content);    
                }
            }
        }

        public string CharSet { get; set; }

        public string HttpEquiv { get; set; }

        public bool RequireBinding {
            get
            {
                if (this.ValueQuery !=null)
                {
                    return this.ValueQuery.RequireRender; 
                }
                return false; 
            }
        }          
                                                      
        public HeaderValueQuery ValueQuery { get; set; }

        public string GetContent(RenderContext context)
        {
            if (this.IsCustomHeader)
            {
                return this.Content;
            }
            else
            {
                if (this.ValueQuery != null)
                {
                    return this.ValueQuery.Render(context);
                }
                return null;
            }
        }

    }
}
