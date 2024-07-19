//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Reflection;
using Kooboo.Api;
using Kooboo.Data.Events;
using Kooboo.Data.Interface;
using Kooboo.Data.Permission;
using Kooboo.Events;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.Implementation
{
    public class CoreSettingApi : IApi
    {
        public string ModelName => "CoreSetting";

        public bool RequireSite => true;

        public bool RequireUser => true;

        [Permission(Feature.CONFIG, Action = Data.Permission.Action.VIEW)]
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

        [Permission(Feature.CONFIG, Action = Data.Permission.Action.VIEW)]
        public List<CoreSettingViewModel> List(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var result = new List<CoreSettingViewModel>();

            foreach (var item in Kooboo.Sites.Service.CoreSettingService.types)
            {
                var value = sitedb.CoreSetting.Get(item.Key);
                var instance = Activator.CreateInstance(item.Value);
                var group = instance is ISettingDescription ? (instance as ISettingDescription).Group : "Others";
                var alert = instance is ISettingDescription ? (instance as ISettingDescription).GetAlert(call.Context) : "";

                if (value == null)
                {
                    if (instance != null)
                    {
                        var name = GetName(instance);

                        result.Add(new CoreSettingViewModel()
                        {
                            Name = name,
                            Group = group,
                            Alert = alert
                        });
                    }

                }
                else
                {
                    var values = GetValues(value.Values, item.Value)
                        .ToDictionary(x => x.Name,
                            x => x.Type == "file" ? new SettingFile(x.Value?.ToString()).Name : x.Value);
                    var json = Lib.Helper.JsonHelper.Serialize(values);

                    var vm = new CoreSettingViewModel()
                    {
                        Name = value.Name,
                        Group = group,
                        Alert = alert,
                        Value = json,
                        lastModify = value.LastModified
                    };

                    result.Add(vm);
                }

            }

            return result;
        }

        [Permission(Feature.CONFIG, Action = Data.Permission.Action.VIEW)]
        private List<SettingItem> GetValues(Dictionary<string, string> values, Type type)
        {
            values = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);
            var result = new List<SettingItem>();
            var allfields = Lib.Reflection.TypeHelper.GetPublicPropertyOrFields(type);
            var defaultObject = Activator.CreateInstance(type);
            foreach (var field in allfields)
            {
                values.TryGetValue(field.Name, out var rawValue);

                if (string.IsNullOrEmpty(rawValue))
                {
                    rawValue = type.GetProperty(field.Name).GetValue(defaultObject)?.ToString();
                }

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

        [Permission(Feature.CONFIG, Action = Data.Permission.Action.EDIT)]
        public void Update(string name, Dictionary<string, string> model, ApiCall call)
        {
            var Core = new CoreSetting
            {
                Name = name,
                Values = model
            };
            call.WebSite.SiteDb().CoreSetting.AddOrUpdate(Core);

            var coreSettingEvent = new CoreSettingChangeEvent()
            {
                Name = name,
                CoreSettingRenderContext = call.Context
            };
            EventBus.Raise(coreSettingEvent, call.Context);
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

        public string Group { get; set; }

        public string Alert { get; set; }

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
