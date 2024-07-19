//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS.Tokens
{
    public class number_token : cssToken
    {

        public number_token()
        {
            this.Type = enumTokenType.number;
        }


        public string representation;

        public double Number;

        public enumNumericType TypeFlag;
    }
}
