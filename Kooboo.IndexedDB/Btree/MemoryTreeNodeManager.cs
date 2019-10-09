//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Btree
{
    public class MemoryTreeNodeManager
    { 
        /// <summary>
        /// Find by key, return the lowerest node... 
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static MemoryTreeNode FindLeafByKey(TreeFile Tree, MemoryTreeNode currentNode, byte[] Keybytes)
        {
            NodePointer pointer = currentNode.TreeNode.FindPointer(Keybytes);

            if (pointer.PositionPointer == 0)
            {
                Tree.CreateFirstLeaf(currentNode.TreeNode);
                NodeChange(Tree, currentNode); 
                return FindLeafByKey(Tree, Tree.RootCache, Keybytes); 
            }

            MemoryTreeNode foundnode = GetOrLoadNode(Tree, currentNode, pointer);
             
            if (foundnode == null || foundnode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
            {
                return foundnode;
            }
            else
            {
                return FindLeafByKey(Tree, foundnode, Keybytes);
            }
        }
        
        public static MemoryTreeNode FindFirstLeaf(TreeFile TreeFile, MemoryTreeNode currentNode= null)
        {
            if (currentNode == null)
            {
                currentNode = TreeFile.RootCache;
            }
 
            NodePointer pointer = new NodePointer();
            pointer.PointerBytes = currentNode.TreeNode.PreviousPointer;

            if (pointer.PositionPointer == 0)
            {
                var key = KeyFinder.FindFirstKey(currentNode.TreeNode.KeyArray, TreeFile.comparer); 
                if (key ==null)
                {
                    return null;
                }
                pointer = currentNode.TreeNode.FindPointer(key);
            }
             
            if (pointer.PointerBytes != null && pointer.PositionPointer > 0 && pointer.Indicator != EnumValues.TypeIndicator.leaf)
            {
                var newcachenode = GetOrLoadNode(TreeFile, currentNode, pointer); 
                return FindFirstLeaf(TreeFile, newcachenode); 
            }

            ///find the leaf that contains the key. 
            if (pointer.PointerBytes != null && pointer.PositionPointer > 0 && pointer.Indicator == EnumValues.TypeIndicator.leaf)
            { 
                MemoryTreeNode newcacheleaf = GetOrLoadNode(TreeFile, currentNode, pointer); 
                return newcacheleaf; 
            }
      
             return null;  
        }

        public static MemoryTreeNode FindLastLeaf(TreeFile TreeFile, MemoryTreeNode currentNode = null)
        {
            if (currentNode == null)
            {
                currentNode = TreeFile.RootCache;
            } 

            NodePointer pointer = new NodePointer();
            var key = KeyFinder.FindLastKey(currentNode.TreeNode.KeyArray, TreeFile.comparer);

            if (key == null)
            {
                pointer.PointerBytes = currentNode.TreeNode.PreviousPointer;
            }
            else
            {
                pointer = currentNode.TreeNode.FindPointer(key); 
            }
             

            if  (pointer.PositionPointer > 0 && pointer.Indicator != EnumValues.TypeIndicator.leaf)
            {
                MemoryTreeNode newcachenode = GetOrLoadNode(TreeFile, currentNode, pointer); 
                return FindLastLeaf(TreeFile, newcachenode); 

            }

            ///find the leaf that contains the key. 
            if (pointer.PointerBytes != null && pointer.PositionPointer > 0 && pointer.Indicator == EnumValues.TypeIndicator.leaf)
            {
                MemoryTreeNode newcacheleaf = GetOrLoadNode(TreeFile, currentNode, pointer);
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
                var treenode =  tree.ReadNode(node.TreeNode.DiskPosition);
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
        
        public static MemoryTreeNode GetOrLoadNode(TreeFile TreeFile, MemoryTreeNode ParentNode, NodePointer Pointer)
        {
            if (Pointer.IsFirstPreviousPointer)
            {
                if (ParentNode.PreviousPointer != null)
                {
                    return ParentNode.PreviousPointer; 
                }
                else
                { 
                    TreeNode node = TreeFile.ReadNode(Pointer.PositionPointer);
                    if(node == null)
                    {
                        return null;
                    }

                    MemoryTreeNode newcachenode = new MemoryTreeNode(node);
                    newcachenode.Parent = ParentNode;
                    newcachenode.IsParentPreviousPointer = Pointer.IsFirstPreviousPointer;
                    newcachenode.ParentNodeKey = Pointer.KeyToPosition;
                    newcachenode.Level = ParentNode.Level + 1;  

                    if (newcachenode.Level < TreeFile.MaxCacheLevel)
                    {
                       ParentNode.PreviousPointer = newcachenode; 
                    }
                    return newcachenode;   
                }
            }
            else
            {
                if (ParentNode.Children.ContainsKey(Pointer.PositionPointer))
                {
                    return ParentNode.Children[Pointer.PositionPointer];
                }
                else
                {
                    TreeNode node = TreeFile.ReadNode(Pointer.PositionPointer);
                    if (node == null)
                    {
                        return null;
                    } 
                    MemoryTreeNode newcachenode = new MemoryTreeNode(node);
                    newcachenode.Parent = ParentNode;
                    newcachenode.IsParentPreviousPointer = Pointer.IsFirstPreviousPointer;
                    newcachenode.ParentNodeKey = Pointer.KeyToPosition;
                    newcachenode.Level = ParentNode.Level + 1;

                    if (newcachenode.Level < TreeFile.MaxCacheLevel)
                    {
                        ParentNode.Children[Pointer.PositionPointer] = newcachenode;
                    }
                    return newcachenode;
                } 

            }
        }

        public static MemoryTreeNode FindNextLeaf(TreeFile TreeFile, MemoryTreeNode CurrentLeaf)
        {
            if (CurrentLeaf.TreeNode.TypeIndicator != EnumValues.TypeIndicator.leaf)
            {
                throw new Exception("this method only accept leaf"); 
            }
            byte[] key = KeyFinder.FindSmallestBiggerKey(CurrentLeaf.ParentNodeKey, CurrentLeaf.Parent.TreeNode.KeyArray, TreeFile.comparer); 

            while (key != null)
            {
                var pointer = CurrentLeaf.Parent.TreeNode.FindPointer(key);
                if (pointer != null && pointer.PositionPointer > 0)
                {
                    var nextleaf = GetOrLoadNode(TreeFile, CurrentLeaf.Parent, pointer); 
                    if (nextleaf != null && nextleaf.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
                    {
                        return nextleaf; 
                    }
                }

                key = KeyFinder.FindSmallestBiggerKey(key, CurrentLeaf.Parent.TreeNode.KeyArray, TreeFile.comparer);
            }
         
            return GetUpLinkFirstLeaf(TreeFile, CurrentLeaf.Parent); 
        }
        private static  MemoryTreeNode GetUpLinkFirstLeaf(TreeFile TreeFile, MemoryTreeNode ParentNode )
        {
            if (ParentNode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                return null;    
            }
            
            MemoryTreeNode siblingNode;
            byte[] siblingkey=KeyFinder.FindSmallestBiggerKey(ParentNode.ParentNodeKey, ParentNode.Parent.TreeNode.KeyArray, TreeFile.comparer);
           
            while (siblingkey != null)
            {
                var pointer = ParentNode.Parent.TreeNode.FindPointer(siblingkey);
                if (pointer != null && pointer.PositionPointer > 0)
                {
                    siblingNode =  GetOrLoadNode(TreeFile, ParentNode.Parent, pointer);

                    var result = FindContainerFirstLeaf(TreeFile, siblingNode); 
                    if (result != null)
                    { return result;  }
                }
                siblingkey = KeyFinder.FindSmallestBiggerKey(siblingkey, ParentNode.Parent.TreeNode.KeyArray, TreeFile.comparer);
            }

            return GetUpLinkFirstLeaf(TreeFile, ParentNode.Parent); 
             
        }

        private static MemoryTreeNode FindContainerFirstLeaf(TreeFile TreeFile, MemoryTreeNode ContainerNode)
        {
            if (ContainerNode == null)
            {
                return null; 
            }
           if (ContainerNode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
            {
                return ContainerNode; 
            }

            NodePointer pointer = new NodePointer();

            pointer.PointerBytes = ContainerNode.TreeNode.PreviousPointer;

            if (pointer.PositionPointer > 0)
            {
                var subnode = GetOrLoadNode(TreeFile, ContainerNode, pointer); 
                if (subnode != null)
                {
                    var result = FindContainerFirstLeaf(TreeFile, subnode);
                    if (result != null)
                    {
                        return result; 
                    }
                }
            }

            // did not get return, try one key by one key. 
            byte[] key = KeyFinder.FindSmallestBiggerKey(null, ContainerNode.TreeNode.KeyArray, TreeFile.comparer);
            
             while(key != null)
            {
                var nodepointer = ContainerNode.TreeNode.FindPointer(key);

                if (nodepointer != null && nodepointer.PositionPointer > 0)
                {
                    var keynode = GetOrLoadNode(TreeFile, ContainerNode, nodepointer);
                    if (keynode != null)
                    {
                        var result = FindContainerFirstLeaf(TreeFile, keynode);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }

                key = KeyFinder.FindSmallestBiggerKey(key, ContainerNode.TreeNode.KeyArray, TreeFile.comparer);
            }

            return null;
        }

        public static MemoryTreeNode FindPreviousLeaf(TreeFile TreeFile, MemoryTreeNode CurrentLeaf)
        {
            if (CurrentLeaf.TreeNode.TypeIndicator != EnumValues.TypeIndicator.leaf)
            {
                throw new Exception("this method only accept leaf");
            }

            if (CurrentLeaf.IsParentPreviousPointer && CurrentLeaf.ParentNodeKey == null)
            {
                return GetUpLinkLastLeaf(TreeFile, CurrentLeaf.Parent);
            }

            byte[] key = KeyFinder.FindBiggestSmallerKey(CurrentLeaf.ParentNodeKey, CurrentLeaf.Parent.TreeNode.KeyArray, TreeFile.comparer);

            while (key != null)
            {
                var pointer = CurrentLeaf.Parent.TreeNode.FindPointer(key);
                if (pointer != null && pointer.PositionPointer > 0)
                {
                    var nextleaf = GetOrLoadNode(TreeFile, CurrentLeaf.Parent, pointer);
                    if (nextleaf != null && nextleaf.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
                    {
                        return nextleaf;
                    }
                } 
                key = KeyFinder.FindBiggestSmallerKey(key, CurrentLeaf.Parent.TreeNode.KeyArray, TreeFile.comparer);
            }

            // check the previous pointer. 
            NodePointer previousPointer = new NodePointer();
            previousPointer.PointerBytes = CurrentLeaf.Parent.TreeNode.PreviousPointer;

            var PreviousPointerLeaf = GetOrLoadNode(TreeFile, CurrentLeaf.Parent, previousPointer);
            if (PreviousPointerLeaf != null && PreviousPointerLeaf.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
            {
                return PreviousPointerLeaf;
            }

            return GetUpLinkLastLeaf(TreeFile, CurrentLeaf.Parent);
        }

        private static MemoryTreeNode GetUpLinkLastLeaf(TreeFile TreeFile, MemoryTreeNode ParentNode)
        { 
            if (ParentNode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                return null;
            }

            MemoryTreeNode siblingNode;
            byte[] siblingkey = KeyFinder.FindBiggestSmallerKey(ParentNode.ParentNodeKey, ParentNode.Parent.TreeNode.KeyArray, TreeFile.comparer);

            while (siblingkey != null)
            {
                var pointer = ParentNode.Parent.TreeNode.FindPointer(siblingkey);
                if (pointer != null && pointer.PositionPointer > 0)
                {
                    siblingNode = GetOrLoadNode(TreeFile, ParentNode.Parent, pointer);

                    var result = FindContainerLastLeaf(TreeFile, siblingNode);
                    if (result != null)
                    { return result; }
                }
                siblingkey = KeyFinder.FindBiggestSmallerKey(siblingkey, ParentNode.Parent.TreeNode.KeyArray, TreeFile.comparer);
            }

            /// need to check the first previous pointer after process all key arrays. 
            NodePointer previousPointer = new NodePointer();
            previousPointer.PointerBytes = ParentNode.Parent.TreeNode.PreviousPointer;

            var PreviousPointerNode = GetOrLoadNode(TreeFile, ParentNode.Parent, previousPointer);
            //parentNode.Parent.PreviousPointerNode can't be the same with ParentNode,otherwise it will cause a dead cycle.
            if (PreviousPointerNode != null && ParentNode != PreviousPointerNode)
            {
                var result = FindContainerLastLeaf(TreeFile, PreviousPointerNode);
                if (result != null)
                { return result; }
            }

            return GetUpLinkLastLeaf(TreeFile, ParentNode.Parent);
        }
        private static MemoryTreeNode FindContainerLastLeaf(TreeFile TreeFile, MemoryTreeNode ContainerNode)
        {
            if (ContainerNode == null)
            {
                return null;
            }
            if (ContainerNode.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf)
            {
                return ContainerNode;
            }

            // did not get return, try one key by one key. 
            byte[] key = KeyFinder.FindLastKey(ContainerNode.TreeNode.KeyArray, TreeFile.comparer);

            while (key != null)
            {
                var nodepointer = ContainerNode.TreeNode.FindPointer(key);

                if (nodepointer != null && nodepointer.PositionPointer > 0)
                {
                    var keynode = GetOrLoadNode(TreeFile, ContainerNode, nodepointer);
                    if (keynode != null)
                    {
                        var result = FindContainerFirstLeaf(TreeFile, keynode);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }

                key = KeyFinder.FindBiggestSmallerKey(key, ContainerNode.TreeNode.KeyArray, TreeFile.comparer);
            }

            NodePointer pointer = new NodePointer();

            pointer.PointerBytes = ContainerNode.TreeNode.PreviousPointer;

            if (pointer.PositionPointer > 0)
            {
                var subnode = GetOrLoadNode(TreeFile, ContainerNode, pointer);
                if (subnode != null)
                {
                    var result = FindContainerFirstLeaf(TreeFile, subnode);
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
