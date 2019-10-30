//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Extensions;
using Kooboo.Lib.Reflection;
using Kooboo.Sites.DataSources.New.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Kooboo.Attributes;

namespace Kooboo.Sites.DataSources
{
    public static class DataSourceHelper
    {
        public static void InitIDataSource()
        {
            var dataMethodStore = GlobalDb.DataMethodSettings;
            // check all types have been loaded into system...
            var allCurrentSettings = dataMethodStore.All();
            foreach (var typeItem in allCurrentSettings.GroupBy(o => o.DeclareType))
            {
                var type = Kooboo.Data.TypeCache.GetType(typeItem.Key);
                if (type == null)
                {
                    foreach (var item in typeItem)
                    {
                        dataMethodStore.Delete(item);
                    }
                }
            }

            var alltypes = AssemblyLoader.LoadTypeByInterface(typeof(IDataSource));

            foreach (var item in alltypes)
            {
                Guid typehash = item.FullName.ToHashGuid();

                var oldsetting = dataMethodStore.Query.Where(o => o.DeclareTypeHash == typehash).SelectAll();

                var newsettings = GetDefaultMethodSettings(item, false);

                foreach (var methodItem in oldsetting)
                {
                    if (!newsettings.Any(o => o.MethodSignatureHash == methodItem.MethodSignatureHash))
                    {
                        dataMethodStore.Delete(methodItem.Id);
                    }
                }

                foreach (var methodItem in newsettings)
                {
                    dataMethodStore.AddOrUpdate(methodItem);
                }
            }
        }

        internal static List<DataMethodSetting> GetDefaultMethodSettings(Type type, bool isThirdPartyType = false)
        {
            var list = new List<DataMethodSetting>();

            var descriptionAttributeType = typeof(System.ComponentModel.DescriptionAttribute);

            foreach (var item in TypeHelper.GetPublicMethods(type))
            {
                var returntype = ReturnType(item);

                var globalmethod = new DataMethodSetting
                {
                    DeclareType = type.FullName,
                    OriginalMethodName = item.Name,
                    MethodName = item.Name,
                    MethodSignatureHash = TypeHelper.GetMethodSignatureHash(item),
                    IsThirdPartyType = isThirdPartyType,
                    IsStatic = item.IsStatic,
                    IsVoid = (returntype == typeof(void)),
                    Parameters = GetMethodParametes(item),
                    ReturnType = returntype.FullName,
                    IsPagedResult = IsPagedResult(item),
                    // set is pagedresult.
                    Description = (item.GetCustomAttribute(descriptionAttributeType) as System.ComponentModel.DescriptionAttribute)?.Description
                };

                if (globalmethod.Parameters.Count > 0)
                {
                    globalmethod.ParameterBinding = GetDefaultBinding(globalmethod.Parameters);

                    if (Attributes.AttributeHelper.RequireFolder(item))
                    {
                        AppendContentFolderParameter(globalmethod.ParameterBinding);
                    }

                    if (Attributes.AttributeHelper.RequireProductType(item))
                    {
                        AppendProductTypeParameter(globalmethod.ParameterBinding);
                    }
                }
                list.Add(globalmethod);
            }

            return list;
        }

        public static void AppendContentFolderParameter(Dictionary<string, ParameterBinding> currentBinding)
        {
            bool hasfolder = false;
            foreach (var item in currentBinding)
            {
                string lowerkey = item.Key.ToLower();
                if (lowerkey.Contains("folderid") || lowerkey.Contains("folder"))
                {
                    hasfolder = true;
                    item.Value.IsContentFolder = true;
                    break;
                }
            }
            if (!hasfolder)
            {
                ParameterBinding binding = new ParameterBinding
                {
                    FullTypeName = typeof(System.Guid).Name,
                    Binding = default(Guid).ToString(),
                    IsContentFolder = true
                };
                currentBinding.Add("FolderId", binding);
            }
        }

        public static void AppendProductTypeParameter(Dictionary<string, ParameterBinding> currentBinding)
        {
            bool hasfolder = false;
            foreach (var item in currentBinding)
            {
                string lowerkey = item.Key.ToLower();
                if (lowerkey.Contains("producttype") || lowerkey.Contains("producttypeid"))
                {
                    hasfolder = true;
                    item.Value.IsProductType = true;
                    break;
                }
            }
            if (!hasfolder)
            {
                ParameterBinding binding = new ParameterBinding
                {
                    FullTypeName = typeof(System.Guid).Name,
                    Binding = default(Guid).ToString(),
                    IsProductType = true
                };
                currentBinding.Add("ProductTypeId", binding);
            }
        }

