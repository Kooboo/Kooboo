//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using KScript.KscriptConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kooboo.Sites.Service
{
    public static class CoreSettingService
    {
        private static readonly object _locker = new object();

        private static Dictionary<string, Type> _types;
        public static Dictionary<string, Type> types
        {
            get
            {
                if (_types == null)
                {
                    lock (_locker)
                    {
                        if (_types == null)
                        {
                            _types = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

                            var alltypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(ISiteSetting));
                            foreach (var item in alltypes)
                            {
                                var instance = Activator.CreateInstance(item) as ISiteSetting;
                                if (instance != null)
                                {
                                    var name = instance.Name;
                                    if (string.IsNullOrWhiteSpace(name))
                                    {
                                        name = item.Name;
                                    }

                                    _types[name] = item;
                                }
                            }

                            if (KscriptConfigContainer.KscriptSettings != null)
                            {
                                foreach (var setting in KscriptConfigContainer.KscriptSettings)
                                {
                                    _types[setting.Key] = setting.Value;

                                }
                            }
                        }
                    }
                }
                return _types;
            }
        }

        public static string GetName(Type type)
        {
            foreach (var item in types)
            {
                if (item.Value == type || item.Value.FullName.Equals(type.FullName))
                {
                    return item.Key;
                }
            }
            return null;
        }

        public static Type GetSettingType(string name)
        {
            if (types.ContainsKey(name))
            {
                return types[name];
            }
            return null;
        }

        public static CoreSetting GetCoreSetting(ISiteSetting siteSetting)
        {
            var dbsettiing = new CoreSetting
            {
                Name = siteSetting.Name
            };

            var allprops = Lib.Reflection.TypeHelper.GetPublicPropertyOrFields(siteSetting.GetType());

            foreach (var item in allprops)
            {
                if (item is PropertyInfo)
                {
                    var info = item as PropertyInfo;

                    var value = info.GetValue(siteSetting);

                    if (value != null)
                    {
                        dbsettiing.Values.Add(info.Name, value.ToString());
                    }
                }
                else if (item is FieldInfo)
                {
                    var info = item as FieldInfo;

                    var value = info.GetValue(siteSetting);

                    if (value != null)
                    {
                        dbsettiing.Values.Add(info.Name, value.ToString());
                    }
                }

            }

            return dbsettiing;
        }

        public static T GetSiteSetting<T>(CoreSetting coresetting) where T : ISiteSetting
        {
            var type = typeof(T);

            var result = Activator.CreateInstance<T>();

            if (coresetting != null && coresetting.Values != null && coresetting.Values.Any())
            {
                var properties = type.GetProperties().ToList();
                var fields = type.GetFields().ToList();
                foreach (var item in coresetting.Values)
                {
                    if (item.Value != null)
                    {
                        var prop = properties.Find(p => p.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase));

                        if (prop != null)
                        {
                            var value = Lib.Reflection.TypeHelper.ChangeType(item.Value, prop.PropertyType);
                            prop.SetValue(result, value);
                        }
                        else
                        {
                            var field = fields.Find(f => f.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase));
                            if (field != null)
                            {
                                var value = Lib.Reflection.TypeHelper.ChangeType(item.Value, field.FieldType);
                                field.SetValue(result, value);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static ISiteSetting GetSiteSetting(CoreSetting coresetting, Type SettingType)
        {
            return GetSetting(coresetting, SettingType) as ISiteSetting;
        }

        public static object GetSetting(CoreSetting coresetting, Type SettingType)
        {
            if (SettingType == null)
            {
                return null;
            }

            var result = Activator.CreateInstance(SettingType);

            if (coresetting != null && coresetting.Values != null && coresetting.Values.Any())
            {
                var properties = SettingType.GetProperties().ToList();
                var fields = SettingType.GetFields().ToList();
                foreach (var item in coresetting.Values)
                {
                    if (item.Value != null)
                    {
                        var prop = properties.Find(p => p.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase));

                        if (prop != null)
                        {
                            var value = ChangeType(item.Value, prop.PropertyType);
                            prop.SetValue(result, value);
                        }
                        else
                        {
                            var field = fields.Find(f => f.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase));
                            if (field != null)
                            {
                                var value = ChangeType(item.Value, field.FieldType);
                                field.SetValue(result, value);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static object ChangeType(string value, Type targetType)
        {
            if (targetType == typeof(SettingFile))
            {
                return new SettingFile(value);
            }

            return Lib.Reflection.TypeHelper.ChangeType(value, targetType);
        }
    }
}
