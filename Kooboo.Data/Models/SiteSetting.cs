//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class SiteSetting
    {
        private Dictionary<string, string> _culture;
        public Dictionary<string, string> Culture
        {
            get
            {
                if (_culture == null)
                {
                    _culture = new Dictionary<string, string>();
                }
                return _culture;
            }
            set
            {
                _culture = value;
            }
        }

        private Dictionary<string, string> _sitepath;
        public Dictionary<string, string> SitePath
        {
            get
            {
                if (_sitepath == null)
                {
                    _sitepath = new Dictionary<string, string>();
                }
                return _sitepath;
            }
            set
            {
                _sitepath = value;
            }
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
            get
            {
                if (_customererrors == null)
                {
                    _customererrors = new Dictionary<int, string>();
                }
                return _customererrors;
            }
            set
            {
                _customererrors = value;
            } 
        }

        private Dictionary<string, string> _CustomSettings; 
        public Dictionary<string, string> CustomSettings {

            get
            {
                if (_CustomSettings == null)
                {
                    _CustomSettings = new Dictionary<string, string>(); 
                }
                return _CustomSettings;
            }
            set
            {
                _CustomSettings = value;
            }

        }

        public string KoobooVersion { get; set; }

        public bool AutoDetectCulture { get; set; }

        public bool ForceSsl { get; set; }


        public Data.Definition.WebsiteType SiteType { get; set; } = Definition.WebsiteType.p;

        public bool IsApp { get; set; }
    }
}
