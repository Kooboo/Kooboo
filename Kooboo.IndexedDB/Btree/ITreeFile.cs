using System;

namespace Kooboo.IndexedDB.BTree
{
    public interface ITreeFile
    {
        TreeNode ReadNode(Int64 diskPosition);

        TreeConfig config { get; set; }

        void CreateFirstLeaf(TreeNode ParentNode);

        MemoryTreeNode RootNode { get; set; }
    }
}
