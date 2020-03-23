using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    static class StringBuilderUtils
    {
        public static StringBuilder AppendLLine(this StringBuilder sb, string aString)
            => sb.Append(aString).Append("\n");
    }
}
