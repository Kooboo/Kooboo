//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Tag
{
public    class Sementic
    {

        private static List<string> _classNames;

        /// <summary>
        /// the lowercase name list of common sementic id/class names.
        /// </summary>
        public static List<string> ClassNames
        {
            get
            {
                if (_classNames == null)
                {
                    _classNames = new List<string>();
                    _classNames.Add("sidebar");
                    _classNames.Add("side");
                    _classNames.Add("left");
                    _classNames.Add("right");
                    _classNames.Add("content");
                    _classNames.Add("wrapper");
                    _classNames.Add("container");
                    _classNames.Add("header");
                    _classNames.Add("head");
                    _classNames.Add("footer");
                    _classNames.Add("main");
                    _classNames.Add("site");
                    _classNames.Add("menu");
                    _classNames.Add("body");
                    _classNames.Add("section");
                    _classNames.Add("top");
                    _classNames.Add("nav");
                    _classNames.Add("bar");
                    _classNames.Add("col");    // col based layout css like bootstrap. 
                    //global-width

                }

                return _classNames;

            }
        }
    

    }
}
