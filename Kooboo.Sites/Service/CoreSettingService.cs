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

        private static Dictionary<string, Type>  _types;
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

                            var alltypes  = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(ISiteSetting));
                            foreach (var item in alltypes)
                            {
                                var instance = Activator.CreateInstance(item) as ISiteSetting; 
                                if (instance !=null)
                                {
                                    _types[instance.Name] = item; 
                                }
                            }
                        }
                    }
                }
                return _types;
            } 
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
            CoreSetting dbsettiing = new CoreSetting();

            dbsettiing.Name = siteSetting.GetType().Name;

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
                foreach (var item in coresetting.Values)
                {
                    if (item.Value != null)
                    {   
                        var prop = type.GetProperty(item.Key);

                        if (prop != null)
                        {
                            var value = Lib.Reflection.TypeHelper.ChangeType(item.Value, prop.PropertyType); 
                            prop.SetValue(result, value);
                        }
                        else
                        {
                            var field = type.GetField(item.Key);
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
            if (SettingType == null)
            {
                return null; 
            }

            var result = Activator.CreateInstance(SettingType) as ISiteSetting;

            if (coresetting != null && coresetting.Values != null && coresetting.Values.Any())
            {
                foreach (var item in coresetting.Values)
                {
                    if (item.Value != null)
                    {
                        var prop = SettingType.GetProperty(item.Key);

                        if (prop != null)
                        {
                            var value = Lib.Reflection.TypeHelper.ChangeType(item.Value, prop.PropertyType);
                            prop.SetValue(result, value);
                        }
                        else
                        {
                            var field = SettingType.GetField(item.Key);
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
