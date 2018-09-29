//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System;
using Kooboo.Data.Models;
using Kooboo.Lib.Reflection;
using Kooboo.Data.Context;

namespace Kooboo.Sites.DataSources
{
    public static class ParameterBinder
    {
        public static List<object> Bind(Dictionary<string, string> Parameters, Dictionary<string, ParameterBinding> Bindings, DataContext dataContext)
        {
            List<object> paras = new List<object>();

            foreach (var item in Parameters)
            {
                object value = GetValueByObjectAndProperty(item.Key, item.Value, Parameters, Bindings, dataContext);
                paras.Add(value);
            }
            return paras;
        }

        public static Dictionary<string, object> BindKScript(Dictionary<string, string> Parameters, Dictionary<string, ParameterBinding> Bindings, DataContext dataContext)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>(); 

            foreach (var item in Parameters)
            {
                var type = item.Value; 
                if (type == null)
                {
                    type = typeof(string).FullName; 
                }
                object value = GetValueByObjectAndProperty(item.Key, type, Parameters, Bindings, dataContext);
                paras.Add(item.Key, value);
            }
            return paras;
        }


        private static object GetValueByObjectAndProperty(string ParentKey, string ParentType, Dictionary<string, string> Parameters, Dictionary<string, ParameterBinding> Bindings, DataContext dataContext)
        {
            Type returntype = Data.TypeCache.GetType(ParentType);

            if (Bindings.ContainsKey(ParentKey))
            {
                var binding = Bindings[ParentKey];
                string key = binding.Binding;

                if (binding.IsCollection)
                {
                    return BindList(ParentKey, ParentType, Parameters, Bindings, dataContext);
                }
                else if (binding.IsDictionary)
                {
                    return BindDictionary(ParentKey, ParentType, Parameters, Bindings, dataContext);
                }

                if (!string.IsNullOrWhiteSpace(key))
                {
                    if (!IsValueBinding(key))
                    {
                        return TypeHelper.ChangeType(key, returntype);
                    }

                    key = GetBindingKey(key);
                    object value = GetValue(key, ParentType, dataContext);

                    if (value != null && !IsValueBinding(value.ToString()))
                    {
                        return TypeHelper.ChangeType(value, returntype);
                    }
                }
            }

            var subs = GetSubPropertyBindings(ParentKey, Bindings);
            if (subs.Count > 0)
            {
                object instance = Activator.CreateInstance(returntype);
                bool hasvalue = false;

                foreach (var item in subs)
                {
                    object subvalue = GetValueByObjectAndProperty(item.Key, item.Value.FullTypeName, Parameters, Bindings, dataContext);
                    if (subvalue != null)
                    {
                        string FieldName = item.Key.Substring(ParentKey.Length + 1);
                        var setvalue = Data.TypeCache.GetSetValue(ParentType, returntype, FieldName);
                        if (setvalue != null)
                        {
                            setvalue(instance, subvalue);
                            hasvalue = true;
                        }
                    }
                }

                if (hasvalue)
                { return instance; }
            }

            return null;
        }

        private static object BindDictionary(string ParentKey, string ParentType, Dictionary<string, string> Parameters, Dictionary<string, ParameterBinding> Bindings, DataContext dataContext)
        {
            ParameterBinding binding;
            if (!Bindings.TryGetValue(ParentKey, out binding))
            {
                return null;
            }

            string BindingString = binding.Binding;
            if (string.IsNullOrWhiteSpace(BindingString))
            {
                return null;
            }

            Type DictType = Data.TypeCache.GetType(ParentType);
            Type KeyType = Data.TypeCache.GetType(binding.KeyType);
            Type ValueType = Data.TypeCache.GetType(binding.ValueType);

            // try... jsonbinding first.  
            Dictionary<string, string> dictjson = null;

            try
            {
                dictjson = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, string>>(BindingString);
            }
            catch (Exception)
            {
                // throw;
            }

            if (dictjson != null)
            {
                var dict = Activator.CreateInstance(DictType) as System.Collections.IDictionary;

                Dictionary<string, object> RightValues = new Dictionary<string, object>();

                foreach (var item in dictjson)
                {
                    string ValueExpression = item.Value;
                    object valueback;
                    if (ValueExpression.Contains("{"))
                    {
                        valueback = GetValue(ValueExpression, binding.ValueType, dataContext);
                    }
                    else
                    {
                        valueback = ValueExpression;
                    }

                    string KeyExpression = item.Key;
                    object KeyBack;
                    if (KeyExpression.Contains("{"))
                    {
                        KeyBack = GetValue(KeyExpression, binding.KeyType, dataContext);
                    }
                    else
                    {
                        KeyBack = KeyExpression;
                    }

                    RightValues.Add(item.Key, valueback);
                }

                foreach (var item in RightValues)
                {
                    object key = TypeHelper.ChangeType(item.Key, KeyType);
                    object value = TypeHelper.ChangeType(item.Value, ValueType);
                    dict.Add(key, value);
                }
                return dict;
            }

            else
            {
                string key = GetBindingKey(BindingString);
                var valueback = GetValue(key, binding.FullTypeName, dataContext);
                if (valueback != null && valueback.GetType() == DictType)
                {
                    return valueback;
                }
            }


