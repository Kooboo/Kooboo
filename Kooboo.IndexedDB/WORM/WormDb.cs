using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Helper;
using Kooboo.IndexedDB.WORM.Query;
using Kooboo.IndexedDB.WORM.Restore;

namespace Kooboo.IndexedDB.WORM
{
    /* 
     * This database only allow to insert key that is bigger than current. 
     * Because it will write the key in a sequence order into disk..
     * This is special designed for event, packages, etc... for the most efficiency.  
     *///0-100 byte = setting... if not setting, keep blank for future usage. 
    //100-200 the Index search start....
    //200-300 the init block...

    public class WormDb<T> : IDisposable
    {
        // internal object _ReadLocker { get; set; } = new object();

        internal object locker { get; set; } = new object();

        public IByteConverter<T> ValueConverter { get; set; }

        public string keyFieldName { get; set; }

        public Func<T, long> GetKey { get; set; }

        public Action<T, long> SetKey { get; set; }

        public string FullFileName { get; set; }

        public long CurrentId { get; set; }

        internal int MetaByteLen { get; set; }
        internal int PointerLen { get; set; }
        internal int NodeLen { get; set; }

        internal bool SkipValueBlock { get; set; }

        internal long PositionRootNodeDisk { get; set; }

        internal long PositionRootNode { get; set; } = 4;   // log of RootNodeDiskPos.

        internal long PositionCurrentId { get; set; } = 12;

        internal long PositionKeyLen { get; set; } = 20;  // 20-24. 

        internal long PositionMetaByteLen { get; set; } = 24;  // 24-18; 

        internal long PositionKeyFieldName { get; set; } = 50;

        public Node RootNode { get; set; }

        public bool readOnly { get; set; } = false;

        public bool OverWriteValueBLock { get; set; } = false;

        public int MinExtraSpace { get; set; } = 0;


        public WormDb(string fullFileName)
        {
            startup(fullFileName);
        }

        public WormDb(string fullFileName, bool ReadOnly)
        {
            this.readOnly = ReadOnly;
            startup(fullFileName);
        }

        public WormDb(string FullFileName, string KeyFieldName)
        {
            startup(FullFileName, KeyFieldName);
        }

        public WormDb(string FullFileName, string KeyFieldName, bool ReadOnly)
        {
            this.readOnly = ReadOnly;
            startup(FullFileName, KeyFieldName);
        }

        public WormDb(string FullFileName, string KeyFieldName, bool ReadOnly, bool OverWriteValue)
        {
            this.readOnly = ReadOnly;
            this.OverWriteValueBLock = OverWriteValue;
            startup(FullFileName, KeyFieldName);
        }

        public WormDb(string FullFileName, string KeyFieldName, IByteConverter<T> converter)
        {
            startup(FullFileName, KeyFieldName, converter);
        }

        private void startup(string fullFileName, string KeyField = null, IByteConverter<T> suppliedConverter = null)
        {
            this.FullFileName = fullFileName;
            this.keyFieldName = KeyField;


            if (this.keyFieldName != null && this.keyFieldName.Length > 30)
            {
                throw new Exception("Max length of key field name is 30");
            }

            if (!this.FullFileName.EndsWith(".data"))
            {
                this.FullFileName += ".data";
            }

            this.ValueConverter = suppliedConverter != null ? suppliedConverter : ObjectContainer.GetConverter<T>();

            int metaByteLenPassIn = 0;

            if (TypeHelper.HasInterface(typeof(T), typeof(MetaObject.IMetaObject)))
            {
                //  metaByteLenPassIn = MetaObject.MetaObjectHelper.GetMetaByteLen<T>();
                //  this.NoValueBlock = MetaObject.MetaObjectHelper.GetNoValueBlock<T>();
                metaByteLenPassIn = TypeHelper.GetPropertyValue<int, T>(nameof(MetaObject.IMetaObject.MetaByteLen));
                this.SkipValueBlock = TypeHelper.GetPropertyValue<bool, T>(nameof(MetaObject.IMetaObject.SkipValueBlock));
            }
            this.MetaByteLen = metaByteLenPassIn;
            this.PointerLen = PositionPointer.GetPointerLen(this.MetaByteLen);
            this.NodeLen = Node.GetNodelen(this.PointerLen);

            if (!File.Exists(this.FullFileName))
            {
                byte[] fieldNameBytes = null;
                if (!string.IsNullOrWhiteSpace(this.keyFieldName))
                {
                    fieldNameBytes = System.Text.Encoding.ASCII.GetBytes(this.keyFieldName);
                    if (fieldNameBytes.Length > 30)
                    {
                        throw new Exception("Field name too long");
                    }
                }

                // file not exists.first check directory exists or not.
                string dirName = Path.GetDirectoryName(this.FullFileName);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }

                // init bytes... 
                // first 100 bytes for nothing now. reserved for future usage.
                var firstHeader = new byte[100];
                firstHeader[0] = 1;
                firstHeader[1] = 2;
                firstHeader[2] = 3;
                firstHeader[3] = 4;

                //System.IO.File.WriteAllBytes(this.FullFileName, firstheader);

                this.Write(firstHeader);

                long rootPos = InitNodeBlock(false, 0, 1);  // init root. 

                var rootPosBytes = BitConverter.GetBytes(rootPos);

                this.SetLong(rootPos, this.PositionRootNode);
                //  this.SetLong(0, this.PositionCurrentId);

                if (fieldNameBytes != null)
                {
                    this.Write(fieldNameBytes, this.PositionKeyFieldName);
                    this.SetInt(fieldNameBytes.Length, this.PositionKeyLen);
                }

                this.SetInt(metaByteLenPassIn, this.PositionMetaByteLen);

                this.Stream.Close();
            }


