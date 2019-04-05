//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Lib.Reflection;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.HtmlForm
{
   public static class FormManager
    { 
        private static object _locker = new object();

        static FormManager()
        {
            List = new List<Data.Interface.IFormSubmitter>();
            var types = AssemblyLoader.LoadTypeByInterface(typeof(Data.Interface.IFormSubmitter));

            foreach (var item in types)
            {
                var instance = Activator.CreateInstance(item) as Data.Interface.IFormSubmitter;
                if (instance != null)
                {
                    List.Add(instance);
                }
            }   
        }

   
        public static List<Data.Interface.IFormSubmitter> List
        {
            get;set;
        }

        public static Data.Interface.IFormSubmitter GetSubmitter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null; 
            }  
            foreach (var item in List)
            {
                if (item.Name == name)
                {
                    return item; 
                } 
            }
            return null; 
        }

        public static string FormUrlName { get; set; } = "_kooboourl"; 


        public static string GetSubmitUrl(Kooboo.Sites.Models.Form form, FormSetting setting, RenderContext context)
        {

            var submitter = GetSubmitter(setting.FormSubmitter); 

            if (submitter !=null)
            {
                var url = submitter.CustomActionUrl(context, setting.Setting); 
                if(!string.IsNullOrEmpty(url))
                {
                    return url; 
                }
            }                
                  
            string shortformid = Lib.Security.ShortGuid.Encode(form.Id);

            string submiturl = "/_api/submit/form/" + shortformid;

            //string baseulr = context.WebSite.BaseUrl();
            //submiturl = Kooboo.Lib.Helper.UrlHelper.Combine(baseulr, submiturl);

            return submiturl; 
        }

    }
}
