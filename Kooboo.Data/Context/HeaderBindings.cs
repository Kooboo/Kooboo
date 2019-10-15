//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

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
            get => _content;
            set
            {
                _content = value;

                if (!string.IsNullOrEmpty(_content) && !IsCustomHeader)
                {
                    ValueQuery = new HeaderValueQuery(_content);
                }
            }
        }

        public string CharSet { get; set; }

        public string HttpEquiv { get; set; }

        public bool RequireBinding => ValueQuery != null && ValueQuery.RequireRender;

        public HeaderValueQuery ValueQuery { get; set; }

        public string GetContent(RenderContext context)
        {
            if (IsCustomHeader)
            {
                return Content;
            }

            return ValueQuery?.Render(context);
        }
    }
}