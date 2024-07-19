//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Dom.CSS.rawmodel
{
    /// <summary>
    /// A qualified rule has a prelude consisting of a list of component values, and a block consisting of a simple {} block.
    /// </summary>
    public class QualifiedRule : Rule
    {
        //Note: Most qualified rules will be style rules, where the prelude is a selector [SELECT] and the block a list of declarations.
        public QualifiedRule()
        {
            this.prelude = new List<ComponentValue>();
            this.Type = enumRuleType.QualifiedRule;
        }



    }
}
