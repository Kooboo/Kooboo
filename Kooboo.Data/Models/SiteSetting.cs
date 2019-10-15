//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Data.Models
{
    public class SiteSetting
    {
        private Dictionary<string, string> _culture;

        public Dictionary<string, string> Culture
        {
            get => _culture ?? (_culture = new Dictionary<string, string>());
            set => _culture = value;
        }

        private Dictionary<string, string> _sitepath;

        public Dictionary<string, string> SitePath
        {
            get => _sitepath ?? (_sitepath = new Dictionary<string, string>());
            set => _sitepath = value;
        }

        public string DefaultCulture
        {
            get; set;
        }

        public bool EnableSitePath { get; set; }

        public bool EnableMultilingual { get; set; }

        public bool EnableFullTextSearch { get; set; }

        public bool EnableConstraintFixOnSave { get; set; }

        public bool EnableFrontEvents { get; set; } = true;

        public bool EnableConstraintChecker { get; set; }

        public bool EnableCache { get; set; } = true;

        public bool EnableECommerce { get; set; }

        private Dictionary<int, string> _customererrors;

        public Dictionary<int, string> CustomErrors
        {
            get => _customererrors ?? (_customererrors = new Dictionary<int, string>());
            set => _customererrors = value;
        }

        private Dictionary<string, string> _customSettings;

        public Dictionary<string, string> CustomSettings
        {
            get => _customSettings ?? (_customSettings = new Dictionary<string, string>());
            set => _customSettings = value;
        }

        public string KoobooVersion { get; set; }

        public bool AutoDetectCulture { get; set; }

        public bool ForceSsl { get; set; }

        public Data.Definition.WebsiteType SiteType { get; set; } = Definition.WebsiteType.p;

        public bool IsApp { get; set; }
    }
}