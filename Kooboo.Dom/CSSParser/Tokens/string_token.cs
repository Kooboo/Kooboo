//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS.Tokens
{
    public class string_token : cssToken
    {

        public string_token()
        {
            this.Type = enumTokenType.String;
        }

        public string value;
    }
}
