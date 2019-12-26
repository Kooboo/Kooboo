//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class CoreSettingApi : IApi
    {
        public string ModelName => "CoreSetting";

        public bool RequireSite => true;

        public bool RequireUser => true;

        public Dictionary<string, string> Get(ApiCall call, string name)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var item = sitedb.CoreSetting.Get(name);

            var values = new Dictionary<string, string>();
            if (item !=null)
            {
                values= item.Values; 
            }

            var type = Sites.Service.CoreSettingService.GetSettingType(name);

            if (type != null)
            {
                return GetValues(values, type);
            }
            return null; 
        }

        public List<CoreSettingViewModel> List(ApiCall call)
        {  
            var sitedb = call.WebSite.SiteDb();

            List<CoreSettingViewModel> result = new List<CoreSettingViewModel>(); 
             
            foreach (var item in Kooboo.Sites.Service.CoreSettingService.types)
            {
                var value = sitedb.CoreSetting.Get(item.Key); 
               
                if (value == null)
                {
                    var instance = Activator.CreateInstance(item.Value);
                    if (instance != null)
                    {
                        var name = GetName(instance);
                        result.Add(new CoreSettingViewModel() { Name = name });
                    }
                    
                }
                else
                {
                    var values = GetValues(value.Values, item.Value);
                    var json = Lib.Helper.JsonHelper.Serialize(values); 

                    result.Add(new CoreSettingViewModel() { Name =value.Name, Value = json, lastModify = value.LastModified });  
                }

            }

            return result; 
        }

        private Dictionary<string,string> GetValues(Dictionary<string,string> values,Type type)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var allfields = Lib.Reflection.TypeHelper.GetPublicPropertyOrFields(type);
            foreach (var field in allfields)
            {
                var key = values.Keys.ToList().Find(k => k.Equals(field.Name, StringComparison.OrdinalIgnoreCase));
                result[field.Name] = !string.IsNullOrEmpty(key) ? values[key] : "";
            }

            return result;
        }

        public void Update(string name, Dictionary<string, string> model, ApiCall call)
        {
            var Core = new CoreSetting();
            Core.Name = name;
            Core.Values = model;
            call.WebSite.SiteDb().CoreSetting.AddOrUpdate(Core); 

        }

        private string GetName(object instance)
        {
            var type = instance.GetType();
            var sitesettingInstance = instance as ISiteSetting;

            var name = string.Empty;
            if (sitesettingInstance != null)
            {
                name = sitesettingInstance.Name;
            }
            else
            {
                var nameProp = type.GetProperty("Name");
                if (nameProp != null)
                {
                    name = nameProp.GetValue(instance) as string;
                }
                if (string.IsNullOrEmpty(name))
                {
                    name = type.Name;
                }
            }

            return name;
        }
    }

    public class CoreSettingViewModel
    {
       
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime lastModify { get; set; }
    }
}