            this.PositionRootNodeDisk = ReadLong(this.PositionRootNode);
            this.RootNode = this.LoadNode(PositionRootNodeDisk);

            var metaLenInDisk = this.ReadInt(this.PositionMetaByteLen);
            if (metaLenInDisk != metaByteLenPassIn)
            {
                throw new Exception("Change the meta byte length will cause unstable of database and is not allowed, you should rebuild a new database table");
            }


            if (string.IsNullOrEmpty(this.keyFieldName))
            {
                // if not pass in...look into disk... 
                var keyLen = this.ReadInt(this.PositionKeyLen);
                if (keyLen > 0)
                {
                    this.keyFieldName = this.ReadText(PositionKeyFieldName, keyLen);
                }
            }


            if (!string.IsNullOrWhiteSpace(this.keyFieldName))
            {
                var type = typeof(T);

                if (Helper.TypeHelper.IsDictionary(type))
                {
                    this.GetKey = this.DictGetKey;
                    this.SetKey = this.DictSetKey;
                }
                else
                {
                    var keyType = Helper.TypeHelper.GetFieldType(type, KeyField);

                    if (keyType == null || keyType != typeof(long))
                    {
                        throw new Exception("key type must be long/int64");
                    }
                    this.SetKey = Helper.ObjectHelper.GetSetValue<T, long>(this.keyFieldName);

                    this.GetKey = Helper.ObjectHelper.GetGetValue<T, long>(this.keyFieldName);
                }
            }

            // this.CurrentId = this.FindKey(false);   // ReadLong(this.PositionCurrentId);
            this.CurrentId = this.LastKey();
        }


        private long DictGetKey(T input)
        {
            var dict = input as IDictionary;

            if (dict != null && dict.Contains(this.keyFieldName))
            {
                var obj = dict[this.keyFieldName];
                if (obj != null)
                {
                    return Dynamic.Accessor.ChangeType<long>(obj);
                }
            }
            return -1;
        }

        private void DictSetKey(T input, long KeyValue)
        {
            var dict = input as IDictionary;

            if (dict != null)
            {
                dict[this.keyFieldName] = KeyValue;
            }

        }

        private long LastKey()
        {
            var query = this.Query;

            var leaf = query.FindLeaf(this.RootNode, long.MaxValue);

            if (leaf != null && leaf.Node != null && leaf.Node.Pointer != null)
            {
                var len = leaf.Node.Pointer.Length;
                for (int i = len - 1; i >= 0; i--)
                {
                    var item = leaf.Node.Pointer[i];
                    if (item != null)
                    {
                        if (item.Id > 0)
                        {
                            return item.Id;
                        }
                    }
                }
            }

            // search from small to up, should not be, just in case.
            long startId = 0;
            leaf = query.FindLeaf(this.RootNode, startId);
            int loopCounter = 0;

            while (leaf != null && leaf.Node != null && leaf.Node.Pointer != null)
            {
                foreach (var item in leaf.Node.Pointer)
                {
                    if (item != null)
                    {
                        if (item.Id > 0 && item.Id > startId)
                        {
                            startId = item.Id;
                        }
                    }
                }

                var currentLeafPosition = leaf.Node.DiskPosition;
                var currentLeafStartId = leaf.Node.StartId;

                leaf = query.FindLeaf(this.RootNode, startId + 1);

                if (leaf != null && leaf.Node != null && leaf.Node.DiskPosition == currentLeafPosition && leaf.Node.StartId == currentLeafStartId)
                {
                    break;
                }

                loopCounter += 1;
                if (loopCounter > 999999)
                {
                    break;
                }
            }

            return startId;
        }

