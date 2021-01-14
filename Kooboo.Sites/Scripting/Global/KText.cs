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
          return  Lib.Helper.StringHelper.SementicSubString(input, start, count); 
        }

        public string Replace(string input, string oldValue, string newValue)
        {
            return Lib.Helper.StringHelper.ReplaceIgnoreCase(input, oldValue, newValue);  
        }
    }
}
