//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using Kooboo.IndexedDB.BTree.Comparer;
using Kooboo.IndexedDB.ByteConverter;

namespace Kooboo.IndexedDB.BTree
{
    /// <summary>
    /// string index. 
    /// </summary>
    public class BTreeIndex<T>
    {
        public string fieldname;
        public bool unique;
        public int keyLength;

        private string FullIndexFileName;

        private string FullDuplicateFileName;

        internal int MaxCacheLevel { get; set; }


        private object _lockObject = new object();

        private TreeFile _tree;

        internal TreeFile Tree
        {
            get
            {

                if (_tree == null)
                {
                    lock (_lockObject)
                    {
                        if (_tree == null)
                        {
                            _tree = new TreeFile(this.FullIndexFileName, this.unique, typeof(T), keyLength, this.Comparer, this.EqualityComparer, MaxCacheLevel);
                        }
                    }
                }

                return _tree;
            }
        }

        private IComparer<byte[]> Comparer;

        private IByteConverter<T> Converter;

        private IEqualityComparer<byte[]> EqualityComparer;  /// only used for dictionary to find key. 

        ///only string key has the keyLength, the rest does not have. 
        private bool IsKeyLenVaries = false;

        public BTreeIndex(string fieldname, bool unique, int keyLength, string fullIndexFileName, int MaxCacheLevel = 0)
        {
            this.fieldname = fieldname;
            this.unique = unique;
            Type keyType = typeof(T);
            this.keyLength = Helper.KeyHelper.GetKeyLen(keyType, keyLength);

            this.FullIndexFileName = fullIndexFileName;

            this.MaxCacheLevel = MaxCacheLevel;

            this.FullDuplicateFileName = FullIndexFileName + ".duplicate";

            Helper.IndexHelper.VerifyIndexType(keyType);

            this.Converter = ObjectContainer.GetConverter<T>();
            this.EqualityComparer = new EqualityComparer(this.keyLength);
            this.Comparer = ObjectContainer.getComparer(keyType, keyLength);

            this.IsKeyLenVaries = Helper.KeyHelper.IsKeyLenVar(keyType);


        }

        public bool Exists
        {
            get
            {
                return File.Exists(this.FullIndexFileName);
            }
        }

        public void Close()
        {
            lock (_lockObject)
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
            lock (_lockObject)
            {
                if (_tree != null)
                {
                    _tree.Close();
                    _tree = null;
                }
                if (System.IO.File.Exists(FullIndexFileName))
                {
                    File.Delete(FullIndexFileName);
                }
            }
        }

        public void Flush()
        {
            lock (_lockObject)
            {
                if (_tree != null)
                {
                    _tree.IndexStream.Flush();
                }
            }
        }


        private bool Add(byte[] keyBytes, Int64 blockPosition)
        {
            lock (_lockObject)
            {
                return this.Tree.Add(this.appendToFixedLength(keyBytes), blockPosition);
            }
        }

        public bool Add(T key, Int64 blockPosition)
        {

            if (key == null)
            {
                key = default(T);
            }


            return Add(this.Converter.ToByte(key), blockPosition);
        }

        public void Update(T oldKey, T newKey, Int64 oldBlockPosition, Int64 newBlockPosition)
        {
            if (oldKey == null)
            {
                oldKey = default(T);
            }
            if (newKey == null)
            {
                newKey = default(T);
            }

            byte[] oldKeyByte = this.appendToFixedLength(this.Converter.ToByte(oldKey));
            byte[] newKeyByte = this.appendToFixedLength(this.Converter.ToByte(newKey));

            if (ByteEqualComparer.isEqual(oldKeyByte, newKeyByte, this.keyLength))
            {
                Update(oldKey, oldBlockPosition, newBlockPosition);
            }
            else
            {
                this.Add(newKeyByte, newBlockPosition);
                this.Del(oldKeyByte, oldBlockPosition);
            }
        }

        public void Update(T key, Int64 oldBlockPosition, Int64 newBlockPosition)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keyBytes = this.Converter.ToByte(key);

            lock (_lockObject)
            {
                this.Tree.Update(this.appendToFixedLength(keyBytes), oldBlockPosition, newBlockPosition);
            }
        }

        public Int64 Get(T key)
        {
            if (key == null)
            {
                key = default(T);
            }
            byte[] keyBytes = this.Converter.ToByte(key);
            return Get(keyBytes);
        }

        public Int64 Get(byte[] keyBytes)
        {
            lock (_lockObject)
            {
                return this.Tree.Get(this.appendToFixedLength(keyBytes));
            }
        }

        public bool TryGet(T key, out Int64 position)
        {
            if (key == null)
            {
                key = default(T);
            }
            byte[] keyBytes = this.Converter.ToByte(key);
            return TryGet(keyBytes, out position);
        }

        public bool TryGet(byte[] keyBytes, out Int64 position)
        {
            lock (_lockObject)
            {
                return this.Tree.TryGet(keyBytes, out position);
            }
        }

        public List<Int64> List(T key)
        {
            if (key == null)
            {
                key = default;
            }

            byte[] keyBytes = this.Converter.ToByte(key);

            return List(keyBytes);
        }

        public List<Int64> List(byte[] keyBytes)
        {
            lock (_lockObject)
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
                key = default;
            }