        public long FindKey(bool SmallKey)
        {
            var query = this.Query;
            if (SmallKey)
            {
                long startId = 0;
                var leaf = query.FindLeaf(this.RootNode, startId);
                int loopCounter = 0;

                while (leaf != null && leaf.Node != null && leaf.Node.Pointer != null)
                {
                    foreach (var item in leaf.Node.Pointer)
                    {
                        if (item != null)
                        {
                            if (!item.IsDeleted)
                            {
                                return item.Id;
                            }
                            if (item.Id > 0)
                            {
                                startId = item.Id;
                            }
                        }
                    }
                    startId += 1;
                    leaf = query.FindLeaf(this.RootNode, startId);
                    loopCounter += 1;
                    if (loopCounter > 9999)
                    {
                        break;
                    }
                }

            }
            else
            {
                long startId = long.MaxValue;

                var leaf = query.FindLeaf(this.RootNode, startId);
                int loopCounter = 0;

                while (leaf != null && leaf.Node != null && leaf.Node.Pointer != null)
                {
                    var len = leaf.Node.Pointer.Length;
                    for (int i = len - 1; i >= 0; i--)
                    {
                        var item = leaf.Node.Pointer[i];

                        if (item != null)
                        {
                            if (!item.IsDeleted)
                            {
                                return item.Id;
                            }
                            if (item.Id > 0)
                            {
                                startId = item.Id;
                            }
                        }
                    }
                    startId = startId - 1;

                    leaf = query.FindLeaf(this.RootNode, startId);

                    loopCounter += 1;
                    if (loopCounter > 9999)
                    {
                        break;
                    }

                }
            }

            return 0;
        }

        public NodePath LoadPath(long id)
        {
            var node = this.RootNode;
            var index = FindAvailablePointer(node);

            var pointer = node.Pointer[index];

            if (index == 0 && pointer == null)
            {
                // this is the first leaf creation...
                // TODO: implement a double check.
                var firstLeaf = InitNodeBlock(true, 0);
                pointer = new PositionPointer(this.MetaByteLen);
                pointer.Id = id;
                pointer.Position = firstLeaf;
                this.UpdatePointer(node, pointer, 0);
                return LoadPath(id);
            }

            NodePath path = new NodePath();
            path.AppendChain(node, index);

            while (!node.Isleaf)
            {
                node = this.LoadNode(pointer.Position);

                if (node.Isleaf)
                {
                    break;
                }

                var pointerIndex = FindAvailablePointer(node);
                pointer = node.Pointer[pointerIndex];
                if (pointer == null)
                {
                    throw new Exception("database error");
                }
                path.AppendChain(node, pointerIndex);
            }

            path.AppendChain(node, 0);

            path.Leaf = node;
            return path;
        }

        //this should be executed only when previous path is full. 
        public NodePath RenewPath(NodePath currentFullPath, long newId)
        {
            NodePath newPath = new NodePath();

            long PositionSub = 0;

            int count = currentFullPath.Chain.Count;
            bool IsDone = false;

            for (int i = count - 1; i > -1; i--)
            {
                var item = currentFullPath.Chain[i];
                if (IsDone)
                {
                    newPath.Chain.Add(item);
                    continue;
                }

                if (item.Node.Isleaf)
                {
                    //the last leaf. 
                    Node newLeaf = null;
                    if (item.Node.NextNode > 0)
                    {
                        newLeaf = this.LoadNode(item.Node.NextNode);
                    }
                    else
                    {
                        var newLeafPos = InitNodeBlock(true, item.Node.DiskPosition);
                        this.UpdateNodeRelation(item.Node, newLeafPos, EnumNodeRelation.NextNode);
                        newLeaf = this.LoadNode(newLeafPos);
                    }
                    PositionSub = newLeaf.DiskPosition;
                    newPath.AppendChain(newLeaf, 0);
                    newPath.Leaf = newLeaf;
                }
                else
                {
                    var availablePointerIndex = FindAvailablePointer(item.Node, true);

                    if (availablePointerIndex >= 0)
                    {
                        var availablePointer = item.Node.Pointer[availablePointerIndex];
                        availablePointer = new PositionPointer(this.MetaByteLen);
                        availablePointer.Id = newId;
                        availablePointer.Position = PositionSub;
                        this.UpdatePointer(item.Node, availablePointer, availablePointerIndex);
                        newPath.AppendChain(item.Node, availablePointerIndex);

                        // done.
                        IsDone = true;
                    }
                    else
                    {
                        // node is full.
                        if (i == 0)
                        {
                            // from root. 0 is the root node. 
                            var oldRootNewNode = this.MoveRootNode();

                            var newNodePos = InitNodeBlock(oldRootNewNode.Isleaf, oldRootNewNode.DiskPosition);

                            var newNode = this.LoadNode(newNodePos);
                            var pointer = new PositionPointer(this.MetaByteLen);
                            pointer.Id = newId;
                            pointer.Position = PositionSub;
                            this.UpdatePointer(newNode, pointer, 0);
                            newPath.AppendChain(newNode, 0);

                            //append new root. 
                            var rootPointer = new PositionPointer(this.MetaByteLen);
                            rootPointer.Id = newId;
                            rootPointer.Position = newNode.DiskPosition;
                            this.UpdatePointer(this.RootNode, rootPointer, 1);
                            newPath.AppendChain(this.RootNode, 1);

                        }
                        else
                        {
                            var newNodePos = InitNodeBlock(item.Node.Isleaf, item.Node.DiskPosition);

                            this.UpdateNodeRelation(item.Node, newNodePos, EnumNodeRelation.NextNode);

                            var newNode = this.LoadNode(newNodePos);
                            var pointer = new PositionPointer(this.MetaByteLen);
                            pointer.Id = newId;
                            pointer.Position = PositionSub;

                            this.UpdatePointer(newNode, pointer, 0);

                            newPath.AppendChain(newNode, 0);

                            PositionSub = newNode.DiskPosition;
                        }
                    }

                }
            }

            newPath.Leaf = newPath.Chain[0].Node;

            newPath.Chain.Reverse();

            return newPath;
        }

