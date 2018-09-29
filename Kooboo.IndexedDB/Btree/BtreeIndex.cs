//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Btree.Comparer;

namespace Kooboo.IndexedDB.Btree
{
    /// <summary>
    /// string index. 
    /// </summary>
    public class BtreeIndex<T>
    {

        public string fieldname;
        public bool unique;
        public int keylength;

        private string fullindexfilename;

        private string fullduplicatefilename;

        internal int MaxCacheLevel { get; set; }

        /// <summary>
        /// the object for lock.
        /// </summary>
        private object _object = new object();

        private TreeFile _tree;

        internal TreeFile Tree
        {
            get
            {

                if (_tree == null)
                {
                    lock (_object)
                    {
                        if (_tree == null)
                        {
                            _tree = new TreeFile(this.fullindexfilename, this.unique, typeof(T), keylength, this.Comparer, this.EqualityComparer, MaxCacheLevel);
                        }
                    }
                }

                return _tree;
            }
        }

        private IComparer<byte[]> Comparer;

        private IByteConverter<T> Converter;

        private IEqualityComparer<byte[]> EqualityComparer;  /// only used for dictionary to find key. 

        ///only string key has the keylength, the rest does not have. 
        private bool isString = false;

        public BtreeIndex(string fieldname, bool unique, int keylength, string fullIndexFileName, int MaxCacheLevel = 0)
        {
            this.fieldname = fieldname;
            this.unique = unique;
            Type keytype = typeof(T);
            this.keylength = Helper.KeyHelper.GetKeyLen(keytype, keylength);

            this.fullindexfilename = fullIndexFileName;

            this.MaxCacheLevel = MaxCacheLevel;

            this.fullduplicatefilename = fullindexfilename + ".duplicate";

            Helper.IndexHelper.VerifyIndexType(keytype);

            this.Converter = ObjectContainer.GetConverter<T>();
            this.EqualityComparer = new EqualityComparer(this.keylength);
            this.Comparer = ObjectContainer.getComparer(keytype, keylength);

            if (keytype == typeof(string))
            {
                isString = true;
            }
            else
            {
                isString = false;
            }

        }

        public bool Exists
        {
            get
            {
                return File.Exists(this.fullindexfilename);
            }
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
                if (System.IO.File.Exists(fullindexfilename))
                {
                    File.Delete(fullindexfilename);
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


        private bool Add(byte[] keybytes, Int64 blockposition)
        {
            lock (_object)
            {
                return this.Tree.Add(this.appendToFixedLength(keybytes), blockposition);
            }
        }

        public bool Add(T key, Int64 blockposition)
        {

            if (key == null)
            {
                key = default(T);
            }


            return Add(this.Converter.ToByte(key), blockposition);
        }

        /// <summary>
        /// update key and blockposition.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Update(T oldkey, T newkey, Int64 oldBlockPosition, Int64 newBlockPosition)
        {
            if (oldkey == null)
            {
                oldkey = default(T);
            }
            if (newkey == null)
            {
                newkey = default(T);
            }

            byte[] oldkeybyte = this.appendToFixedLength(this.Converter.ToByte(oldkey));
            byte[] newkeybyte = this.appendToFixedLength(this.Converter.ToByte(newkey));

            if (ByteEqualComparer.isEqual(oldkeybyte, newkeybyte, this.keylength))
            {
                Update(oldkey, oldBlockPosition, newBlockPosition);
            }
            else
            {
                this.Add(newkeybyte, newBlockPosition);
                this.Del(oldkeybyte, oldBlockPosition);
            }
        }

        public void Update(T key, Int64 oldBlockPosition, Int64 newBlockPosition)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keybytes = this.Converter.ToByte(key);

            lock (_object)
            {
                this.Tree.Update(this.appendToFixedLength(keybytes), oldBlockPosition, newBlockPosition);
            }
        }

        public Int64 Get(T key)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keybytes = this.Converter.ToByte(key);

            return Get(keybytes);
        }

        public Int64 Get(byte[] keybytes)
        {
            lock (_object)
            {
                return this.Tree.Get(this.appendToFixedLength(keybytes));
            }
        }



        public List<Int64> List(T key)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keybytes = this.Converter.ToByte(key);

            return List(keybytes);
        }

        public List<Int64> List(byte[] keybytes)
        {
            lock (_object)
            {
                return this.Tree.ListAll(this.appendToFixedLength(keybytes));
            }
        }

        public bool Del(T key, Int64 blockposition)
        {
            if (blockposition <= 0)
            {
                throw new Exception("block position not privoided");
            }

            if (key == null)
            {
                key = default(T);
            }

            byte[] keybytes = this.Converter.ToByte(key);
            return Del(keybytes, blockposition);
        }

        private bool Del(byte[] keybytes, Int64 blockposition)
        {
            lock (_object)
            {
                return this.Tree.Del(this.appendToFixedLength(keybytes), blockposition);
            }
        }

        public List<Int64> Del(T key)
        {
            if (key == null)
            {
                key = default(T);
            }

            byte[] keybytes = this.Converter.ToByte(key);
            return Del(keybytes);
        }

        private List<Int64> Del(byte[] keybytes)
        {
            lock (_object)
            {
                return this.Tree.Del(this.appendToFixedLength(keybytes));
            }
        }

        /// <summary>
        /// append byte 0 to the iput to make it match the keylength. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] appendToFixedLength(byte[] input)
        {
            return Helper.KeyHelper.AppendToKeyLength(input, isString, this.keylength);
        }

