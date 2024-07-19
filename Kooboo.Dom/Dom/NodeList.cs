//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom
{
    [Serializable]
    public class NodeList
    {

        public List<Node> item = new List<Node>();

        public int length
        {
            get
            {
                return item.Count();
            }
        }

    }
}
