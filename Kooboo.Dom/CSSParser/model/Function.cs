//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;
using Kooboo.Dom.CSS.Tokens;

namespace Kooboo.Dom.CSS.rawmodel
{
    /// <summary>
    /// A function has a name and a value consisting of a list of component values.
    /// </summary>
    public class Function : ComponentValue
    {
        public Function()
        {
            this.Type = CompoenentValueType.function;
            this.startindex = -1;
            this.endindex = -1;
        }

        public string name;

        public List<ComponentValue> value = new List<ComponentValue>();


        public string getString(ref string CssText)
        {
            string valuelist = string.Empty;

            foreach (var item in this.value)
            {
                if (item.Type == CompoenentValueType.preservedToken)
                {
                    PreservedToken pretoken = item as PreservedToken;
                    valuelist += pretoken.token.GetString(ref CssText);
                }
                else if (item.Type == CompoenentValueType.function)
                {
                    Function func = item as Function;
                    valuelist += func.getString(ref CssText);

                }
                else if (item.Type == CompoenentValueType.simpleBlock)
                {
                    SimpleBlock simpleblock = item as SimpleBlock;
                    valuelist += simpleblock.getString(ref CssText);
                }
            }

            return this.name + "(" + valuelist + ")";

        }
    }
}
