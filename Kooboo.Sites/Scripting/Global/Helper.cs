using KScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
    public static class Helper
    {
        public static object CleanDynamicObject(object Value)
        {
            if (Value is IDynamicTableObject)
            {
                var dynamictable = Value as IDynamicTableObject;
                return dynamictable.obj;
            }

            return Value;
        }
    }
}
