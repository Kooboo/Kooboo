using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Attributes
{
    /// <summary>
    /// skip this member when generate typescript define
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class KIgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// customize method type
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class KDefineTypeAttribute : Attribute
    {
        public Type Return { get; set; }
        public Type[] Params { get; set; }
    }

    /// <summary>
    /// mark field is extension 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class KExtensionAttribute : Attribute
    {

    }

    /// <summary>
    /// assign this[string key] type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class KValueTypeAttribute : Attribute
    {
        public KValueTypeAttribute(Type type)
        {
            Type = type;
        }

        Type Type { get; }
    }
}
