//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using Kooboo.Sites.Routing;
using System;

namespace Kooboo.Sites.Service
{
    public class ConstTypeService
    {
        public static byte GetConstType(Type valueType)
        {
            if (valueType == typeof(Binding))
            {
                return ConstObjectType.Binding;
            }
            else if (valueType == typeof(ContentCategory))
            {
                return ConstObjectType.ContentCategory;
            }
            else if (valueType == typeof(ContentFolder))
            {
                return ConstObjectType.ContentFolder;
            }
            else if (valueType == typeof(ContentType))
            {
                return ConstObjectType.ContentType;
            }
            else if (valueType == typeof(CmsCssRule))
            {
                return ConstObjectType.CssRule;
            }
            else if (valueType == typeof(DataMethodSetting))
            {
                return ConstObjectType.DataMethodSetting;
            }
            else if (valueType == typeof(Domain))
            {
                return ConstObjectType.Domain;
            }
            else if (valueType == typeof(ExternalResource))
            {
                return ConstObjectType.ExternalResource;
            }
            else if (valueType == typeof(CmsFile))
            {
                return ConstObjectType.CmsFile;
            }
            else if (valueType == typeof(Folder))
            {
                return ConstObjectType.Folder;
            }
            else if (valueType == typeof(Form))
            {
                return ConstObjectType.Form;
            }
            else if (valueType == typeof(FormValue))
            {
                return ConstObjectType.FormValue;
            }
            else if (valueType == typeof(HtmlBlock))
            {
                return ConstObjectType.HtmlBlock;
            }
            else if (valueType == typeof(Image))
            {
                return ConstObjectType.Image;
            }
            else if (valueType == typeof(Label))
            {
                return ConstObjectType.Label;
            }
            else if (valueType == typeof(Layout))
            {
                return ConstObjectType.Layout;
            }
            else if (valueType == typeof(Menu))
            {
                return ConstObjectType.Menu;
            }
            else if (valueType == typeof(Page))
            {
                return ConstObjectType.Page;
            }
            else if (valueType == typeof(ObjectRelation))
            {
                return ConstObjectType.ObjectRelation;
            }
            else if (valueType == typeof(ResourceGroup))
            {
                return ConstObjectType.ResourceGroup;
            }
            else if (valueType == typeof(Route))
            {
                return ConstObjectType.Route;
            }
            else if (valueType == typeof(Script))
            {
                return ConstObjectType.Script;
            }
            else if (valueType == typeof(Style))
            {
                return ConstObjectType.Style;
            }
            else if (valueType == typeof(SyncSetting))
            {
                return ConstObjectType.SyncSetting;
            }
            else if (valueType == typeof(TextContent))
            {
                return ConstObjectType.TextContent;
            }
            else if (valueType == typeof(Thumbnail))
            {
                return ConstObjectType.Thumbnail;
            }
            else if (valueType == typeof(User))
            {
                return ConstObjectType.User;
            }
            else if (valueType == typeof(Organization))
            {
                return ConstObjectType.UserGroup;
            }
            else if (valueType == typeof(View))
            {
                return ConstObjectType.View;
            }
            else if (valueType == typeof(ViewDataMethod))
            {
                return ConstObjectType.ViewDataMethod;
            }
            else if (valueType == typeof(WebSite))
            {
                return ConstObjectType.WebSite;
            }
            else if (valueType == typeof(Synchronization))
            {
                return ConstObjectType.Synchronization;
            }
            else if (valueType == typeof(SiteCluster))
            {
                return ConstObjectType.SiteCluster;
            }
            else if (valueType == typeof(FormSetting))
            {
                return ConstObjectType.FormSetting;
            }
            else if (valueType == typeof(Code))
            {
                return ConstObjectType.Code;
            }
            else if (valueType == typeof(CoreSetting))
            {
                return ConstObjectType.CoreSetting;
            }
            else if (valueType == typeof(SiteUser))
            {
                return ConstObjectType.SiteUser;
            }
            else if (valueType == typeof(BusinessRule))
            {
                return ConstObjectType.BusinessRule;
            }
            else if (valueType == typeof(KConfig))
            {
                return ConstObjectType.Kconfig;
            }

            return ConstObjectType.Unknown;
        }

        public static Type GetModelType(byte constType)
        {
            return ConstTypeContainer.GetModelType(constType);
        }

        public static byte GetConstTypeByUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return ConstObjectType.Unknown;
            }
            var filetype = Lib.Helper.UrlHelper.GetFileType(url);

            switch (filetype)
            {
                case Lib.Helper.UrlHelper.UrlFileType.Unknow:
                    return ConstObjectType.Page;

                case Lib.Helper.UrlHelper.UrlFileType.Image:
                    return ConstObjectType.Image;

                case Lib.Helper.UrlHelper.UrlFileType.JavaScript:
                    return ConstObjectType.Script;

                case Lib.Helper.UrlHelper.UrlFileType.Style:
                    return ConstObjectType.Style;

                case Lib.Helper.UrlHelper.UrlFileType.File:
                    return ConstObjectType.CmsFile;

                case Lib.Helper.UrlHelper.UrlFileType.PageOrView:
                    return ConstObjectType.Page;

                default:
                    return ConstObjectType.Page;
            }
        }

        public static string GetContentType(Type modelType)
        {
            if (modelType == typeof(Style))
            {
                return "text/css;charset=utf-8;";
            }
            else if (modelType == typeof(Script))
            {
                return "application/javascript;charset=utf-8;";
            }

            if (Kooboo.Lib.Reflection.TypeHelper.IsOfBaseTypeOrInterface(modelType, typeof(Kooboo.Data.Interface.IBinaryFile)))
            {
                return "application/octet-stream";
            }

            return "text/html;charset=utf-8;";
        }

        public static string GetContentType(byte constType)
        {
            var type = GetModelType(constType);
            return GetContentType(type);
        }

        public static Type GetModelTypeByUrl(string relativeUrl)
        {
            var filetype = Kooboo.Lib.Helper.UrlHelper.GetFileType(relativeUrl);
            switch (filetype)
            {
                case Lib.Helper.UrlHelper.UrlFileType.Image:
                    return typeof(Image);

                case Lib.Helper.UrlHelper.UrlFileType.JavaScript:
                    return typeof(Script);

                case Lib.Helper.UrlHelper.UrlFileType.Style:
                    return typeof(Style);

                case Lib.Helper.UrlHelper.UrlFileType.File:
                    return typeof(CmsFile);

                case Lib.Helper.UrlHelper.UrlFileType.PageOrView:
                    return typeof(Page);

                default:
                    return typeof(Page);
            }
        }
    }
}