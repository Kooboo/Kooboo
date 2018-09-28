using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.CustomAttributes
{
     
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KoobooIgnore : Attribute
    {

    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KoobooKeyIgnoreCase : Attribute
    {
    }

}