        public static Dictionary<string, string> GetMethodParametes(MethodInfo method)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (var item in method.GetParameters())
            {
                parameters.Add(item.Name, item.ParameterType.FullName);
            }
            return parameters;
        }

        public static Dictionary<string, ParameterBinding> GetDefaultBinding(Dictionary<string, string> parameters)
        {
            Dictionary<string, ParameterBinding> bindings = new Dictionary<string, ParameterBinding>();

            foreach (var item in parameters)
            {
                Type itemtype = Kooboo.Data.TypeCache.GetType(item.Value);
                ParameterBinding binding = new ParameterBinding {FullTypeName = itemtype.FullName};

                if (TypeHelper.IsFieldType(itemtype))
                {
                    binding.Binding = "{" + item.Key + "}";
                    binding.IsContentFolder = IsContentFolder(item.Key);
                    binding.IsProductType = IsProductType(item.Key);
                    binding.IsData = IsData(item.Key);
                    binding.IsOrderBy = IsOrderBy(item.Key);
                }
                else if (TypeHelper.IsDictionary(itemtype))
                {
                    binding.KeyType = TypeHelper.GetDictionaryKeyType(itemtype).FullName;
                    binding.ValueType = TypeHelper.GetDictionaryValueType(itemtype).FullName;
                    binding.IsDictionary = true;
                    binding.Binding = "{}";
                }
                else if (TypeHelper.IsGenericCollection(itemtype))
                {
                    binding.IsCollection = true;
                    binding.IsContentFolder = IsContentFolder(item.Key);
                    binding.IsProductType = IsProductType(item.Key);
                    binding.Binding = "[]";
                    binding.KeyType = TypeHelper.GetEnumberableType(itemtype).FullName;
                }
                else if (itemtype.IsClass)
                {
                    string className = itemtype.Name;
                    binding.Binding = "{" + className + "}";
                    binding.IsClass = true;

                    foreach (var fieldItem in Lib.Reflection.TypeHelper.GetPublicFieldOrProperties(itemtype))
                    {
                        var subitems = GetSubBinding(fieldItem.Key, fieldItem.Value);
                        foreach (var subitem in subitems)
                        {
                            string key = item.Key + "." + subitem.Key;
                            var value = subitem.Value;
                            if (!value.IsCollection && !value.IsDictionary)
                            {
                                if (!string.IsNullOrEmpty(value.Binding) && !value.Binding.Contains("."))
                                {
                                    value.Binding = "{" + className + "." + value.Binding + "}";
                                }
                            }
                            value.IsContentFolder = IsContentFolder(subitem.Key);
                            value.IsProductType = IsProductType(subitem.Key);
                            value.IsData = IsData(subitem.Key);
                            value.IsOrderBy = IsOrderBy(subitem.Key);
                            bindings.Add(key, value);
                        }
                    }
                }
                else
                {
                    continue;
                }

                bindings.Add(item.Key, binding);
            }
            return bindings;
        }

        private static bool IsContentFolder(string name)
        {
            return name.ToLower() == "folder" || name.ToLower() == "folderid";
        }

        private static bool IsProductType(string name)
        {
            return name.ToLower() == "producttype" || name.ToLower() == "producttypeid";
        }

        private static bool IsData(string name)
        {
            return name.ToLower().Contains("sample");
        }

        private static bool IsOrderBy(string name)
        {
            var lower = name.ToLower();

            return lower.Contains("sortfield") || lower.Contains("orderby");
        }

        private static Dictionary<string, ParameterBinding> GetSubBinding(string fieldName, Type fieldType)
        {
            Dictionary<string, ParameterBinding> bindings = new Dictionary<string, ParameterBinding>();

            if (TypeHelper.IsFieldType(fieldType))
            {
                ParameterBinding binding = new ParameterBinding
                {
                    Binding = fieldName, FullTypeName = fieldType.FullName
                };
                bindings.Add(fieldName, binding);
            }
            else if (TypeHelper.IsDictionary(fieldType))
            {
                ParameterBinding binding = new ParameterBinding
                {
                    FullTypeName = fieldType.FullName,
                    IsDictionary = true,
                    KeyType = Lib.Reflection.TypeHelper.GetDictionaryKeyType(fieldType).FullName,
                    ValueType = Lib.Reflection.TypeHelper.GetDictionaryValueType(fieldType).FullName
                };
                bindings.Add(fieldName, binding);
            }
            else if (TypeHelper.IsGenericCollection(fieldType))
            {
                ParameterBinding binding = new ParameterBinding
                {
                    FullTypeName = fieldType.FullName,
                    IsCollection = true,
                    IsContentFolder = IsContentFolder(fieldName),
                    IsProductType = IsProductType(fieldName),
                    KeyType = Lib.Reflection.TypeHelper.GetEnumberableType(fieldType).FullName
                };
                bindings.Add(fieldName, binding);
            }
            else if (fieldType.IsClass)
            {
                string classname = fieldType.Name;

                ParameterBinding binding = new ParameterBinding
                {
                    Binding = fieldName, FullTypeName = fieldType.FullName
                };
                bindings.Add(fieldName, binding);

                foreach (var fieldItem in fieldType.GetPublicFieldOrProperties())
                {
                    var dict = GetSubBinding(fieldItem.Key, fieldItem.Value);

                    foreach (var item in dict)
                    {
                        if (!string.IsNullOrEmpty(item.Value.Binding) && !item.Value.Binding.Contains("."))
                        {
                            item.Value.Binding = classname + "." + item.Value.Binding;
                        }
                        bindings.Add(fieldName + "." + item.Key, item.Value);
                    }
                }
            }
            return bindings;
        }

