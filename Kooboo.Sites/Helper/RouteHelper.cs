//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Helper
{
 public static    class RouteHelper
    {

        public static string ToValidRoute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            } 
            input = input.Replace("\\", "/");
            if (input.IndexOf("/") == -1) 
            {
                return Lib.Helper.StringHelper.ToValidFileName(input); 
            }

            string[] segs = input.Split('/');

            for (int i = 0; i < segs.Count(); i++)
            {
                segs[i] = Lib.Helper.StringHelper.ToValidFileName(segs[i]); 
            } 
            return string.Join("/", segs);  
        }
    }
}
