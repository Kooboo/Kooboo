using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.Site
{
    public class kCoreSettings
    {
        private readonly SiteDb _sitedb;
        private Dictionary<string, Type> _mapping;

        public kCoreSettings(SiteDb sitedb)
        {
            _sitedb = sitedb;
            _mapping = CoreSettingService.types.ToDictionary(k => k.Key.ToLower(), v => v.Value);
        }

        [Description(@"
Get site setting use name
var setting = k.site.settings.get('mongo') or k.site.settings.mongo
return object or null
")]
        public object Get(string name)
        {
            _mapping.TryGetValue(name.ToLower(), out var type);
            return type == null ? null : _sitedb.CoreSetting.GetSiteSetting(type);
        }

        public object this[string key]
        {
            get
            {
                return Get(key);
            }
        }
    }
}
