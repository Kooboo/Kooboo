//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.SiteTransfer.Model;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class ContinueConvertRepository : SiteRepositoryBase<ContinueConverter>
    {
        public void AddConverter(string convertType, Guid originalPageId, string convertedTag, string objectNameOrId, string koobooId, List<string> elementPaths, string elementTag)
        {
            ContinueConverter converter = new ContinueConverter
            {
                ConvertType = convertType,
                OriginalPageId = originalPageId,
                ConvertedTag = convertedTag,
                ObjectNameOrId = objectNameOrId,
                KoobooId = koobooId,
                ElementPaths = elementPaths,
                ElementTag = elementTag
            };
            this.AddOrUpdate(converter);
        }
    }
}