//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS.Tokens
{
    /// <summary>
    /// all the CSS token types.
    /// </summary>
    public enum enumTokenType
    {
        //<ident-token>,
        ident,
        //<function-token>, 
        function,
        //<at-keyword-token>,
        at_keyword,
        //<hash-token>, 
        hash,
        // <string-token>, 
        String,
        //<bad-string-token>,
        bad_string,
        // <url-token>,
        url,
        //<bad-url-token>,
        bad_url,
        //<delim-token>,
        delim,
        //<number-token>, 
        number,
        //<percentage-token>, 
        percentage,
        //<dimension-token>, 
        dimension,
        // <unicode-range-token>,
        unicode_range,
        // <include-match-token>, 
        include_match,
        //<dash-match-token>, 
        dash_match,
        //<prefix-match-token>, 
        prefix_match,
        //<suffix-match-token>, 
        suffix_match,
        //<substring-match-token>, 
        substring_match,
        //<column-token>, 
        column,
        //<whitespace-token>, 
        whitespace,
        //<CDO-token>,
        CDO,
        //<CDC-token>,
        CDC,
        //<colon-token>, 
        colon,
        //<semicolon-token>, 
        semicolon,
        //<comma-token>,
        comma,
        //<[-token>, 
        square_bracket_left,
        //<]-token>, 
        square_bracket_right,


        //<(-token>,  
        round_bracket_left,
        //<)-token>, 
        round_bracket_right,
        //<{-token>,
        curly_bracket_left,

        //<}-token>.
        curly_bracket_right,

        //EOD token  A conceptual token representing the end of the list of tokens. Whenever the list of tokens is empty, the next input token is always an EOF-token.
        EOF,

    }
}
