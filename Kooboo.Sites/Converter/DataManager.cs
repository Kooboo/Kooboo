//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Contents.Models;
using Newtonsoft.Json.Linq;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Models;
using Kooboo.Data.Definition;

namespace Kooboo.Sites.Converter
{
    public class DataManager
    { 
        /// <summary>
        /// Add data and return the folder id. 
        /// </summary>
        /// <param name="SiteDb"></param>
        /// <param name="TypeAndFolderName"></param>
        /// <param name="JsonData"></param>
        /// <returns></returns>
        public static DataAddResponse AddData(SiteDb SiteDb, string TypeAndFolderName, Object JsonData)
        {
            var Fields = ConvertToDataFields(JsonData);
        
            string ContentTypeName = GetContentTypeName(SiteDb, TypeAndFolderName);
            string ContentFolderName = GetContentFolderName(SiteDb, TypeAndFolderName);

            var contenttype = GetContentType(ContentTypeName, Fields);
            SiteDb.ContentTypes.AddOrUpdate(contenttype);

            var contentfolder = new ContentFolder();
            contentfolder.Name = ContentFolderName;
            contentfolder.ContentTypeId = contenttype.Id; 
            SiteDb.ContentFolders.AddOrUpdate(contentfolder);

            AddValue(SiteDb, contenttype.Id, contentfolder.Id, Fields);

            DataAddResponse response = new DataAddResponse();
            response.contentFolder = contentfolder;

            foreach (var item in Fields)
            {
                if (item.ControlType == ControlTypes.DateTime)
                {
                    var datefield = new DateField();
                    datefield.Name = item.Name;
                    datefield.Format = Lib.Helper.DateTimeHelper.ParseDateFormat(item.Values);
                    response.DateList.Add(datefield); 
                }
            }
            
            return response; 
        }
          
        public static void AddValue(SiteDb SiteDb, Guid ContentTypeId, Guid FolderId,  List<DataField> Fields, Guid ParentContentId=default(Guid))
        {
            int maxi = 0;
            foreach (var item in Fields)
            {
                if (item.Values.Count() > maxi)
                {
                    maxi = item.Values.Count();
                }
            }
            if (maxi == 0) return;

            for (int i = maxi-1; i>=0 ; i--)
            {
                TextContent content = new TextContent(); 
                foreach (var item in Fields)
                {
                   ///var value = GetValueByControlType(item.Values[i], item.ControlType); 
                    content.SetValue(item.Name, item.Values[i],SiteDb.WebSite.DefaultCulture);
                }

                content.ContentTypeId = ContentTypeId;
                content.FolderId = FolderId;
                content.ParentId = ParentContentId; 

                SiteDb.TextContent.AddOrUpdate(content); 
            } 

        }

        public static ViewDataMethod AddGetContentListDataMethod(SiteDb SiteDb, Guid ViewId, Guid FolderId, string AliasName)
        {  
            var contentdatasorcetype = typeof(Kooboo.Sites.DataSources.ContentList);

            var method = Kooboo.Data.GlobalDb.DataMethodSettings.TableScan.Where(o => o.DeclareType == contentdatasorcetype.FullName).SelectAll().Where(o => o.MethodName == "ByFolder").First();
             
            var binding = method.ParameterBinding["FolderId"];
            binding.Binding = FolderId.ToString();
            method.MethodName = System.Guid.NewGuid().ToString();
            method.Id = default(Guid); 
            method.IsPublic = false;
            SiteDb.DataMethodSettings.AddOrUpdate(method);

            ViewDataMethod viewmethod = new ViewDataMethod();
            viewmethod.AliasName = "List";
            viewmethod.Name = "List";
            viewmethod.MethodId = method.Id;
            viewmethod.ViewId = ViewId; 
            SiteDb.ViewDataMethods.AddOrUpdate(viewmethod); 
            return viewmethod; 
        }
          
        private static string GetContentTypeName(SiteDb SiteDb, string Name)
        {
            if (!SiteDb.ContentTypes.IsNameExists(Name))
            {
                return Name;
            }

            for (int i = 1; i < 999; i++)
            {
                Name = Name + i.ToString();
                if (!SiteDb.ContentTypes.IsNameExists(Name))
                {
                    return Name;
                }
            }

            return null;
        }

        private static string GetContentFolderName(SiteDb SiteDb, string Name)
        {
            if (!SiteDb.ContentFolders.IsFolderNameExists(Name))
            {
                return Name;
            }

            for (int i = 1; i < 999; i++)
            {
                Name = Name + i.ToString();
                if (!SiteDb.ContentFolders.IsFolderNameExists(Name))
                {
                    return Name;
                }
            }

            return null;

        }

        internal static List<DataField> ConvertToDataFields(Object Data)
        {   
            var dictvalues = ConvertToFieldValues(Data);  
            List<DataField> result = new List<DataField>();

            foreach (var item in dictvalues)
            {
                DataField field = new DataField();
                field.Name = item.Key;
                field.Values = item.Value;
                field.ControlType = EvaluateControlType(field.Values);
                result.Add(field); 
            }
         
            return result;
        } 
 
        public static Dictionary<string, List<string>> ConvertToFieldValues(Object JsonData)
        {
            var jobject = (JObject)JsonData;
             
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
         
        internal static ContentType GetContentType(string ContentTypeName, List<DataField> Fields)
        {
            ContentType contenttype = new  ContentType();

            contenttype.Name = ContentTypeName; 
  
            foreach (var item in Fields)
            {
                ContentProperty property = new ContentProperty();
                property.Name = item.Name;
                property.ControlType = item.ControlType;
                property.Editable = true; 
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
            bool parsevalue;
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
                    if (!bool.TryParse(item, out parsevalue))
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
            DateTime parsevalue;
 
            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item))
                { 
                    if (!DateTime.TryParse(item, out parsevalue))
                    {
                        return false;
                    } 
                }
            }
            return true;
        }

        private static bool IsInt(List<string> values)
        {
            int parsevalue;

            foreach (var item in values)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (!int.TryParse(item, out parsevalue))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsLongText(List<string> Values)
        {
            foreach (var item in Values)
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
