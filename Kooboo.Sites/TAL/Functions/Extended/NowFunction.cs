using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.TAL.Functions;

namespace Kooboo.TAL.Extended
{
   public  class NowFunction : IFunction
    {
        public string Name
        {
            get { return "now"; }
        }

        public string Description
        {
            get { return "the current date time value."; }
        }

        public object Execute(params object[] paras)
        {
            return DateTime.Now;
        }
    }
}
