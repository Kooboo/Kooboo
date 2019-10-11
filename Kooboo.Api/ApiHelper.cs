//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Api
{
    public class ApiHelper
    {
        public static string GetHelper(ApiCall request)
        {
            return null;
        }

        public static int GetPageNr(ApiCall call)
        {
            var pagenr = 1;
            var value = call.GetValue("pagenr");

            if (!string.IsNullOrEmpty(value))
                if (int.TryParse(value, out pagenr))
                    return pagenr;
            return pagenr;
        }

        public static int GetPageSize(ApiCall call, int defaultSize = 20)
        {
            var value = call.GetRequestValue("pagesize");

            if (!string.IsNullOrEmpty(value))
                if (int.TryParse(value, out int pagesize))
                    return pagesize;
            return defaultSize;
        }

        public static Pager GetPager(ApiCall call, int defaultPageSize = 20)
        {
            Pager pager = new Pager
            {
                PageNr = GetPageNr(call),
                PageSize = GetPageSize(call, defaultPageSize)
            };
            return pager;
        }

        public static int GetPageCount(int totalcount, int pagesize)
        {
            if (totalcount <= 0)
                return 0;

            if (pagesize <= 1) pagesize = 1;

            var number = totalcount / pagesize;

            var newtotal = pagesize * number;

            return newtotal < totalcount ? number + 1 : number;
        }

        public static List<Parameter> GetParameters(System.Reflection.MethodInfo method)
        {
            var result = new List<Parameter>();

            var paras = method.GetParameters();
            foreach (var item in paras)
            {
                result.Add(new Parameter { Name = item.Name, ClrType = item.ParameterType });
            }
            return result;
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
                var num = (PageNr - 1) * PageSize;
                return num >= 0 ? num : 0;
            }
        }
    }
}