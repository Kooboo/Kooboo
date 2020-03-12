//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Converter;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;


namespace Kooboo.Sites.SiteTransfer.Continue
{
   public class ContinueTask
    {
        public static void Convert(SiteDb SiteDb, SiteObject DownloadObject)
        {
            if (SiteDb.WebSite.ContinueConvert)
            {
                if (DownloadObject is  Page)
                {
                    var page = DownloadObject as Page;
                    var continueconverters = SiteDb.ContinueConverter.All();

                    var converters =  ConvertManager.GetConverters(); 
                     
                    foreach (var item in continueconverters)
                    {
                        var converter = converters.Find(o => o.Type == item.ConvertType);
                        if (converter != null && converter is IContinueConverter)
                        {
                            IContinueConverter continueconverter = converter as IContinueConverter;
                            continueconverter.ContinueConvert(SiteDb, item.OriginalPageId,  item.ConvertedTag, item.ObjectNameOrId, item.KoobooId, page, item.ElementPaths); 
                        }
                    }
                }
            }
        }
    }
}
