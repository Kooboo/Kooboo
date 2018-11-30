using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Api.Methods
{
  public interface  IDynamicMethod
    { 
        MethodInfo GetMethod(string name);  
    }
}
