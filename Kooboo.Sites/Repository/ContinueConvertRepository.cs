//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.SiteTransfer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Repository
{
    public class ContinueConvertRepository : SiteRepositoryBase<ContinueConverter>
    {

        public void AddConverter(string ConvertType, Guid OriginalPageId, string ConvertedTag, string ObjectNameOrId, string KoobooId, List<string> ElementPaths, string ElementTag)
        { 
            ContinueConverter converter = new ContinueConverter();
            converter.ConvertType = ConvertType; 
            converter.OriginalPageId = OriginalPageId;
            converter.ConvertedTag = ConvertedTag;
            converter.ObjectNameOrId = ObjectNameOrId;
            converter.KoobooId = KoobooId;
            converter.ElementPaths = ElementPaths;
            converter.ElementTag = ElementTag; 
            this.AddOrUpdate(converter);  
        }

    }
}