        public T FirstKey
        {
            get
            {
                lock (_object)
                {
                    byte[] keybytes = this.Tree.FirstKey();

                    if (keybytes == null)
                    {
                        return default(T);
                    }
                    else
                    {
                        return this.Converter.FromByte(keybytes);
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
                    byte[] keybytes = this.Tree.LastKey();

                    if (keybytes == null)
                    {
                        return default(T);
                    }
                    else
                    {
                        return this.Converter.FromByte(keybytes);
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
            lock (_object)
            {
                byte[] startKeyBytes = this.Converter.ToByte(range.lower);
                byte[] endKeyBytes = this.Converter.ToByte(range.upper);

                startKeyBytes = this.appendToFixedLength(startKeyBytes);
                endKeyBytes = this.appendToFixedLength(endKeyBytes);

                MemoryTreeNode startnode = MemoryTreeNodeManager.FindLeafByKey(this.Tree, this.Tree.RootCache, startKeyBytes);
                MemoryTreeNode endnode = MemoryTreeNodeManager.FindLeafByKey(this.Tree, this.Tree.RootCache, endKeyBytes);

                if (startnode.TreeNode.DiskPosition == endnode.TreeNode.DiskPosition)
                {

                    int count = 0;
                    foreach (var item in startnode.TreeNode.KeyArray)
                    {
                        if ((this.Comparer.Compare(startKeyBytes, item.Key) < 0 || (this.Comparer.Compare(startKeyBytes, item.Key) == 0 && !range.lowerOpen)) && ((this.Comparer.Compare(endKeyBytes, item.Key) > 0 || (this.Comparer.Compare(endKeyBytes, item.Key) == 0 && !range.upperOpen))))

                            if (!distinct)
                            {
                                NodePointer pointer = new NodePointer();
                                pointer.PointerBytes = item.Value;

                                if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                                {
                                    count = count + this.Tree.duplicate.count(pointer.PositionPointer);
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
                    int firstnodecount = 0;
                    int lastnodecount = 0;
                    int middlecount = 0;

                    foreach (var item in startnode.TreeNode.KeyArray)
                    {
                        if (this.Comparer.Compare(startKeyBytes, item.Key) < 0 || (this.Comparer.Compare(startKeyBytes, item.Key) == 0 && !range.lowerOpen))
                        {
                            if (!distinct)
                            {
                                NodePointer pointer = new NodePointer();
                                pointer.PointerBytes = item.Value;

                                if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                                {
                                    firstnodecount = firstnodecount + this.Tree.duplicate.count(pointer.PositionPointer);
                                }
                                else
                                {
                                    firstnodecount += 1;
                                }
                            }
                            else
                            {
                                firstnodecount += 1;
                            }
                        }
                    }

                    foreach (var item in endnode.TreeNode.KeyArray)
                    {

                        if (this.Comparer.Compare(endKeyBytes, item.Key) > 0 || (this.Comparer.Compare(endKeyBytes, item.Key) == 0 && !range.upperOpen))
                        {
                            if (!distinct)
                            {
                                NodePointer pointer = new NodePointer();
                                pointer.PointerBytes = item.Value;

                                if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                                {
                                    lastnodecount = lastnodecount + this.Tree.duplicate.count(pointer.PositionPointer);
                                }
                                else
                                {
                                    lastnodecount += 1;
                                }
                            }
                            else
                            {
                                lastnodecount += 1;
                            }
                        }

                    }

                    var middlenode = MemoryTreeNodeManager.FindNextLeaf(this.Tree, startnode);

                    while (middlenode.TreeNode.DiskPosition != endnode.TreeNode.DiskPosition)
                    {
                        foreach (var item in middlenode.TreeNode.KeyArray)
                        {
                            if (!distinct)
                            {
                                NodePointer pointer = new NodePointer();
                                pointer.PointerBytes = item.Value;

                                if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                                {
                                    middlecount = middlecount + this.Tree.duplicate.count(pointer.PositionPointer);
                                }
                                else
                                {
                                    middlecount += 1;
                                }
                            }
                            else
                            {
                                middlecount += 1;
                            }
                        }

                        middlenode = MemoryTreeNodeManager.FindNextLeaf(this.Tree, middlenode);
                    }

                    return firstnodecount + middlecount + lastnodecount;
                }
            }
        }

        public int Count(bool distinct)
        {
            Range<T> range = Range<T>.bound(this.FirstKey, this.LastKey, false, false);
            return Count(range, distinct);
        }

        public List<Int64> getRange(Range<T> range, int skipcount, int takecount, bool ascending)
        {

            int skipped = 0;
            int taken = 0;
            List<long> list = new List<long>();
            if (takecount <= 0)
            {
                return list;
            }

            foreach (Int64 item in getCollection(range, ascending))
            {
                if (skipped < skipcount)
                {
                    skipped += 1;
                    continue;
                }

                list.Add(item);

                taken += 1;

                if (taken >= takecount)
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

        /// <summary>
        /// get the collectiont that contains all items in the index. 
        /// </summary>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public ItemCollection allItemCollection(bool ascending)
        {
            Range<T> range = Range<T>.bound(this.FirstKey, this.LastKey, false, false);
            return getCollection(range, ascending);
        }

        public List<Int64> All(bool acsending = false)
        {
            List<long> list = new List<long>();
            foreach (var item in allItemCollection(acsending))
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