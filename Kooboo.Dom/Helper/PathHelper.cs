//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{
    public static class PathHelper
    {

        /// <summary>
        /// combine base url with sub url.
        /// </summary>
        /// <param name="basestring"></param>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public static string combine(string basestring, string subpath)
        {

            if (subpath.ToLower().StartsWith("http") || string.IsNullOrEmpty(basestring))
            {
                return subpath;
            }

            if (basestring.ToLower().StartsWith("http"))
            {
                Uri baseuri = new Uri(basestring);
                Uri cssuri = new Uri(baseuri, subpath);
                return cssuri.AbsoluteUri;
            }
            else
            {

                if (basestring.EndsWith(@"\") || basestring.EndsWith(@"/"))
                {
                    return System.IO.Path.Combine(basestring, subpath);
                }
                else
                {
                    int lastleft = basestring.LastIndexOf(@"/");
                    int lastright = basestring.LastIndexOf(@"\");

                    string newbase = string.Empty;

                    if (lastleft > lastright)
                    {
                        newbase = basestring.Substring(0, lastleft + 1);
                    }
                    else if (lastright > lastleft)
                    {
                        newbase = basestring.Substring(0, lastright + 1);
                    }
                    else
                    {
                        newbase = basestring;
                    }

                    return System.IO.Path.Combine(newbase, subpath);


                }
            }

        }


    }
}
