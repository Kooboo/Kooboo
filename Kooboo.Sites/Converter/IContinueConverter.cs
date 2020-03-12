//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Converter
{
  public  interface IContinueConverter
    {

        void ContinueConvert(SiteDb siteDb, Guid OriginalPageId, string ConvertedTag,string ObjectNameOrId, string KoobooId, Page CurrentPage, List<string> ElementPath);  
    }
}
