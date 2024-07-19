using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kooboo.IndexedDB.BTree;

namespace Kooboo.IndexedDB.BPlusTree
{
    public class BPlusTreeFile : ITreeFile
    {
        private string fullFileName;
        private int _maxCacheLevel;
        internal int MaxCacheLevel
        {
            get
            {
                if (_maxCacheLevel < 1)
                {
                    _maxCacheLevel = GlobalSettings.DefaultTreeNodeCacheMaxLevel; if (_maxCacheLevel < 1)
                    {
                        _maxCacheLevel = 1;
                    }
                }
                return _maxCacheLevel;
            }
            set { _maxCacheLevel = value; }
        }

        private FileStream _indexStream;

        private HashSet<Int64> _freeNodeAddress = new HashSet<long>();

        private MemoryTreeNode _rootCache;
        public MemoryTreeNode RootNode
        {
            get
            {
                if (_rootCache == null)
                {
                    var root = this.ReadNode(this.config.ConfigDiskBytes);
                    _rootCache = new MemoryTreeNode(root);
                }
                return _rootCache;
            }
            set
            {
                _rootCache = value;
            }
        }

        internal IComparer<byte[]> comparer;

        internal IEqualityComparer<byte[]> equalityComparer;

        public TreeConfig config { get; set; }

        private object _Locker = new object();

        private BTreeIndexDuplicate _duplicate;
        public BTreeIndexDuplicate duplicate
        {
            get
            {
                if (_duplicate == null)
                {
                    lock (_Locker)
                    {
                        if (_duplicate == null)
                        {
                            var duplicate = new BTreeIndexDuplicate(this.fullFileName + ".duplicate");
                            duplicate.OpenOrCreate();
                            _duplicate = duplicate;
                        }
                    }
                }
                return _duplicate;
            }

        }

        public BPlusTreeFile(string FullFileName, Type keyType, int keyLen, IComparer<byte[]> comparer, IEqualityComparer<byte[]> EqualityComparer, int MaxCacheLevel, int BPlusLen)
        {
            var unique = true;// BPlus always unique now. 

            this.fullFileName = FullFileName;
            this.comparer = comparer;
            this.equalityComparer = EqualityComparer;
            this.MaxCacheLevel = MaxCacheLevel;

            if (!File.Exists(this.fullFileName))
            {
                if (keyLen == 0 || keyType == null)
                {
                    throw new Exception("must supply key max len and keyType before continue");
                }
                config = new TreeConfig(keyLen, unique, keyType, this.MaxCacheLevel, this.comparer, this.equalityComparer, BPlusLen);

                var rootNode = new TreeNode(this.config)
                {
                    DiskPosition = config.ConfigDiskBytes,
                    Deletion = Constant.DeleteIndicator.Normal,
                    TypeIndicator = EnumValues.TypeIndicator.root
                };

                lock (_Locker)
                {
                    if (!File.Exists(this.fullFileName))
                    {
                        string dirName = Path.GetDirectoryName(this.fullFileName);
                        if (!System.IO.Directory.Exists(dirName))
                        {
                            System.IO.Directory.CreateDirectory(dirName);
                        }

                        FileStream indexFileStream = new FileStream(this.fullFileName, FileMode.Create, FileAccess.Write);
                        indexFileStream.Write(config.ToBytes(), 0, config.ConfigDiskBytes);
                        indexFileStream.Write(rootNode.ToBytes(), 0, config.NodeDiskSize);
                        indexFileStream.Close();
                    }
                }

            }
            else
            {

                byte[] configBytes = this.Read(0, 100);
                TreeConfig config = TreeConfig.FromBytes(configBytes, this.MaxCacheLevel, this.comparer, this.equalityComparer);
                this.config = config;

                if (this.config.BPlusLen != BPlusLen)
                {
                    throw new Exception("change of BPlus Tree meta bytes length is not allowed, please rebuild this data store");
                }
            }


        }

        public TreeNode ReadNode(Int64 diskPosition)
        {
            if (diskPosition < 1)
            {
                return null;
            }
            byte[] nodeBytes = this.Read(diskPosition, this.config.NodeDiskSize);
            return TreeNode.FromBytes(nodeBytes, this.config, diskPosition);
        }
        /// <summary>
        /// Create the first left leaf for the node. only create previous left pointer, one leaf.
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="key"></param>
        public void CreateFirstLeaf(TreeNode ParentNode)
        {
            TreeNode newLeaf = new TreeNode(this.config);
            newLeaf.Deletion = Constant.DeleteIndicator.Normal;
            newLeaf.TypeIndicator = EnumValues.TypeIndicator.leaf;

            newLeaf.DiskPosition = GetInsertPosition();
            this.Write(newLeaf.ToBytes(), newLeaf.DiskPosition, this.config.NodeDiskSize);

            var leftPointer = NodePointer.CreateBPlus(EnumValues.TypeIndicator.leaf, newLeaf.DiskPosition, this.config.BPlusLen, null);

            ParentNode.PreviousPointer = leftPointer.ToBytes();

            this.Write(ParentNode.ToBytes(), ParentNode.DiskPosition, this.config.NodeDiskSize);
        }

