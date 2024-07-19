//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS.Tokens
{
    public static class TokenExtension
    {

        /// <summary>
        /// this is only used to insert like selectorText into the CssRule. It is not so much of useful.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetString(this cssToken input, ref string OriginalCss)
        {
            if (input.startIndex > -1 && input.endIndex > -1 && input.endIndex < OriginalCss.Length)
            {
                int len = input.endIndex - input.startIndex;
                if (len >= 0)
                {
                    return OriginalCss.Substring(input.startIndex, len + 1);
                }
            }

            switch (input.Type)
            {
                case enumTokenType.ident:
                    ident_token token = input as ident_token;
                    return token.value;

                case enumTokenType.function:
                    function_token functoken = input as function_token;
                    return functoken.Value;

                case enumTokenType.at_keyword:
                    at_keyword_token keywordtoken = input as at_keyword_token;
                    return keywordtoken.value;

                case enumTokenType.hash:
                    hash_token hashtoken = input as hash_token;
                    return "#" + hashtoken.value;

                ///TOBE considered, should not without "
                case enumTokenType.String:
                    string_token stringtoken = input as string_token;
                    return "\"" + stringtoken.value + "\"";

                case enumTokenType.bad_string:
                    return string.Empty;

                case enumTokenType.url:
                    url_token urltoken = input as url_token;
                    return urltoken.value;

                case enumTokenType.bad_url:
                    return string.Empty;

                case enumTokenType.delim:
                    delim_token delim = input as delim_token;
                    return delim.value.ToString();

                case enumTokenType.number:
                    number_token number = input as number_token;
                    return number.representation;

                case enumTokenType.percentage:
                    percentage_token percentage = input as percentage_token;
                    return percentage.representation;

                case enumTokenType.dimension:
                    dimension_token dimension = input as dimension_token;
                    return dimension.representation + dimension.unit;


                //TODO: to be checked.
                case enumTokenType.unicode_range:
                    return string.Empty;

                case enumTokenType.include_match:
                    return string.Empty;

                case enumTokenType.dash_match:
                    return string.Empty;

                case enumTokenType.prefix_match:
                    return string.Empty;

                case enumTokenType.suffix_match:
                    return string.Empty;

                case enumTokenType.substring_match:
                    return string.Empty;

                case enumTokenType.column:
                    return string.Empty;

                case enumTokenType.whitespace:
                    return " ";

                case enumTokenType.CDO:
                    return string.Empty;

                case enumTokenType.CDC:
                    return string.Empty;

                case enumTokenType.colon:
                    return ":";

                case enumTokenType.semicolon:
                    return ";";

                case enumTokenType.comma:
                    return ",";

                case enumTokenType.square_bracket_left:
                    return "[";
                case enumTokenType.square_bracket_right:
                    return "]";
                case enumTokenType.round_bracket_left:
                    return "(";
                case enumTokenType.round_bracket_right:
                    return ")";
                case enumTokenType.curly_bracket_left:
                    return "{";
                case enumTokenType.curly_bracket_right:
                    return "}";

                case enumTokenType.EOF:
                    return string.Empty;
                default:
                    return string.Empty;
            }


        }

    }
}
