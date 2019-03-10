//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using Kooboo.Sites.Routing; 
using System;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Service
{
    public class ConstTypeService
    {
        public static byte GetConstType(Type TValueType)
        {
            if (TValueType == typeof(Binding))
            {
                return ConstObjectType.Binding;
            }
            else if (TValueType == typeof(ContentCategory))
            {
                return ConstObjectType.ContentCategory;
            }
            else if (TValueType == typeof(ContentFolder))
            {
                return ConstObjectType.ContentFolder;
            }
            else if (TValueType == typeof(ContentType))
            {
                return ConstObjectType.ContentType;
            }

            else if (TValueType == typeof(CmsCssRule))
            {
                return ConstObjectType.CssRule;
            }
            else if (TValueType == typeof(DataMethodSetting))
            {
                return ConstObjectType.DataMethodSetting;
            }
            else if (TValueType == typeof(Domain))
            {
                return ConstObjectType.Domain;
            }
            else if (TValueType == typeof(ExternalResource))
            {
                return ConstObjectType.ExternalResource;
            }
            else if (TValueType == typeof(CmsFile))
            {
                return ConstObjectType.CmsFile;
            }
            else if (TValueType == typeof(Folder))
            {
                return ConstObjectType.Folder;
            }
            else if (TValueType == typeof(Form))
            {
                return ConstObjectType.Form;
            }

            else if (TValueType == typeof(FormValue))
            {
                return ConstObjectType.FormValue;
            }
            else if (TValueType == typeof(HtmlBlock))
            {
                return ConstObjectType.HtmlBlock;
            }
            else if (TValueType == typeof(Image))
            {
                return ConstObjectType.Image;
            }
            else if (TValueType == typeof(Label))
            {
                return ConstObjectType.Label;
            }
            else if (TValueType == typeof(Layout))
            {
                return ConstObjectType.Layout;
            }
            else if (TValueType == typeof(Menu))
            {
                return ConstObjectType.Menu;
            }
            else if (TValueType == typeof(Page))
            {
                return ConstObjectType.Page;
            }
            else if (TValueType == typeof(ObjectRelation))
            {
                return ConstObjectType.ObjectRelation;
            }
            else if (TValueType == typeof(ResourceGroup))
            {
                return ConstObjectType.ResourceGroup;
            }
            else if (TValueType == typeof(Route))
            {
                return ConstObjectType.Route;
            }
            else if (TValueType == typeof(Script))
            {
                return ConstObjectType.Script;
            }
            else if (TValueType == typeof(Style))
            {
                return ConstObjectType.Style;
            }
            else if (TValueType == typeof(SyncSetting))
            {
                return ConstObjectType.SyncSetting;
            }

            else if (TValueType == typeof(TextContent))
            {
                return ConstObjectType.TextContent;
            }
            else if (TValueType == typeof(Thumbnail))
            {
                return ConstObjectType.Thumbnail;
            }
            else if (TValueType == typeof(User))
            {
                return ConstObjectType.User;
            }
            else if (TValueType == typeof(Organization))
            {
                return ConstObjectType.UserGroup;
            }
            else if (TValueType == typeof(View))
            {
                return ConstObjectType.View;
            }
            else if (TValueType == typeof(ViewDataMethod))
            {
                return ConstObjectType.ViewDataMethod;
            }
            else if (TValueType == typeof(WebSite))
            {
                return ConstObjectType.WebSite;
            }
            else if (TValueType == typeof(Synchronization))
            {
                return ConstObjectType.Synchronization;
            }
            else if (TValueType == typeof(SiteCluster))
            {
                return ConstObjectType.SiteCluster;
            }
            else if (TValueType == typeof(FormSetting))
            {
                return ConstObjectType.FormSetting;
            }
     
      
            else if (TValueType == typeof(Code))
            {
                return ConstObjectType.Code; 
            }
            else if (TValueType == typeof(CoreSetting))
            {
                return ConstObjectType.CoreSetting;
            }
            else if (TValueType ==typeof(SiteUser))
            {
                return ConstObjectType.SiteUser; 
            }
            else if (TValueType == typeof(BusinessRule))
            {
                return ConstObjectType.BusinessRule;
            }
            else if (TValueType == typeof(KConfig))
            {
                return ConstObjectType.Kconfig; 
            }

            return ConstObjectType.Unknown;
        }

        public static Type GetModelType(byte ConstType)
        {
            return ConstTypeContainer.GetModelType(ConstType); 
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

        public static string GetContentType(Type ModelType)
        {
            if (ModelType == typeof(Style))
            {
                return "text/css;charset=utf-8;";
            }
            else if (ModelType == typeof(Script))
            {
                return "application/javascript;charset=utf-8;";
            }
              
            if (Kooboo.Lib.Reflection.TypeHelper.IsOfBaseTypeOrInterface(ModelType, typeof(Kooboo.Data.Interface.IBinaryFile)))
            {
                return "application/octet-stream";
            }

            return "text/html;charset=utf-8;";
        }

        public static string GetContentType(byte ConstType)
        {
            var type = GetModelType(ConstType);
            return GetContentType(type);
        }

        public static Type GetModelTypeByUrl(string RelativeUrl)
        {
            var filetype = Kooboo.Lib.Helper.UrlHelper.GetFileType(RelativeUrl);
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
