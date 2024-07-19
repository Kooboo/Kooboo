//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS.Tokens
{
    public class unicode_range_token : cssToken
    {

        public unicode_range_token()
        {
            this.Type = enumTokenType.unicode_range;

        }

        public int start;

        public int end;

    }
}
