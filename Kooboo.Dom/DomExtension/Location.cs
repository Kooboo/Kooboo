//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{

    /// <summary>
    /// The calculated X, Y location of a CSS dom element. 
    /// </summary>
    [Serializable]
    public struct Location
    {

        public int x;
        public int y;

        // the position in the file. 

        public int openTokenStartIndex;   // the indexof position of start tag in the string. 
        public int openTokenEndIndex;   // the last position of end tag in the string. 

        public int endTokenStartIndex;   // the indexof position of start tag in the string. 
        public int endTokenEndIndex;   // the last position of end tag in the string. 

    }
}
