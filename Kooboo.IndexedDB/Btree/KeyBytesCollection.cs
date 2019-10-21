//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.Btree
{
    public class KeyBytesCollection : IEnumerable<byte[]>
    {
        private TreeFile treefile;
        private byte[] startKeyBytes;
        private byte[] endKeyBytes;
        private bool lowerOpen;
        private bool upperOpen;
        private IComparer<byte[]> comparer;

        private bool ascending = true;

        public KeyBytesCollection(TreeFile treefile, IComparer<byte[]> comparer, byte[] startkeybytes, byte[] endkeybytes, bool loweropen, bool upperopen, bool ascending)
        {
            this.treefile = treefile;
            this.startKeyBytes = startkeybytes;
            this.endKeyBytes = endkeybytes;
            this.lowerOpen = loweropen;
            this.upperOpen = upperopen;
            this.comparer = comparer;
            this.ascending = ascending;
        }

        public IEnumerator<byte[]> GetEnumerator()
        {
            return new Enumerator(this.treefile, this.comparer, this.startKeyBytes, this.endKeyBytes, this.lowerOpen, this.upperOpen, this.ascending);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<byte[]> IEnumerable<byte[]>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public class Enumerator : IEnumerator<byte[]>
        {
            private TreeFile _treefile;
            private byte[] startKeyBytes;
            private byte[] endKeyBytes;
            private bool lowerOpen;
            private bool upperOpen;
            private IComparer<byte[]> comparer;

            private bool ascending;

            public Enumerator(TreeFile treefile, IComparer<byte[]> comparer, byte[] startkeybytes, byte[] endkeybytes, bool loweropen, bool upperopen, bool ascending)
            {
                this._treefile = treefile;
                this.startKeyBytes = startkeybytes;
                this.endKeyBytes = endkeybytes;
                this.lowerOpen = loweropen;
                this.upperOpen = upperopen;
                this.comparer = comparer;

                this.ascending = ascending;

                init();

                if (recordlistcount == 0)
                {
                    if ((this.isCurrentEndNode && this.isCurrentStartNode) == false)
                    {
                        getnextnode();
                    }
                }
            }

            private MemoryTreeNode _startnode;
            private MemoryTreeNode _endnode;
            private MemoryTreeNode _currentnode;

            private bool isCurrentStartNode;
            private bool isCurrentEndNode;

            private int recordIndex;
            private List<NodePointer> recordlist = new List<NodePointer>();

            private List<Records> newRecordList { get; set; } = new List<Records>();

            private int recordlistcount;

            private byte[] currentValue;

            private void init()
            {
                if (ascending)
                {
                    this._startnode = MemoryTreeNodeManager.FindLeafByKey(this._treefile, this._treefile.RootCache, startKeyBytes);
                    this._endnode = MemoryTreeNodeManager.FindLeafByKey(this._treefile, this._treefile.RootCache, endKeyBytes);
                }
                else
                {
                    //load the last key as startnode, firstkey as endnode.
                    this._endnode = MemoryTreeNodeManager.FindLeafByKey(this._treefile, this._treefile.RootCache, startKeyBytes);
                    this._startnode = MemoryTreeNodeManager.FindLeafByKey(this._treefile, this._treefile.RootCache, endKeyBytes);
                }

                this._currentnode = _startnode;
                if (_currentnode == null)
                {
                    return;
                }

                isCurrentStartNode = true;

                if (_currentnode.TreeNode.DiskPosition == _endnode.TreeNode.DiskPosition)
                {
                    isCurrentEndNode = true;
                }
                recordIndex = -1;
                recordlistcount = 0;

                if (ascending)
                {
                    loadNodeRecord(_currentnode);
                }
                else
                {
                    loadNodeRecordDESC(_currentnode);
                }
            }

            private void loadNodeRecord(MemoryTreeNode currentnode)
            {
                //recordIndex = -1;
                //recordlistcount = 0;
                //recordlist.Clear();
                foreach (var item in currentnode.TreeNode.KeyArray.OrderBy(o => o.Key, this.comparer))
                {
                    if (isCurrentStartNode)
                    {
                        if (isCurrentEndNode)
                        {
                            //both start and end in the same leaf.
                            if ((this.comparer.Compare(startKeyBytes, item.Key) < 0 || (this.comparer.Compare(startKeyBytes, item.Key) == 0 && !this.lowerOpen)) && (this.comparer.Compare(endKeyBytes, item.Key) > 0 || (this.comparer.Compare(endKeyBytes, item.Key) == 0 && !this.upperOpen)))
                            {
                                NodePointer pointer = new NodePointer {PointerBytes = item.Value};
                                recordlist.Add(pointer);
                                newRecordList.Add(new Records() { Key = item.Key, Pointer = pointer });
                            }
                        }
                        else
                        {
                            // the start node.
                            if (this.comparer.Compare(startKeyBytes, item.Key) < 0 || (this.comparer.Compare(startKeyBytes, item.Key) == 0 && !this.lowerOpen))
                            {
                                NodePointer pointer = new NodePointer {PointerBytes = item.Value};
                                recordlist.Add(pointer);
                                newRecordList.Add(new Records() { Key = item.Key, Pointer = pointer });
                            }
                        }
                    }
                    else
                    {
                        // the end node.

                        if (isCurrentEndNode)
                        {
                            if (this.comparer.Compare(endKeyBytes, item.Key) > 0 || (this.comparer.Compare(endKeyBytes, item.Key) == 0 && !this.upperOpen))
                            {
                                NodePointer pointer = new NodePointer {PointerBytes = item.Value};
                                recordlist.Add(pointer);
                                newRecordList.Add(new Records() { Key = item.Key, Pointer = pointer });
                            }
                        }
                        else
                        {
                            //not start, not end, the middle one, insert everything.
                            NodePointer pointer = new NodePointer {PointerBytes = item.Value};
                            recordlist.Add(pointer);
                            newRecordList.Add(new Records() { Key = item.Key, Pointer = pointer });
                        }
                    }
                }

                recordlistcount = recordlist.Count;
                recordlistcount = newRecordList.Count();
            }

            /// <summary>
            /// load the record, this is in the DESC mode.
            /// </summary>
            /// <param name="currentnode"></param>
            private void loadNodeRecordDESC(MemoryTreeNode currentnode)
            {
                //recordIndex = -1;
                //recordlistcount = 0;
                //recordlist.Clear();

                foreach (var item in currentnode.TreeNode.KeyArray.OrderByDescending(o => o.Key, this.comparer))
                {
                    if (isCurrentStartNode)
                    {
                        if (isCurrentEndNode)
                        {
                            //both start and end in the same leaf.
                            if ((this.comparer.Compare(startKeyBytes, item.Key) < 0 || (this.comparer.Compare(startKeyBytes, item.Key) == 0 && !this.lowerOpen)) && (this.comparer.Compare(endKeyBytes, item.Key) > 0 || (this.comparer.Compare(endKeyBytes, item.Key) == 0 && !this.upperOpen)))
                            {
                                NodePointer pointer = new NodePointer {PointerBytes = item.Value};
                                recordlist.Add(pointer);
                                newRecordList.Add(new Records() { Key = item.Key, Pointer = pointer });
                            }
                        }
                        else
                        {
                            // the start node.

                            if (this.comparer.Compare(endKeyBytes, item.Key) > 0 || (this.comparer.Compare(endKeyBytes, item.Key) == 0 && !this.upperOpen))
                            {
                                NodePointer pointer = new NodePointer {PointerBytes = item.Value};
                                recordlist.Add(pointer);
                                newRecordList.Add(new Records() { Key = item.Key, Pointer = pointer });
                            }
                        }
                    }
                    else
                    {
                        // the end node.

                        if (isCurrentEndNode)
                        {
                            if (this.comparer.Compare(startKeyBytes, item.Key) < 0 || (this.comparer.Compare(startKeyBytes, item.Key) == 0 && !this.lowerOpen))
                            {
                                NodePointer pointer = new NodePointer {PointerBytes = item.Value};
                                recordlist.Add(pointer);
                                newRecordList.Add(new Records() { Key = item.Key, Pointer = pointer });
                            }
                        }
                        else
                        {
                            //not start, not end, the middle one, insert everything.
                            NodePointer pointer = new NodePointer {PointerBytes = item.Value};
                            recordlist.Add(pointer);
                            newRecordList.Add(new Records() { Key = item.Key, Pointer = pointer });
                        }
                    }
                }

                recordlistcount = recordlist.Count;
                recordlistcount = newRecordList.Count();
            }

            private void getnextnode()
            {
                recordIndex = -1;
                recordlistcount = 0;
                recordlist.Clear();
                newRecordList.Clear();

                if (this._currentnode.TreeNode.DiskPosition == this._endnode.TreeNode.DiskPosition)
                {
                    return;
                }

                if (ascending)
                {
                    this._currentnode = MemoryTreeNodeManager.FindNextLeaf(this._treefile, _currentnode);
                }
                else
                {
                    this._currentnode = MemoryTreeNodeManager.FindPreviousLeaf(this._treefile, _currentnode);
                }

                if (this._currentnode == null)
                {
                    return;
                }

                this.isCurrentStartNode = this._currentnode.TreeNode.DiskPosition == this._startnode.TreeNode.DiskPosition;

                this.isCurrentEndNode = this._currentnode.TreeNode.DiskPosition == this._endnode.TreeNode.DiskPosition;

                if (ascending)
                {
                    loadNodeRecord(_currentnode);
                }
                else
                {
                    loadNodeRecordDESC(_currentnode);
                }
            }

            private Records getnextpointer()
            {
                if (recordlistcount == 0)
                {
                    return null;
                }

                if (recordIndex < (recordlistcount - 1))
                {
                    recordIndex += 1;
                    return newRecordList[recordIndex];
                }
                else
                {
                    getnextnode();
                    return getnextpointer();
                }
            }

            public byte[] Current
            {
                get { return currentValue; }
            }

            public void Dispose()
            {
                //release the reference.
                this._treefile = null;
                this._startnode = null;
                this._endnode = null;
                this._currentnode = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            private bool _inDuplicateMode = false;
            private BtreeIndexDuplicateReader _duplicatereader;

            public bool MoveNext()
            {
                // if in duplicate mode. // TODO: this may not happend and should not care
                // becaust it returns the keys only.
                if (_inDuplicateMode)
                {
                    byte[] nextdup = _duplicatereader.ReadNextPointerBytes();
                    if (nextdup != null)
                    {
                        this.currentValue = nextdup;
                        return true;
                    }
                    else
                    {   // end of duplicate.
                        this._inDuplicateMode = false;
                        this._duplicatereader = null;
                        // cotinue with the getnextpointer;
                    }
                }

                var record = getnextpointer();
                //  NodePointer  pointer = getnextpointer();
                if (record == null)
                {
                    return false;
                }
                else
                {
                    this.currentValue = record.Key;
                    return true;
                }
                ///// else
                //if (record.Pointer.Indicator == EnumValues.TypeIndicator.block)
                //{
                //    this.currentValue = record.Key;
                //    return true;
                //}
                //else if (record.Pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                //{
                //    duplicatereader = this.treefile.duplicate.getReader(record.Pointer.PositionPointer);

                //    byte[] nextdup = duplicatereader.ReadNextPointerBytes();

                //    if (nextdup != null)
                //    {
                //        this.currentValue = nextdup;
                //        this.inDuplicateMode = true;
                //        return true;
                //    }
                //    else
                //    {
                //        // there is no item in the duplicate, should not be possible, just to make sure.
                //        this.duplicatereader = null;
                //        this.inDuplicateMode = false;
                //        // go to the next record again.
                //        return MoveNext();
                //    }

                //}
                //else
                //{
                //    /// should not be possible.
                //    return false;
                //}
            }

            public void Reset()
            {
                init();
            }
        }

        public class Records
        {
            public byte[] Key { get; set; }

            public NodePointer Pointer { get; set; }
        }
    }
}