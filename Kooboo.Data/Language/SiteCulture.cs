//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Language
{
    public static class SiteCulture
    {
        private static object _culturelock = new object();

        private static Dictionary<string, string> _initcultures;
        public static Dictionary<string, string> InitCultures
        {
            get
            {
                if (_initcultures == null)
                {
                    lock (_culturelock)
                    {
                        if (_initcultures == null)
                        {

                            List<string> startlist = new List<string>();
                            startlist.Add("zh");
                            startlist.Add("es");
                            startlist.Add("en");
                            startlist.Add("es");
                            startlist.Add("hi");
                            startlist.Add("ar");
                            startlist.Add("pt");
                            startlist.Add("bn");
                            startlist.Add("ru");
                            startlist.Add("ja");
                            startlist.Add("pa");
                            startlist.Add("de");
                            startlist.Add("jv");
                            startlist.Add("ms");
                            startlist.Add("jv");
                            startlist.Add("te");
                            startlist.Add("vi");
                            startlist.Add("ko");
                            startlist.Add("fr");
                            startlist.Add("mr");
                            startlist.Add("ta");
                            startlist.Add("ur");
                            startlist.Add("tr");
                            startlist.Add("it");
                            startlist.Add("th");
                            startlist.Add("nl");
                            startlist.Add("el");
                            startlist.Add("sv");


                            _initcultures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (var item in startlist)
                            {
                                if (LanguageSetting.ISOTwoLetterCode.ContainsKey(item))
                                {
                                    var value = LanguageSetting.ISOTwoLetterCode[item];
                                    if (!_initcultures.ContainsKey(item))
                                    {
                                        _initcultures.Add(item, value);
                                    }
                                }
                            }

                        }
                    }
                }
                return _initcultures;
            }
        }

        public static Dictionary<string, string> List(Guid SiteId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var site = GlobalDb.WebSites.Get(SiteId);

            if (site != null)
            {
                var setting = GlobalDb.GlobalSetting.Query.Where(o => o.OrganizationId == site.OrganizationId).SelectAll().Find(o => o.Name == "culture");

                if (setting != null)
                {
                    foreach (var item in setting.KeyValues)
                    {
                        result[item.Key] = item.Value;
                    }
                }
            }

            foreach (var item in InitCultures.ToList())
            {
                if (!result.ContainsKey(item.Key))
                {
                    result[item.Key] = item.Value;
                }
            }
            return result;
        }
    }
}
