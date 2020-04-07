//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Data.Models
{
    [Serializable]
    public class WebSite : IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    string uniquestring = ConstObjectType.WebSite.ToString() + this.Name + this.OrganizationId.ToString();
                    _id = IDGenerator.GetId(uniquestring);
                }
                return _id;
            }
        }

        public Guid OrganizationId { get; set; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (!string.IsNullOrEmpty(_name))
                {
                    _name = Lib.Helper.StringHelper.ToValidFileName(_name);
                    if (_name.Length > 50)
                    {
                        _name = _name.Substring(0, 50);
                    }
                }
            }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return String.IsNullOrEmpty(_displayName) ? Name : _displayName; }
            set { _displayName = value; }
        }

        private HashSet<string> _Cultures;

        [Obsolete]
        public HashSet<string> Cultures
        {
            get
            {
                if (_Cultures == null)
                {
                    _Cultures = new HashSet<string>();
                    // _Cultures.Add(this.DefaultCulture);
                }
                return _Cultures;
            }
            set
            {
                _Cultures = value;
            }
        }

        private Dictionary<string, string> _culture;
        public virtual Dictionary<string, string> Culture
        {
            get
            {
                if (_culture == null)
                {
                    _culture = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                foreach (var item in this.Cultures)
                {
                    string langcode = item;
                    if (!string.IsNullOrEmpty(langcode))
                    {
                        if (langcode.Length > 2)
                        {
                            langcode = langcode.Substring(0, 2);
                        }
                        if (!_culture.ContainsKey(langcode))
                        {
                            if (Data.Language.LanguageSetting.ISOTwoLetterCode.ContainsKey(langcode))
                            {
                                var value = Data.Language.LanguageSetting.ISOTwoLetterCode[langcode];
                                _culture[langcode] = value;
                            }
                            else
                            {
                                _culture[langcode] = langcode;
                            }
                        }
                    }
                }

                if (_culture.Count() == 0)
                {
                    string defaultlang = Kooboo.Data.Language.LanguageSetting.SystemLangCode;

                    string value = defaultlang;
                    if (Language.LanguageSetting.ISOTwoLetterCode.ContainsKey(defaultlang))
                    {
                        value = Language.LanguageSetting.ISOTwoLetterCode[defaultlang];
                        _culture.Add(defaultlang, value);
                    }
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

        private string _defaultculture;

        public string DefaultCulture
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultculture))
                {
                    return Language.LanguageSetting.SystemLangCode;
                }
                return _defaultculture;
            }
            set
            {
                _defaultculture = value;
                if (!string.IsNullOrEmpty(_defaultculture) && _defaultculture.Length > 2)
                {
                    _defaultculture = _defaultculture.Substring(0, 2);
                }
            }
        }

        public bool AutoDetectCulture { get; set; }

        public bool ContinueDownload { get; set; } = false;

        public bool Published { get; set; } = true;

        /// <summary>
        /// If set, this is a local Http server. 
        /// The local file disk path or the absolutely file path + name. 
        /// </summary>
        public string LocalRootPath { get; set; }

        public bool EnableCluster { get; set; }

        /// <summary>
        /// The base url of the mirror website like http://www.kooboo.com
        /// </summary>
        public string MirrorWebSiteBaseUrl { get; set; }

        public bool EnableVisitorLog { get; set; } = true;

        public bool EnableImageLog { get; set; } = true;
         

        private bool _enabledisksync;
        public bool EnableDiskSync
        {
            get
            {
                if (Data.AppSettings.IsOnlineServer)
                {
                    return false;
                }
                return _enabledisksync;
            }
            set
            {
                _enabledisksync = value;
            }
        }

        public bool EnableSitePath { get; set; }

        public bool EnableFullTextSearch { get; set; }

        public bool EnableMultilingual { get; set; }

        public bool EnableConstraintFixOnSave { get; set; } = true;

        public bool EnableFrontEvents { get; set; } = false;

        public bool EnableConstraintChecker { get; set; }

        public bool EnableCache { get; set; } = true;

        public bool EnableECommerce { get; set; }

        //Enable direct access to view, htmlblock etc, via system routes. 
        public bool EnableSystemRoute { get; set; }

        public bool EnableFileIOUrl { get; set; } = true;

        public bool EnableJsCssCompress { get; set; }

        //by append a version to js css file. 
        public bool EnableJsCssBrowerCache { get; set; }

        public bool EnableImageBrowserCache { get; set; }

        public int ImageCacheDays { get; set; } = 1; 

