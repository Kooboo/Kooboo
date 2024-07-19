//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS.Tokens
{
    public class hash_token : cssToken
    {

        public hash_token()
        {
            this.Type = enumTokenType.hash;
        }


        public string value;

        public enumHashFlag typeFlag;

    }
}
