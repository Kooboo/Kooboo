//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Dom.CSS.rawmodel
{

    /// <summary>
    /// A declaration has a name, a value consisting of a list of component values, and an important flag which is initially unset.
    /// </summary>
    public class Declaration
    {

        //Declarations are further categorized as "properties" or "descriptors", with the former typically appearing in qualified rules and the latter appearing in at-rules. (This categorization does not occur at the Syntax level; instead, it is a product of where the declaration appears, and is defined by the respective specifications defining the given rule.)

        public Declaration()
        {
            value = new List<ComponentValue>();
            important = false;
        }

        public string name;

        public List<ComponentValue> value;

        public bool important;


    }
}