        private Int64 GetInsertPosition()
        {
            Int64 pos;
            if (this._freeNodeAddress.Count > 0)
            {
                pos = this._freeNodeAddress.FirstOrDefault();
                this._freeNodeAddress.Remove(pos);
            }
            else
            {
                pos = IndexStream.Length;
            }
            /// Check to make sure that POS is the right spot.
            return pos;
        }
        /// <summary>
        /// add a new key,  duplicate value should has been checked by another method. 
        /// </summary>
        /// <param name="keyBytes"></param>
        /// <param name="blockPosition"></param>
        /// <returns>True = add OK, False = duplicate not added. </returns>
        public bool Add(byte[] keyBytes, Int64 blockPosition, byte[] BPlusBytes)
        {
            MemoryTreeNode memoryLeaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keyBytes);

            if (memoryLeaf.TreeNode.KeyArray.ContainsKey(keyBytes))
            {
                return false;
            }

            else
            {
                var blockPointer = NodePointer.CreateBPlus(EnumValues.TypeIndicator.block, blockPosition, this.config.BPlusLen, BPlusBytes);

                _addNew(memoryLeaf, keyBytes, blockPointer);

                if (memoryLeaf.TreeNode.Count > this.config.SplitCount)
                {
                    SplitLeaf(memoryLeaf);
                }
                else
                {
                    MemoryTreeNodeManager.NodeChange(this, memoryLeaf);
                }

                return true;
            }
        }

        /// <summary>
        /// Get the block position that attached to this key. 
        /// Remove 0 if not found. 
        /// </summary>
        /// <param name="keyBytes"></param>
        /// <returns></returns>
        public Int64 GetBlockPosition(byte[] keyBytes)
        {
            var pointer = GetPointer(keyBytes);
            return pointer != null ? pointer.PositionPointer : -1;
        }

        public byte[] GetBPlus(byte[] keyBytes)
        {
            var pointer = GetPointer(keyBytes);
            return pointer != null ? pointer.BPlusBytes : null;
        }