        private Node MoveRootNode()
        {
            // root only always has one Node connected.
            var newBlock = InitNodeBlock(false, 0, 0);
            var node = this.LoadNode(newBlock);

            // copy all point from root... 
            for (int i = 0; i < WormSetting.NodePointerCount; i++)
            {
                var rootItem = this.RootNode.Pointer[i];
                node.Pointer[i] = rootItem;
            }

            this.Write(node.ToBytes(), node.DiskPosition);

            Node newRootNode = new Node(false, this.NodeLen, this.PointerLen, this.MetaByteLen);
            var newPointer = new PositionPointer(this.MetaByteLen);
            newPointer.Id = this.RootNode.Pointer[0].Id;
            newPointer.Position = newBlock;

            newRootNode.Pointer[0] = newPointer;

            this.Write(newRootNode.ToBytes(), this.PositionRootNodeDisk);
            newRootNode.DiskPosition = this.PositionRootNodeDisk;
            this.RootNode = newRootNode;
            return node;
        }

        private int FindAvailablePointer(Node current)
        {
            bool RequireEmpty = current.Isleaf;

            for (int i = 0; i < WormSetting.NodePointerCount; i++)
            {
                var item = current.Pointer[i];
                if (item == null)
                {
                    if (!RequireEmpty)
                    {
                        i = i - 1;
                    }
                    if (i < 0)
                    {
                        i = 0;
                    }
                    return i;
                }
            }

            if (!RequireEmpty)
            {
                return WormSetting.NodePointerCount - 1;  // the last one. 
            }

            return -1;
        }

        private int FindAvailablePointer(Node current, bool RequireEmpty)
        {
            for (int i = 0; i < WormSetting.NodePointerCount; i++)
            {
                var item = current.Pointer[i];
                if (item == null)
                {
                    if (!RequireEmpty)
                    {
                        i = i - 1;
                    }
                    if (i < 0)
                    {
                        i = 0;
                    }
                    return i;
                }
            }
            if (!RequireEmpty)
            {
                return WormSetting.NodePointerCount - 1;  // the last one. 
            }
            return -1;
        }

        private NodePath editPath { get; set; }

        private WritingLeaf writingNode { get; set; }
        public WritingLeaf GetWritingNode(long Id)
        {
            if (writingNode == null)
            {
                this.editPath = this.LoadPath(Id);
                writingNode = WritingLeaf.FromPath(this.editPath);
            }

            if (writingNode.IsFull)
            {
                while (writingNode.IsFull)
                {
                    this.editPath = this.RenewPath(this.editPath, Id);
                    writingNode = WritingLeaf.FromPath(this.editPath);
                }
            }

            return writingNode;

        }

        #region Byte operation 
        private void SetLong(long currentValue, long position)
        {
            var bytes = BitConverter.GetBytes(currentValue);
            this.Write(bytes, position);
        }
        private long ReadLong(long position)
        {
            var bytes = GetPartial(position, 8);
            return BitConverter.ToInt64(bytes, 0);
        }

        private string ReadText(long position, int len)
        {
            var bytes = GetPartial(position, len);
            var text = System.Text.Encoding.UTF8.GetString(bytes);
            return text.Trim();
        }

        private void SetInt(int inputValue, long position)
        {
            var bytes = BitConverter.GetBytes(inputValue);
            this.Write(bytes, position);
        }

