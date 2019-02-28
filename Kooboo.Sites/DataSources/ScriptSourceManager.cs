//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.Data.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
  
namespace Kooboo.Sites.DataSources
{
  public static  class ScriptSourceManager
    { 
        public static List<DataMethodSetting> GetCodeMethods(SiteDb sitedb)
        { 
            var type = typeof(Kooboo.Sites.DataSources.kScript);

            var allcodes = sitedb.Code.ListByCodeType(Sites.Models.CodeType.Datasource);

            List<DataMethodSetting> settings = new List<DataMethodSetting>();

            foreach (var item in allcodes)
            {
                DataMethodSetting setting = new DataMethodSetting();
                setting.DeclareType = type.FullName;
                setting.OriginalMethodName = item.Name;
                setting.MethodName = item.Name;
                setting.CodeId = item.Id;
                setting.IsPublic = true;

                setting.ReturnType = typeof(IJson).FullName; 
                
                setting.MethodSignatureHash = methodSignatureHash(item.Id); 
                 
                var config = Kooboo.Sites.Scripting.Manager.GetSetting(sitedb.WebSite, item);
                if (config != null && config.Count > 0)
                {
                    foreach (var con in config)
                    { 
                        setting.Parameters.Add(con.Name, typeof(string).FullName);

                        ParameterBinding binding = new ParameterBinding();
                        binding.DisplayName = con.Name;

                        setting.ParameterBinding.Add(con.Name, binding);  
                    } 
                }

                // add the samplecode. 
                ParameterBinding samplecode = new ParameterBinding();
                samplecode.IsData = true;
                samplecode.DisplayName = SampleResponseFieldName;
                setting.Parameters.Add(samplecode.DisplayName, typeof(string).FullName);
                setting.ParameterBinding.Add(SampleResponseFieldName, samplecode);
                 

                settings.Add(setting);
            } 
            return settings;  
        }
         
        private static Guid methodSignatureHash(Guid codeid)
        {
            string unique = typeof(Kooboo.Sites.DataSources.kScript).FullName;
            unique += codeid.ToString();
            return Lib.Security.Hash.ComputeGuidIgnoreCase(unique);  
        }

        public static DataMethodSetting Get(SiteDb sitedb, Guid methodId)
        {
            var list = GetCodeMethods(sitedb);

            return list.Find(o => o.Id == methodId);  
        }

        public static DataMethodSetting GetByMethodHash(SiteDb sitedb, Guid MethodHashId)
        {

            var all = GetCodeMethods(sitedb);

            return all.Find(o => o.MethodSignatureHash == MethodHashId);  

        }

        public static string SampleResponseFieldName { get; set; } = "SampleJonResponse"; 
    }
}
