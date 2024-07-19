//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Web.ViewModel
{
    public class CssRuleViewModel
    {
        public CssRuleViewModel()
        {
            this.Rules = new List<CssRuleViewModel>();
        }


        public Guid Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RuleType RuleType { get; set; }


        public string Selector { get; set; }

        public List<CssRuleViewModel> Rules { get; set; }

        public List<DeclarationViewModel> Declarations { get; set; }
    }


    public class DeclarationViewModel
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public bool Important { get; set; }
    }


    public class UpdateStyleRuleViewModel
    {
        public Guid Id { get; set; }

        public Dictionary<int, CssRuleViewModel> Added { get; set; }

        public Dictionary<Guid, CssRuleViewModel> Modified { get; set; }

        public List<Guid> Removed { get; set; }

    }

    public class InlineStyleViewModel
    {
        public Guid Id { get; set; }

        public string Selector { get; set; }

        public List<Kooboo.Sites.Models.CmsCssDeclaration> Declarations { get; set; }
    }


    public class StyleViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsEmbedded
        {
            get;
            set;
        }
        public string Extension { get; set; } = "css";

        public string FullUrl { get; set; }

        public string DisplayName
        {
            get; set;
        }

        public string Body { get; set; }

        public bool SourceChange { get; set; }

    }

    public class StyleEditViewModel : IDiffChecker
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string Extension { get; set; }
        public long Version { get; set; }
        public bool? EnableDiffChecker { get; set; }
    }

}
