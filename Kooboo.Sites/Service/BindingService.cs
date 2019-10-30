//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.Service
{
    public class BindingService
    {
        /// <summary>
        /// when this input contains binding fields or not.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsBinding(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            if (input.Contains("{") && input.Contains("}"))
            {
                return true;
            }

            return false;
        }
    }
}