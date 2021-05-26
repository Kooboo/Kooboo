//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using Kooboo.Sites.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Contents.Models;
using System.Collections.Generic;
using Kooboo.Sites.Repository;
using Kooboo.Data.Interface;
using System.Text;
using Kooboo.IndexedDB.Serializer.Simple;
using System.Collections;
using System.Reflection;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.Sites.Service
{
    public static class ObjectService
    {
        public static IByteConverter<Guid> KeyConverter = new Kooboo.IndexedDB.ByteConverter.GuidConverter();

        /// <summary>
        /// Get the object information for display.
        /// If it is the declaration or embedded, get the owner object... 
        /// </summary>
        /// <param name="SiteDb"></param>
        /// <param name="ObjectId"></param>
        /// <param name="ConstType"></param>
        /// <returns></returns>
        public static ObjectInfo GetObjectInfo(SiteDb SiteDb, Guid ObjectId, byte ConstType)
        {
            var siteobject = GetSiteObject(SiteDb, ObjectId, ConstType);
            if (siteobject == null)
            {
                return null;
            }
            return GetObjectInfo(SiteDb, siteobject);

        }

        public static long GetSize(ISiteObject siteobject)
        {
            if (siteobject is IBinaryFile)
            {
                return ((IBinaryFile)siteobject).Size;
            }
            else
            {
                var json = Lib.Helper.JsonHelper.Serialize(siteobject);
                return json.Length;
            }
        }

        public static ObjectInfo GetObjectInfo(SiteDb SiteDb, ISiteObject siteobject)
        {
            if (siteobject == null)
            {
                return null;
            }
            ObjectInfo info = new ObjectInfo();
            info.ObjectId = siteobject.Id;
            info.ConstType = siteobject.ConstType;

            if (info.ConstType > 0)
            {
                info.ModelType = ConstTypeService.GetModelType(siteobject.ConstType);
            }

            if (info.ModelType == null)
            {
                info.ModelType = siteobject.GetType();
            }

            if (siteobject is IBinaryFile)
            {
                info.Size = ((IBinaryFile)siteobject).Size;
            }
            else if (siteobject is ITextObject)
            {
                info.Size = (((ITextObject)siteobject).Body ?? "").Length;
            }


            if (Kooboo.Lib.Reflection.TypeHelper.HasInterface(info.ModelType, typeof(IEmbeddable)))
            {
                var embeddable = siteobject as IEmbeddable;
                if (embeddable.IsEmbedded)
                {
                    return GetObjectInfo(SiteDb, embeddable.OwnerObjectId, embeddable.OwnerConstType);
                }
            }

            if (Kooboo.Sites.Service.ObjectService.IsRoutable(siteobject, false))
            {
                info.Url = GetObjectRelativeUrl(SiteDb, siteobject as SiteObject);
                info.DisplayName = info.Url;
                info.Name = siteobject.Name;
                return info;
            }
            else
            {


                if (info.ModelType == typeof(CmsCssRule))
                {
                    var rule = siteobject as CmsCssRule;
                    if (rule == null)
                    {
                        return null;
                    }
                    if (rule.IsInline)
                    {
                        return GetObjectInfo(SiteDb, rule.OwnerObjectId, rule.OwnerObjectConstType);
                    }
                    else
                    {
                        return GetObjectInfo(SiteDb, rule.ParentStyleId, ConstObjectType.Style);
                    }
                }

                info.Url = "/__kb/" + info.ModelType.Name + "/" + info.ObjectId.ToString();
                info.DisplayName = siteobject.Name;
                info.Name = siteobject.Name;

                if (info.ModelType == typeof(TextContent))
                {
                    info.Name = Kooboo.Sites.Helper.ContentHelper.GetSummary(siteobject as TextContent, SiteDb.WebSite.DefaultCulture);
                    info.DisplayName = info.Name;
                }

                if (info.ModelType == typeof(DataMethodSetting))
                {
                    var datamethod = siteobject as DataMethodSetting;
                    if (datamethod != null)
                    {
                        info.Name = datamethod.OriginalMethodName;
                        info.DisplayName = datamethod.OriginalMethodName;
                    }

                }

                return info;
            }
        }

        public static ISiteObject GetSiteObject(SiteDb SiteDb, LogEntry log)
        {
            var repo = SiteDb.GetRepository(log.StoreName);
            if (repo == null)
            {
                return null;
            }

            var objectid = KeyConverter.FromByte(log.KeyBytes);

            var siteobject = repo.Get(objectid);
            if (siteobject == null)
            {
                siteobject = repo.GetLastEntryFromLog(objectid);
            }
            return siteobject;
        }

        public static ISiteObject GetSiteObject(SiteDb SiteDb, Guid ObjectId, byte ConstType, bool UseColumnOnly = false)
        {
            var type = ConstTypeService.GetModelType(ConstType);

            var repo = SiteDb.GetRepository(type);

            if (repo == null || ObjectId == default(Guid))
            {
                return null;
            }

            var siteobject = repo.Get(ObjectId);

            if (siteobject != null)
            {
                return siteobject as ISiteObject;
            }

            else
            {
                var item = repo.GetLastEntryFromLog(ObjectId);
                if (item != null)
                {
                    return item as ISiteObject;
                }
            }

            return null;
        }


        public static string GetObjectFullUrl(WebSite website, Guid ObjectId)
        {
            string baseurl = website.BaseUrl();
            string objectUrl = website.SiteDb().Routes.GetObjectPrimaryRelativeUrl(ObjectId);
            if (!string.IsNullOrEmpty(objectUrl))
            {
                objectUrl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, objectUrl);
            }
            return objectUrl;
        }

        public static string GetObjectRelativeUrl(SiteDb SiteDb, SiteObject SiteObject)
        {
            if (SiteObject == null)
            {
                return null;
            }

            if (SiteObject is IEmbeddable)
            {
                var embedded = SiteObject as IEmbeddable;
                if (embedded.IsEmbedded)
                {
                    var modeltype = ConstTypeService.GetModelType(embedded.OwnerConstType);
                    var repo = SiteDb.GetRepository(modeltype);
                    var parentobject = repo.Get(embedded.OwnerObjectId) as SiteObject;
                    return GetObjectRelativeUrl(SiteDb, parentobject);
                }
            }

            if (SiteObject is CmsCssRule)
            {
                var rule = SiteObject as CmsCssRule;
                var style = SiteDb.Styles.Get(rule.ParentStyleId);
                return GetObjectRelativeUrl(SiteDb, style);
            }

            if (SiteObject is CmsCssDeclaration)
            {
                var decl = SiteObject as CmsCssDeclaration;
                var style = SiteDb.Styles.Get(decl.ParentStyleId);
                return GetObjectRelativeUrl(SiteDb, style);
            }

            if (Kooboo.Sites.Service.ObjectService.IsRoutable(SiteObject, false))
            {
                return SiteDb.Routes.GetObjectPrimaryRelativeUrl(SiteObject.Id);
            }

            string name = SiteObject.Id.ToString();
            if (Attributes.AttributeHelper.IsNameAsId(SiteObject))
            {
                name = SiteObject.Name;
            }

            var modeltypex = ConstTypeService.GetModelType(SiteObject.ConstType);

            return Systems.Routes.SystemRouteTemplate.Replace("{objecttype}", modeltypex.Name).Replace("{nameorid}", name);
        }

        public static string GetObjectRelativeUrl(SiteDb SiteDb, Guid ObjectId, byte ConstType)
        {
            var siteobject = GetSiteObject(SiteDb, ObjectId, ConstType);
            return GetObjectRelativeUrl(SiteDb, siteobject as SiteObject);
        }

        public static string GetSummaryText(Object value, string FieldName = null)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var type = value.GetType();

            if (Lib.Reflection.TypeHelper.IsDictionary(type))
            {
                var dict = value as System.Collections.IDictionary;
                if (dict == null || dict.Count == 0)
                {
                    return null;
                }
                StringBuilder sb = new StringBuilder();
                foreach (var item in dict.Keys)
                {
                    string dictkey = GetSummaryText(item, FieldName);
                    string dictvalue = GetSummaryText(dict[item], FieldName);

                    if (!string.IsNullOrEmpty(FieldName))
                    {
                        dictkey = FieldName + "." + dictkey;
                    }
                    sb.AppendLine($"------{dictkey}------");
                    sb.AppendLine(dictvalue);
                }
                return sb.ToString();
            }
            else if (ObjectHelper.IsCollection(type))
            {
                StringBuilder sb = new StringBuilder();

                foreach (var item in (IEnumerable)value)
                {
                    sb.AppendLine(GetSummaryText(item));
                }
                return sb.ToString();
            }
            else if (type.IsClass && type != typeof(string))
            {
                var properties = Lib.Reflection.TypeHelper.GetPublicMembers(type);

                StringBuilder sb = new StringBuilder();

                foreach (var item in properties)
                {
                    if (!Attributes.AttributeHelper.IsSummaryIgnore(item))
                    {
                        object fieldvalue = null;
                        if (item is PropertyInfo)
                        {
                            var info = item as PropertyInfo;
                            fieldvalue = info.GetValue(value);
                        }
                        else if (item is FieldInfo)
                        {
                            var info = item as FieldInfo;
                            fieldvalue = info.GetValue(value);
                        }
                        if (fieldvalue != null)
                        {
                            string name = item.Name;
                            if (!string.IsNullOrEmpty(FieldName))
                            {
                                name = FieldName + "." + name;
                            }
                            string valuetext = GetSummaryText(fieldvalue, name);
                            if (!string.IsNullOrEmpty(valuetext))
                            {
                                sb.AppendLine($"------{name}------");
                                sb.AppendLine(valuetext);
                            }
                        }
                    }
                }

                return sb.ToString();
            }

            else
            {
                return value.ToString();
            }
        }


        public static bool IsRoutable(ISiteObject item, bool IncludeCode = true)
        {
            return IsRoutable(item.GetType(), IncludeCode);
        }

        public static bool IsRoutable(Type ModelType, bool IncludeCode = true)
        {
            bool IsRoutable = Attributes.AttributeHelper.IsRoutable(ModelType);
            if (!IsRoutable && IncludeCode)
            {
                if (ModelType == typeof(Code))
                {
                    IsRoutable = true;
                }
            }
            return IsRoutable;
        }


        public static string GetEditRoute(ISiteObject obj, SiteDb sitedb)
        {
            var modeltype = ConstTypeService.GetModelType(obj.ConstType);

            if (modeltype == typeof(Page))
            {
                // / _Admin / Page / EditPage ? SiteId = 9f4c626c - f49f - 3538 - 05c5 - 97bb3a206f01 & Id = 9207f022 - d11e - 480a - 8ddf - aacb5cb4b1b3
                /// _Admin / Page / EditRichText ? SiteId = 9f4c626c - f49f - 3538 - 05c5 - 97bb3a206f01 & Id = 5fbb992d - 423f - 4a26 - b27e - 067a743eabc6
                ///_Admin/Page/EditLayout?SiteId=9f4c626c-f49f-3538-05c5-97bb3a206f01&Id=685e3801-762d-44be-be1e-7f4d4237d47a&layoutId=7186a2e9-a55b-c58b-4964-fd9b73b82985
                var page = obj as Page;
                if (page.Type == PageType.Normal)
                {
                    return "/_Admin/Page/EditPage?SiteId=" + sitedb.Id.ToString() + "&Id=" + page.Id.ToString();
                }
                else if (page.Type == PageType.Layout)
                {
                    /// _Admin / Page / EditLayout ? SiteId = 9f4c626c - f49f - 3538 - 05c5 - 97bb3a206f01 & Id = 685e3801 - 762d - 44be - be1e - 7f4d4237d47a & layoutId = 7186a2e9 - a55b - c58b - 4964 - fd9b73b82985
                    var layout = new Layout();
                    layout.Name = page.LayoutName;

                    var url = "/_Admin/Page/EditLayout?SiteId=" + sitedb.Id.ToString();
                    url += "&Id=" + page.Id.ToString();
                    url += "&layoutId=" + layout.Id.ToString();
                    return url;
                }
                else if (page.Type == PageType.RichText)
                {
                    /// _Admin / Page / EditRichText ? SiteId = 9f4c626c - f49f - 3538 - 05c5 - 97bb3a206f01 & Id = 5fbb992d - 423f - 4a26 - b27e - 067a743eabc6
                    /// 
                    return "/_Admin/Page/EditRichText?SiteId=" + sitedb.Id.ToString() + "&Id=" + page.Id.ToString();
                }
                else
                {
                    return null;
                }


            }
            else if (modeltype == typeof(View))
            {
                //_Admin/Development/View?SiteId=9f4c626c-f49f-3538-05c5-97bb3a206f01&Id=a16aa2f1-e4ff-f5a3-a49b-c719e1a58d11 

                return "/_Admin/Development/View?SiteId=" + sitedb.Id.ToString() + "&Id=" + obj.Id.ToString();

            }
            else if (modeltype == typeof(Style))
            {
                //_Admin/Development/Style?SiteId=02182faa-207a-0a69-bfd3-677269d75360&Id=41be6f57-6d78-a959-38ba-17f628f5c726 
                return "/_Admin/Development/Style?SiteId=" + sitedb.Id.ToString() + "&Id=" + obj.Id.ToString();
            }
            else if ((modeltype == typeof(Script)))
            {
                //_Admin/Development/Script?SiteId=02182faa-207a-0a69-bfd3-677269d75360&Id=c9dc9ec1-c29e-4ce5-b024-f18183c00502 
                return "/_Admin/Development/Script?SiteId=" + sitedb.Id.ToString() + "&Id=" + obj.Id.ToString();
            }
            else if (modeltype == typeof(Code))
            {
                ///_Admin/Development/EditCode?SiteId=02182faa-207a-0a69-bfd3-677269d75360&id=d2e30679-aa6f-4901-bc0d-5b006057d9a3

                return "/_Admin/Development/EditCode?SiteId=" + sitedb.Id.ToString() + "&Id=" + obj.Id.ToString();
            }
            else if (modeltype == typeof(Layout))
            {
                ///_Admin/Development/Layout?SiteId=02182faa-207a-0a69-bfd3-677269d75360&Id=0da593e9-562e-9ad4-9ae4-7e88961ccd36 
                return "/_Admin/Development/Layout?SiteId=" + sitedb.Id.ToString() + "&Id=" + obj.Id.ToString();
            }

            else
            {
        /// system route. 
            //string SystemRouteTemplate = "/__kb/{objecttype}/{nameorid}"; 
            return "/__kb/" + modeltype.Name + "/" + obj.Id.ToString();

            } 

        }
    }

}