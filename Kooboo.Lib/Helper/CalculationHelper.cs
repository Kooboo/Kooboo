//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Lib.Helper
{
    public static class CalculationHelper
    {
        public static int GetPageCount(int total, int pagesize)
        {
            if (total == 0 || pagesize == 0)
            {
                return 0;
            }
            int totalpage = total / pagesize;

            if (totalpage * pagesize < total)
            {
                return totalpage + 1;
            }
            return totalpage;
        }
    }
}
