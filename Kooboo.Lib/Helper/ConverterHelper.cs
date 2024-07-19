//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Lib.Helper
{
    public static class ConverterHelper
    {
        public static Guid GetGuid(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default(Guid);
            }

            Guid result = default(Guid);

            System.Guid.TryParse(input, out result);

            return result;
        }
    }
}
