//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.DataSources
{
    public static class ParameterBinder
    {
        public static List<object> Bind(Dictionary<string, string> parameters, Dictionary<string, ParameterBinding> bindings, DataContext dataContext)
        {
            List<object> paras = new List<object>();

            foreach (var item in parameters)
            {
                object value = GetValueByObjectAndProperty(item.Key, item.Value, parameters, bindings, dataContext);
                paras.Add(value);
            }
            return paras;
        }

        public static Dictionary<string, object> BindKScript(Dictionary<string, string> parameters, Dictionary<string, ParameterBinding> bindings, DataContext dataContext)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();

            foreach (var item in parameters)
            {
                var type = item.Value ?? typeof(string).FullName;
                object value = GetValueByObjectAndProperty(item.Key, type, parameters, bindings, dataContext);
                paras.Add(item.Key, value);
            }
            return paras;
        }

        private static object GetValueByObjectAndProperty(string parentKey, string parentType, Dictionary<string, string> parameters, Dictionary<string, ParameterBinding> bindings, DataContext dataContext)
        {
            Type returntype = Data.TypeCache.GetType(parentType);

            if (bindings.ContainsKey(parentKey))
            {
                var binding = bindings[parentKey];
                string key = binding.Binding;

                if (binding.IsCollection)
                {
                    return BindList(parentKey, parentType, parameters, bindings, dataContext);
                }
                else if (binding.IsDictionary)
                {
                    return BindDictionary(parentKey, parentType, parameters, bindings, dataContext);
                }

                if (!string.IsNullOrWhiteSpace(key))
                {
                    if (!IsValueBinding(key))
                    {
                        return TypeHelper.ChangeType(key, returntype);
                    }

                    key = GetBindingKey(key);
                    object value = GetValue(key, parentType, dataContext);

                    if (value != null && !IsValueBinding(value.ToString()))
                    {
                        return TypeHelper.ChangeType(value, returntype);
                    }
                }
            }

            var subs = GetSubPropertyBindings(parentKey, bindings);
            if (subs.Count > 0)
            {
                object instance = Activator.CreateInstance(returntype);
                bool hasvalue = false;

                foreach (var item in subs)
                {
                    object subvalue = GetValueByObjectAndProperty(item.Key, item.Value.FullTypeName, parameters, bindings, dataContext);
                    if (subvalue != null)
                    {
                        string fieldName = item.Key.Substring(parentKey.Length + 1);
                        var setvalue = Data.TypeCache.GetSetValue(parentType, returntype, fieldName);
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

        private static object BindDictionary(string parentKey, string parentType, Dictionary<string, string> parameters, Dictionary<string, ParameterBinding> bindings, DataContext dataContext)
        {
            if (!bindings.TryGetValue(parentKey, out var binding))
            {
                return null;
            }

            string bindingString = binding.Binding;
            if (string.IsNullOrWhiteSpace(bindingString))
            {
                return null;
            }

            Type dictType = Data.TypeCache.GetType(parentType);
            Type keyType = Data.TypeCache.GetType(binding.KeyType);
            Type valueType = Data.TypeCache.GetType(binding.ValueType);

            // try... jsonbinding first.
            Dictionary<string, string> dictjson = null;

            try
            {
                dictjson = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, string>>(bindingString);
            }
            catch (Exception)
            {
                // throw;
            }

            if (dictjson != null)
            {
                var dict = Activator.CreateInstance(dictType) as System.Collections.IDictionary;

                Dictionary<string, object> rightValues = new Dictionary<string, object>();

                foreach (var item in dictjson)
                {
                    string valueExpression = item.Value;
                    object valueback;
                    valueback = valueExpression.Contains("{") ? GetValue(valueExpression, binding.ValueType, dataContext) : valueExpression;

                    string keyExpression = item.Key;
                    object keyBack;
                    keyBack = keyExpression.Contains("{") ? GetValue(keyExpression, binding.KeyType, dataContext) : keyExpression;

                    rightValues.Add(item.Key, valueback);
                }

                foreach (var item in rightValues)
                {
                    object key = TypeHelper.ChangeType(item.Key, keyType);
                    object value = TypeHelper.ChangeType(item.Value, valueType);
                    dict?.Add(key, value);
                }
                return dict;
            }
            else
            {
                string key = GetBindingKey(bindingString);
                var valueback = GetValue(key, binding.FullTypeName, dataContext);
                if (valueback != null && valueback.GetType() == dictType)
                {
                    return valueback;
                }
            }

            return null;
        }

        private static object BindList(string parentKey, string parentType, Dictionary<string, string> parameters, Dictionary<string, ParameterBinding> bindings, DataContext dataContext)
        {
            ParameterBinding binding;
            if (!bindings.TryGetValue(parentKey, out binding))
            {
                return null;
            }

            string bindingString = binding.Binding;
            if (string.IsNullOrWhiteSpace(bindingString))
            {
                return null;
            }

            Type keytype = Data.TypeCache.GetType(binding.KeyType);
            Type collectiontype = Data.TypeCache.GetType(parentType);

            try
            {
                var value = Lib.Helper.JsonHelper.Deserialize(bindingString, collectiontype);
                if (value != null)
                {
                    var list = value as System.Collections.IList;
                    if (list != null)
                    {
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
                    }

                    return list;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            string bkey = GetBindingKey(bindingString);
            var valueback = GetValue(bkey, binding.FullTypeName, dataContext);
            if (valueback != null && valueback.GetType() == collectiontype)
            {
                return valueback;
            }

            return null;
        }

        // Get the one level down property of current type...
        private static Dictionary<string, ParameterBinding> GetSubPropertyBindings(string parentKey, Dictionary<string, ParameterBinding> bindings)
        {
            string prefix = parentKey + ".";
            Dictionary<string, ParameterBinding> subs = new Dictionary<string, ParameterBinding>();
            foreach (var item in bindings)
            {
                if (item.Key.StartsWith(prefix))
                {
                    string left = item.Key.Substring(prefix.Length);
                    if (!left.Contains("."))
                    {
                        subs.Add(item.Key, item.Value);
                    }
                }
            }
            return subs;
        }

        public static string GetBindingKey(string bindingString)
        {
            int start = bindingString.IndexOf("{");
            int end = bindingString.IndexOf("}");

            if (end > start && start > -1)
            {
                return bindingString.Substring(start + 1, end - start - 1);
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

        private static object GetValue(string key, string objectTypeName, DataContext dataContext)
        {
            object value = dataContext.GetValue(key) ?? GetValueByObjectType(key, objectTypeName, dataContext);
            if (value == null)
            {
                string subkey = GetSubKey(key);
                if (!string.IsNullOrEmpty(subkey))
                {
                    value = GetValue(subkey, objectTypeName, dataContext);
                }
            }
            return value;
        }

        private static object GetValueByObjectType(string originalKey, string keyTypeName, DataContext dataContext)
        {
            int firstdot = originalKey.IndexOf(".");
            if (firstdot > 0)
            {
                Type type = Data.TypeCache.GetType(keyTypeName);
                if (type.IsClass && type != typeof(string))
                {
                    string typeName = type.Name;
                    string prefixkey = originalKey.Substring(0, firstdot);
                    if (typeName.ToLower() != prefixkey.ToLower())
                    {
                        string newkey = typeName + "." + originalKey.Substring(firstdot + 1);
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
                Type type = Data.TypeCache.GetType(keyTypeName);
                if (type.IsClass && type != typeof(string))
                {
                    string typeName = type.Name;
                    var value = dataContext.GetValueByObjectType(typeName);
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

            bool hasLower = false;
            bool hasUpperAgain = false;

            int currentindex = 0;
            while (currentindex < key.Length)
            {
                var currentchar = key[currentindex];
                if (!hasLower)
                {
                    if (Kooboo.Lib.Helper.CharHelper.isLowercaseAscii(currentchar))
                    {
                        hasLower = true;
                    }
                }
                else
                {
                    if (Kooboo.Lib.Helper.CharHelper.isUppercaseAscii(currentchar))
                    {
                        hasUpperAgain = true;
                        break;
                    }
                }
                currentindex += 1;
            }

            if (hasUpperAgain && currentindex < key.Length)
            {
                string sub = key.Substring(currentindex);
                return sub;
            }
            return null;
        }
    }
}