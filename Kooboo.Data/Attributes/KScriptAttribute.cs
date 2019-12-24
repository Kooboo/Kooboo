using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Attributes
{
    /// <summary>
    /// skip this member when generate typescript define
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// customize method type
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DefineTypeAttribute : Attribute
    {
        public Type Return { get; set; }
        public Type[] Params { get; set; }
    }
}
