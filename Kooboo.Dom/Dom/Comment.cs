//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{
    [Serializable]
    public class Comment : CharacterData
    {
        public Comment()
        {
            nodeType = enumNodeType.COMMENT;
        }

        public Comment(string data) : this()
        {
            this.appendData(data);
        }

    }
}
