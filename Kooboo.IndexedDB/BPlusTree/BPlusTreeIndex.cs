using System;
using System.Collections.Generic;
using System.IO;
using Kooboo.IndexedDB.BTree;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Helper;


namespace Kooboo.IndexedDB.BPlusTree
{
    public class BPlusTreeIndex<T>
    {
        public int keyLength;
        private string fullIndexFileName;
        internal int MaxCacheLevel { get; set; }
        /// <summary>
        /// the object for lock.
        /// </summary>
        private object _object = new object();

        private BPlusTreeFile _tree;

        internal BPlusTreeFile Tree
        {
            get
            {
                if (_tree == null)
                {
                    lock (_object)
                    {
                        if (_tree == null)
                        {
                            _tree = new BPlusTreeFile(this.fullIndexFileName, typeof(T), keyLength, this.Comparer, this.EqualityComparer, MaxCacheLevel, this.BPlusLen);
                        }
                    }
                }
                return _tree;
            }
        }

        private IComparer<byte[]> Comparer;

        private IByteConverter<T> Converter;

        private int BPlusLen;

        private IEqualityComparer<byte[]> EqualityComparer;  /// only used for dictionary to find key. 

        ///only string key has the keyLength, the rest does not have. 
        private bool IsKeyLenVaries = false;

        public BPlusTreeIndex(string fullIndexFileName, int keylength, int BPlusLen, int MaxCacheLevel = 0)
        {
            Type keyType = typeof(T);
            this.keyLength = Helper.KeyHelper.GetKeyLen(keyType, keylength);

            this.fullIndexFileName = fullIndexFileName;

            this.MaxCacheLevel = MaxCacheLevel;

            Helper.IndexHelper.VerifyIndexType(keyType);

            this.Converter = ObjectContainer.GetConverter<T>();
            this.EqualityComparer = new BTree.Comparer.EqualityComparer(this.keyLength);
            this.Comparer = ObjectContainer.getComparer(keyType, keylength);
            this.BPlusLen = BPlusLen;

            this.IsKeyLenVaries = KeyHelper.IsKeyLenVar(keyType);

        }

        public void Close()
        {
            lock (_object)
            {
                if (_tree != null)
                {
                    _tree.Close();
                    _tree = null;
                }
            }
        }

        public void DelSelf()
        {
            lock (_object)
            {
                if (_tree != null)
                {
                    _tree.Close();
                    _tree = null;
                }
                if (System.IO.File.Exists(fullIndexFileName))
                {
                    File.Delete(fullIndexFileName);
                }
            }
        }

        public void Flush()
        {
            lock (_object)
            {
                if (_tree != null)
                {
                    _tree.IndexStream.Flush();
                }
            }
        }


        private bool Add(byte[] keyBytes, Int64 blockPosition, byte[] BPlusBytes)
        {
            lock (_object)
            {
                return this.Tree.Add(this.appendToFixedLength(keyBytes), blockPosition, BPlusBytes);
            }
        }

        public bool Add(T key, Int64 blockPosition, byte[] BPlusBytes)
        {
            if (key == null)
            {
                key = default(T);
            }
            return Add(this.Converter.ToByte(key), blockPosition, BPlusBytes);
        }

        public void Update(T key, byte[] BplusBytes, Int64 newBlockPosition)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);

