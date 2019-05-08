//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data;
using Kooboo.Data.Models;
using System.Reflection;
using Kooboo.Extensions;
using Kooboo.Data.Interface;
using Kooboo.Sites.Repository;
using Kooboo.Sites.DataSources.New.Models;
using Kooboo.Lib.Reflection;
using Kooboo.Sites.ViewModel;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace Kooboo.Sites.DataSources
{
    public static class DataSourceHelper
    {
        public static void InitIDataSource()
        {
            var DataMethodStore = GlobalDb.DataMethodSettings;
            // check all types have been loaded into system... 
            var AllCurrentSettings = DataMethodStore.All();
            foreach (var TypeItem in AllCurrentSettings.GroupBy(o => o.DeclareType))
            {
                var type = Kooboo.Data.TypeCache.GetType(TypeItem.Key);
                if (type == null)
                {
                    foreach (var item in TypeItem)             
                    {
                        DataMethodStore.Delete(item);
                    }
                }
            }

            var alltypes = AssemblyLoader.LoadTypeByInterface(typeof(IDataSource));

            foreach (var item in alltypes)
            {
                Guid typehash = item.FullName.ToHashGuid();

                var oldsetting = DataMethodStore.Query.Where(o => o.DeclareTypeHash == typehash).SelectAll();

                var newsettings = GetDefaultMethodSettings(item, false);

                foreach (var MethodItem in oldsetting)
                {
                    if (!newsettings.Any(o => o.MethodSignatureHash == MethodItem.MethodSignatureHash))
                    {
                        DataMethodStore.Delete(MethodItem.Id);
                    }
                }

                foreach (var MethodItem in newsettings)
                {
                    DataMethodStore.AddOrUpdate(MethodItem);
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
                ParameterBinding binding = new ParameterBinding();
                binding.FullTypeName = typeof(System.Guid).Name;
                binding.Binding = default(Guid).ToString();
                binding.IsContentFolder = true;
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
                ParameterBinding binding = new ParameterBinding();
                binding.FullTypeName = typeof(System.Guid).Name;
                binding.Binding = default(Guid).ToString();
                binding.IsProductType = true;
                currentBinding.Add("ProductTypeId", binding);
            }
        }





        public static Dictionary<string, string> GetMethodParametes(MethodInfo method)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            foreach (var item in method.GetParameters())
            {
                Parameters.Add(item.Name, item.ParameterType.FullName);
            }
            return Parameters;
        }

        public static Dictionary<string, ParameterBinding> GetDefaultBinding(Dictionary<string, string> Parameters)
        {
            Dictionary<string, ParameterBinding> Bindings = new Dictionary<string, ParameterBinding>();

            foreach (var item in Parameters)
            {
                Type itemtype = Kooboo.Data.TypeCache.GetType(item.Value);
                ParameterBinding binding = new ParameterBinding();
                binding.FullTypeName = itemtype.FullName;

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
                    string ClassName = itemtype.Name;
                    binding.Binding = "{" + ClassName + "}";
                    binding.IsClass = true;

                    foreach (var FieldItem in Lib.Reflection.TypeHelper.GetPublicFieldOrProperties(itemtype))
                    {
                        var subitems = GetSubBinding(FieldItem.Key, FieldItem.Value);
                        foreach (var subitem in subitems)
                        {
                            string key = item.Key + "." + subitem.Key;
                            var value = subitem.Value;
                            if (!value.IsCollection && !value.IsDictionary)
                            {
                                if (!string.IsNullOrEmpty(value.Binding) && !value.Binding.Contains("."))
                                {
                                    value.Binding = "{" + ClassName + "." + value.Binding + "}";
                                }
                            }
                            value.IsContentFolder = IsContentFolder(subitem.Key);
                            value.IsProductType = IsProductType(subitem.Key); 
                            value.IsData = IsData(subitem.Key);
                            value.IsOrderBy = IsOrderBy(subitem.Key);
                            Bindings.Add(key, value);
                        }
                    }
                }
                else
                {
                    continue;
                }

                Bindings.Add(item.Key, binding);
            }
            return Bindings;
        }

        private static bool IsContentFolder(string name)
        {
            return name.ToLower() == "folder" || name.ToLower() == "folderid";
        }

        private static bool IsProductType(string name)
        {
            return name.ToLower() == "producttype" || name.ToLower() == "producttypeid";
        }

        private static bool IsData(string Name)
        {
            return Name.ToLower().Contains("sample");
        }

        private static bool IsOrderBy(string name)
        {
            var lower = name.ToLower();

            return lower.Contains("sortfield") || lower.Contains("orderby");
        }

        private static Dictionary<string, ParameterBinding> GetSubBinding(string FieldName, Type FieldType)
        {

            Dictionary<string, ParameterBinding> bindings = new Dictionary<string, ParameterBinding>();

            if (TypeHelper.IsFieldType(FieldType))
            {
                ParameterBinding binding = new ParameterBinding();
                binding.Binding = FieldName;
                binding.FullTypeName = FieldType.FullName;
                bindings.Add(FieldName, binding);
            }
            else if (TypeHelper.IsDictionary(FieldType))
            {
                ParameterBinding binding = new ParameterBinding();
                binding.FullTypeName = FieldType.FullName;
                binding.IsDictionary = true;
                binding.KeyType = Lib.Reflection.TypeHelper.GetDictionaryKeyType(FieldType).FullName;
                binding.ValueType = Lib.Reflection.TypeHelper.GetDictionaryValueType(FieldType).FullName;
                bindings.Add(FieldName, binding);
            }

            else if (TypeHelper.IsGenericCollection(FieldType))
            {
                ParameterBinding binding = new ParameterBinding();
                binding.FullTypeName = FieldType.FullName;
                binding.IsCollection = true;
                binding.IsContentFolder = IsContentFolder(FieldName);
                binding.IsProductType = IsProductType(FieldName); 
                binding.KeyType = Lib.Reflection.TypeHelper.GetEnumberableType(FieldType).FullName;
                bindings.Add(FieldName, binding);
            }
            else if (FieldType.IsClass)
            {
                string classname = FieldType.Name;

                ParameterBinding binding = new ParameterBinding();
                binding.Binding = FieldName;
                binding.FullTypeName = FieldType.FullName;
                bindings.Add(FieldName, binding);

                foreach (var FieldItem in Lib.Reflection.TypeHelper.GetPublicFieldOrProperties(FieldType))
                {
                    var dict = GetSubBinding(FieldItem.Key, FieldItem.Value);

                    foreach (var item in dict)
                    {
                        if (!string.IsNullOrEmpty(item.Value.Binding) && !item.Value.Binding.Contains("."))
                        {
                            item.Value.Binding = classname + "." + item.Value.Binding;
                        }
                        bindings.Add(FieldName + "." + item.Key, item.Value);
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
            List<int> CapitalLetterPosition = new List<int>();

            for (int i = 1; i < input.Length; i++)
            {
                if (Lib.Helper.CharHelper.isUppercaseAscii(input[i]))
                {
                    CapitalLetterPosition.Add(i);
                }
            }

            if (CapitalLetterPosition.Count == 1)
            {
                int position = CapitalLetterPosition[0];
                return input.Substring(0, position) + "." + input.Substring(position);
            }

            return input;
        }

        public static TypeInfoModel GetFields(SiteDb SiteDb, Guid MethoId)
        {
            var methodsetting = SiteDb.DataMethodSettings.Get(MethoId);
            if (methodsetting == null)
            {
                methodsetting = GlobalDb.DataMethodSettings.Get(MethoId);
            }

            if (methodsetting != null)
            {
                return GetFields(SiteDb, methodsetting);
            }
            return null;
        }
        public static TypeInfoModel GetFields(SiteDb siteDb, IDataMethodSetting setting)
        {
            var model = new TypeInfoModel();
            model.Id = setting.Id;
            model.Name = setting.MethodName;
            model.DeclareType = setting.DeclareType;
            model.ItemFields = new List<TypeFieldModel>();
            model.IsPublic = setting.IsPublic;

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
                var folderIdKey = setting.ParameterBinding.Keys.FirstOrDefault(it => Isfolderid(it));
                ParameterBinding binding;
                if (!String.IsNullOrEmpty(folderIdKey) && setting.ParameterBinding.TryGetValue(folderIdKey, out binding))
                {
                    Guid folderId;
                    if (Guid.TryParse(binding.Binding, out folderId))
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
                                    TypeFieldModel fieldmodel = new TypeFieldModel();
                                    fieldmodel.Name = item.Alias;// catfolder.Name;
                                    fieldmodel.Fields = GetContentTypeField(siteDb, catfolder.ContentTypeId);
                                    fieldmodel.IsComplexType = true;
                                    fieldmodel.Enumerable = item.Multiple;
                                    model.ItemFields.Add(fieldmodel);
                                }
                            }

                            foreach (var item in folder.Embedded)
                            {
                                var embedfolder = siteDb.ContentFolders.Get(item.FolderId);
                                if (embedfolder != null)
                                {
                                    TypeFieldModel fieldmodel = new TypeFieldModel();
                                    fieldmodel.Name = item.Alias;  //embedfolder.Name;
                                    fieldmodel.Fields = GetContentTypeField(siteDb, embedfolder.ContentTypeId);
                                    fieldmodel.IsComplexType = true;
                                    fieldmodel.Enumerable = true;
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


        internal static List<TypeFieldModel> GetJsonTypeFields(string Json)
        {
            try
            {
                var jsonobject = (JObject)Lib.Helper.JsonHelper.Deserialize(Json);
                if (jsonobject != null)
                {
                    return _GetJsonTypeInfo(jsonobject);
                }
            }
            catch (Exception ex)
            {

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
            if (xdoc != null && xdoc.Root != null)
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
                if (_xmldatatypes == null)
                {
                    _xmldatatypes = new List<string>();
                    _xmldatatypes.Add("string");
                    _xmldatatypes.Add("boolean");
                    _xmldatatypes.Add("decimal");
                    _xmldatatypes.Add("float");
                    _xmldatatypes.Add("double");
                    _xmldatatypes.Add("dateTime");
                    _xmldatatypes.Add("date");
                    _xmldatatypes.Add("time");
                    _xmldatatypes.Add("integer");
                }
                return _xmldatatypes;
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
                var itemel = item as XElement;
                if (itemel != null)
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
                if (isCollection)
                {
                    isComplex = IsXmlComplexType(elitem.Nodes().First() as XElement);
                }
                else
                {
                    isComplex = IsXmlComplexType(elitem);
                }
                var field = new TypeFieldModel
                {
                    Name = elitem.Name.ToString(),
                    Type = typeof(string).FullName,
                    Enumerable = isCollection,
                    IsComplexType = isComplex
                };

                if (isComplex)
                {
                    field.Type = typeof(object).FullName;

                    if (isCollection)
                    {
                        field.Fields = _GetXmlTypeInfo(elitem.Nodes().First() as XElement);
                    }
                    else
                    {
                        field.Fields = _GetXmlTypeInfo(elitem as XElement);
                    }
                }
                fields.Add(field);
            }
            return fields;
        }
         
        public static List<TypeFieldModel> GetContentTypeField(SiteDb sitedb, Guid ContentTypeId)
        {
            List<TypeFieldModel> result = new List<TypeFieldModel>();
            var contentType = sitedb.ContentTypes.Get(ContentTypeId);
            if (contentType != null)
            {
                foreach (var item in contentType.Properties)
                {
                    if (!item.IsSystemField)
                    {
                        TypeFieldModel model = new TypeFieldModel();
                        model.Name = item.Name;
                        model.IsComplexType = false;
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




        public static Type ReturnType(MethodInfo MethodInfo)
        {
            var defineType = MethodInfo.GetCustomAttribute(typeof(Attributes.ReturnType));
            if (defineType != null)
            {
                var definereturn = defineType as Attributes.ReturnType;
                if (definereturn != null && definereturn.Type != null)
                {
                    return definereturn.Type;
                }
            }
            return MethodInfo.ReturnType;
        }

        private static bool IsPagedResult(MethodInfo MethodInfo)
        {
            return MethodInfo.ReturnType == typeof(Kooboo.Data.Models.PagedResult);
        }
    }

}
