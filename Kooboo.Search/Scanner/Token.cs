//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Search.Scanner
{
    public class Token
    {
        public Token(string input)
        {
            this.Value = input;
        }
        // the token char value. one or more chars. 
        public string Value { get; set; }

        // the revelentindicator if only return unique token. 
        public int RevelentIndicator { get; set; } = 1;
    }
}
