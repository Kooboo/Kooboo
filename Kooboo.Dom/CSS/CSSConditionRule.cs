//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS
{

    /// <summary>
    /// An object implementing the CSSConditionRule interface represents a single 
    /// condition CSS at-rule, which consists of a condition and a statement block. 
    /// It is a child of CSSGroupingRule.
    /// </summary>
   [Serializable]
   public   class CSSConditionRule : CSSGroupingRule
    {
        public string conditionText;
    }
}
