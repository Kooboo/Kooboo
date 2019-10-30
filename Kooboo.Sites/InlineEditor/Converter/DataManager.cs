//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data;

namespace Kooboo.Sites.InlineEditor.Converter
{
    public class DataManager
    {
        /// <summary>
        /// Add data and return the folder id.
        /// </summary>
        /// <param name="siteDb"></param>
        /// <param name="typeAndFolderName"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static DataAddResponse AddData(SiteDb siteDb, string typeAndFolderName, Object jsonData)
        {
            var fields = ConvertToDataFields(jsonData);

            string contentTypeName = GetContentTypeName(siteDb, typeAndFolderName);
            string contentFolderName = GetContentFolderName(siteDb, typeAndFolderName);

            var contenttype = GetContentType(contentTypeName, fields);
            siteDb.ContentTypes.AddOrUpdate(contenttype);

            var contentfolder = new ContentFolder {Name = contentFolderName, ContentTypeId = contenttype.Id};
            siteDb.ContentFolders.AddOrUpdate(contentfolder);

            AddValue(siteDb, contenttype.Id, contentfolder.Id, fields);

            DataAddResponse response = new DataAddResponse {contentFolder = contentfolder};

            foreach (var item in fields)
            {
                if (item.ControlType == ControlTypes.DateTime)
                {
                    var datefield = new DateField
                    {
                        Name = item.Name, Format = Lib.Helper.DateTimeHelper.ParseDateFormat(item.Values)
                    };
                    response.DateList.Add(datefield);
                }
            }

            return response;
        }

        public static void AddValue(SiteDb siteDb, Guid contentTypeId, Guid folderId, List<DataField> fields, Guid parentContentId = default(Guid))
        {
            int maxi = 0;
            foreach (var item in fields)
            {
                if (item.Values.Count() > maxi)
                {
                    maxi = item.Values.Count();
                }
            }
            if (maxi == 0) return;

            for (int i = maxi - 1; i >= 0; i--)
            {
                TextContent content = new TextContent();
                foreach (var item in fields)
                {
                    //var value = GetValueByControlType(item.Values[i], item.ControlType);
                    content.SetValue(item.Name, item.Values[i], siteDb.WebSite.DefaultCulture);
                }

                content.ContentTypeId = contentTypeId;
                content.FolderId = folderId;
                content.ParentId = parentContentId;

                siteDb.TextContent.AddOrUpdate(content);
            }
        }

        public static ViewDataMethod AddGetContentListDataMethod(SiteDb siteDb, Guid viewId, Guid folderId, string aliasName)
        {
            var contentdatasorcetype = typeof(Kooboo.Sites.DataSources.ContentList);

            var method = GlobalDb.DataMethodSettings.TableScan.Where(o => o.DeclareType == contentdatasorcetype.FullName).SelectAll().First(o => o.MethodName == "ByFolder");

            var binding = method.ParameterBinding["FolderId"];
            binding.Binding = folderId.ToString();
            method.MethodName = System.Guid.NewGuid().ToString();
            method.Id = default(Guid);
            method.IsPublic = false;
            siteDb.DataMethodSettings.AddOrUpdate(method);

            ViewDataMethod viewmethod = new ViewDataMethod
            {
                AliasName = "List", Name = "List", MethodId = method.Id, ViewId = viewId
            };
            siteDb.ViewDataMethods.AddOrUpdate(viewmethod);
            return viewmethod;
        }

        private static string GetContentTypeName(SiteDb siteDb, string name)
        {
            if (!siteDb.ContentTypes.IsNameExists(name))
            {
                return name;
            }

            for (int i = 1; i < 999; i++)
            {
                name = name + i.ToString();
                if (!siteDb.ContentTypes.IsNameExists(name))
                {
                    return name;
                }
            }

            return null;
        }

        private static string GetContentFolderName(SiteDb siteDb, string name)
        {
            if (!siteDb.ContentFolders.IsFolderNameExists(name))
            {
                return name;
            }

            for (int i = 1; i < 999; i++)
            {
                name += i.ToString();
                if (!siteDb.ContentFolders.IsFolderNameExists(name))
                {
                    return name;
                }
            }

            return null;
        }

        internal static List<DataField> ConvertToDataFields(Object data)
        {
            var dictvalues = ConvertToFieldValues(data);
            List<DataField> result = new List<DataField>();

            foreach (var item in dictvalues)
            {
                DataField field = new DataField {Name = item.Key, Values = item.Value};
                field.ControlType = EvaluateControlType(field.Values);
                result.Add(field);
            }

            return result;
        }

        public static Dictionary<string, List<string>> ConvertToFieldValues(Object jsonData)
        {
            var jobject = (JObject)jsonData;

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            var values = jobject.Values().ToList();
            int valuecount = 0;

            foreach (var item in jobject.Values().ToList())
            {
                var jitem = (JObject)item;
                var plist = jitem.Properties().ToList();
                foreach (var pitem in plist)
                {
                    var name = pitem.Name;
                    var value = pitem.Value.ToString();

                    if (!string.IsNullOrEmpty(name))
                    {
                        List<string> resultvalues;
                        if (result.ContainsKey(name))
                        {
                            resultvalues = result[name];
                        }
                        else
                        {
                            resultvalues = new List<string>();
                            for (int i = 0; i < valuecount; i++)
                            {
                                resultvalues.Add(string.Empty);
                            }
                            result[name] = resultvalues;
                        }

                        resultvalues.Add(value);
                    }
                }
                valuecount += 1;
            }

            return result;
        }

        internal static ContentType GetContentType(string contentTypeName, List<DataField> fields)
        {
            ContentType contenttype = new ContentType {Name = contentTypeName};


            foreach (var item in fields)
            {
                ContentProperty property = new ContentProperty
                {
                    Name = item.Name, ControlType = item.ControlType, Editable = true
                };
                contenttype.Properties.Add(property);
            }
            return contenttype;
        }

        public static string EvaluateControlType(List<string> values)
        {
            if (IsNullColumn(values))
            {
                return ControlTypes.TextBox;
            }
            else if (IsDateTime(values))
            {
                return ControlTypes.DateTime;
            }
            else if (IsInt(values))
            {
                return ControlTypes.Number;
            }
            else if (IsBool(values))
            {
                return ControlTypes.Checkbox;
            }
            else if (IsImageUrl(values))
            {
                return ControlTypes.MediaFile;
            }
            else if (IsLongText(values))
            {
                return ControlTypes.TextArea;
            }

            return ControlTypes.TextBox;
        }

        private static bool IsBool(List<string> values)
        {
            bool result = false;
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (item.ToLower() == "yes" || item.ToLower() == "no")
                    {
                        result = true;
                        continue;
                    }

                    if (item.ToLower() == "true" || item.ToLower() == "false")
                    {
                        result = true;
                        continue;
                    }
                    if (!bool.TryParse(item, out _))
                    {
                        return false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private static bool IsNullColumn(List<string> values)
        {
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsDateTime(List<string> values)
        {
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (!DateTime.TryParse(item, out _))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsInt(List<string> values)
        {
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (!int.TryParse(item, out _))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsLongText(List<string> values)
        {
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item) && (item.Contains("\r\n") || item.Length > 150))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsImageUrl(List<string> values)
        {
            foreach (var item in values)
            {
                if (!Lib.Helper.UrlHelper.IsValidUrl(item))
                {
                    return false;
                }

                var type = Kooboo.Lib.Helper.UrlHelper.GetFileType(item);
                if (type != Lib.Helper.UrlHelper.UrlFileType.Image)
                {
                    return false;
                }
            }
            return true;
        }
    }
}