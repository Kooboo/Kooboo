using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Models
{
    /// <summary>
    /// skip this member when generate typescript define
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DefineTypeAttribute : Attribute
    {
        public Type Return { get; set; }
        public Type[] Params { get; set; }
    }
}