            lock (_object)
            {
                this.Tree.Update(this.appendToFixedLength(keyBytes), BplusBytes, newBlockPosition);
            }
        }

        public void UpdateBplus(T key, byte[] BPlusBytes)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);

            lock (_object)
            {
                this.Tree.UpdateBplus(this.appendToFixedLength(keyBytes), BPlusBytes);
            }
        }


        public Int64 GetPosition(T key)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);

            lock (_object)
            {
                return this.Tree.GetBlockPosition(this.appendToFixedLength(keyBytes));
            }
        }

        public byte[] GetBplus(T key)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);

            lock (_object)
            {
                return this.Tree.GetBPlus(this.appendToFixedLength(keyBytes));
            }
        }

        public NodePointer GetPointer(T key)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);

            lock (_object)
            {
                return this.Tree.GetPointer(this.appendToFixedLength(keyBytes));
            }
        }


        public List<Int64> List(T key)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);

            return List(keyBytes);
        }

        public List<Int64> List(byte[] keyBytes)
        {
            lock (_object)
            {
                return this.Tree.ListAll(this.appendToFixedLength(keyBytes));
            }
        }

        public bool Del(T key, Int64 blockPosition)
        {
            if (blockPosition <= 0)
            {
                throw new Exception("block position not provided");
            }

            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);
            return Del(keyBytes, blockPosition);
        }

        private bool Del(byte[] keyBytes, Int64 blockPosition)
        {
            lock (_object)
            {
                return this.Tree.Del(this.appendToFixedLength(keyBytes), blockPosition);
            }
        }

        public bool Del(T key)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);
            return Del(keyBytes);
        }

        private bool Del(byte[] keyBytes)
        {
            lock (_object)
            {
                return this.Tree.Del(this.appendToFixedLength(keyBytes));
            }
        }

        /// <summary>
        /// append byte 0 to the input to make it match the keyLength. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] appendToFixedLength(byte[] input)
        {
            return Helper.KeyHelper.AppendToKeyLength(input, IsKeyLenVaries, this.keyLength);
        }

        public T FirstKey
        {
            get
            {
                lock (_object)
                {
                    byte[] keyBytes = this.Tree.FirstKey();

                    if (keyBytes == null)
                    {
                        return default;
                    }
                    else
                    {
                        return this.Converter.FromByte(keyBytes);
                    }
                }
            }
        }

        public T LastKey
        {
            get
            {
                lock (_object)
                {
                    byte[] keyBytes = this.Tree.LastKey();

                    if (keyBytes == null)
                    {
                        return default;
                    }
                    else
                    {
                        return this.Converter.FromByte(keyBytes);
                    }
                }
            }
        }

        /// <summary>
        /// Get the count of records in the range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public int Count(Range<T> range)
        {
            lock (_object)
            {
                byte[] startKeyBytes = this.Converter.ToByte(range.lower);
                byte[] endKeyBytes = this.Converter.ToByte(range.upper);

                startKeyBytes = this.appendToFixedLength(startKeyBytes);
                endKeyBytes = this.appendToFixedLength(endKeyBytes);

                MemoryTreeNode startNode = MemoryTreeNodeManager.FindLeafByKey(this.Tree, this.Tree.RootNode, startKeyBytes);
                MemoryTreeNode endNode = MemoryTreeNodeManager.FindLeafByKey(this.Tree, this.Tree.RootNode, endKeyBytes);

                if (startNode.TreeNode.DiskPosition == endNode.TreeNode.DiskPosition)
                {

                    int count = 0;
                    foreach (var item in startNode.TreeNode.KeyArray)
                    {
                        if ((this.Comparer.Compare(startKeyBytes, item.Key) < 0 || (this.Comparer.Compare(startKeyBytes, item.Key) == 0 && !range.lowerOpen)) && ((this.Comparer.Compare(endKeyBytes, item.Key) > 0 || (this.Comparer.Compare(endKeyBytes, item.Key) == 0 && !range.upperOpen))))

                            count += 1;
                    }
                    return count;
                }
                else
                {
                    int firstNodeCount = 0;
                    int lastNodeCount = 0;
                    int middleCount = 0;

                    foreach (var item in startNode.TreeNode.KeyArray)
                    {
                        if (this.Comparer.Compare(startKeyBytes, item.Key) < 0 || (this.Comparer.Compare(startKeyBytes, item.Key) == 0 && !range.lowerOpen))
                        {

                            firstNodeCount += 1;

                        }
                    }

                    foreach (var item in endNode.TreeNode.KeyArray)
                    {

                        if (this.Comparer.Compare(endKeyBytes, item.Key) > 0 || (this.Comparer.Compare(endKeyBytes, item.Key) == 0 && !range.upperOpen))
                        {

                            lastNodeCount += 1;

                        }

                    }

                    var middleNode = MemoryTreeNodeManager.FindNextLeaf(this.Tree, startNode);

                    while (middleNode.TreeNode.DiskPosition != endNode.TreeNode.DiskPosition)
                    {
                        foreach (var item in middleNode.TreeNode.KeyArray)
                        {
                            middleCount += 1;
                        }

                        middleNode = MemoryTreeNodeManager.FindNextLeaf(this.Tree, middleNode);
                    }

                    return firstNodeCount + middleCount + lastNodeCount;
                }
            }
        }

        public int Count()
        {
            if (this.FirstKey == null && this.LastKey == null)
            {
                return 0;
            }

            Range<T> range = Range<T>.bound(this.FirstKey, this.LastKey, false, false);
            return Count(range);
        }

        public List<Int64> getRange(Range<T> range, int skip, int take, bool ascending)
        {

            int skipped = 0;
            int taken = 0;
            List<long> list = new List<long>();
            if (take <= 0)
            {
                return list;
            }

            foreach (var item in getCollection(range, ascending))
            {
                if (skipped < skip)
                {
                    skipped += 1;
                    continue;
                }

                list.Add(item.PositionPointer);

                taken += 1;

                if (taken >= take)
                {
                    return list;
                }
            }

            return list;
        }

        /// <summary>
        /// get a collection object that contains of the range records. 
        /// </summary>
        /// <param name="range"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public BPlusCollection getCollection(Range<T> range, bool ascending)
        {
            byte[] startKeyBytes = this.Converter.ToByte(range.lower);
            byte[] endKeyBytes = this.Converter.ToByte(range.upper);

            return getCollection(startKeyBytes, endKeyBytes, range.lowerOpen, range.upperOpen, ascending);
        }


        /// <summary>
        /// get a collection object that contains of the range records. 
        /// </summary>
        /// <param name="startKey"></param>
        /// <param name="endKey"></param>
        /// <param name="lowerOpen"></param>
        /// <param name="upperOpen"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public BPlusCollection getCollection(byte[] startKey, byte[] endKey, bool lowerOpen, bool upperOpen, bool ascending)
        {
            if (startKey == null)
            {
                startKey = this.Tree.FirstKey();
                lowerOpen = false;
            }

            if (endKey == null)
            {
                endKey = this.Tree.LastKey();
                upperOpen = false;
            }

            startKey = this.appendToFixedLength(startKey);
            endKey = this.appendToFixedLength(endKey);

            BPlusCollection collection = new BPlusCollection(this.Tree, this.Comparer, startKey, endKey, lowerOpen, upperOpen, this.BPlusLen, ascending);

            return collection;
        }

        /// <summary>
        /// get the collection that contains all items in the index. 
        /// </summary>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public BPlusCollection allItemCollection(bool ascending)
        {
            Range<T> range = Range<T>.bound(this.FirstKey, this.LastKey, false, false);
            return getCollection(range, ascending);
        }

        public List<Int64> All(bool ascending = false)
        {
            List<long> list = new List<long>();
            foreach (var item in allItemCollection(ascending))
            {
                list.Add(item.PositionPointer);
            }

            return list;
        }
    }

}
