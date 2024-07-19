//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Helper
{
    public static class IDHelper
    {

        public static int ToInt(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }
            var id = Lib.Security.Hash.ComputeInt(input);
            if (id < 0)
            {
                return 0 - id;
            }

            if (id == 0)
            {
                return ToInt(input + "0");
            }
            return id;
        }
    }
}
