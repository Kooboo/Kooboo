//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo
{
   public static class Constants
    { 
        public static class Site
        {
            public const string DefaultContentType = "text/html; charset=UTF-8";

            public const string PageInlineParameter = "koobooinline";

            public const string AlternativeViewQueryName = "koobooalterview";

            public const string PageDesignParameterValue = "design";
            public const string PageDraftParameterValue = "draft";
            public const string PageSettingParameterValue = "pagesetting";

            public const string PageAnalyzeParameterValue = "analyze";

            public const string PageDesignVersion = "koobooversion";

            public const string PageTaskIdParameter = "taskId";

            public static class RequestChannel
            {
                public const string Design = "design";

                public const string Draft = "draft"; 
            }

        }
  
    }
}
