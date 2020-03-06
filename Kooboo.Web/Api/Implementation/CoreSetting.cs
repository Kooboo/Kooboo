//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kooboo.Web.Api.Implementation
{
    public class CoreSettingApi : IApi
    {
        public string ModelName => "CoreSetting";

        public bool RequireSite => true;

        public bool RequireUser => true;

        public List<SettingItem> Get(ApiCall call, string name)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var item = sitedb.CoreSetting.Get(name);

            var values = new Dictionary<string, string>();
            if (item != null)
            {
                values = item.Values;
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

            var result = new List<CoreSettingViewModel>();

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
                    var values = GetValues(value.Values, item.Value)
                        .ToDictionary(x => x.Name,
                            x => x.Type == "file" ? new SettingFile(x.Value?.ToString()).Name : x.Value);
                    var json = Lib.Helper.JsonHelper.Serialize(values);

                    result.Add(new CoreSettingViewModel() { Name = value.Name, Value = json, lastModify = value.LastModified });
                }

            }

            return result;
        }

        private List<SettingItem> GetValues(Dictionary<string, string> values, Type type)
        {
            values = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);
            var result = new List<SettingItem>();
            var allfields = Lib.Reflection.TypeHelper.GetPublicPropertyOrFields(type);
            foreach (var field in allfields)
            {
                values.TryGetValue(field.Name, out var rawValue);
                var item = new SettingItem { Name = field.Name, Value = rawValue };

                var valueType = GetValueType(field);
                if (valueType == typeof(bool))
                {
                    item.Type = "checkbox";
                    bool.TryParse(rawValue, out var boolValue);
                    item.Value = boolValue;
                }

                if (valueType == typeof(SettingFile))
                {
                    item.Type = "file";
                }

                result.Add(item);
            }

            return result;
        }

        private static Type GetValueType(MemberInfo field)
        {
            if (field is PropertyInfo p)
            {
                return p.PropertyType;
            }

            if (field is FieldInfo f)
            {
                return f.FieldType;
            }

            return typeof(string);
        }

        public void Update(string name, Dictionary<string, string> model, ApiCall call)
        {
            var Core = new CoreSetting
            {
                Name = name,
                Values = model
            };
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

    public class SettingItem
    {
        public string Name { get; set; }

        public string Type { get; set; } = "text";

        public object Value { get; set; }
    }
}
