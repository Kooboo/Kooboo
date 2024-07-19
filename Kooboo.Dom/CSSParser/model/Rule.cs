//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Dom.CSS.rawmodel
{
    public class Rule
    {
        public List<ComponentValue> prelude;

        public SimpleBlock block;

        public enumRuleType Type;

        /// <summary>
        ///  the start position on the underlining css text string.
        /// </summary>
        public int startindex;

        /// <summary>
        /// the end position on the underlining css text string.
        /// </summary>
        public int endindex;

    }
}
