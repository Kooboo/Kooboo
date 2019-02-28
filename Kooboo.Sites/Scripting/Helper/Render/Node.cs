//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Helper
{
    public class Node
    {
        public string ShowName { get; set; }
        public string Text
        {
            get
            {
                return DocumentHelper.LowerCaseFirstChar(ShowName);
            }
        }

        public NodeState State
        {
            get
            {
                return new NodeState();
            }
        }
        public SettingBase setting { get; set; }
        public List<Node> Nodes { get; set; }

        public string Url { get; set; }
    }
    public class NodeState
    {
        public bool Expanded { get; set; } = true;
    }
}
