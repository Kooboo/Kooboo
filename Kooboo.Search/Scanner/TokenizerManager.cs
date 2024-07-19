//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;

namespace Kooboo.Search.Scanner
{
    public static class TokenizerManager
    {
        public static Itokenizer GetTokenizer(string input)
        {
            return new DefaultTokenizer();
        }

        public static Itokenizer GetTokenizer(Encoding encoding)
        {
            return new DefaultTokenizer();
        }
    }
}
