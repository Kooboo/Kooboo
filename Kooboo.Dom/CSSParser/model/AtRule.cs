//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS.rawmodel
{
    /// <summary>
    /// An at-rule has a name, a prelude consisting of a list of component values, and an optional block consisting of a simple {} block.
    /// </summary>
    public class AtRule : Rule
    {
        //Note: This specification places no limits on what an at-rule’s block may contain. Individual at-rules must define whether they accept a block, and if so, how to parse it (preferably using one of the parser algorithms or entry points defined in this specification).


        public AtRule()
        {
            this.prelude =  new List<ComponentValue>();
            this.Type = enumRuleType.AtRule;
        }

        public string name;

    }
}
