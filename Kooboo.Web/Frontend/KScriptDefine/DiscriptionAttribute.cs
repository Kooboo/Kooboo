using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DiscriptionAttribute : Attribute
    {
        public string Discription { get; private set; }
        public DiscriptionAttribute(string discription)
        {
            Discription = discription;
        }
    }
}
