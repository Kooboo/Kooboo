//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
 public   class DownloadEncoding
    {

        public string GetDefaultEncoding(List<string> AcceptLanguages)
        {
            string encoding = null; 
            foreach (var item in AcceptLanguages)
            {
                if (Kooboo.Dom.EncodingDetector.DefaultEncodingSet.TryGetValue(item, out encoding))
                {
                    return encoding; 
                }
            }
            return null; 
        }         

    }
}
