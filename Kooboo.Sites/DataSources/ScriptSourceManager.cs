//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.Data.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.DataSources
{
    public static class ScriptSourceManager
    {
        public static List<DataMethodSetting> GetCodeMethods(SiteDb sitedb)
        {
            var type = typeof(Kooboo.Sites.DataSources.kScript);

            var allcodes = sitedb.Code.ListByCodeType(Sites.Models.CodeType.Datasource);

            List<DataMethodSetting> settings = new List<DataMethodSetting>();

            foreach (var item in allcodes)
            {
                DataMethodSetting setting = new DataMethodSetting
                {
                    DeclareType = type.FullName,
                    OriginalMethodName = item.Name,
                    MethodName = item.Name,
                    CodeId = item.Id,
                    IsPublic = true,
                    ReturnType = typeof(IJson).FullName,
                    MethodSignatureHash = MethodSignatureHash(item.Id)
                };



                var config = Kooboo.Sites.Scripting.Manager.GetSetting(sitedb.WebSite, item);
                if (config != null && config.Count > 0)
                {
                    foreach (var con in config)
                    {
                        setting.Parameters.Add(con.Name, typeof(string).FullName);

                        ParameterBinding binding = new ParameterBinding {DisplayName = con.Name};

                        setting.ParameterBinding.Add(con.Name, binding);
                    }
                }

                // add the samplecode.
                ParameterBinding samplecode = new ParameterBinding
                {
                    IsData = true, DisplayName = SampleResponseFieldName
                };
                setting.Parameters.Add(samplecode.DisplayName, typeof(string).FullName);
                setting.ParameterBinding.Add(SampleResponseFieldName, samplecode);

                settings.Add(setting);
            }
            return settings;
        }

        private static Guid MethodSignatureHash(Guid codeid)
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

        public static DataMethodSetting GetByMethodHash(SiteDb sitedb, Guid methodHashId)
        {
            var all = GetCodeMethods(sitedb);

            return all.Find(o => o.MethodSignatureHash == methodHashId);
        }

        public static string SampleResponseFieldName { get; set; } = "SampleJonResponse";
    }
}