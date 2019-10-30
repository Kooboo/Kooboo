//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.Service
{
    public class DownloadEncoding
    {
        public string GetDefaultEncoding(List<string> acceptLanguages)
        {
            string encoding = null;
            foreach (var item in acceptLanguages)
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