        /// <summary>
        /// Convert ArticleId into Article.Id for default binding.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string ConvertParameterName(string input)
        {
            List<int> capitalLetterPosition = new List<int>();

            for (int i = 1; i < input.Length; i++)
            {
                if (Lib.Helper.CharHelper.isUppercaseAscii(input[i]))
                {
                    capitalLetterPosition.Add(i);
                }
            }

            if (capitalLetterPosition.Count == 1)
            {
                int position = capitalLetterPosition[0];
                return input.Substring(0, position) + "." + input.Substring(position);
            }

            return input;
        }

        public static TypeInfoModel GetFields(SiteDb siteDb, Guid methoId)
        {
            var methodsetting = siteDb.DataMethodSettings.Get(methoId) ?? GlobalDb.DataMethodSettings.Get(methoId);

            return methodsetting != null ? GetFields(siteDb, methodsetting) : null;
        }

        public static TypeInfoModel GetFields(SiteDb siteDb, IDataMethodSetting setting)
        {
            var model = new TypeInfoModel
            {
                Id = setting.Id,
                Name = setting.MethodName,
                DeclareType = setting.DeclareType,
                ItemFields = new List<TypeFieldModel>(),
                IsPublic = setting.IsPublic
            };

            var type = TypeCache.GetType(setting.ReturnType);
            if (type == null)
            {
                return model;
            }

            model.ModelType = type.Name;
            model.Enumerable = TypeHelper.IsGenericCollection(type);
            model.IsPagedResult = setting.IsPagedResult;
            if (model.Enumerable)
            {
                type = TypeHelper.GetEnumberableType(type);
            }

            if (type == typeof(TextContentViewModel))
            {
                var folderIdKey = setting.ParameterBinding.Keys.FirstOrDefault(Isfolderid);
                if (!String.IsNullOrEmpty(folderIdKey) && setting.ParameterBinding.TryGetValue(folderIdKey, out var binding))
                {
                    if (Guid.TryParse(binding.Binding, out var folderId))
                    {
                        var folder = siteDb.ContentFolders.Get(folderId);
                        if (folder != null)
                        {
                            model.ItemFields = GetContentTypeField(siteDb, folder.ContentTypeId);

                            foreach (var item in folder.Category)
                            {
                                var catfolder = siteDb.ContentFolders.Get(item.FolderId);
                                if (catfolder != null)
                                {
                                    TypeFieldModel fieldmodel = new TypeFieldModel
                                    {
                                        Name = item.Alias,
                                        Fields = GetContentTypeField(siteDb, catfolder.ContentTypeId),
                                        IsComplexType = true,
                                        Enumerable = item.Multiple
                                    };
                                    // catfolder.Name;
                                    model.ItemFields.Add(fieldmodel);
                                }
                            }

                            foreach (var item in folder.Embedded)
                            {
                                var embedfolder = siteDb.ContentFolders.Get(item.FolderId);
                                if (embedfolder != null)
                                {
                                    TypeFieldModel fieldmodel = new TypeFieldModel
                                    {
                                        Name = item.Alias,
                                        Fields = GetContentTypeField(siteDb, embedfolder.ContentTypeId),
                                        IsComplexType = true,
                                        Enumerable = true
                                    };
                                    //embedfolder.Name;
                                    model.ItemFields.Add(fieldmodel);
                                }
                            }
                        }
                    }
                }
            }

            //else if (type == typeof(Kooboo.Sites.Ecommerce.ViewModel.ProductViewModel))
            //{
            //    var alltype = siteDb.ProductType.All().Select(o=>o.Id).ToList();

            //    model.ItemFields = GetProductTypeField(siteDb, alltype);

            //}
            else if (type == typeof(Data.Definition.IJson))
            {
                // get sample and return.
                var sample = _GetSampleData(setting);
                if (!string.IsNullOrEmpty(sample))
                {
                    model.ItemFields = GetJsonTypeFields(sample);
                }
            }
            else if (type == typeof(Data.Definition.IXml))
            {
                var sample = _GetSampleData(setting);
                if (!string.IsNullOrEmpty(sample))
                {
                    model.ItemFields = GetXmlTypeFields(sample);
                }
            }
            else
            {
                model.ItemFields = GetTypeFields(type);
            }
            return model;
        }

