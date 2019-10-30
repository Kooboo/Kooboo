//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.InlineEditor.Converter;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.SiteTransfer.Continue
{
    public class ContinueTask
    {
        public static void Convert(SiteDb siteDb, SiteObject downloadObject)
        {
            if (siteDb.WebSite.ContinueConvert)
            {
                if (downloadObject is Page page)
                {
                    var continueconverters = siteDb.ContinueConverter.All();

                    var converters = ConvertManager.GetConverters();

                    foreach (var item in continueconverters)
                    {
                        var converter = converters.Find(o => o.Type == item.ConvertType);
                        if (converter != null && converter is IContinueConverter continueconverter)
                        {
                            continueconverter.ContinueConvert(siteDb, item.OriginalPageId, item.ConvertedTag, item.ObjectNameOrId, item.KoobooId, page, item.ElementPaths);
                        }
                    }
                }
            }
        }
    }
}