using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
    public class KText
    {
        public string RemoveHtml(string input)
        {
            return Kooboo.Search.Utility.RemoveHtml(input);
        }

        public string SementicSubString(string input, int start, int count)
        {
            if (start < 0)
            {
                start = 0;
            } 
            return Lib.Helper.StringHelper.SementicSubString(input, start, count);
        }

        public string SubString(string input, int start, int count)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (start <0)
            {
                start = 0; 
            }

            return input.Substring(start, count); 

        }

        public string Replace(string input, string oldValue, string newValue)
        {
            return Lib.Helper.StringHelper.ReplaceIgnoreCase(input, oldValue, newValue);
        }

        public string Remove(string input, string ValueToRemove)
        {
            return Lib.Helper.StringHelper.ReplaceIgnoreCase(input, ValueToRemove, "");
        }
    }
}
