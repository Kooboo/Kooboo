//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using Kooboo.Lib.Helper;

namespace Kooboo.Api
{
    public class ApiHelper
    {
        public static string GetHelper(ApiCall request)
        {
            return null;
        }

        public static string GetVersion(IApi api)
        {
            if (api == null) return ApiVersion.V1;

            var versionAttribute = api.GetType()
                .GetCustomAttribute(typeof(ApiVersionAttribute)) as ApiVersionAttribute;

            return versionAttribute?.Version ?? ApiVersion.V1;
        }

        public static int GetPageNr(ApiCall call)
        {
            int pagenr = 1;
            string value = call.GetValue("pagenr");

            if (!string.IsNullOrEmpty(value))
            {
                if (int.TryParse(value, out pagenr))
                {
                    return pagenr;
                }
            }

            return pagenr;
        }

        public static int GetPageSize(ApiCall call, int defaultSize = 20)
        {
            int pagesize = 20;
            string value = call.GetValue("pagesize");

            if (!string.IsNullOrEmpty(value))
            {
                if (int.TryParse(value, out pagesize))
                {
                    return pagesize == 0 ? defaultSize : pagesize;
                }
            }

            return defaultSize;
        }

        public static bool GetIsDevMode(ApiCall call)
        {
            return call.GetValue("devMode")?.ToLower() == "true";
        }

        public static Pager GetPager(ApiCall call, int defaultPageSize = 20)
        {
            Pager pager = new Pager();
            pager.PageNr = GetPageNr(call);
            pager.PageSize = GetPageSize(call, defaultPageSize);
            return pager;
        }

        public static int GetPageCount(int totalcount, int pagesize)
        {
            return MiscHelper.GetPageCount(totalcount, pagesize);
        }

        public static List<Parameter> GetParameters(System.Reflection.MethodInfo method)
        {
            List<Parameter> result = new List<Parameter>();

            var paras = method.GetParameters();
            foreach (var item in paras)
            {
                result.Add(new Parameter() { Name = item.Name, ClrType = item.ParameterType });
            }

            return result;
        }


        public static string MakeUrl(string relativeUrl)
        {
            var host = GetLocalHost();

            if (!host.StartsWith("http") && !host.StartsWith("https"))
            {
                host = $"http://{host}";
            }

            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }

            var url = host + relativeUrl;

            var uri = new Uri(url);

            return uri.AbsoluteUri;

        }

        private static string GetLocalHost()
        {
            if (Data.AppSettings.IsOnlineServer)
            {
                if (Data.AppSettings.ServerSetting != null && Data.AppSettings.ServerSetting.HostDomain != null)
                {
                    return Data.AppSettings.ServerSetting.HostDomain;
                }
            }

            return "http://www.localkooboo.com";

        }
    }

    public class Pager
    {
        public int PageNr { get; set; }

        public int PageSize { get; set; }

        public int SkipCount
        {
            get
            {
                int num = (PageNr - 1) * PageSize;
                if (num >= 0)
                {
                    return num;
                }

                return 0;
            }
        }
    }
}