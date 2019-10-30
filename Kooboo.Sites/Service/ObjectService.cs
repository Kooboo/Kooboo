//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Helper;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Kooboo.Sites.Service
{
    public static class ObjectService
    {
        public static IByteConverter<Guid> KeyConverter = new Kooboo.IndexedDB.ByteConverter.GuidConverter();

        /// <summary>
        /// Get the object information for display.
        /// If it is the declaration or embedded, get the owner object...
        /// </summary>
        /// <param name="siteDb"></param>
        /// <param name="objectId"></param>
        /// <param name="constType"></param>
        /// <returns></returns>
        public static ObjectInfo GetObjectInfo(SiteDb siteDb, Guid objectId, byte constType)
        {
            var siteobject = GetSiteObject(siteDb, objectId, constType);
            return siteobject == null ? null : GetObjectInfo(siteDb, siteobject);
        }

        public static long GetSize(ISiteObject siteobject)
        {
            if (siteobject is IBinaryFile binaryFile)
            {
                return binaryFile.Size;
            }
            else
            {
                var json = Lib.Helper.JsonHelper.Serialize(siteobject);
                return json.Length;
            }
        }

        public static ObjectInfo GetObjectInfo(SiteDb siteDb, ISiteObject siteobject)
        {
            if (siteobject == null)
            {
                return null;
            }

            ObjectInfo info = new ObjectInfo
            {
                ObjectId = siteobject.Id,
                ConstType = siteobject.ConstType,
                ModelType = ConstTypeService.GetModelType(siteobject.ConstType)
            };

            if (siteobject is IBinaryFile binaryFile)
            {
                info.Size = binaryFile.Size;
            }
            else if (siteobject is ITextObject textObject)
            {
                info.Size = (textObject.Body ?? "").Length;
            }

            if (Kooboo.Lib.Reflection.TypeHelper.HasInterface(info.ModelType, typeof(IEmbeddable)))
            {
                if (siteobject is IEmbeddable embeddable && embeddable.IsEmbedded)
                {
                    return GetObjectInfo(siteDb, embeddable.OwnerObjectId, embeddable.OwnerConstType);
                }
            }

            if (Attributes.AttributeHelper.IsRoutable(siteobject))
            {
                info.Url = GetObjectRelativeUrl(siteDb, siteobject as SiteObject);
                info.DisplayName = info.Url;
                info.Name = siteobject.Name;
                return info;
            }
            else
            {
                if (info.ModelType == typeof(CmsCssRule))
                {
                    if (!(siteobject is CmsCssRule rule))
                    {
                        return null;
                    }

                    return rule.IsInline ? GetObjectInfo(siteDb, rule.OwnerObjectId, rule.OwnerObjectConstType) : GetObjectInfo(siteDb, rule.ParentStyleId, ConstObjectType.Style);
                }

                info.Url = "/__kb/" + info.ModelType.Name + "/" + info.ObjectId.ToString();
                info.DisplayName = siteobject.Name;
                info.Name = siteobject.Name;

                if (info.ModelType == typeof(TextContent))
                {
                    info.Name = Kooboo.Sites.Helper.ContentHelper.GetSummary(siteobject as TextContent, siteDb.WebSite.DefaultCulture);
                    info.DisplayName = info.Name;
                }

                if (info.ModelType == typeof(DataMethodSetting))
                {
                    if (siteobject is DataMethodSetting datamethod)
                    {
                        info.Name = datamethod.OriginalMethodName;
                        info.DisplayName = datamethod.OriginalMethodName;
                    }
                }

                return info;
            }
        }

        public static ISiteObject GetSiteObject(SiteDb siteDb, LogEntry log)
        {
            var repo = siteDb.GetRepository(log.StoreName);
            if (repo == null)
            {
                return null;
            }

            var objectid = KeyConverter.FromByte(log.KeyBytes);

            var siteobject = repo.Get(objectid) ?? repo.GetLastEntryFromLog(objectid);
            return siteobject;
        }

        public static ISiteObject GetSiteObject(SiteDb siteDb, Guid objectId, byte constType, bool useColumnOnly = false)
        {
            var type = ConstTypeService.GetModelType(constType);

            var repo = siteDb.GetRepository(type);

            if (repo == null || objectId == default(Guid))
            {
                return null;
            }

            var siteobject = repo.Get(objectId);

            if (siteobject != null)
            {
                return siteobject;
            }
            else
            {
                var item = repo.GetLastEntryFromLog(objectId);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        public static string GetObjectFullUrl(WebSite website, Guid objectId)
        {
            string baseurl = website.BaseUrl();
            string objectUrl = website.SiteDb().Routes.GetObjectPrimaryRelativeUrl(objectId);
            if (!string.IsNullOrEmpty(objectUrl))
            {
                objectUrl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, objectUrl);
            }
            return objectUrl;
        }

        public static string GetObjectRelativeUrl(SiteDb siteDb, SiteObject siteObject)
        {
            if (siteObject == null)
            {
                return null;
            }

            if (siteObject is IEmbeddable embedded)
            {
                if (embedded.IsEmbedded)
                {
                    var modeltype = ConstTypeService.GetModelType(embedded.OwnerConstType);
                    var repo = siteDb.GetRepository(modeltype);
                    var parentobject = repo.Get(embedded.OwnerObjectId) as SiteObject;
                    return GetObjectRelativeUrl(siteDb, parentobject);
                }
            }

            if (siteObject is CmsCssRule rule)
            {
                var style = siteDb.Styles.Get(rule.ParentStyleId);
                return GetObjectRelativeUrl(siteDb, style);
            }

            if (siteObject is CmsCssDeclaration decl)
            {
                var style = siteDb.Styles.Get(decl.ParentStyleId);
                return GetObjectRelativeUrl(siteDb, style);
            }

            if (Attributes.AttributeHelper.IsRoutable(siteObject))
            {
                return siteDb.Routes.GetObjectPrimaryRelativeUrl(siteObject.Id);
            }

            string name = siteObject.Id.ToString();
            if (Attributes.AttributeHelper.IsNameAsId(siteObject))
            {
                name = siteObject.Name;
            }

            var modeltypex = ConstTypeService.GetModelType(siteObject.ConstType);

            return Systems.Routes.SystemRouteTemplate.Replace("{objecttype}", modeltypex.Name).Replace("{nameorid}", name);
        }

        public static string GetObjectRelativeUrl(SiteDb siteDb, Guid objectId, byte constType)
        {
            var siteobject = GetSiteObject(siteDb, objectId, constType);
            return GetObjectRelativeUrl(siteDb, siteobject as SiteObject);
        }

        public static string GetSummaryText(Object value, string fieldName = null)
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
                    string dictkey = GetSummaryText(item, fieldName);
                    string dictvalue = GetSummaryText(dict[item], fieldName);

                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        dictkey = fieldName + "." + dictkey;
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
                            if (!string.IsNullOrEmpty(fieldName))
                            {
                                name = fieldName + "." + name;
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
    }
}