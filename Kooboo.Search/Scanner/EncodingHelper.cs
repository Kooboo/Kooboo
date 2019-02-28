//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Search.Scanner
{
public  static  class EncodingHelper
    {
        private static List<Encoding> _list; 
        public static List<Encoding> List
        {
            get
            {
                if (_list == null)
                {
                    _list = new List<Encoding>();
                    foreach (var item in System.Text.Encoding.GetEncodings())
                    {
                       _list.Add(item.GetEncoding()); 
                    }
                }
                return List; 
            }
        } 
    }
}
