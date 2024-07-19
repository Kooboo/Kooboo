//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.IndexedDB.BTree
{
    public class TreeFile : ITreeFile
    {
        private string fullfilename;
        private int _maxcachelevel;
        internal int MaxCacheLevel
        {
            get
            {
                if (_maxcachelevel < 1)
                {
                    _maxcachelevel = GlobalSettings.DefaultTreeNodeCacheMaxLevel; if (_maxcachelevel < 1)
                    {
                        _maxcachelevel = 1;
                    }
                }
                return _maxcachelevel;
            }
            set { _maxcachelevel = value; }
        }

        private FileStream _indexstream;

        private HashSet<Int64> _freeNodeAddress = new HashSet<long>();

        private MemoryTreeNode _rootcache;
        public MemoryTreeNode RootNode
        {
            get
            {
                if (_rootcache == null)
                {
                    var root = this.ReadNode(this.config.ConfigDiskBytes);
                    _rootcache = new MemoryTreeNode(root);
                }
                return _rootcache;
            }
            set
            {
                _rootcache = value;
            }
        }

        internal IComparer<byte[]> comparer;

        internal IEqualityComparer<byte[]> equalitycomparer;

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
                            var duplicate = new BTreeIndexDuplicate(this.fullfilename + ".duplicate");
                            duplicate.OpenOrCreate();
                            _duplicate = duplicate;
                        }
                    }
                }
                return _duplicate;
            }

        }

        public TreeFile(string fullfilename, bool unique, Type keytype, int keylen, IComparer<byte[]> comparer, IEqualityComparer<byte[]> equalitycomparer, int MaxCacheLevel)
        {

            this.fullfilename = fullfilename;
            this.comparer = comparer;
            this.equalitycomparer = equalitycomparer;
            this.MaxCacheLevel = MaxCacheLevel;

            if (!File.Exists(this.fullfilename))
            {
                if (keylen == 0 || keytype == null)
                {
                    throw new Exception("must supply key max len and keytype before continue");
                }
                config = new TreeConfig(keylen, unique, keytype, this.MaxCacheLevel, this.comparer, this.equalitycomparer);

                var rootnode = new TreeNode(this.config);
                rootnode.DiskPosition = config.ConfigDiskBytes;
                rootnode.Deletion = Constant.DeleteIndicator.Normal;
                rootnode.TypeIndicator = EnumValues.TypeIndicator.root;

                lock (_Locker)
                {
                    if (!File.Exists(this.fullfilename))
                    {
                        string dirname = Path.GetDirectoryName(this.fullfilename);
                        if (!System.IO.Directory.Exists(dirname))
                        {
                            System.IO.Directory.CreateDirectory(dirname);
                        }

                        FileStream indexFileStream = new FileStream(this.fullfilename, FileMode.Create, FileAccess.Write);
                        indexFileStream.Write(config.ToBytes(), 0, config.ConfigDiskBytes);
                        indexFileStream.Write(rootnode.ToBytes(), 0, config.NodeDiskSize);
                        indexFileStream.Close();
                    }
                }

            }
            else
            {

                byte[] configbytes = this.Read(0, 100);
                TreeConfig config = TreeConfig.FromBytes(configbytes, this.MaxCacheLevel, this.comparer, this.equalitycomparer);
                this.config = config;
            }


        }

        public TreeNode ReadNode(Int64 diskposition)
        {
            if (diskposition < 1)
            {
                return null;
            }
            byte[] nodebytes = this.Read(diskposition, this.config.NodeDiskSize);
            return TreeNode.FromBytes(nodebytes, this.config, diskposition);
        }
        /// <summary>
        /// Create the first left leaf for the node. only create previous left pointer, one leaf.
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="key"></param>
        public void CreateFirstLeaf(TreeNode ParentNode)
        {
            TreeNode newleaf = new TreeNode(this.config);
            newleaf.Deletion = Constant.DeleteIndicator.Normal;
            newleaf.TypeIndicator = EnumValues.TypeIndicator.leaf;

            newleaf.DiskPosition = GetInsertPosition();
            this.Write(newleaf.ToBytes(), newleaf.DiskPosition, this.config.NodeDiskSize);

            var leftpointer = NodePointer.Create(EnumValues.TypeIndicator.leaf, newleaf.DiskPosition);

            ParentNode.PreviousPointer = leftpointer.ToBytes();

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
        /// <param name="keybytes"></param>
        /// <param name="blockposition"></param>
        /// <returns>True = add OK, False = duplicate not added. </returns>
        public bool Add(byte[] keybytes, Int64 blockposition)
        {
            MemoryTreeNode memoryleaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keybytes);

            if (memoryleaf.TreeNode.KeyArray.ContainsKey(keybytes))
            {
                if (this.config.unique)
                {
                    return false;
                }
                else
                {
                    // this is duplicate
                    // check whether duplicate already there or not.  
                    var pointer = NodePointer.FromBytes(memoryleaf.TreeNode.KeyArray[keybytes]);

                    if (pointer.Indicator == EnumValues.TypeIndicator.block)
                    {
                        ///get current block position, 
                        ///insert both position and update positionpointer into new duplicate section start pointer. 

                        Int64 currentposition = pointer.PositionPointer;

                        Int64 duplicateposition = this.duplicate.AddFirst(currentposition, blockposition);

                        ///update duplicate counter. 
                        pointer.Indicator = EnumValues.TypeIndicator.duplicate;
                        pointer.PositionPointer = duplicateposition;

                        Int64 pointerdiskposition = memoryleaf.TreeNode.DiskPosition + pointer.RelativePosition + this.config.KeyLength + 1;

                        this.Write(pointer.ToBytes(), pointerdiskposition);

                    }
                    else if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                    {
                        //only need to insert one item into the duplicate index. 
                        this.duplicate.Add(pointer.PositionPointer, blockposition);
                    }

                    MemoryTreeNodeManager.NodeChange(this, memoryleaf);
                    return true;
                }

            }

            else
            {
                var blockpointer = NodePointer.Create(EnumValues.TypeIndicator.block, blockposition);

                _addnew(memoryleaf, keybytes, blockpointer);

                if (memoryleaf.TreeNode.Count > this.config.SplitCount)
                {
                    SplitLeaf(memoryleaf);
                }
                else
                {
                    MemoryTreeNodeManager.NodeChange(this, memoryleaf);
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
        public Int64 Get(byte[] keyBytes)
        {
            MemoryTreeNode leaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keyBytes);
            if (leaf == null)
            {
                return 0;
            }
            foreach (var item in leaf.TreeNode.KeyArray)
            {
                if (Comparer.ByteEqualComparer.isEqual(item.Key, keyBytes, this.config.KeyLength))
                {
                    var pointer = NodePointer.FromBytes(item.Value);
                    if (this.config.unique)
                    {
                        return pointer.PositionPointer;
                    }
                    else
                    {
                        if (pointer.Indicator != EnumValues.TypeIndicator.duplicate)
                        {
                            return pointer.PositionPointer;
                        }
                        else
                        {
                            ///duplicate keys, return the first key within the duplicate. 
                            return this.duplicate.GetOne(pointer.PositionPointer);

                        }

                    }
                }
            }

            return 0;
        }

        public bool TryGet(byte[] keyBytes, out Int64 outValue)
        {
            MemoryTreeNode leaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keyBytes);
            if (leaf == null)
            {
                outValue = 0;
                return false;
            }
            foreach (var item in leaf.TreeNode.KeyArray)
            {
                if (Comparer.ByteEqualComparer.isEqual(item.Key, keyBytes, this.config.KeyLength))
                {
                    var pointer = NodePointer.FromBytes(item.Value);
                    if (this.config.unique)
                    {
                        outValue = pointer.PositionPointer;
                        return true;
                    }
                    else
                    {
                        if (pointer.Indicator != EnumValues.TypeIndicator.duplicate)
                        {
                            outValue = pointer.PositionPointer;
                            return true;
                        }
                        else
                        {
                            outValue = this.duplicate.GetOne(pointer.PositionPointer);
                            return true;
                        }

                    }
                }
            }

            outValue = 0;
            return false;
        }

        // list all items of this key.... 
        public List<Int64> ListAll(byte[] keybytes)
        {
            List<long> result = new List<long>();
            MemoryTreeNode leaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keybytes);
            if (leaf != null)
            {

                foreach (var item in leaf.TreeNode.KeyArray)
                {
                    if (Comparer.ByteEqualComparer.isEqual(item.Key, keybytes, this.config.KeyLength))
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
        /// remove a key, identified by key and block position. blockposition >0.
        /// </summary>
        /// <param name="keybytes"></param>
        /// <param name="blockposition"></param>
        /// <returns>True = DEL OK, false = DEL failed.</returns>
        public bool Del(byte[] keybytes, Int64 blockposition)
        {
            MemoryTreeNode foundleaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keybytes);

            if (!foundleaf.TreeNode.KeyArray.ContainsKey(keybytes))
            {
                return false;
            }
            var pointer = NodePointer.FromBytes(foundleaf.TreeNode.KeyArray[keybytes]);

            if (pointer.PositionPointer <= 0)
            {
                return false;
            }

            if (pointer.Indicator == EnumValues.TypeIndicator.block)
            {

                if (blockposition > 0 && pointer.PositionPointer != blockposition)
                {
                    /// sanity verification. 
                    return false;
                }

                this.WriteByte(foundleaf.TreeNode.DiskPosition + pointer.RelativePosition, Constant.DeleteIndicator.Deleted);

                //write the new counter.
                foundleaf.TreeNode.KeyArray.Remove(keybytes);
                foundleaf.TreeNode.Count = (Int16)(foundleaf.TreeNode.KeyArray.Count);

                this.Write(BitConverter.GetBytes(foundleaf.TreeNode.Count), foundleaf.TreeNode.DiskPosition + TreeNode.NodeCounterPosition, 2);


                if (foundleaf.TreeNode.Count < this.config.MergeCount)
                {
                    Merge(foundleaf, keybytes);
                }
                else
                {
                    MemoryTreeNodeManager.NodeChange(this, foundleaf);
                }

                return true;
            }

            else
            {
                /// this is record in the duplicate.  
                bool delok = this.duplicate.Del(pointer.PositionPointer, blockposition);
                if (!delok)
                {
                    return false;
                }

                if (!this.duplicate.hasMoreThanOne(pointer.PositionPointer))
                {
                    // this is only one record in the duplidate list  now. 
                    // convert it back to a block. 
                    Int64 existsblockposition = this.duplicate.GetOne(pointer.PositionPointer);
                    if (existsblockposition > 0)
                    {
                        ///update pointer
                        pointer.Indicator = EnumValues.TypeIndicator.block;
                        pointer.PositionPointer = existsblockposition;

                        Int64 pointerdiskposition = foundleaf.TreeNode.DiskPosition + pointer.RelativePosition + this.config.KeyLength + 1;

                        this.Write(pointer.ToBytes(), pointerdiskposition);

                        MemoryTreeNodeManager.NodeChange(this, foundleaf);

                    }

                }

                return true;
            }


        }

        /// <summary>
        /// Delete a key and return the list of keys that have been deleted. 
        /// </summary>
        /// <param name="keybytes"></param>
        /// <returns></returns>
        public List<Int64> Del(byte[] keybytes)
        {
            List<Int64> KeyList = new List<Int64>();

            MemoryTreeNode foundleaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keybytes);

            if (foundleaf == null || !foundleaf.TreeNode.KeyArray.ContainsKey(keybytes))
            {
                return KeyList;
            }
            var pointer = NodePointer.FromBytes(foundleaf.TreeNode.KeyArray[keybytes]);

            if (pointer.PositionPointer <= 0)
            {
                return KeyList;
            }

            if (pointer.Indicator == EnumValues.TypeIndicator.block)
            {
                KeyList.Add(pointer.PositionPointer);

                this.WriteByte(foundleaf.TreeNode.DiskPosition + pointer.RelativePosition, Constant.DeleteIndicator.Deleted);

                //write the new counter.
                foundleaf.TreeNode.KeyArray.Remove(keybytes);
                foundleaf.TreeNode.Count = (Int16)(foundleaf.TreeNode.KeyArray.Count());

                this.Write(BitConverter.GetBytes(foundleaf.TreeNode.Count), foundleaf.TreeNode.DiskPosition + TreeNode.NodeCounterPosition, 2);

                if (foundleaf.TreeNode.Count < this.config.MergeCount)
                {
                    Merge(foundleaf, keybytes);
                }

                return KeyList;
            }

            else
            {
                /// this is record in the duplicate. 

                KeyList = this.duplicate.GetAll(pointer.PositionPointer);

                this.WriteByte(foundleaf.TreeNode.DiskPosition + pointer.RelativePosition, Constant.DeleteIndicator.Deleted);

                //write the new counter.
                foundleaf.TreeNode.KeyArray.Remove(keybytes);
                foundleaf.TreeNode.Count = (Int16)(foundleaf.TreeNode.KeyArray.Count);

                this.Write(BitConverter.GetBytes(foundleaf.TreeNode.Count), foundleaf.TreeNode.DiskPosition + TreeNode.NodeCounterPosition, 2);

                if (foundleaf.TreeNode.Count < this.config.MergeCount)
                {
                    Merge(foundleaf, keybytes);
                }

                return KeyList;
            }
        }

        /// <summary>
        /// remove a key, identified by key and block position. blockposition >0.
        /// </summary>
        /// <param name="keybytes"></param>
        /// <param name="blockposition"></param>
        /// <returns>True = DEL OK, false = DEL failed.</returns>
        public bool Update(byte[] keybytes, Int64 oldBlockPosition, Int64 newBlockPosition)
        {
            MemoryTreeNode foundleaf = MemoryTreeNodeManager.FindLeafByKey(this, this.RootNode, keybytes);

            if (!foundleaf.TreeNode.KeyArray.ContainsKey(keybytes))
            {
                return false;
            }

            var pointer = NodePointer.FromBytes(foundleaf.TreeNode.KeyArray[keybytes]);

            if (pointer.PositionPointer <= 0 && pointer.PositionPointer != oldBlockPosition)
            {
                return false;
            }

            if (pointer.Indicator == EnumValues.TypeIndicator.block)
            {

                if (pointer.PositionPointer != oldBlockPosition)
                {
                    ///sanity verification. 
                    return false;
                }

                pointer.PositionPointer = newBlockPosition;

                Int64 pointerdiskposition = foundleaf.TreeNode.DiskPosition + pointer.RelativePosition + this.config.KeyLength + 1;

                this.Write(pointer.ToBytes(), pointerdiskposition);

                MemoryTreeNodeManager.NodeChange(this, foundleaf);

                return true;

            }

            else if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
            {
                /// this is record in the duplicate. 

                // remove the old record and insert new reocrd. 

                /// check whether the key exists or not. 

                if (!this.duplicate.HasKey(pointer.PositionPointer, oldBlockPosition))
                {
                    // the old key does not exists. 
                    return false;
                }

                bool delok = this.duplicate.Del(pointer.PositionPointer, oldBlockPosition);
                if (!delok)
                {
                    return false;
                }

                bool addok = this.duplicate.Add(pointer.PositionPointer, newBlockPosition);

                if (!addok)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// add a new record, return true = add ok, otherwise, duplicate found. 
        /// </summary>
        /// <param name="MemoryLeaf"></param>
        /// <param name="keybytes"></param>
        /// <param name="blockPointer"></param>
        /// <returns></returns>
        private void _addnew(MemoryTreeNode MemoryLeaf, byte[] keybytes, NodePointer blockPointer)
        {
            if (!MemoryLeaf.TreeNode.KeyArray.ContainsKey(keybytes))
            {
                int relativeposition = MemoryLeaf.TreeNode.GetFreeRelativePosition();

                blockPointer.RelativePosition = (Int16)relativeposition;

                byte[] TotalRecordBytes = new byte[this.config.RecordLen];

                TotalRecordBytes[0] = Constant.DeleteIndicator.Normal;

                System.Buffer.BlockCopy(keybytes, 0, TotalRecordBytes, 1, this.config.KeyLength);

                var pointerBytes = blockPointer.ToBytes();
                System.Buffer.BlockCopy(pointerBytes, 0, TotalRecordBytes, this.config.KeyLength + 1, pointerBytes.Length);

                this.Write(TotalRecordBytes, MemoryLeaf.TreeNode.DiskPosition + relativeposition, this.config.RecordLen);

                MemoryLeaf.TreeNode.KeyArray.Add(keybytes, blockPointer.ToBytes());

                MemoryLeaf.TreeNode.Count = (Int16)MemoryLeaf.TreeNode.KeyArray.Count;

                this.Write(BitConverter.GetBytes(MemoryLeaf.TreeNode.Count), MemoryLeaf.TreeNode.DiskPosition + TreeNode.NodeCounterPosition, 2);
            }
        }

        private void DeleteNode(Int64 diskposition)
        {
            if (diskposition > 0)
            {
                this.WriteByte(diskposition + TreeNode.DeletionIndicationPosition, Constant.DeleteIndicator.Deleted);
                this._freeNodeAddress.Add(diskposition);
            }
        }

        private void DeleteKey(TreeNode node, byte[] keybytes)
        {
            // mark that record pointer as null byte. 
            var pointer = NodePointer.FromBytes(node.KeyArray[keybytes]);

            this.WriteByte(node.DiskPosition + pointer.RelativePosition, Constant.DeleteIndicator.Deleted);

            node.KeyArray.Remove(keybytes);
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

            Int64 recordposition = node.DiskPosition + PreviousPointerPosition;

            byte[] nullbytes = new byte[this.config.PointerLen];
            nullbytes[0] = Constant.DeleteIndicator.Deleted;

            this.Write(nullbytes, recordposition, this.config.PointerLen);
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
                var previouspointer = NodePointer.FromBytes(node.TreeNode.PreviousPointer, true);

                // if there is no key and also not any left previous pointer. 
                if (node.TreeNode.KeyArray.Count == 0 & previouspointer.PositionPointer == 0)
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

            Dictionary<byte[], byte[]> NewKeyArray = new Dictionary<byte[], byte[]>(this.equalitycomparer);
            // get first half of the record into new leaf. this is based on the decision key is on left or right.
            Dictionary<byte[], byte[]> RemainingKeyArray = new Dictionary<byte[], byte[]>(this.equalitycomparer);

            int halfcount = (int)(MemoryLeaf.TreeNode.KeyArray.Count / 2);

            int i = 0;

            byte[] newLeafKey = null;

            foreach (var item in MemoryLeaf.TreeNode.KeyArray.OrderBy(o => o.Key, comparer))
            {
                if (i < halfcount)
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
            TreeNode newleaf = new TreeNode(this.config);
            newleaf.Deletion = Constant.DeleteIndicator.Normal;
            newleaf.TypeIndicator = EnumValues.TypeIndicator.leaf;

            newleaf.KeyArray = NewKeyArray;

            long diskposition = GetInsertPosition();

            this.Write(newleaf.ToBytes(), diskposition, this.config.NodeDiskSize);

            // 2. update to the parent node of the new leaf.  
            ///TreeNode ParentNode = LoadParentNode(MemoryLeaf);

            var newkeypointer = NodePointer.Create(EnumValues.TypeIndicator.leaf, diskposition);

            _addnew(MemoryLeaf.Parent, newLeafKey, newkeypointer);

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

        //This is very similiar like SplitLeaf, should be combined.
        private void SplitNode(MemoryTreeNode node)
        {
            /// 4 steps to split, 
            /// 1. create a new node with half of the records.
            /// 2. update the old node to  half left record node.
            /// 3. update to the parent node of the new node by adding a new record. 
            /// 4. Check whether the parent updated node needs split or not.
            ///  NOTE: Node has previouspointer, Leaf does not have.
            ///  When split a node, the the first key of the new node shoulbe be removed and move into previouspointer.

            if (node.TreeNode.TypeIndicator == EnumValues.TypeIndicator.root)
            {
                SplitRoot(node);
                return;
            }

            Dictionary<byte[], byte[]> NewKeyArray = new Dictionary<byte[], byte[]>(this.equalitycomparer);
            // get first half of the record into new leaf. this is based on the decision key is on left or right.
            Dictionary<byte[], byte[]> leftKeyArray = new Dictionary<byte[], byte[]>(this.equalitycomparer);

            int halfcount = (int)node.TreeNode.KeyArray.Count / 2;
            int i = 0;

            // first pair used at the previous pointer.
            byte[] firstkey = null;
            byte[] firstPreviousPointer = null;

            foreach (var item in node.TreeNode.KeyArray.OrderBy(o => o.Key, this.comparer))
            {
                if (i < halfcount)
                {
                    leftKeyArray.Add(item.Key, item.Value);
                }
                else
                {
                    if (firstkey == null)
                    {
                        firstkey = item.Key;
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
            TreeNode newnode = new TreeNode(this.config);
            newnode.Deletion = Constant.DeleteIndicator.Normal;
            newnode.TypeIndicator = EnumValues.TypeIndicator.node;

            newnode.PreviousPointer = firstPreviousPointer;

            newnode.KeyArray = NewKeyArray;

            long diskposition = GetInsertPosition();

            this.Write(newnode.ToBytes(), diskposition, this.config.NodeDiskSize);

            // 2. update to the parent node of the new node.  
            var newkeypointer = NodePointer.Create(EnumValues.TypeIndicator.node, diskposition);

            _addnew(node.Parent, firstkey, newkeypointer);

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

        private void SplitRoot(MemoryTreeNode rootnode)
        {
            // 1. move the right half to a new node. 
            // 2. move the left left side half to another new node. 
            // 3. update the root with one new keyvalue pair only to the this two new nodes.

            Dictionary<byte[], byte[]> RightKeyArray = new Dictionary<byte[], byte[]>(this.equalitycomparer);
            // get first half of the record into new leaf. this is based on the decision key is on left or right.
            Dictionary<byte[], byte[]> leftKeyArray = new Dictionary<byte[], byte[]>(this.equalitycomparer);

            int halfcount = (int)rootnode.TreeNode.KeyArray.Count / 2;
            int i = 0;

            // first pair used at the previous pointer.
            byte[] firstkey = null;

            byte[] firstPreviousPointer = null;

            foreach (var item in rootnode.TreeNode.KeyArray.OrderBy(o => o.Key, this.comparer))
            {
                if (i < halfcount)
                {
                    leftKeyArray.Add(item.Key, item.Value);
                }
                else
                {
                    if (firstkey == null)
                    {
                        firstkey = item.Key;
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
            TreeNode rightnode = new TreeNode(this.config);
            rightnode.Deletion = Constant.DeleteIndicator.Normal;
            rightnode.TypeIndicator = EnumValues.TypeIndicator.node;

            rightnode.PreviousPointer = firstPreviousPointer;
            rightnode.KeyArray = RightKeyArray;

            long rightnodediskposition = GetInsertPosition();

            this.Write(rightnode.ToBytes(), rightnodediskposition, this.config.NodeDiskSize);

            //2. create a left new node with half of the records. 
            TreeNode leftnode = new TreeNode(this.config);
            leftnode.Deletion = Constant.DeleteIndicator.Normal;
            leftnode.TypeIndicator = EnumValues.TypeIndicator.node;

            leftnode.PreviousPointer = rootnode.TreeNode.PreviousPointer;
            leftnode.KeyArray = leftKeyArray;

            long leftnodediskposition = GetInsertPosition();

            this.Write(leftnode.ToBytes(), leftnodediskposition, this.config.NodeDiskSize);

            // 3. update the root with one new keyvalue pair only to the this two new nodes.

            TreeNode newroot = new TreeNode(this.config);
            newroot.Deletion = Constant.DeleteIndicator.Normal;
            newroot.TypeIndicator = EnumValues.TypeIndicator.root;

            var leftpointer = NodePointer.Create(EnumValues.TypeIndicator.node, leftnodediskposition);

            var rightpointer = NodePointer.Create(EnumValues.TypeIndicator.node, rightnodediskposition);

            newroot.PreviousPointer = leftpointer.ToBytes();
            newroot.KeyArray.Add(firstkey, rightpointer.ToBytes());

            this.Write(newroot.ToBytes(), rootnode.TreeNode.DiskPosition, this.config.NodeDiskSize);

            MemoryTreeNodeManager.NodeChange(this, rootnode);
        }

        private void Merge(MemoryTreeNode leaf, byte[] keybytes)
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
            var hasmerge = MergeLeaf(leaf, parent);

            // after leaf is merge, check node. 
            if (hasmerge)
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
        /// <param name="leafx"></param>
        /// <param name="leafy"></param>
        /// <returns></returns>
        private bool canMerge(TreeNode leafx, TreeNode leafy)
        {
            if (leafx.TypeIndicator != leafy.TypeIndicator)
            {
                return false;
            }

            if (leafx.DiskPosition != 0 && leafx.DiskPosition == leafy.DiskPosition)
            {
                return false;
            }

            if ((leafx.KeyArray.Count + leafy.KeyArray.Count) > this.config.SplitCount)
            {
                return false;
            }
            return true;
        }

        private bool MergeLeaf(MemoryTreeNode leaf, MemoryTreeNode ParentNode)
        {
            bool hasmerge = false;
            // Find the nodepointertoleaf that is just  key < that leaf.
            byte[] leafParentKey = leaf.ParentNodeKey;  // null = the previouspointer..

            //if (ParentNode.TreeNode.KeyArray.Count == 0)
            //{
            //    /// TODO: verify this condition & action.
            //    return false;
            //}

            byte[] biggerkey = KeyFinder.FindSmallestBiggerKey(leafParentKey, ParentNode.TreeNode.KeyArray, this.comparer);

            if (biggerkey != null)
            {
                //move all the found keys to current leaf and remove the found leaf.  
                var foundpointer = NodePointer.FromBytes(ParentNode.TreeNode.KeyArray[biggerkey], biggerkey);

                TreeNode foundleaf = ReadNode(foundpointer.PositionPointer);

                if (canMerge(foundleaf, leaf.TreeNode))
                {
                    foreach (var item in foundleaf.KeyArray)
                    {
                        leaf.TreeNode.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(leaf.TreeNode.ToBytes(), leaf.TreeNode.DiskPosition, this.config.NodeDiskSize);

                    // remove from parent node. 
                    DeleteKey(ParentNode.TreeNode, biggerkey);
                    DeleteNode(foundleaf.DiskPosition);
                    // MemoryTreeNodeManager.NodeReload(this, ParentNode); 
                    hasmerge = true;
                }
            }

            else
            {
                // this leaf is the biggest key in the parent keyarray.
                // Find the second biggest key
                // move this leaf to the found, and remove this key. 
                byte[] foundkey = KeyFinder.FindBiggestSmallerKey(leafParentKey, ParentNode.TreeNode.KeyArray, this.comparer);

                NodePointer smallerpointer = null;

                if (foundkey != null)
                {
                    smallerpointer = NodePointer.FromBytes(ParentNode.TreeNode.KeyArray[foundkey], foundkey);
                }
                else
                {
                    smallerpointer = NodePointer.FromBytes(ParentNode.TreeNode.PreviousPointer, true);
                }

                TreeNode foundleaf = ReadNode(smallerpointer.PositionPointer);

                if (canMerge(foundleaf, leaf.TreeNode))
                {
                    // this.CacheNodes.Remove(foundleaf);

                    foreach (var item in leaf.TreeNode.KeyArray)
                    {
                        foundleaf.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(foundleaf.ToBytes(), foundleaf.DiskPosition, this.config.NodeDiskSize);

                    DeleteKey(ParentNode.TreeNode, leafParentKey);
                    DeleteNode(leaf.TreeNode.DiskPosition);

                    // MemoryTreeNodeManager.NodeChange(this, ParentNode); 
                    hasmerge = true;

                }

            }

            return hasmerge;
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

            // Find the nodepointer that is just  key < that leaf.
            // The dictionary.OrderBy is very expensive, that is why we have to use this complicate way.
            MemoryTreeNode ParentNode = node.Parent;

            if (ParentNode.TreeNode.KeyArray.Count == 0)
            {
                /// TODO: verify this condition  & action.
                return false;
            }

            byte[] biggerkey = KeyFinder.FindSmallestBiggerKey(node.ParentNodeKey, ParentNode.TreeNode.KeyArray, this.comparer);

            if (biggerkey != null)
            {
                //move all the found keys to current node and remove the found node. 
                //NodePointer foundpointer = new NodePointer();
                //foundpointer.PointerBytes = ParentNode.TreeNode.KeyArray[biggerkey];
                //foundpointer.KeyToPosition = biggerkey;
                var foundpointer = NodePointer.FromBytes(ParentNode.TreeNode.KeyArray[biggerkey], biggerkey);

                TreeNode foundnode = ReadNode(foundpointer.PositionPointer);

                if (canMerge(node.TreeNode, foundnode))
                {
                    byte[] foundnode_firstkey = KeyFinder.FindFirstKey(foundnode.KeyArray, this.comparer);

                    /// This is to check whether the biggerKey is the first key of foundnode or not. 
                    /// if yes, means that is not a previous pointer in foundnode.
                    if (this.comparer.Compare(biggerkey, foundnode_firstkey) != 0)
                    {
                        //NodePointer foundprevious = new NodePointer();
                        //foundprevious.PointerBytes = foundnode.PreviousPointer;
                        var foundprevious = NodePointer.FromBytes(foundnode.PreviousPointer, true);

                        if (foundpointer.PositionPointer > 0)
                        {
                            node.TreeNode.KeyArray.Add(biggerkey, foundprevious.ToBytes());
                        }
                    }

                    foreach (var item in foundnode.KeyArray)
                    {
                        node.TreeNode.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(node.TreeNode.ToBytes(), node.TreeNode.DiskPosition, this.config.NodeDiskSize);

                    // remove from parent node. 
                    DeleteKey(ParentNode.TreeNode, biggerkey);
                    DeleteNode(foundnode.DiskPosition);
                    hasMerge = true;
                }
            }
            else
            {
                // this node is the biggest key in the parent keyarray.
                // Find the second biggest key
                // move this node to the found, and remove this key. 

                byte[] foundkey = KeyFinder.FindBiggestSmallerKey(node.ParentNodeKey, ParentNode.TreeNode.KeyArray, this.comparer);

                NodePointer smallerpointer = null;

                if (foundkey == null)
                {
                    //smallerpointer.PointerBytes = ParentNode.TreeNode.PreviousPointer;
                    //smallerpointer.KeyToPosition = null;
                    smallerpointer = NodePointer.FromBytes(ParentNode.TreeNode.PreviousPointer, true);
                }
                else
                {
                    //smallerpointer.PointerBytes = ParentNode.TreeNode.KeyArray[foundkey];
                    //smallerpointer.KeyToPosition = foundkey;
                    smallerpointer = NodePointer.FromBytes(ParentNode.TreeNode.KeyArray[foundkey], foundkey);
                }

                TreeNode _foundnode = ReadNode(smallerpointer.PositionPointer);

                if (canMerge(node.TreeNode, _foundnode))
                {
                    //The previous pointer, biggestone is the key pointer to current node. 
                    //NodePointer previous = new NodePointer();
                    //previous.PointerBytes = node.TreeNode.PreviousPointer;
                    var previous = NodePointer.FromBytes(node.TreeNode.PreviousPointer, true);

                    if (previous.PositionPointer > 0)
                    {
                        _foundnode.KeyArray.Add(node.ParentNodeKey, previous.ToBytes());
                    }

                    foreach (var item in node.TreeNode.KeyArray)
                    {
                        _foundnode.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(_foundnode.ToBytes(), _foundnode.DiskPosition, this.config.NodeDiskSize);

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
                    var domerge = MergeNode(ParentNode);
                    if (!domerge)
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
        /// Before call this method, must already check that node.keyarray.count = 1; and this is the root.
        /// </summary>
        /// <param name="node"></param>
        private bool CombineSub(TreeNode root)
        {
            //NodePointer previous = new NodePointer();
            //previous.PointerBytes = root.PreviousPointer;
            var previous = NodePointer.FromBytes(root.PreviousPointer, true);

            TreeNode newroot;

            if (previous.PositionPointer != 0 && previous.Indicator == EnumValues.TypeIndicator.node)
            {
                newroot = ReadNode(previous.PositionPointer);

                //can only be 1 or 0, 0 do nothing, 1, check sub node = type node and  move the right node to leaf node. 
                if (root.KeyArray.Count == 1)
                {
                    byte[] rightkey = root.KeyArray.First().Key;
                    //NodePointer rightpointer = new NodePointer();
                    //rightpointer.PointerBytes = root.KeyArray[rightkey];
                    //rightpointer.KeyToPosition = rightkey;
                    var rightpointer = NodePointer.FromBytes(root.KeyArray[rightkey], rightkey);

                    if (rightpointer.Indicator != EnumValues.TypeIndicator.node)
                    {
                        return false;
                    }

                    TreeNode rightnode = ReadNode(rightpointer.PositionPointer);

                    if (!this.canMerge(newroot, rightnode))
                    {
                        return false;
                    }

                    //NodePointer subprevious = new NodePointer();
                    //subprevious.PointerBytes = rightnode.PreviousPointer;
                    var subprevious = NodePointer.FromBytes(rightnode.PreviousPointer, true);

                    if (subprevious.PositionPointer > 0)
                    {
                        newroot.KeyArray.Add(rightkey, subprevious.ToBytes());
                    }

                    foreach (var item in rightnode.KeyArray)
                    {
                        newroot.KeyArray.Add(item.Key, item.Value);
                    }

                    this.Write(newroot.ToBytes(), root.DiskPosition, this.config.NodeDiskSize);

                    /// remove the two nodes. 
                    DeleteNode(previous.PositionPointer);
                    DeleteNode(rightpointer.PositionPointer);

                    return true;
                }

            }
            else
            {
                // there is not previous pointer, make check whether there is a need to make node upgrade to root.  
                //can only be 1 or 0, 0 do nothing, 1, move the right node to root node. 
                if (root.KeyArray.Count == 1)
                {
                    byte[] rightkey = root.KeyArray.First().Key;
                    //NodePointer rightpointer = new NodePointer();
                    //rightpointer.PointerBytes = root.KeyArray[rightkey];
                    //rightpointer.KeyToPosition = rightkey;
                    var rightpointer = NodePointer.FromBytes(root.KeyArray[rightkey], rightkey);

                    if (rightpointer.Indicator != EnumValues.TypeIndicator.node)
                    {
                        return false;
                    }


                    if (rightpointer.PositionPointer > 0)
                    {

                        //use the right node to replace root node. 
                        //IndexStream.Position = rightpointer.PositionPointer; 
                        //byte[] rightnodebytes = new byte[this.config.NodeDiskSize]; 
                        //IndexStream.Read(rightnodebytes, 0, this.config.NodeDiskSize);
                        byte[] rightnodebytes = this.Read(rightpointer.PositionPointer, this.config.NodeDiskSize);

                        this.Write(rightnodebytes, root.DiskPosition, this.config.NodeDiskSize);

                        ////DELTE the NODE
                        DeleteNode(rightpointer.PositionPointer);

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
                if (_indexstream == null || _indexstream.CanRead == false)
                {
                    lock (_Locker)
                    {
                        if (_indexstream == null || _indexstream.CanRead == false)
                        {
                            if (File.Exists(fullfilename))
                            {
                                _indexstream = StreamManager.GetFileStream(this.fullfilename);
                            }
                        }
                    }
                }
                return _indexstream;
            }
        }

        public void Close()
        {
            if (_indexstream != null)
            {
                lock (_Locker)
                {
                    if (_indexstream != null)
                    {
                        _indexstream.Close();
                        _indexstream = null;
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
                if (_indexstream != null)
                {
                    _indexstream.Close();
                    _indexstream = null;
                }
                File.Delete(this.fullfilename);
            }
        }

        public void Flush()
        {
            if (_indexstream != null)
            {
                lock (_Locker)
                {
                    if (_indexstream != null)
                    {
                        _indexstream.Flush();
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
            var lastleaf = MemoryTreeNodeManager.FindLastLeaf(this);

            if (lastleaf != null)
            {
                return KeyFinder.FindLastKey(lastleaf.TreeNode.KeyArray, this.comparer);
            }
            return null;
        }

    }

}
