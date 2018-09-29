//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;
using System.Globalization;

namespace Kooboo.Sites.Contents
{
    public static class UserKeyHelper
    {
        public static string ToSafeUserKey(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            input = RemoveDiacritics(input);
            input = input.Replace("+", " ");
            //input = input.Replace("-", " ");
            input = input.Replace("!", " ");
            input = input.Replace("?", " ");
            input = input.Replace("@", " ");
            input = input.Replace("%", " ");
            input = input.Replace(":", " ");
            input = input.Replace("=", " ");
            input = input.Replace("#", " "); 
            input = input.Replace("$", " ");
            input = input.Replace("  ", " ");
            input = input.Replace(" ", "-");
            // 空格  !	#	$	%	+	@	:	=	? 
            string back =  System.Net.WebUtility.UrlEncode(input);
            back = back.Replace("%", "");
            return back; 
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