            byte[] keyBytes = this.Converter.ToByte(key);
            return Del(keyBytes, blockPosition);
        }

        private bool Del(byte[] keyBytes, Int64 blockPosition)
        {
            lock (_lockObject)
            {
                return this.Tree.Del(this.appendToFixedLength(keyBytes), blockPosition);
            }
        }

        public List<Int64> Del(T key)
        {
            if (key == null)
            {
                key = default;
            }

            byte[] keyBytes = this.Converter.ToByte(key);
            return Del(keyBytes);
        }

        private List<Int64> Del(byte[] keyBytes)
        {
            lock (_lockObject)
            {
                return this.Tree.Del(this.appendToFixedLength(keyBytes));
            }
        }


        private byte[] appendToFixedLength(byte[] input)
        {
            return Helper.KeyHelper.AppendToKeyLength(input, IsKeyLenVaries, this.keyLength);
        }

        public T FirstKey
        {
            get
            {
                lock (_lockObject)
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
                lock (_lockObject)
                {
                    byte[] keyBytes = this.Tree.LastKey();

                    if (keyBytes == null)
                    {
                        return default(T);
                    }
                    else
                    {
                        return this.Converter.FromByte(keyBytes);
                    }
                }
            }
        }

        /// <summary>
        /// Get the count of records in the range. count distinct will have better performance. 
        /// </summary>
        /// <param name="range"></param>
        /// <param name="distinct"></param>
        /// <returns></returns>
        public int Count(Range<T> range, bool distinct)
        {
            lock (_lockObject)
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

                            if (!distinct)
                            {
                                var pointer = NodePointer.FromBytes(item.Value);

                                if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                                {
                                    count += this.Tree.duplicate.count(pointer.PositionPointer);
                                }
                                else
                                {
                                    count += 1;
                                }

                            }
                            else
                            {
                                count += 1;
                            }

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
                            if (!distinct)
                            {
                                var pointer = NodePointer.FromBytes(item.Value);

                                if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                                {
                                    firstNodeCount += this.Tree.duplicate.count(pointer.PositionPointer);
                                }
                                else
                                {
                                    firstNodeCount += 1;
                                }
                            }
                            else
                            {
                                firstNodeCount += 1;
                            }
                        }
                    }

                    foreach (var item in endNode.TreeNode.KeyArray)
                    {

                        if (this.Comparer.Compare(endKeyBytes, item.Key) > 0 || (this.Comparer.Compare(endKeyBytes, item.Key) == 0 && !range.upperOpen))
                        {
                            if (!distinct)
                            {
                                var pointer = NodePointer.FromBytes(item.Value);

                                if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                                {
                                    lastNodeCount += this.Tree.duplicate.count(pointer.PositionPointer);
                                }
                                else
                                {
                                    lastNodeCount += 1;
                                }
                            }
                            else
                            {
                                lastNodeCount += 1;
                            }
                        }

                    }

                    var middleNode = MemoryTreeNodeManager.FindNextLeaf(this.Tree, startNode);

                    while (middleNode.TreeNode.DiskPosition != endNode.TreeNode.DiskPosition)
                    {
                        foreach (var item in middleNode.TreeNode.KeyArray)
                        {
                            if (!distinct)
                            {
                                var pointer = NodePointer.FromBytes(item.Value);

                                if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                                {
                                    middleCount += this.Tree.duplicate.count(pointer.PositionPointer);
                                }
                                else
                                {
                                    middleCount += 1;
                                }
                            }
                            else
                            {
                                middleCount += 1;
                            }
                        }

                        middleNode = MemoryTreeNodeManager.FindNextLeaf(this.Tree, middleNode);
                    }

                    return firstNodeCount + middleCount + lastNodeCount;
                }
            }
        }

        public int Count(bool distinct)
        {
            if (this.FirstKey == null && this.LastKey == null)
            {
                return 0;
            }

            Range<T> range = Range<T>.bound(this.FirstKey, this.LastKey, false, false);
            return Count(range, distinct);
        }

        public List<Int64> getRange(Range<T> range, int skipCount, int takeCount, bool ascending)
        {

            int skipped = 0;
            int taken = 0;
            List<long> list = new List<long>();
            if (takeCount <= 0)
            {
                return list;
            }

            foreach (Int64 item in getCollection(range, ascending))
            {
                if (skipped < skipCount)
                {
                    skipped += 1;
                    continue;
                }

                list.Add(item);

                taken += 1;

                if (taken >= takeCount)
                {
                    return list;
                }
            }

            return list;
        }


        public ItemCollection getCollection(Range<T> range, bool ascending)
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
        public ItemCollection getCollection(byte[] startKey, byte[] endKey, bool lowerOpen, bool upperOpen, bool ascending)
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

            ItemCollection collection = new ItemCollection(this.Tree, this.Comparer, startKey, endKey, lowerOpen, upperOpen, ascending);

            return collection;
        }


        public ItemCollection allItemCollection(bool ascending)
        {
            Range<T> range = Range<T>.bound(this.FirstKey, this.LastKey, false, false);
            return getCollection(range, ascending);
        }

        public List<Int64> All(bool ascending = false)
        {
            List<long> list = new List<long>();
            foreach (var item in allItemCollection(ascending))
            {
                list.Add(item);
            }

            return list;
        }

        public KeyBytesCollection AllKeyBytesCollection(bool ascending)
        {
            Range<T> range = Range<T>.bound(this.FirstKey, this.LastKey, false, false);
            return GetKeyBytesCollection(range, ascending);
        }

        private KeyBytesCollection GetKeyBytesCollection(byte[] startKey, byte[] endKey, bool lowerOpen, bool upperOpen, bool ascending)
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

            KeyBytesCollection collection = new KeyBytesCollection(this.Tree, this.Comparer, startKey, endKey, lowerOpen, upperOpen, ascending);

            return collection;
        }

        private KeyBytesCollection GetKeyBytesCollection(Range<T> range, bool ascending)
        {
            byte[] startKeyBytes = this.Converter.ToByte(range.lower);
            byte[] endKeyBytes = this.Converter.ToByte(range.upper);

            return GetKeyBytesCollection(startKeyBytes, endKeyBytes, range.lowerOpen, range.upperOpen, ascending);
        }
    }


}