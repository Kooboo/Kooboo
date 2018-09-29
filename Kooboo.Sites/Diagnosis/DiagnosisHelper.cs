//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Language;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
  
namespace Kooboo.Sites.Diagnosis
{
 public static   class DiagnosisHelper
    { 
        public static string DisplayUsedBy( RenderContext context, List<Data.Models.UsedByRelation> usedby)
        {
            string baseurl = context.WebSite.BaseUrl();
            string message = "<br/> " +  Hardcoded.GetValue("Used by: ", context);

            foreach (var useobj in usedby.GroupBy(o=>o.ModelType))
            {
                var typename = useobj.Key.Name;

                message += " " + typename + ": "; 

                foreach (var item in useobj.ToList())
                {
                    string fullurl = Lib.Helper.UrlHelper.Combine(baseurl, item.Url);

                    string usestatement =  "<a target='_blank' href='" + fullurl + "'>" +item.Name + "</a>; ";

                    message += usestatement;
                }  
            }
            return message;
        }

        public static string DisplayUsedBy(RenderContext context,  Kooboo.Sites.Models.SiteObject usedObject)
        {
            List<SiteObject> models = new List<SiteObject>();
            models.Add(usedObject);
            return DisplayUsedBy(context, models); 
        }
           
        public static string DisplayUsedBy(RenderContext context, List<Kooboo.Sites.Models.SiteObject> usedObjects)
        {
            string baseurl = context.WebSite.BaseUrl();
            string message = "<br/> " + Hardcoded.GetValue("Used by: ", context);
            var sitedb = context.WebSite.SiteDb(); 

            foreach (var useobj in usedObjects.GroupBy(o=>o.ConstType))
            {
                var consttype = useobj.Key;
                var model = Kooboo.Sites.Service.ConstTypeService.GetModelType(consttype);
                
                message += " " +  model.Name + ": ";

                foreach (var item in useobj.ToList())
                { 
                    var url = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, item);
                    string fullurl = Lib.Helper.UrlHelper.Combine(baseurl, url);

                    string usestatement = "<a target='_blank' href='" + fullurl + "'>" + item.Name + "</a>; ";

                    message += usestatement;
                } 
            }
            return message;
        }
         
        private static string GetRelatetionByType(SiteDb sitedb, ISiteObject item, byte type)
        {
            var relations = sitedb.Relations.GetReferredBy(item as SiteObject, type);
            StringBuilder responseMessage = new StringBuilder();
            foreach (var jitem in relations)
            {
                Guid id = jitem.objectXId;
                string typeName = "";
                var obj = GetSiteObject(sitedb, type, id, ref typeName);

                if (obj != null)
                {
                    //ToDo rewrite
                    var url = string.Format("/_Admin/Development/{2}?SiteId={0}&Id={1}", sitedb.Id, obj.Id, typeName);
                    responseMessage.AppendFormat("<a href='{0}' target='_blank'>{1}</a> ", url, obj.Name);
                }
            }
            return responseMessage.ToString();
        }

        private static ISiteObject GetSiteObject(SiteDb sitedb, byte type, Guid id, ref string typeName)
        {
            switch (type)
            {
                case ConstObjectType.View:
                    typeName = "View";
                    return sitedb.Views.Get(id);
                case ConstObjectType.Layout:
                    typeName = "Layout";
                    return sitedb.Layouts.Get(id);
                case ConstObjectType.Menu:
                    typeName = "Menu";
                    return sitedb.Menus.Get(id);
            }
            return null;
        }
    }
}
