//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kooboo.Sites.Service
{
    public static class CoreSettingService
    {
        private static object _locker = new object();

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
                                if (Activator.CreateInstance(item) is ISiteSetting instance)
                                {
                                    var name = instance.Name;
                                    if (string.IsNullOrWhiteSpace(name))
                                    {
                                        name = item.Name;
                                    }

                                    _types[name] = item;
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
                if (item.Value == type)
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
            CoreSetting dbsettiing = new CoreSetting {Name = siteSetting.Name};


            var allprops = Lib.Reflection.TypeHelper.GetPublicPropertyOrFields(siteSetting.GetType());

            foreach (var item in allprops)
            {
                if (item is PropertyInfo info1)
                {
                    var value = info1.GetValue(siteSetting);

                    if (value != null)
                    {
                        dbsettiing.Values.Add(info1.Name, value.ToString());
                    }
                }
                else if (item is FieldInfo info)
                {
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

            if (coresetting?.Values != null && coresetting.Values.Any())
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

        public static ISiteSetting GetSiteSetting(CoreSetting coresetting, Type settingType)
        {
            if (settingType == null)
            {
                return null;
            }

            var result = Activator.CreateInstance(settingType) as ISiteSetting;

            if (coresetting?.Values != null && coresetting.Values.Any())
            {
                var properties = settingType.GetProperties().ToList();
                var fields = settingType.GetFields().ToList();
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
    }
}