        private int ReadInt(long position)
        {
            var bytes = GetPartial(position, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public Node LoadNode(long position)
        {
            if (position == this.PositionRootNodeDisk)
            {
                if (this.RootNode != null)
                {
                    return this.RootNode;
                }
            }
            var bytes = GetPartial(position, this.NodeLen);
            var node = Node.FromBytes(bytes, this.NodeLen, this.PointerLen, this.MetaByteLen);
            node.DiskPosition = position;
            return node;
        }

        internal void UpdatePointer(Node container, PositionPointer pointer, int Index)
        {
            long position = container.DiskPosition + container.GetPointerRelativePosition(Index);
            this.Write(pointer.ToBytes(), position);

            container.Pointer[Index] = pointer;
        }

        private void UpdateNodeRelation(Node node, long destinationPosition, EnumNodeRelation relationType)
        {
            long position = node.DiskPosition + node.GetRelationRelativePosition(relationType);
            this.Write(BitConverter.GetBytes(destinationPosition), position);

            if (relationType == EnumNodeRelation.NextNode)
            {
                node.NextNode = destinationPosition;
            }
            else if (relationType == EnumNodeRelation.PreviousNode)
            {
                node.PreviousNode = destinationPosition;
            }

            else if (relationType == EnumNodeRelation.StartId)
            {
                node.StartId = destinationPosition;
            }
        }

        internal byte[] GetPartial(long position, int count)
        {
            byte[] partial = new byte[count];
            if (Stream.Length >= position + count)
            {
                lock (locker)
                {
                    Stream.Position = position;
                    Stream.Read(partial, 0, count);
                    return partial;
                }
            }
            return null;
        }

        public long Write(byte[] bytes, long position = 0)
        {
            lock (locker)
            {
                int len = bytes.Length;
                long currentPosition = position == 0 ? Stream.Length : position;

                Stream.Position = currentPosition;
                Stream.Write(bytes, 0, len);
                Stream.Flush();
                return currentPosition;
            }
        }

        #endregion

        public long InitNodeBlock(bool IsLeaf, long previousPos, int count = 0)
        {
            var counter = WormSetting.ConnectedNodeCount;
            if (count > 0)
            {
                counter = count;
            }

            var bytes = new byte[this.NodeLen * counter];

            lock (locker)
            {
                var diskPos = this.Stream.Length;
                long currentNodePos = diskPos;
                int relativeIndex = 0;

                for (int i = 0; i < counter; i++)
                {
                    Node node = new Node(IsLeaf, this.NodeLen, this.PointerLen, this.MetaByteLen);

                    node.PreviousNode = previousPos;

                    previousPos = diskPos + relativeIndex; // set previous for next node. 
                    long nextDiskPos = diskPos + relativeIndex + this.NodeLen;

                    if (i != counter - 1)
                    {
                        node.NextNode = nextDiskPos;
                    }

                    var nodeBytes = node.ToBytes();

                    System.Buffer.BlockCopy(nodeBytes, 0, bytes, relativeIndex, nodeBytes.Length);

                    relativeIndex += this.NodeLen;

                }

                return this.Write(bytes, diskPos);

            }

        }

        private FileStream _fileStream;

        public FileStream Stream
        {
            get
            {
                if (_fileStream == null || (!readOnly && !_fileStream.CanWrite))
                {
                    lock (locker)
                    {
                        if (this.readOnly)
                        {
                            if (_fileStream == null)
                            {
                                if (System.IO.File.Exists(FullFileName))
                                {
                                    _fileStream = File.Open(FullFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                                }
                                else
                                {
                                    _fileStream = File.Open(FullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                                }
                            }
                        }
                        else
                        {
                            if (_fileStream == null || !_fileStream.CanWrite)
                            {
                                _fileStream = File.Open(FullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                            }
                        }
                    }
                }
                return _fileStream;
            }
        }

        private object ReadWriteLock = new object();

        /// <summary>
        /// Add a value and return the inserted ID. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long Add(T value, long key = 0)
        {
            lock (ReadWriteLock)
            {

                long setKey(long insertKey)
                {
                    if (insertKey <= 0)
                    {
                        if (this.GetKey != null)
                        {
                            insertKey = this.GetKey(value);
                        }
                    }

                    if (insertKey <= 0)
                    {
                        insertKey = this.CurrentId + 1;
                        if (this.SetKey != null)
                        {
                            this.SetKey(value, insertKey);
                        }
                    }
                    else
                    {
                        if (insertKey == this.CurrentId)
                        {
                            insertKey = this.CurrentId + 1;
                            if (this.SetKey != null)
                            {
                                this.SetKey(value, insertKey);
                            }
                        }
                        else if (insertKey < this.CurrentId)
                        {
                            throw new Exception("This database only allowed for increased id, new id must bigger than all old ids, or set key value  to <=0 for auto generation");
                        }
                        else
                        {
                            if (this.SetKey != null)
                            {
                                this.SetKey(value, insertKey);
                            }
                        }
                    }

                    this.CurrentId = insertKey;
                    return insertKey;
                }

                lock (locker)
                {
                    key = setKey(key);
                }


                long position = 0;

                if (!this.SkipValueBlock)
                {
                    position = _AddValueBlock(value);
                }

                if (position > 0 || this.SkipValueBlock)
                {
                    // find the empty pointer and write to it.   
                    var node = this.GetWritingNode(key);

                    var pointer = node.UpdatePointerPosition(position, key);

                    if (this.MetaByteLen > 0)
                    {
                        var metaObj = value as MetaObject.IMetaObject;
                        if (metaObj != null)
                        {
                            pointer.MetaBytes = metaObj.GetMetaBytes();
                        }
                    }

                    this.UpdatePointer(node.Leaf, pointer, node.EmptySlotIndex);

                    node.EmptySlotIndex += 1;
                }

                return key;
            }
        }

        public T Get(long key)
        {
            lock (ReadWriteLock)
            {
                var leaf = this.Query.FindLeaf(this.RootNode, key);

                if (leaf != null)
                {
                    var pointer = leaf.Node.Pointer[leaf.PointerIndex];
                    if (pointer != null && pointer.Id == key)
                    {
                        return this.LoadValue(pointer);
                    }
                    else
                    {

                    }
                }
                return default(T);
            }
        }

        public bool Exist(long key)
        {
            return GetPointer(key) != default;
        }

        internal PositionPointer GetPointer(long key)
        {
            var leaf = this.Query.FindLeaf(this.RootNode, key);

            if (leaf != null)
            {
                var pointer = leaf.Node.Pointer[leaf.PointerIndex];
                if (pointer != null && pointer.Id == key)
                {
                    return pointer;
                }
            }
            return null;

        }

        public T GetMeta(long key)
        {
            var leaf = this.Query.FindLeaf(this.RootNode, key);

            if (leaf != null)
            {
                var pointer = leaf.Node.Pointer[leaf.PointerIndex];
                if (pointer != null && pointer.Id == key && pointer.Position > 0)
                {
                    return LoadMeta(pointer);
                }
            }
            return default(T);

        }

        public long Update(T newValue, long key = 0)
        {
            lock (ReadWriteLock)
            {
                if (key == 0 && this.GetKey != null)
                {
                    key = this.GetKey(newValue);
                }

                var leaf = this.Query.FindLeaf(this.RootNode, key);

                if (leaf != null)
                {
                    var pointer = leaf.Node.Pointer[leaf.PointerIndex];
                    if (pointer != null && pointer.Id == key)
                    {
                        var (newBlockPos, HasChange) = this.UpdateOrAddValueBlock(pointer, newValue);

                        if (!HasChange && pointer.IsDeleted == false && this.MetaByteLen == 0)
                        {
                            return key;
                        }

                        pointer.Position = newBlockPos;
                        pointer.IsDeleted = false;

                        if (this.MetaByteLen > 0)
                        {
                            var metaObj = newValue as MetaObject.IMetaObject;
                            if (metaObj != null)
                            {
                                pointer.MetaBytes = metaObj.GetMetaBytes();
                            }
                        }
                        this.UpdatePointer(leaf.Node, pointer, leaf.PointerIndex);
                    }
                }
                return key;
            }
        }

        public long UpdateValueBlock(T newValue, long key = 0)
        {
            lock (ReadWriteLock)
            {

                if (key == 0 && this.GetKey != null)
                {
                    key = this.GetKey(newValue);
                }

                var leaf = this.Query.FindLeaf(this.RootNode, key);

                if (leaf != null)
                {
                    var pointer = leaf.Node.Pointer[leaf.PointerIndex];
                    if (pointer != null && pointer.Id == key)
                    {
                        var (newBlockPos, HasChange) = this.UpdateOrAddValueBlock(pointer, newValue);

                        if (HasChange)
                        {
                            pointer.Position = newBlockPos;
                            this.UpdatePointer(leaf.Node, pointer, leaf.PointerIndex);
                        }
                    }
                }
                return key;
            }
        }

        public long UpdateMeta(T newValue, long key = 0)
        {
            lock (ReadWriteLock)
            {
                if (key == 0 && this.GetKey != null)
                {
                    key = this.GetKey(newValue);
                }

                var leaf = this.Query.FindLeaf(this.RootNode, key);

                if (leaf != null)
                {
                    var pointer = leaf.Node.Pointer[leaf.PointerIndex];
                    if (pointer != null && pointer.Id == key)
                    {
                        var metaObj = newValue as MetaObject.IMetaObject;

                        if (metaObj != null && metaObj.SkipValueBlock == false && pointer.Position <= 0)
                        {
                            return key;
                        }
                        if (this.MetaByteLen > 0)
                        {
                            if (metaObj != null)
                            {
                                pointer.MetaBytes = metaObj.GetMetaBytes();
                            }
                            this.UpdatePointer(leaf.Node, pointer, leaf.PointerIndex);
                        }
                    }
                }
                return key;
            }
        }


        public bool Delete(long key)
        {
            lock (ReadWriteLock)
            {
                var leaf = this.Query.FindLeaf(this.RootNode, key);

                if (leaf != null)
                {
                    var pointer = leaf.Node.Pointer[leaf.PointerIndex];
                    if (pointer != null && pointer.Id == key && pointer.Position > 0)
                    {
                        //Mark Pointer...
                        pointer.IsDeleted = true;
                        this.UpdatePointer(leaf.Node, pointer, leaf.PointerIndex);

                        this.MarkValueAsDelete(pointer);
                        return true;
                    }
                }
                return false;
            }
        }

        public IEnumerable<long> DeleteRange(long LowKey, long HighKey)
        {
            lock (ReadWriteLock)
            {
                var start = this.Query.FindLeaf(this.RootNode, LowKey);
                var end = this.Query.FindLeaf(this.RootNode, HighKey);

                var collection = new PositionPointerCollection<T>(this, start, end, true);
                return collection.DeleteAll();
            }
        }


        private long _AddValueBlock(T value)
        {
            var valueBytes = this.ValueConverter.ToByte(value);

            var totalLen = valueBytes != null ? valueBytes.Length : 0;

            byte[] header = new byte[10];
            header[0] = 1;
            header[1] = 2;
            System.Buffer.BlockCopy(BitConverter.GetBytes(totalLen), 0, header, 2, 4);

            byte[] extraEmptyBytes = null;
            int extraSpaceCounter = 0;
            if (this.OverWriteValueBLock)
            {
                int CalculatedExtraSpace = (Int32)(totalLen * 0.2);
                extraSpaceCounter = CalculatedExtraSpace > 20480 ? 20480 : CalculatedExtraSpace;

                if (extraSpaceCounter < this.MinExtraSpace)
                {
                    extraSpaceCounter = this.MinExtraSpace;
                }

                extraEmptyBytes = new byte[extraSpaceCounter];

                var ExtraSpaceBytes = ByteHelper.IntToThreeBytes(extraSpaceCounter);
                System.Buffer.BlockCopy(ExtraSpaceBytes, 0, header, 7, 3);
            }

            lock (locker)
            {
                Int64 currentPosition = Stream.Length;
                Stream.Position = currentPosition;
                Stream.Write(header, 0, 10);
                Stream.Write(valueBytes);
                if (this.OverWriteValueBLock)
                {
                    Stream.Write(extraEmptyBytes, 0, extraSpaceCounter);
                }
                Stream.Flush();
                return currentPosition;
            }
        }


        private (long, bool) UpdateOrAddValueBlock(PositionPointer pointer, T value)
        {
            if (!this.OverWriteValueBLock)
            {
                return (this._AddValueBlock(value), true);
            }

            byte[] counterBytes = GetPartial(pointer.Position, 10);

            if (counterBytes == null || counterBytes[0] != 1 || counterBytes[1] != 2)
            {
                return (this._AddValueBlock(value), true);
            }
            int counter = BitConverter.ToInt32(counterBytes, 2);
            if (counter <= 0)
            {
                return (this._AddValueBlock(value), true);
            }

            int ExtraSpaceCounter = ByteHelper.ThreeBytesToInt(counterBytes, 7);

            var valueBytes = this.ValueConverter.ToByte(value);

            var totalLen = valueBytes != null ? valueBytes.Length : 0;

            if (totalLen < counter + ExtraSpaceCounter)
            {
                int LeftSpace = counter + ExtraSpaceCounter - totalLen;

                byte[] EmptyBytes = new byte[LeftSpace];

                byte[] header = new byte[10];
                header[0] = 1;
                header[1] = 2;
                System.Buffer.BlockCopy(BitConverter.GetBytes(totalLen), 0, header, 2, 4);

                var ExtraSpaceBytes = ByteHelper.IntToThreeBytes(LeftSpace);
                System.Buffer.BlockCopy(ExtraSpaceBytes, 0, header, 7, 3);

                lock (locker)
                {
                    Stream.Position = pointer.Position;
                    Stream.Write(header, 0, 10);
                    Stream.Write(valueBytes);
                    Stream.Write(EmptyBytes, 0, LeftSpace);
                    Stream.Flush();
                    return (pointer.Position, false);
                }
                // can update...write to same position. 
            }
            else
            {
                return (this._AddValueBlock(value), true);
            }
        }

        internal void MarkValueAsDelete(PositionPointer pointer)
        {
            //Mark Value Block..
            byte[] counterBytes = GetPartial(pointer.Position, 10);
            if (counterBytes != null && counterBytes[0] == 1 && counterBytes[1] == 2)
            {
                var absPosition = pointer.Position + 6;
                lock (locker)
                {
                    this.Stream.Position = absPosition;
                    this.Stream.WriteByte(1);
                }
            }
        }

        internal T LoadBlockValue(long diskPosition)
        {
            byte[] counterBytes = GetPartial(diskPosition, 10);

            if (counterBytes == null || counterBytes[0] != 1 || counterBytes[1] != 2 || counterBytes[6] == 1)
            {
                return default;
            }

            int counter = BitConverter.ToInt32(counterBytes, 2);
            if (counter <= 0)
            {
                return default;
            }

            var bytes = GetPartial(diskPosition + 10, counter);

            if (bytes != null)
            {
                return this.ValueConverter.FromByte(bytes);
            }
            return default;
        }

        internal T LoadValue(PositionPointer pointer)
        {
            if (pointer == null || pointer.Position < 0)
            {
                return default(T);
            }

            if (this.SkipValueBlock && this.MetaByteLen > 0)
            {
                var result = Activator.CreateInstance<T>();

                var meta = result as MetaObject.IMetaObject;
                meta.MetaKey = pointer.Id;
                meta.ParseMetaBytes(pointer.MetaBytes);

                if (this.SetKey != null)
                {
                    this.SetKey(result, pointer.Id);
                }
                return result;
            }


            byte[] counterBytes = GetPartial(pointer.Position, 10);

            if (counterBytes == null || counterBytes[0] != 1 || counterBytes[1] != 2 || counterBytes[6] == 1)
            {
                return default(T);
            }


            int counter = BitConverter.ToInt32(counterBytes, 2);
            if (counter <= 0)
            {
                return default(T);
            }

            var bytes = GetPartial(pointer.Position + 10, counter);

            if (bytes != null)
            {
                var result = this.ValueConverter.FromByte(bytes);

                if (this.MetaByteLen > 0)
                {
                    var meta = result as MetaObject.IMetaObject;
                    meta.MetaKey = pointer.Id;
                    meta.ParseMetaBytes(pointer.MetaBytes);
                }

                if (this.SetKey != null)
                {
                    this.SetKey(result, pointer.Id);
                }
                return result;
            }
            else
            {

                if (this.MetaByteLen > 0)
                {
                    var result = Activator.CreateInstance<T>();
                    var meta = result as MetaObject.IMetaObject;
                    meta.MetaKey = pointer.Id;
                    meta.ParseMetaBytes(pointer.MetaBytes);

                    if (this.SetKey != null)
                    {
                        this.SetKey(result, pointer.Id);
                    }
                    return result;
                }

            }


            return default(T);
        }

        internal T LoadMeta(PositionPointer pointer)
        {
            if (this.MetaByteLen > 0)
            {
                T value = Activator.CreateInstance<T>();
                var meta = value as MetaObject.IMetaObject;
                meta.ParseMetaBytes(pointer.MetaBytes);
                meta.MetaKey = pointer.Id;

                if (this.SetKey != null)
                {
                    this.SetKey(value, pointer.Id);
                }
                return value;
            }

            return default(T);
        }

        public void Close()
        {
            if (this._fileStream != null && this._fileStream.CanRead)
            {
                this._fileStream.Flush();
                this._fileStream.Close();
                this._fileStream.Dispose();
            }
        }

        public void Dispose()
        {
            this.Close();
            if (this._fileStream != null && this._fileStream.CanRead)
            {
                this._fileStream.Close();
            }

            this._fileStream = null;
            this.RootNode = null;
            this.writingNode = null;
        }

        public Query.WormQuery<T> Query
        {
            get
            {
                return new Query.WormQuery<T>(this);
            }
        }

        public Query.MetaQuery<T> MetaQuery
        {
            get
            {
                return new Query.MetaQuery<T>(this);
            }
        }


        public bool Restore()
        {
            lock (ReadWriteLock)
            {
                var folder = WormSetting.RestoreFolder;
                var id = System.Guid.NewGuid().ToString();
                string log = "restoring db: \r\n" + this.FullFileName + "\r\n";

                var logFile = Path.Combine(folder, id + ".log");

                var backupFile = Path.Combine(folder, id + ".backup");

                PathHelper.EnsureFileDirectoryExists(backupFile);

                File.Copy(this.FullFileName, backupFile);  // backup current. 

                var newTemp = Path.Combine(folder, id + ".data");

                var diskReader = new DiskReader<T>(this);

                var list = diskReader.ReadDiskBlock();

                log += "\r\n total count: " + list.Count.ToString() + "\r\n";

                diskReader.db = null;
                diskReader.Stream.Dispose();
                diskReader = null;

                var rest = new RestoreProcess<T>(list, this);

                log += "\r\n Value Block: " + rest.ValueBlock.Count.ToString() + ", Node count: " + rest.NodeList.Count.ToString() + "\r\n";

                var newDb = rest.RestoreTo(newTemp);
                newDb.Close();

                this.Close();
                _fileStream.Dispose();

                var currentFile = this.FullFileName;
                bool deleteOK = true;

                try
                {
                    File.Delete(currentFile);
                }
                catch (Exception)
                {
                    deleteOK = false;
                }

                if (!deleteOK)
                {
                    System.Threading.Thread.Sleep(100);
                    this.Close();

                    try
                    {
                        File.Delete(currentFile);
                    }
                    catch (Exception)
                    {
                        deleteOK = false;   //not delete ok, restore failed. 
                    }
                }

                if (deleteOK)
                {
                    File.Copy(newTemp, currentFile);
                    this.startup(this.FullFileName, this.keyFieldName, this.ValueConverter);  // reload; 
                }
                else
                {
                    log += "delete old file failed...";
                }

                log += "\r\nResult: " + deleteOK.ToString();

                File.WriteAllText(logFile, log);

                return deleteOK;
            }
        }


        public WormDb<T> RestoreTo(string fullFileName)
        {
            lock (locker)
            {
                if (!Path.IsPathRooted(fullFileName))
                {
                    fullFileName = System.IO.Path.Combine(WormSetting.RestoreFolder, fullFileName);
                }
                var id = System.Guid.NewGuid().ToString();
                var diskReader = new DiskReader<T>(this);
                var list = diskReader.ReadDiskBlock();
                diskReader.Stream.Dispose();
                diskReader = null;
                var rest = new RestoreProcess<T>(list, this);
                return rest.RestoreTo(fullFileName);
            }
        }
    }

}