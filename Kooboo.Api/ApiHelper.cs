//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static int GetPageSize(ApiCall call, int defaultSize =20)
        {
            int pagesize = 20;
            string value = call.GetRequestValue("pagesize");

            if (!string.IsNullOrEmpty(value))
            {
                if (int.TryParse(value, out pagesize))
                {
                    return pagesize;
                }
            }
            return defaultSize;
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
            if (totalcount <=0)
            {
                return 0; 
            }

            if (pagesize <=1)
            {
                pagesize = 1; 
            }

             int number  = (int)totalcount/pagesize;

             int newtotal = pagesize * number;

            return newtotal < totalcount ? number + 1 : number;  

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
                if (num >=0)
                {
                    return num; 
                }
                return 0; 
            }
        }
    }
}
