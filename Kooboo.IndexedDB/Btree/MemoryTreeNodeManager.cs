//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.Btree
{
    public class MemoryTreeNodeManager
    {
        /// <summary>
        /// Find by key, return the lowerest node...
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="currentNode"></param>
        /// <param name="keybytes"></param>
        /// <returns></returns>
        public static MemoryTreeNode FindLeafByKey(TreeFile tree, MemoryTreeNode currentNode, byte[] keybytes)
        {
            NodePointer pointer = currentNode.TreeNode.FindPointer(keybytes);

            if (pointer.PositionPointer == 0)
            {
                tree.CreateFirstLeaf(currentNode.TreeNode);
                NodeChange(tree, currentNode);
                return FindLeafByKey(tree, tree.RootCache, keybytes);
            }

            MemoryTreeNode foundnode = GetOrLoadNode(tree, currentNode, pointer);

            if (foundnode == null || foundnode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
            {
                return foundnode;
            }
            else
            {
                return FindLeafByKey(tree, foundnode, keybytes);
            }
        }

        public static MemoryTreeNode FindFirstLeaf(TreeFile treeFile, MemoryTreeNode currentNode = null)
        {
            if (currentNode == null)
            {
                currentNode = treeFile.RootCache;
            }

            NodePointer pointer = new NodePointer {PointerBytes = currentNode.TreeNode.PreviousPointer};

            if (pointer.PositionPointer == 0)
            {
                var key = KeyFinder.FindFirstKey(currentNode.TreeNode.KeyArray, treeFile.comparer);
                if (key == null)
                {
                    return null;
                }
                pointer = currentNode.TreeNode.FindPointer(key);
            }

            if (pointer.PointerBytes != null && pointer.PositionPointer > 0 && pointer.Indicator != EnumValues.TypeIndicator.leaf)
            {
                var newcachenode = GetOrLoadNode(treeFile, currentNode, pointer);
                return FindFirstLeaf(treeFile, newcachenode);
            }

            //find the leaf that contains the key.
            if (pointer.PointerBytes != null && pointer.PositionPointer > 0 && pointer.Indicator == EnumValues.TypeIndicator.leaf)
            {
                MemoryTreeNode newcacheleaf = GetOrLoadNode(treeFile, currentNode, pointer);
                return newcacheleaf;
            }

            return null;
        }

        public static MemoryTreeNode FindLastLeaf(TreeFile treeFile, MemoryTreeNode currentNode = null)
        {
            if (currentNode == null)
            {
                currentNode = treeFile.RootCache;
            }

            NodePointer pointer = new NodePointer();
            var key = KeyFinder.FindLastKey(currentNode.TreeNode.KeyArray, treeFile.comparer);

            if (key == null)
            {
                pointer.PointerBytes = currentNode.TreeNode.PreviousPointer;
            }
            else
            {
                pointer = currentNode.TreeNode.FindPointer(key);
            }

            if (pointer.PositionPointer > 0 && pointer.Indicator != EnumValues.TypeIndicator.leaf)
            {
                MemoryTreeNode newcachenode = GetOrLoadNode(treeFile, currentNode, pointer);
                return FindLastLeaf(treeFile, newcachenode);
            }

            //find the leaf that contains the key.
            if (pointer.PointerBytes != null && pointer.PositionPointer > 0 && pointer.Indicator == EnumValues.TypeIndicator.leaf)
            {
                MemoryTreeNode newcacheleaf = GetOrLoadNode(treeFile, currentNode, pointer);
                return newcacheleaf;
            }
            return null;
        }

        public static void NodeChange(TreeFile tree, MemoryTreeNode node)
        {
            if (node == null)
            {
                return;
            }
            if (node.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                var treenode = tree.ReadNode(node.TreeNode.DiskPosition);
                var newrootcache = new MemoryTreeNode(treenode);
                tree.RootCache = newrootcache;
            }
            else
            {
                var parent = node.Parent;
                if (parent != null)
                {
                    if (node.IsParentPreviousPointer)
                    {
                        parent.PreviousPointer = null;
                    }
                    else
                    {
                        parent.Children.Remove(node.TreeNode.DiskPosition);
                    }
                }
            }
            node = null;
        }

        public static void NodeReload(TreeFile tree, MemoryTreeNode node)
        {
            var newnode = tree.ReadNode(node.TreeNode.DiskPosition);
            node.TreeNode = newnode;
        }

        public static MemoryTreeNode GetOrLoadNode(TreeFile treeFile, MemoryTreeNode parentNode, NodePointer pointer)
        {
            if (pointer.IsFirstPreviousPointer)
            {
                if (parentNode.PreviousPointer != null)
                {
                    return parentNode.PreviousPointer;
                }
                else
                {
                    TreeNode node = treeFile.ReadNode(pointer.PositionPointer);
                    if (node == null)
                    {
                        return null;
                    }

                    MemoryTreeNode newcachenode = new MemoryTreeNode(node)
                    {
                        Parent = parentNode,
                        IsParentPreviousPointer = pointer.IsFirstPreviousPointer,
                        ParentNodeKey = pointer.KeyToPosition,
                        Level = parentNode.Level + 1
                    };

                    if (newcachenode.Level < treeFile.MaxCacheLevel)
                    {
                        parentNode.PreviousPointer = newcachenode;
                    }
                    return newcachenode;
                }
            }
            else
            {
                if (parentNode.Children.ContainsKey(pointer.PositionPointer))
                {
                    return parentNode.Children[pointer.PositionPointer];
                }
                else
                {
                    TreeNode node = treeFile.ReadNode(pointer.PositionPointer);
                    if (node == null)
                    {
                        return null;
                    }

                    MemoryTreeNode newcachenode = new MemoryTreeNode(node)
                    {
                        Parent = parentNode,
                        IsParentPreviousPointer = pointer.IsFirstPreviousPointer,
                        ParentNodeKey = pointer.KeyToPosition,
                        Level = parentNode.Level + 1
                    };

                    if (newcachenode.Level < treeFile.MaxCacheLevel)
                    {
                        parentNode.Children[pointer.PositionPointer] = newcachenode;
                    }
                    return newcachenode;
                }
            }
        }

        public static MemoryTreeNode FindNextLeaf(TreeFile treeFile, MemoryTreeNode currentLeaf)
        {
            if (currentLeaf.TreeNode.TypeIndicator != EnumValues.TypeIndicator.leaf)
            {
                throw new Exception("this method only accept leaf");
            }
            byte[] key = KeyFinder.FindSmallestBiggerKey(currentLeaf.ParentNodeKey, currentLeaf.Parent.TreeNode.KeyArray, treeFile.comparer);

            while (key != null)
            {
                var pointer = currentLeaf.Parent.TreeNode.FindPointer(key);
                if (pointer != null && pointer.PositionPointer > 0)
                {
                    var nextleaf = GetOrLoadNode(treeFile, currentLeaf.Parent, pointer);
                    if (nextleaf != null && nextleaf.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
                    {
                        return nextleaf;
                    }
                }

                key = KeyFinder.FindSmallestBiggerKey(key, currentLeaf.Parent.TreeNode.KeyArray, treeFile.comparer);
            }

            return GetUpLinkFirstLeaf(treeFile, currentLeaf.Parent);
        }

        private static MemoryTreeNode GetUpLinkFirstLeaf(TreeFile treeFile, MemoryTreeNode parentNode)
        {
            if (parentNode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                return null;
            }

            MemoryTreeNode siblingNode;
            byte[] siblingkey = KeyFinder.FindSmallestBiggerKey(parentNode.ParentNodeKey, parentNode.Parent.TreeNode.KeyArray, treeFile.comparer);

            while (siblingkey != null)
            {
                var pointer = parentNode.Parent.TreeNode.FindPointer(siblingkey);
                if (pointer != null && pointer.PositionPointer > 0)
                {
                    siblingNode = GetOrLoadNode(treeFile, parentNode.Parent, pointer);

                    var result = FindContainerFirstLeaf(treeFile, siblingNode);
                    if (result != null)
                    { return result; }
                }
                siblingkey = KeyFinder.FindSmallestBiggerKey(siblingkey, parentNode.Parent.TreeNode.KeyArray, treeFile.comparer);
            }

            return GetUpLinkFirstLeaf(treeFile, parentNode.Parent);
        }

        private static MemoryTreeNode FindContainerFirstLeaf(TreeFile treeFile, MemoryTreeNode containerNode)
        {
            if (containerNode == null)
            {
                return null;
            }
            if (containerNode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
            {
                return containerNode;
            }

            NodePointer pointer = new NodePointer {PointerBytes = containerNode.TreeNode.PreviousPointer};


            if (pointer.PositionPointer > 0)
            {
                var subnode = GetOrLoadNode(treeFile, containerNode, pointer);
                if (subnode != null)
                {
                    var result = FindContainerFirstLeaf(treeFile, subnode);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            // did not get return, try one key by one key.
            byte[] key = KeyFinder.FindSmallestBiggerKey(null, containerNode.TreeNode.KeyArray, treeFile.comparer);

            while (key != null)
            {
                var nodepointer = containerNode.TreeNode.FindPointer(key);

                if (nodepointer != null && nodepointer.PositionPointer > 0)
                {
                    var keynode = GetOrLoadNode(treeFile, containerNode, nodepointer);
                    if (keynode != null)
                    {
                        var result = FindContainerFirstLeaf(treeFile, keynode);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }

                key = KeyFinder.FindSmallestBiggerKey(key, containerNode.TreeNode.KeyArray, treeFile.comparer);
            }

            return null;
        }

        public static MemoryTreeNode FindPreviousLeaf(TreeFile treeFile, MemoryTreeNode currentLeaf)
        {
            if (currentLeaf.TreeNode.TypeIndicator != EnumValues.TypeIndicator.leaf)
            {
                throw new Exception("this method only accept leaf");
            }

            if (currentLeaf.IsParentPreviousPointer && currentLeaf.ParentNodeKey == null)
            {
                return GetUpLinkLastLeaf(treeFile, currentLeaf.Parent);
            }

            byte[] key = KeyFinder.FindBiggestSmallerKey(currentLeaf.ParentNodeKey, currentLeaf.Parent.TreeNode.KeyArray, treeFile.comparer);

            while (key != null)
            {
                var pointer = currentLeaf.Parent.TreeNode.FindPointer(key);
                if (pointer != null && pointer.PositionPointer > 0)
                {
                    var nextleaf = GetOrLoadNode(treeFile, currentLeaf.Parent, pointer);
                    if (nextleaf != null && nextleaf.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
                    {
                        return nextleaf;
                    }
                }
                key = KeyFinder.FindBiggestSmallerKey(key, currentLeaf.Parent.TreeNode.KeyArray, treeFile.comparer);
            }

            // check the previous pointer.
            NodePointer previousPointer = new NodePointer();
            previousPointer.PointerBytes = currentLeaf.Parent.TreeNode.PreviousPointer;

            var previousPointerLeaf = GetOrLoadNode(treeFile, currentLeaf.Parent, previousPointer);
            if (previousPointerLeaf != null && previousPointerLeaf.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
            {
                return previousPointerLeaf;
            }

            return GetUpLinkLastLeaf(treeFile, currentLeaf.Parent);
        }

        private static MemoryTreeNode GetUpLinkLastLeaf(TreeFile treeFile, MemoryTreeNode parentNode)
        {
            if (parentNode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                return null;
            }

            MemoryTreeNode siblingNode;
            byte[] siblingkey = KeyFinder.FindBiggestSmallerKey(parentNode.ParentNodeKey, parentNode.Parent.TreeNode.KeyArray, treeFile.comparer);

            while (siblingkey != null)
            {
                var pointer = parentNode.Parent.TreeNode.FindPointer(siblingkey);
                if (pointer != null && pointer.PositionPointer > 0)
                {
                    siblingNode = GetOrLoadNode(treeFile, parentNode.Parent, pointer);

                    var result = FindContainerLastLeaf(treeFile, siblingNode);
                    if (result != null)
                    { return result; }
                }
                siblingkey = KeyFinder.FindBiggestSmallerKey(siblingkey, parentNode.Parent.TreeNode.KeyArray, treeFile.comparer);
            }

            // need to check the first previous pointer after process all key arrays.
            NodePointer previousPointer = new NodePointer {PointerBytes = parentNode.Parent.TreeNode.PreviousPointer};

            var previousPointerNode = GetOrLoadNode(treeFile, parentNode.Parent, previousPointer);
            //parentNode.Parent.PreviousPointerNode can't be the same with ParentNode,otherwise it will cause a dead cycle.
            if (previousPointerNode != null && parentNode != previousPointerNode)
            {
                var result = FindContainerLastLeaf(treeFile, previousPointerNode);
                if (result != null)
                { return result; }
            }

            return GetUpLinkLastLeaf(treeFile, parentNode.Parent);
        }

        private static MemoryTreeNode FindContainerLastLeaf(TreeFile treeFile, MemoryTreeNode containerNode)
        {
            if (containerNode == null)
            {
                return null;
            }
            if (containerNode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
            {
                return containerNode;
            }

            // did not get return, try one key by one key.
            byte[] key = KeyFinder.FindLastKey(containerNode.TreeNode.KeyArray, treeFile.comparer);

            while (key != null)
            {
                var nodepointer = containerNode.TreeNode.FindPointer(key);

                if (nodepointer != null && nodepointer.PositionPointer > 0)
                {
                    var keynode = GetOrLoadNode(treeFile, containerNode, nodepointer);
                    if (keynode != null)
                    {
                        var result = FindContainerFirstLeaf(treeFile, keynode);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }

                key = KeyFinder.FindBiggestSmallerKey(key, containerNode.TreeNode.KeyArray, treeFile.comparer);
            }

            NodePointer pointer = new NodePointer {PointerBytes = containerNode.TreeNode.PreviousPointer};


            if (pointer.PositionPointer > 0)
            {
                var subnode = GetOrLoadNode(treeFile, containerNode, pointer);
                if (subnode != null)
                {
                    var result = FindContainerFirstLeaf(treeFile, subnode);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}