//      public bool ImageBrowserVersion { get; set; } = false; 
//      public int ImageExpireDays  { get; set; } = 365;

        // the version of Kooboo application. 
        // public int KoobooVersion { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public bool ContinueConvert { get; set; } = true;

        public bool HasSitePath()
        {
            var values = this.SitePath.Values.ToList();
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item))
                { return true; }
            }
            return false;
        }

        private string _LocalDiskSyncFolder;

        public string DiskSyncFolder
        {
            get
            {

                if (string.IsNullOrEmpty(this._LocalDiskSyncFolder))
                {
                    _LocalDiskSyncFolder = System.IO.Path.Combine(AppSettings.GetOrganizationFolder(this.OrganizationId), "___disksync", this.Name);
                    Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(_LocalDiskSyncFolder);
                }
                return _LocalDiskSyncFolder;
            }
            set
            {
                _LocalDiskSyncFolder = value;
            }
        }
        internal Dictionary<int, string> _customererrors;

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

        internal Dictionary<string, string> _customsettings;

        public Dictionary<string, string> CustomSettings
        {
            get
            {
                if (_customsettings == null)
                {
                    _customsettings = new Dictionary<string, string>();
                }
                return _customsettings;
            }
            set
            {
                _customsettings = value;
            }
        }

        public bool ForceSSL { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Data.Definition.WebsiteType SiteType { get; set; } = Definition.WebsiteType.p;

        public bool IsApp { get; set; }

        public override int GetHashCode()
        {
            string unique = string.Empty;
            unique += this.Name + this.DisplayName + this.OrganizationId;
            unique += this.ContinueConvert.ToString() + this.ContinueDownload.ToString() + this.EnableCache.ToString() + this.EnableCluster.ToString() + this.EnableConstraintChecker.ToString() + this.EnableConstraintFixOnSave.ToString() + this.EnableDiskSync.ToString() + this.EnableFrontEvents.ToString() + this.EnableMultilingual.ToString() + this.EnableSitePath.ToString() + this.EnableVisitorLog.ToString() + this.EnableFullTextSearch.ToString() + this.EnableJsCssCompress.ToString();

            unique += this.EnableJsCssBrowerCache.ToString();
            unique += this.EnableImageBrowserCache.ToString();
            unique += this.ImageCacheDays.ToString(); 

            unique += this.EnableSystemRoute.ToString();

            unique += this.EnableECommerce.ToString();

            unique += this.EnableFileIOUrl.ToString();

            //public bool EnableECommerce { get; set; } 
            //Enable direct access to view, htmlblock etc, via system routes. 
            //public bool EnableSystemRoute { get; set; }
            //public bool EnableFileIOUrl { get; set; } = true;


            unique += this.LocalRootPath + this.MirrorWebSiteBaseUrl + this._LocalDiskSyncFolder;

            unique += this.DefaultCulture + this.AutoDetectCulture.ToString();

            foreach (var item in this.Cultures)
            {
                unique += item;
            }

            foreach (var item in this.Culture)
            {
                unique += item.Key + item.Value;
            }


            if (_customsettings != null)
            {
                foreach (var item in _customsettings)
                {
                    unique += item.Key + item.Value;
                }
            }

            if (_customererrors != null)
            {
                foreach (var item in _customererrors)
                {
                    unique += item.Key.ToString() + item.Value;
                }
            }

            unique += this.Published.ToString();
            unique += this.SiteType.ToString();
            unique += this.IsApp.ToString();
            unique += this.ForceSSL.ToString();
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}
