//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS.Tokens
{
    public class cssToken
    {
        public enumTokenType Type { get; set; }

        /// <summary>
        /// the  start index position of this token in the css text. 
        /// </summary>
        public int startIndex;

        /// <summary>
        /// the end index position of this token in the css text. 
        /// </summary>
        public int endIndex;



    }
}