        private static string _GetSampleData(IDataMethodSetting setting)
        {
            string sample = null;
            foreach (var item in setting.ParameterBinding)
            {
                if (item.Key.ToLower() == Kooboo.Sites.DataSources.ScriptSourceManager.SampleResponseFieldName.ToLower())
                {
                    sample = item.Value.Binding;
                }
            }

            if (sample == null)
            {
                foreach (var item in setting.ParameterBinding)
                {
                    if (item.Key.ToLower().Contains("url"))
                    {
                        sample = Lib.Helper.HttpHelper.Get<string>(item.Value.Binding);
                    }
                }
            }
            return sample;
        }

        private static bool Isfolderid(string bindingkey)
        {
            if (string.IsNullOrEmpty(bindingkey))
            {
                return false;
            }
            if (bindingkey.ToLower().Contains("folderid"))
            {
                return true;
            }
            return false;
        }

        private static List<TypeFieldModel> GetTypeFields(Type type)
        {
            List<TypeFieldModel> fields = new List<TypeFieldModel>();

            foreach (var item in TypeHelper.GetPublicFieldOrProperties(type))
            {
                var subType = item.Value;
                var field = new TypeFieldModel
                {
                    Name = item.Key,
                    Type = subType.FullName,
                    Enumerable = TypeHelper.IsGenericCollection(subType),
                    IsComplexType = TypeHelper.IsComplexType(subType)
                };

                if (field.IsComplexType)
                {
                    field.Fields = GetTypeFields(subType);
                }

                fields.Add(field);
            }
            return fields;
        }

