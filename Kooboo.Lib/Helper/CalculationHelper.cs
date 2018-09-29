//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
   public static class CalculationHelper
    {
        public static int GetPageCount(int total, int pagesize)
        {
            if (total ==0 || pagesize == 0)
            {
                return 0; 
            }
            int totalpage= total / pagesize; 

            if (totalpage *pagesize <total)
            {
                return totalpage + 1; 
            }
            return totalpage; 
        }
    }
}
