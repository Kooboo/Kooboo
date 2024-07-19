//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree
{
    public class MemoryTreeNode
    {
        /// <summary>
        /// The level of this node. root =0
        /// </summary>
        public int Level { get; set; }

        public byte[] ParentNodeKey { get; set; }

        public byte[] KeyBytes { get; set; }

        public MemoryTreeNode Parent { get; set; }

        /// <summary>
        /// Current tree node. 
        /// </summary>
        public TreeNode TreeNode { get; set; }

        private Dictionary<long, MemoryTreeNode> _children;
        /// <summary>
        /// long is the disk position, or the positionPointer. 
        /// </summary>
        public Dictionary<long, MemoryTreeNode> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new Dictionary<long, MemoryTreeNode>();
                }
                return _children;
            }
            set
            { _children = value; }
        }

        public void AddChild(MemoryTreeNode memorynode)
        {
            memorynode.Parent = this;
            memorynode.Level = this.Level + 1;
            this.Children.Add(memorynode.TreeNode.DiskPosition, memorynode);
        }

        public MemoryTreeNode PreviousPointer { get; set; }

        /// <summary>
        /// is this node belong to the PreviousPointer of parent. 
        /// </summary>
        public bool IsParentPreviousPointer { get; set; }

        public MemoryTreeNode(TreeNode treenode)
        {
            this.TreeNode = treenode;
        }
    }

}