        internal static List<TypeFieldModel> GetJsonTypeFields(string json)
        {
            try
            {
                var jsonobject = (JObject)Lib.Helper.JsonHelper.Deserialize(json);
                if (jsonobject != null)
                {
                    return _GetJsonTypeInfo(jsonobject);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return new List<TypeFieldModel>();
        }

        private static List<TypeFieldModel> _GetJsonTypeInfo(JObject jsonobject)
        {
            List<TypeFieldModel> fields = new List<TypeFieldModel>();

            foreach (var item in jsonobject.Properties())
            {
                bool isComplex = false;
                bool isCollection = item.Value is JArray;
                if (isCollection)
                {
                    var array = item.Value as JArray;
                    if (array.Any())
                    {
                        var itemone = array.First();
                        isComplex = itemone is JObject;
                    }
                }
                else
                {
                    isComplex = item.Value is JObject;
                }
                var field = new TypeFieldModel
                {
                    Name = item.Name,
                    Type = typeof(string).FullName,
                    Enumerable = isCollection,
                    IsComplexType = isComplex
                };

                if (field.IsComplexType)
                {
                    var value = item.Value;
                    field.Type = typeof(object).FullName;

                    if (isCollection)
                    {
                        var array = item.Value as JArray;
                        if (array.Any())
                        {
                            var itemone = array.First();
                            field.Fields = _GetJsonTypeInfo(itemone as JObject);
                        }
                    }
                    else
                    {
                        field.Fields = _GetJsonTypeInfo(item.Value as JObject);
                    }
                }
                fields.Add(field);
            }
            return fields;
        }

        internal static List<TypeFieldModel> GetXmlTypeFields(string xml)
        {
            var xdoc = Lib.Helper.XmlHelper.DeSerialize(xml);
            if (xdoc?.Root != null)
            {
                return _GetXmlTypeInfo(xdoc.Root);
            }
            return new List<New.Models.TypeFieldModel>();
        }

        private static List<string> _xmldatatypes;

        private static List<string> XmlDataType
        {
            get
            {
                return _xmldatatypes ?? (_xmldatatypes = new List<string>
                {
                    "string",
                    "boolean",
                    "decimal",
                    "float",
                    "double",
                    "dateTime",
                    "date",
                    "time",
                    "integer"
                });
            }
        }

        private static bool IsXmlCollection(XElement el)
        {
            var subs = el.Nodes();
            if (subs == null || subs.Count() == 0 || subs.Count() == 1)
            {
                return false;
            }
            string itemname = null;
            foreach (var item in subs)
            {
                var itemel = item as XElement;
                if (itemel == null)
                {
                    return false;
                }
                if (itemname == null)
                {
                    itemname = SubNames(itemel).ToLower();
                }
                else
                {
                    if (itemname != SubNames(itemel).ToLower())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool IsXmlComplexType(XElement el)
        {
            if (el == null)
            {
                return false;
            }
            var subnodes = el.Nodes();
            if (subnodes.Count() == 0)
            {
                return false;
            }
            string name = null;
            foreach (var item in subnodes)
            {
                var itemel = item as XElement;
                if (itemel == null)
                {
                    continue;
                }
                if (name == null)
                {
                    name = itemel.Name.ToString().ToLower();
                    if (!XmlDataType.Contains(name))
                    {
                        return true;
                    }
                }
                else
                {
                    string newname = itemel.Name.ToString().ToLower();
                    if (name != newname)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string SubNames(XElement el)
        {
            string name = el.Name.ToString();
            foreach (var item in el.Nodes())
            {
                if (item is XElement itemel)
                {
                    name += itemel.Name.ToString();
                }
            }
            return name;
        }

        private static bool IsXmlPrimaryDataType(string name)
        {
            return XmlDataType.Contains(name);
        }

        private static List<TypeFieldModel> _GetXmlTypeInfo(XElement el)
        {
            List<TypeFieldModel> fields = new List<TypeFieldModel>();
            foreach (var item in el.Nodes())
            {
                var elitem = item as XElement;

                bool isComplex = false;
                bool isCollection = IsXmlCollection(elitem);
                isComplex = isCollection ? IsXmlComplexType(elitem?.Nodes().First() as XElement) : IsXmlComplexType(elitem);
                var field = new TypeFieldModel
                {
                    Name = elitem?.Name.ToString(),
                    Type = typeof(string).FullName,
                    Enumerable = isCollection,
                    IsComplexType = isComplex
                };

                if (isComplex)
                {
                    field.Type = typeof(object).FullName;

                    field.Fields = isCollection ? _GetXmlTypeInfo(elitem?.Nodes().First() as XElement) : _GetXmlTypeInfo(elitem);
                }
                fields.Add(field);
            }
            return fields;
        }

        public static List<TypeFieldModel> GetContentTypeField(SiteDb sitedb, Guid contentTypeId)
        {
            List<TypeFieldModel> result = new List<TypeFieldModel>();
            var contentType = sitedb.ContentTypes.Get(contentTypeId);
            if (contentType != null)
            {
                foreach (var item in contentType.Properties)
                {
                    if (!item.IsSystemField)
                    {
                        TypeFieldModel model = new TypeFieldModel {Name = item.Name, IsComplexType = false};
                        if (item.MultipleValue)
                        {
                            model.Enumerable = true;
                        }
                        model.Type = item.DataType.ToString();
                        result.Add(model);
                    }
                }
            }
            return result;
        }

        //public static List<TypeFieldModel> GetProductTypeField(SiteDb sitedb, List<Guid> ProductTypeIds)
        //{
        //    List<TypeFieldModel> result = new List<TypeFieldModel>();

        //    foreach (var producttypeid in ProductTypeIds)
        //    {
        //        var ProductType = sitedb.ProductType.Get(producttypeid);
        //        if (ProductType != null)
        //        {
        //            foreach (var item in ProductType.Properties)
        //            {
        //                if (!item.IsSystemField)
        //                {
        //                    var find = result.Find(o => o.Name == item.Name);

        //                    if (find == null)
        //                    {
        //                        TypeFieldModel model = new TypeFieldModel();
        //                        model.Name = item.Name;
        //                        model.IsComplexType = false;
        //                        if (item.MultipleValue)
        //                        {
        //                            model.Enumerable = true;
        //                        }
        //                        model.Type = item.DataType.ToString();
        //                        result.Add(model);
        //                    }
        //                }
        //            }

        //        }

        //    }

        //    return result;
        //}

        public static Type ReturnType(MethodInfo methodInfo)
        {
            var defineType = methodInfo.GetCustomAttribute(typeof(Attributes.ReturnType));
            if (defineType is ReturnType definereturn && definereturn.Type != null)
            {
                return definereturn.Type;
            }
            return methodInfo.ReturnType;
        }

        private static bool IsPagedResult(MethodInfo methodInfo)
        {
            return methodInfo.ReturnType == typeof(Kooboo.Data.Models.PagedResult);
        }
    }
}