            return null;

        }

        private static object BindList(string ParentKey, string ParentType, Dictionary<string, string> Parameters, Dictionary<string, ParameterBinding> Bindings, DataContext dataContext)
        {
            ParameterBinding binding;
            if (!Bindings.TryGetValue(ParentKey, out binding))
            {
                return null;
            }

            string BindingString = binding.Binding;
            if (string.IsNullOrWhiteSpace(BindingString))
            {
                return null;
            }

            Type keytype = Data.TypeCache.GetType(binding.KeyType);
            Type collectiontype = Data.TypeCache.GetType(ParentType);

            try
            {
                var value =  Lib.Helper.JsonHelper.Deserialize(BindingString, collectiontype);
                if (value != null)
                {
                    var list = value as System.Collections.IList;
                    var count = list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var item = list[i];
                        if (item != null)
                        {
                            object KeyBack;
                            string stritem = item.ToString();
                            if (IsValueBinding(stritem))
                            {
                                string key = GetBindingKey(stritem);
                                KeyBack = GetValue(key, binding.KeyType, dataContext);
                                list[i] = TypeHelper.ChangeType(KeyBack, keytype);
                            }
                        }
                    }
                    return list;
                }
            }
            catch (Exception)
            {
            }


            string bkey = GetBindingKey(BindingString);
            var valueback = GetValue(bkey, binding.FullTypeName, dataContext);
            if (valueback != null && valueback.GetType() == collectiontype)
            {
                return valueback;
            } 

            return null;
        }

        // Get the one level down property of current type... 
        private static Dictionary<string, ParameterBinding> GetSubPropertyBindings(string ParentKey, Dictionary<string, ParameterBinding> Bindings)
        {
            string Prefix = ParentKey + ".";
            Dictionary<string, ParameterBinding> subs = new Dictionary<string, ParameterBinding>();
            foreach (var item in Bindings)
            {
                if (item.Key.StartsWith(Prefix))
                {
                    string left = item.Key.Substring(Prefix.Length);
                    if (!left.Contains("."))
                    {
                        subs.Add(item.Key, item.Value);
                    }
                }
            }
            return subs;
        }

        public static string GetBindingKey(string BindingString)
        {
            int start = BindingString.IndexOf("{");
            int end = BindingString.IndexOf("}");

            if (end > start && start > -1)
            {
                return BindingString.Substring(start + 1, end - start - 1);
            }
            return string.Empty;
        }

        public static bool IsValueBinding(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return input.Contains("{") && input.Contains("}");
        }

        private static object GetValue(string Key, string ObjectTypeName, DataContext dataContext)
        {
            object value = dataContext.GetValue(Key);
            if (value == null)
            {
                value = GetValueByObjectType(Key, ObjectTypeName, dataContext);
            }
            if (value == null)
            {
                string subkey = GetSubKey(Key);
                if (!string.IsNullOrEmpty(subkey))
                {
                    value = GetValue(subkey, ObjectTypeName, dataContext);
                }
            }
            return value;
        }

        private static object GetValueByObjectType(string OriginalKey, string KeyTypeName, DataContext dataContext)
        {
            int firstdot = OriginalKey.IndexOf(".");
            if (firstdot > 0)
            {
                Type type = Data.TypeCache.GetType(KeyTypeName);
                if (type.IsClass && type != typeof(string))
                {
                    string TypeName = type.Name;
                    string prefixkey = OriginalKey.Substring(0, firstdot);
                    if (TypeName.ToLower() != prefixkey.ToLower())
                    {
                        string newkey = TypeName + "." + OriginalKey.Substring(firstdot + 1);
                        var value = dataContext.GetValueByObjectType(newkey);
                        if (value != null)
                        {
                            return value;
                        }
                    }
                }
            }
            else
            {
                Type type = Data.TypeCache.GetType(KeyTypeName);
                if (type.IsClass && type != typeof(string))
                {
                    string TypeName = type.Name;
                    var value = dataContext.GetValueByObjectType(TypeName);
                    if (value != null)
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        public static string GetSubKey(string key)
        {
            int dotindex = key.IndexOf(".");

            if (dotindex > 0)
            {
                string sub = key.Substring(dotindex + 1);
                if (!string.IsNullOrEmpty(sub))
                {
                    return sub;
                }
            }

            bool HasLower = false;
            bool HasUpperAgain = false;

            int currentindex = 0;
            while (currentindex < key.Length)
            {
                var currentchar = key[currentindex];
                if (!HasLower)
                {
                    if (Kooboo.Lib.Helper.CharHelper.isLowercaseAscii(currentchar))
                    {
                        HasLower = true;
                    }
                }
                else
                {
                    if (Kooboo.Lib.Helper.CharHelper.isUppercaseAscii(currentchar))
                    {
                        HasUpperAgain = true;
                        break;
                    }
                }
                currentindex += 1;
            }

            if (HasUpperAgain && currentindex < key.Length)
            {
                string sub = key.Substring(currentindex);
                return sub;
            }
            return null;
        }
    }
}