        public NodePointer GetPointer(byte[] keyBytes)
        {
            MemoryTreeNode leaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keyBytes);
            if (leaf == null)
            {
                return null;
            }
            foreach (var item in leaf.TreeNode.KeyArray)
            {
                if (BTree.Comparer.ByteEqualComparer.isEqual(item.Key, keyBytes, this.config.KeyLength))
                {
                    var pointer = NodePointer.FromBPlusBytes(item.Value, item.Key, this.config.BPlusLen);
                    return pointer;
                }
            }
            return null;
        }


        // list all items of this key.... 
        public List<Int64> ListAll(byte[] keyBytes)
        {
            List<long> result = new List<long>();
            MemoryTreeNode leaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keyBytes);
            if (leaf != null)
            {
                foreach (var item in leaf.TreeNode.KeyArray)
                {
                    if (BTree.Comparer.ByteEqualComparer.isEqual(item.Key, keyBytes, this.config.KeyLength))
                    {
                        var pointer = NodePointer.FromBytes(item.Value);
                        if (this.config.unique)
                        {
                            result.Add(pointer.PositionPointer);
                        }
                        else
                        {
                            if (pointer.Indicator != EnumValues.TypeIndicator.duplicate)
                            {
                                result.Add(pointer.PositionPointer);
                            }
                            else
                            {
                                return this.duplicate.GetAll(pointer.PositionPointer);
                            }

                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// remove a key, identified by key and block position. blockPosition >0.
        /// </summary>
        /// <param name="keyBytes"></param>
        /// <param name="blockPosition"></param>
        /// <returns>True = DEL OK, false = DEL failed.</returns>
        public bool Del(byte[] keyBytes, Int64 blockPosition = 0)
        {
            MemoryTreeNode foundLeaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keyBytes);

            if (!foundLeaf.TreeNode.KeyArray.ContainsKey(keyBytes))
            {
                return false;
            }
            var pointer = NodePointer.FromBPlusBytes(foundLeaf.TreeNode.KeyArray[keyBytes], this.config.BPlusLen);

            if (pointer.PositionPointer <= 0)
            {
                return false;
            }

            if (pointer.Indicator == EnumValues.TypeIndicator.block)
            {
                if (blockPosition > 0 && pointer.PositionPointer != blockPosition)
                {
                    return false;
                }

                this.WriteByte(foundLeaf.TreeNode.DiskPosition + pointer.RelativePosition, Constant.DeleteIndicator.Deleted);

                //write the new counter.
                foundLeaf.TreeNode.KeyArray.Remove(keyBytes);
                foundLeaf.TreeNode.Count = (Int16)(foundLeaf.TreeNode.KeyArray.Count);

                this.Write(BitConverter.GetBytes(foundLeaf.TreeNode.Count), foundLeaf.TreeNode.DiskPosition + TreeNode.NodeCounterPosition, 2);

                if (foundLeaf.TreeNode.Count < this.config.MergeCount)
                {
                    Merge(foundLeaf, keyBytes);
                }
                else
                {
                    MemoryTreeNodeManager.NodeChange(this, foundLeaf);
                }

                return true;
            }

            return false;
        }

        public bool Update(byte[] keyBytes, byte[] BPlusBytes, Int64 newBlockPosition)
        {
            MemoryTreeNode foundLeaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keyBytes);

            if (!foundLeaf.TreeNode.KeyArray.ContainsKey(keyBytes))
            {
                return false;
            }

            var pointer = NodePointer.FromBPlusBytes(foundLeaf.TreeNode.KeyArray[keyBytes], this.config.BPlusLen);

            if (pointer.Indicator == EnumValues.TypeIndicator.block)
            {
                pointer.PositionPointer = newBlockPosition;
                pointer.BPlusBytes = BPlusBytes;

                Int64 pointerDiskPosition = foundLeaf.TreeNode.DiskPosition + pointer.RelativePosition + this.config.KeyLength + 1;

                this.Write(pointer.ToBytes(), pointerDiskPosition);

                MemoryTreeNodeManager.NodeChange(this, foundLeaf);

                return true;

            }
            return false;
        }

        public bool UpdateBplus(byte[] keyBytes, byte[] BPlusBytes)
        {
            MemoryTreeNode foundLeaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keyBytes);

            if (!foundLeaf.TreeNode.KeyArray.ContainsKey(keyBytes))
            {
                return false;
            }

            var pointer = NodePointer.FromBPlusBytes(foundLeaf.TreeNode.KeyArray[keyBytes], this.config.BPlusLen);

            if (pointer.Indicator == EnumValues.TypeIndicator.block)
            {
                pointer.BPlusBytes = BPlusBytes;

                Int64 pointerDiskPosition = foundLeaf.TreeNode.DiskPosition + pointer.RelativePosition + this.config.KeyLength + 1;

                this.Write(pointer.ToBytes(), pointerDiskPosition);

                MemoryTreeNodeManager.NodeChange(this, foundLeaf);

                return true;
            }
            return false;
        }

        /// <summary>
        /// add a new record, return true = add ok, otherwise, duplicate found. 
        /// </summary>
        /// <param name="MemoryLeaf"></param>
        /// <param name="keyBytes"></param>
        /// <param name="blockPointer"></param>
        /// <returns></returns>
        private void _addNew(MemoryTreeNode MemoryLeaf, byte[] keyBytes, NodePointer blockPointer)
        {
            if (!MemoryLeaf.TreeNode.KeyArray.ContainsKey(keyBytes))
            {
                int relativePosition = MemoryLeaf.TreeNode.GetFreeRelativePosition();

                blockPointer.RelativePosition = (Int16)relativePosition;

                byte[] TotalRecordBytes = new byte[this.config.RecordLen];

                TotalRecordBytes[0] = Constant.DeleteIndicator.Normal;

                System.Buffer.BlockCopy(keyBytes, 0, TotalRecordBytes, 1, this.config.KeyLength);

                var pointerBytes = blockPointer.ToBytes();
                System.Buffer.BlockCopy(pointerBytes, 0, TotalRecordBytes, this.config.KeyLength + 1, pointerBytes.Length);

                this.Write(TotalRecordBytes, MemoryLeaf.TreeNode.DiskPosition + relativePosition, this.config.RecordLen);

                MemoryLeaf.TreeNode.KeyArray.Add(keyBytes, blockPointer.ToBytes());

                MemoryLeaf.TreeNode.Count = (Int16)MemoryLeaf.TreeNode.KeyArray.Count;

                this.Write(BitConverter.GetBytes(MemoryLeaf.TreeNode.Count), MemoryLeaf.TreeNode.DiskPosition + TreeNode.NodeCounterPosition, 2);
            }
        }

        private void DeleteNode(Int64 diskPosition)
        {
            if (diskPosition > 0)
            {
                this.WriteByte(diskPosition + TreeNode.DeletionIndicationPosition, Constant.DeleteIndicator.Deleted);
                this._freeNodeAddress.Add(diskPosition);
            }
        }

        private void DeleteKey(TreeNode node, byte[] keyBytes)
        {
            // mark that record pointer as null byte. 
            var pointer = NodePointer.FromBytes(node.KeyArray[keyBytes]);

            this.WriteByte(node.DiskPosition + pointer.RelativePosition, Constant.DeleteIndicator.Deleted);

            node.KeyArray.Remove(keyBytes);
            // now update the counter.   
            this.Write(BitConverter.GetBytes(node.KeyArray.Count()), node.DiskPosition + TreeNode.NodeCounterPosition, 2);

        }

        /// <summary>
        /// Delete the previous pointer of a node.
        /// </summary>
        /// <param name="node"></param>
        private void DeletePreviousPointer(TreeNode node)
        {
            Int16 PreviousPointerPosition = 16;

            Int64 recordPosition = node.DiskPosition + PreviousPointerPosition;

            byte[] nullBytes = new byte[this.config.PointerLen];
            nullBytes[0] = Constant.DeleteIndicator.Deleted;

            this.Write(nullBytes, recordPosition, this.config.PointerLen);
        }

        /// <summary>
        /// When a leaf does not have record any more, remove itself and all possible empty parents. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keytosearch"></param>
        private void RemoveEmptyChains(MemoryTreeNode node)
        {
            if (node.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                return;
            }

            var parent = node.Parent;

            if (node.TreeNode.TypeIndicator == EnumValues.TypeIndicator.leaf && node.TreeNode.Count == 0)
            {
                DeleteNode(node.TreeNode.DiskPosition);
                MemoryTreeNodeManager.NodeChange(this, node);
            }
            else
            {
                var previousPointer = NodePointer.FromBytes(node.TreeNode.PreviousPointer, true);

                // if there is no key and also not any left previous pointer. 
                if (node.TreeNode.KeyArray.Count == 0 & previousPointer.PositionPointer == 0)
                {
                    DeleteNode(node.TreeNode.DiskPosition);
                    MemoryTreeNodeManager.NodeChange(this, node);
                }

                else
                { return; }

            }

            // has delete node. 
            if (node.IsParentPreviousPointer)
            {
                DeletePreviousPointer(parent.TreeNode);
                parent.TreeNode = ReadNode(parent.TreeNode.DiskPosition);
            }
            else
            {
                if (node.ParentNodeKey != null)
                {
                    DeleteKey(parent.TreeNode, node.ParentNodeKey);
                }
                parent.TreeNode = ReadNode(parent.TreeNode.DiskPosition);
            }

            RemoveEmptyChains(parent);
        }

        private void SplitLeaf(MemoryTreeNode MemoryLeaf)
        {
            /// 4 steps to split, 
            /// 1. create a new leaf with half of the records.
            /// 2. update to the parent node of the new leaf. 
            /// 3. update the old leaf to new half left record leaf.
            /// 4. Check whether the new updated node needs split or not.

            Dictionary<byte[], byte[]> NewKeyArray = new Dictionary<byte[], byte[]>(this.equalityComparer);
            // get first half of the record into new leaf. this is based on the decision key is on left or right.
            Dictionary<byte[], byte[]> RemainingKeyArray = new Dictionary<byte[], byte[]>(this.equalityComparer);

            int halfCount = (int)(MemoryLeaf.TreeNode.KeyArray.Count / 2);

            int i = 0;

            byte[] newLeafKey = null;

            foreach (var item in MemoryLeaf.TreeNode.KeyArray.OrderBy(o => o.Key, comparer))
            {
                if (i < halfCount)
                {
                    RemainingKeyArray.Add(item.Key, item.Value);
                }
                else
                {
                    if (newLeafKey == null)
                    {
                        newLeafKey = item.Key;
                    }
                    NewKeyArray.Add(item.Key, item.Value);
                }
                i++;
            }

            ///1. create a new leaf with half of the records.
            TreeNode newLeaf = new TreeNode(this.config);
            newLeaf.Deletion = Constant.DeleteIndicator.Normal;
            newLeaf.TypeIndicator = EnumValues.TypeIndicator.leaf;

            newLeaf.KeyArray = NewKeyArray;

            long diskPosition = GetInsertPosition();

            this.Write(newLeaf.ToBytes(), diskPosition, this.config.NodeDiskSize);

            // 2. update to the parent node of the new leaf.  
            ///TreeNode ParentNode = LoadParentNode(MemoryLeaf);

            var newKeyPointer = NodePointer.Create(EnumValues.TypeIndicator.leaf, diskPosition);

            _addNew(MemoryLeaf.Parent, newLeafKey, newKeyPointer);

            //3. update the old leaf to new half left record leaf.
            MemoryLeaf.TreeNode.KeyArray = RemainingKeyArray;

            this.Write(MemoryLeaf.TreeNode.ToBytes(), MemoryLeaf.TreeNode.DiskPosition, this.config.NodeDiskSize);

            /// 4. Check whether the new updated node needs split or not. 
            if ((MemoryLeaf.Parent.TreeNode.Count) > this.config.SplitCount)
            {
                SplitNode(MemoryLeaf.Parent);
            }
            else
            {
                MemoryTreeNodeManager.NodeChange(this, MemoryLeaf.Parent);
            }
        }

        //This is very similar like SplitLeaf, should be combined.
        private void SplitNode(MemoryTreeNode node)
        {
            /// 4 steps to split, 
            /// 1. create a new node with half of the records.
            /// 2. update the old node to  half left record node.
            /// 3. update to the parent node of the new node by adding a new record. 
            /// 4. Check whether the parent updated node needs split or not.
            ///  NOTE: Node has previousPointer, Leaf does not have.
            ///  When split a node, the the first key of the new node should be removed and move into PreviousPointer.

            if (node.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                SplitRoot(node);
                return;
            }

            Dictionary<byte[], byte[]> NewKeyArray = new Dictionary<byte[], byte[]>(this.equalityComparer);
            // get first half of the record into new leaf. this is based on the decision key is on left or right.
            Dictionary<byte[], byte[]> leftKeyArray = new Dictionary<byte[], byte[]>(this.equalityComparer);

            int halfCount = (int)node.TreeNode.KeyArray.Count / 2;
            int i = 0;

            // first pair used at the previous pointer.
            byte[] firstKey = null;
            byte[] firstPreviousPointer = null;

            foreach (var item in node.TreeNode.KeyArray.OrderBy(o => o.Key, this.comparer))
            {
                if (i < halfCount)
                {
                    leftKeyArray.Add(item.Key, item.Value);
                }
                else
                {
                    if (firstKey == null)
                    {
                        firstKey = item.Key;
                        firstPreviousPointer = item.Value;
                    }
                    else
                    {
                        NewKeyArray.Add(item.Key, item.Value);
                    }
                }

                i++;
            }

            ///1. create a new node with half of the records.
            TreeNode newNode = new TreeNode(this.config);
            newNode.Deletion = Constant.DeleteIndicator.Normal;
            newNode.TypeIndicator = EnumValues.TypeIndicator.node;

            newNode.PreviousPointer = firstPreviousPointer;

            newNode.KeyArray = NewKeyArray;

            long diskPosition = GetInsertPosition();

            this.Write(newNode.ToBytes(), diskPosition, this.config.NodeDiskSize);

            // 2. update to the parent node of the new node.  
            var newKeyPointer = NodePointer.Create(EnumValues.TypeIndicator.node, diskPosition);

            _addNew(node.Parent, firstKey, newKeyPointer);

            //3. update the old node to new half left record leaf.
            node.TreeNode.KeyArray = leftKeyArray;

            this.Write(node.TreeNode.ToBytes(), node.TreeNode.DiskPosition, this.config.NodeDiskSize);

            /// 4. Check whether the new updated node needs split or not.
            if (node.Parent.TreeNode.Count > this.config.SplitCount)
            {
                SplitNode(node.Parent);
            }
            else
            {
                MemoryTreeNodeManager.NodeChange(this, node.Parent);
            }
        }

        private void SplitRoot(MemoryTreeNode rootNode)
        {
            // 1. move the right half to a new node. 
            // 2. move the left left side half to another new node. 
            // 3. update the root with one new keyValue pair only to the this two new nodes.

            Dictionary<byte[], byte[]> RightKeyArray = new Dictionary<byte[], byte[]>(this.equalityComparer);
            // get first half of the record into new leaf. this is based on the decision key is on left or right.
            Dictionary<byte[], byte[]> leftKeyArray = new Dictionary<byte[], byte[]>(this.equalityComparer);

            int halfCount = (int)rootNode.TreeNode.KeyArray.Count / 2;
            int i = 0;

            // first pair used at the previous pointer.
            byte[] firstKey = null;

            byte[] firstPreviousPointer = null;

            foreach (var item in rootNode.TreeNode.KeyArray.OrderBy(o => o.Key, this.comparer))
            {
                if (i < halfCount)
                {
                    leftKeyArray.Add(item.Key, item.Value);
                }
                else
                {
                    if (firstKey == null)
                    {
                        firstKey = item.Key;
                        firstPreviousPointer = item.Value;
                    }
                    else
                    {

                        RightKeyArray.Add(item.Key, item.Value);
                    }
                }

                i++;
            }

            ///1. create a right new node with half of the records.
            TreeNode rightNode = new TreeNode(this.config);
            rightNode.Deletion = Constant.DeleteIndicator.Normal;
            rightNode.TypeIndicator = EnumValues.TypeIndicator.node;

            rightNode.PreviousPointer = firstPreviousPointer;
            rightNode.KeyArray = RightKeyArray;

            long rightNodeDiskPosition = GetInsertPosition();

            this.Write(rightNode.ToBytes(), rightNodeDiskPosition, this.config.NodeDiskSize);

            //2. create a left new node with half of the records. 
            TreeNode leftNode = new TreeNode(this.config);
            leftNode.Deletion = Constant.DeleteIndicator.Normal;
            leftNode.TypeIndicator = EnumValues.TypeIndicator.node;

            leftNode.PreviousPointer = rootNode.TreeNode.PreviousPointer;
            leftNode.KeyArray = leftKeyArray;

            long leftNodeDiskPosition = GetInsertPosition();

            this.Write(leftNode.ToBytes(), leftNodeDiskPosition, this.config.NodeDiskSize);

            // 3. update the root with one new keyValue pair only to the this two new nodes.

            TreeNode newRoot = new TreeNode(this.config);
            newRoot.Deletion = Constant.DeleteIndicator.Normal;
            newRoot.TypeIndicator = EnumValues.TypeIndicator.root;

            var leftPointer = NodePointer.Create(EnumValues.TypeIndicator.node, leftNodeDiskPosition);

            var rightPointer = NodePointer.Create(EnumValues.TypeIndicator.node, rightNodeDiskPosition);

            newRoot.PreviousPointer = leftPointer.ToBytes();
            newRoot.KeyArray.Add(firstKey, rightPointer.ToBytes());

            this.Write(newRoot.ToBytes(), rootNode.TreeNode.DiskPosition, this.config.NodeDiskSize);

            MemoryTreeNodeManager.NodeChange(this, rootNode);
        }

        private void Merge(MemoryTreeNode leaf, byte[] keyBytes)
        {
            /// find the next or previous leaf and merge with it.
            //1. find the bigger or smaller key. 
            //2. move all record into the bigger key.
            //3. remove itself by remove its own key. 
            //If it is the last key, remove the previous key. 

            if (leaf.TreeNode.Count == 0)
            {
                RemoveEmptyChains(leaf);
                return;
            }
            var parent = leaf.Parent;
            var hasMerge = MergeLeaf(leaf, parent);

            // after leaf is merge, check node. 
            if (hasMerge)
            {
                if (parent.TreeNode.KeyArray.Count < this.config.MergeCount)
                {
                    MemoryTreeNodeManager.NodeReload(this, parent);
                    bool done = MergeNode(parent);
                    if (!done)
                    {
                        MemoryTreeNodeManager.NodeChange(this, parent);
                    }
                }
                else
                {
                    MemoryTreeNodeManager.NodeChange(this, parent);
                }
            }
        }

        /// <summary>
        /// verify that two item can be merged, they must be the same type, and meet certain conditions.
        /// </summary>
        /// <param name="leafX"></param>
        /// <param name="leafY"></param>
        /// <returns></returns>
        private bool canMerge(TreeNode leafX, TreeNode leafY)
        {
            if (leafX.TypeIndicator != leafY.TypeIndicator)
            {
                return false;
            }

            if (leafX.DiskPosition != 0 && leafX.DiskPosition == leafY.DiskPosition)
            {
                return false;
            }

            if ((leafX.KeyArray.Count + leafY.KeyArray.Count) > this.config.SplitCount)
            {
                return false;
            }
            return true;
        }

        private bool MergeLeaf(MemoryTreeNode leaf, MemoryTreeNode ParentNode)
        {
            bool hasMerge = false;
            // Find the nodePointerToLeaf that is just  key < that leaf.
            byte[] leafParentKey = leaf.ParentNodeKey;  // null = the previousPointer..

            //if (ParentNode.TreeNode.KeyArray.Count == 0)
            //{
            //    /// TODO: verify this condition & action.
            //    return false;
            //}

            byte[] biggerKey = KeyFinder.FindSmallestBiggerKey(leafParentKey, ParentNode.TreeNode.KeyArray, this.comparer);

            if (biggerKey != null)
            {
                //move all the found keys to current leaf and remove the found leaf.  
                var foundPointer = NodePointer.FromBytes(ParentNode.TreeNode.KeyArray[biggerKey], biggerKey);

                TreeNode foundLeaf = ReadNode(foundPointer.PositionPointer);

                if (canMerge(foundLeaf, leaf.TreeNode))
                {
                    foreach (var item in foundLeaf.KeyArray)
                    {
                        leaf.TreeNode.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(leaf.TreeNode.ToBytes(), leaf.TreeNode.DiskPosition, this.config.NodeDiskSize);

                    // remove from parent node. 
                    DeleteKey(ParentNode.TreeNode, biggerKey);
                    DeleteNode(foundLeaf.DiskPosition);
                    // MemoryTreeNodeManager.NodeReload(this, ParentNode); 
                    hasMerge = true;
                }
            }

            else
            {
                // this leaf is the biggest key in the parent keyArray.
                // Find the second biggest key
                // move this leaf to the found, and remove this key. 
                byte[] foundKey = KeyFinder.FindBiggestSmallerKey(leafParentKey, ParentNode.TreeNode.KeyArray, this.comparer);

                NodePointer smallerPointer = null;

                if (foundKey != null)
                {
                    smallerPointer = NodePointer.FromBytes(ParentNode.TreeNode.KeyArray[foundKey], foundKey);
                }
                else
                {
                    smallerPointer = NodePointer.FromBytes(ParentNode.TreeNode.PreviousPointer, true);
                }

                TreeNode foundLeaf = ReadNode(smallerPointer.PositionPointer);

                if (canMerge(foundLeaf, leaf.TreeNode))
                {
                    // this.CacheNodes.Remove(foundLeaf);

                    foreach (var item in leaf.TreeNode.KeyArray)
                    {
                        foundLeaf.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(foundLeaf.ToBytes(), foundLeaf.DiskPosition, this.config.NodeDiskSize);

                    DeleteKey(ParentNode.TreeNode, leafParentKey);
                    DeleteNode(leaf.TreeNode.DiskPosition);

                    // MemoryTreeNodeManager.NodeChange(this, ParentNode); 
                    hasMerge = true;

                }

            }

            return hasMerge;
        }

        /// <summary>
        /// Merge current node with others. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keytosearch"></param>
        private bool MergeNode(MemoryTreeNode node)
        {
            bool hasMerge = false;
            if (node.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                if (node.TreeNode.KeyArray.Count == 1)
                {

                    if (CombineSub(node.TreeNode))
                    {
                        MemoryTreeNodeManager.NodeChange(this, node);
                    }
                }
                return false;  /// there is no need to do anything any more since root cache is updated now, so return false. 
            }

            // Find the nodePointer that is just  key < that leaf.
            // The dictionary.OrderBy is very expensive, that is why we have to use this complicate way.
            MemoryTreeNode ParentNode = node.Parent;

            if (ParentNode.TreeNode.KeyArray.Count == 0)
            {
                /// TODO: verify this condition  & action.
                return false;
            }

            byte[] biggerKey = KeyFinder.FindSmallestBiggerKey(node.ParentNodeKey, ParentNode.TreeNode.KeyArray, this.comparer);

            if (biggerKey != null)
            {
                //move all the found keys to current node and remove the found node. 
                //NodePointer foundPointer = new NodePointer();
                //foundPointer.PointerBytes = ParentNode.TreeNode.KeyArray[biggerKey];
                //foundPointer.KeyToPosition = biggerKey;
                var foundPointer = NodePointer.FromBytes(ParentNode.TreeNode.KeyArray[biggerKey], biggerKey);

                TreeNode foundNode = ReadNode(foundPointer.PositionPointer);

                if (canMerge(node.TreeNode, foundNode))
                {
                    byte[] foundNode_FirstKey = KeyFinder.FindFirstKey(foundNode.KeyArray, this.comparer);

                    /// This is to check whether the biggerKey is the first key of foundNode or not. 
                    /// if yes, means that is not a previous pointer in foundNode.
                    if (this.comparer.Compare(biggerKey, foundNode_FirstKey) != 0)
                    {
                        //NodePointer foundPrevious = new NodePointer();
                        //foundPrevious.PointerBytes = foundNode.PreviousPointer;
                        var foundPrevious = NodePointer.FromBytes(foundNode.PreviousPointer, true);

                        if (foundPointer.PositionPointer > 0)
                        {
                            node.TreeNode.KeyArray.Add(biggerKey, foundPrevious.ToBytes());
                        }
                    }

                    foreach (var item in foundNode.KeyArray)
                    {
                        node.TreeNode.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(node.TreeNode.ToBytes(), node.TreeNode.DiskPosition, this.config.NodeDiskSize);

                    // remove from parent node. 
                    DeleteKey(ParentNode.TreeNode, biggerKey);
                    DeleteNode(foundNode.DiskPosition);
                    hasMerge = true;
                }
            }
            else
            {
                // this node is the biggest key in the parent keyArray.
                // Find the second biggest key
                // move this node to the found, and remove this key. 

                byte[] foundKey = KeyFinder.FindBiggestSmallerKey(node.ParentNodeKey, ParentNode.TreeNode.KeyArray, this.comparer);

                NodePointer smallerPointer = null;

                if (foundKey == null)
                {
                    //smallerPointer.PointerBytes = ParentNode.TreeNode.PreviousPointer;
                    //smallerPointer.KeyToPosition = null;
                    smallerPointer = NodePointer.FromBytes(ParentNode.TreeNode.PreviousPointer, true);
                }
                else
                {
                    //smallerPointer.PointerBytes = ParentNode.TreeNode.KeyArray[foundKey];
                    //smallerPointer.KeyToPosition = foundKey;
                    smallerPointer = NodePointer.FromBytes(ParentNode.TreeNode.KeyArray[foundKey], foundKey);
                }

                TreeNode _foundNode = ReadNode(smallerPointer.PositionPointer);

                if (canMerge(node.TreeNode, _foundNode))
                {
                    //The previous pointer, biggestOne is the key pointer to current node. 
                    //NodePointer previous = new NodePointer();
                    //previous.PointerBytes = node.TreeNode.PreviousPointer;
                    var previous = NodePointer.FromBytes(node.TreeNode.PreviousPointer, true);

                    if (previous.PositionPointer > 0)
                    {
                        _foundNode.KeyArray.Add(node.ParentNodeKey, previous.ToBytes());
                    }

                    foreach (var item in node.TreeNode.KeyArray)
                    {
                        _foundNode.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(_foundNode.ToBytes(), _foundNode.DiskPosition, this.config.NodeDiskSize);

                    // remove from parent node. 
                    DeleteKey(ParentNode.TreeNode, node.ParentNodeKey);
                    DeleteNode(node.TreeNode.DiskPosition);
                    hasMerge = true;
                }
            }

            if (hasMerge)
            {
                if (ParentNode.TreeNode.KeyArray.Count < this.config.MergeCount)
                {
                    MemoryTreeNodeManager.NodeReload(this, ParentNode);
                    var doMerge = MergeNode(ParentNode);
                    if (!doMerge)
                    {
                        MemoryTreeNodeManager.NodeChange(this, ParentNode);
                    }
                }
                else
                {
                    MemoryTreeNodeManager.NodeChange(this, ParentNode);
                }
            }

            return hasMerge;
        }

        /// <summary>
        /// This node has two sub, combine them into current node and remove subs. 
        /// Before call this method, must already check that node.keyArray.count = 1; and this is the root.
        /// </summary>
        /// <param name="node"></param>
        private bool CombineSub(TreeNode root)
        {
            //NodePointer previous = new NodePointer();
            //previous.PointerBytes = root.PreviousPointer;
            var previous = NodePointer.FromBytes(root.PreviousPointer, true);

            TreeNode newRoot;

            if (previous.PositionPointer != 0 && previous.Indicator == EnumValues.TypeIndicator.node)
            {
                newRoot = ReadNode(previous.PositionPointer);

                //can only be 1 or 0, 0 do nothing, 1, check sub node = type node and  move the right node to leaf node. 
                if (root.KeyArray.Count == 1)
                {
                    byte[] rightKey = root.KeyArray.First().Key;
                    //NodePointer rightPointer = new NodePointer();
                    //rightPointer.PointerBytes = root.KeyArray[rightKey];
                    //rightPointer.KeyToPosition = rightKey;
                    var rightPointer = NodePointer.FromBytes(root.KeyArray[rightKey], rightKey);

                    if (rightPointer.Indicator != EnumValues.TypeIndicator.node)
                    {
                        return false;
                    }

                    TreeNode rightNode = ReadNode(rightPointer.PositionPointer);

                    if (!this.canMerge(newRoot, rightNode))
                    {
                        return false;
                    }

                    //NodePointer subPrevious = new NodePointer();
                    //subPrevious.PointerBytes = rightNode.PreviousPointer;
                    var subPrevious = NodePointer.FromBytes(rightNode.PreviousPointer, true);

                    if (subPrevious.PositionPointer > 0)
                    {
                        newRoot.KeyArray.Add(rightKey, subPrevious.ToBytes());
                    }

                    foreach (var item in rightNode.KeyArray)
                    {
                        newRoot.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(newRoot.ToBytes(), root.DiskPosition, this.config.NodeDiskSize);

                    /// remove the two nodes. 
                    DeleteNode(previous.PositionPointer);
                    DeleteNode(rightPointer.PositionPointer);

                    return true;
                }

            }
            else
            {
                // there is not previous pointer, make check whether there is a need to make node upgrade to root.  
                //can only be 1 or 0, 0 do nothing, 1, move the right node to root node. 
                if (root.KeyArray.Count == 1)
                {
                    byte[] rightKey = root.KeyArray.First().Key;

                    var rightPointer = NodePointer.FromBytes(root.KeyArray[rightKey], rightKey);

                    if (rightPointer.Indicator != EnumValues.TypeIndicator.node)
                    {
                        return false;
                    }


                    if (rightPointer.PositionPointer > 0)
                    {

                        //use the right node to replace root node. 

                        byte[] rightNodeBytes = this.Read(rightPointer.PositionPointer, this.config.NodeDiskSize);

                        this.Write(rightNodeBytes, root.DiskPosition, this.config.NodeDiskSize);

                        ////DELTE the NODE
                        DeleteNode(rightPointer.PositionPointer);

                        return true;
                    }
                }

            }

            return false;
        }


        private void Write(byte[] bytes, long Position, int len = 0)
        {
            // lock (_Locker)   // locker is not needed because it is always locked from parent.
            // {
            if (len == 0)
            {
                len = bytes.Length;
            }
            IndexStream.Position = Position;
            IndexStream.Write(bytes, 0, len);
            //}
        }

        private void WriteByte(long position, byte value)
        {
            IndexStream.Position = position;
            IndexStream.WriteByte(value);
        }

        private byte[] Read(long position, int count)
        {
            byte[] partial = new byte[count];
            IndexStream.Position = position;
            IndexStream.Read(partial, 0, count);
            return partial;
        }

        public FileStream IndexStream
        {
            get
            {
                if (_indexStream == null || _indexStream.CanRead == false)
                {
                    lock (_Locker)
                    {
                        if (_indexStream == null || _indexStream.CanRead == false)
                        {
                            if (File.Exists(fullFileName))
                            {
                                _indexStream = StreamManager.GetFileStream(this.fullFileName);
                            }
                        }
                    }
                }
                return _indexStream;
            }
        }

        public void Close()
        {
            if (_indexStream != null)
            {
                lock (_Locker)
                {
                    if (_indexStream != null)
                    {
                        _indexStream.Close();
                        _indexStream = null;
                    }
                }
            }

            if (_duplicate != null)
            {
                lock (_Locker)
                {
                    if (_duplicate != null)
                    { _duplicate.Close(); }
                }
            }
        }

        public void DelSelf()
        {
            lock (_Locker)
            {
                _duplicate.DelSelf();
                if (_indexStream != null)
                {
                    _indexStream.Close();
                    _indexStream = null;
                }
                File.Delete(this.fullFileName);
            }
        }

        public void Flush()
        {
            if (_indexStream != null)
            {
                lock (_Locker)
                {
                    if (_indexStream != null)
                    {
                        _indexStream.Flush();
                    }
                }
            }

            if (_duplicate != null)
            {
                lock (_Locker)
                {
                    if (_duplicate != null)
                    { _duplicate.Close(); }
                }
            }
        }

        public byte[] FirstKey()
        {
            var leaf = MemoryTreeNodeManager.FindFirstLeaf(this);
            if (leaf != null)
            {
                return KeyFinder.FindFirstKey(leaf.TreeNode.KeyArray, this.comparer);
            }
            return null;
        }

        public byte[] LastKey()
        {
            var lastLeaf = MemoryTreeNodeManager.FindLastLeaf(this);

            if (lastLeaf != null)
            {
                return KeyFinder.FindLastKey(lastLeaf.TreeNode.KeyArray, this.comparer);
            }
            return null;
        }

    }

}
