using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Attributes
{
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Field | AttributeTargets.Method)] 
   public class SummaryIgnore : Attribute
    {

    }
}
