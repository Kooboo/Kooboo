//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;

namespace Kooboo.Web.ViewModel
{
    public class KConfigItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> Binding
        {
            get; set;
        }

        public string TagName { get; set; }

        public string TagHtml { get; set; }

        public DateTime LastModified
        {
            get; set;
        }
        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }

        public Dictionary<string, int> Relations { get; set; }

    }


    public class KConfigEditModel
    {
        public KConfigEditModel(KConfig input)
        {
            this.Binding = input.Binding;
            this.TagHtml = input.TagHtml;
            this.TagName = input.TagName;
        }

        public Dictionary<string, string> Binding
        {
            get; set;
        }

        public string Id { get; set; }

        // The original tag.
        public string TagName { get; set; }

        public string TagHtml { get; set; }

        public string ControlType { get; set; } = "TextBox";
    }


}
