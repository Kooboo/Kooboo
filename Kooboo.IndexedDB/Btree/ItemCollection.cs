//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.BTree
{
    public class ItemCollection : IEnumerable<long>
    {
        private TreeFile treefile;
        private byte[] startKeyBytes;
        private byte[] endKeyBytes;
        private bool lowerOpen;
        private bool upperOpen;
        private IComparer<byte[]> comparer;

        private bool ascending = true;

        public ItemCollection(TreeFile treefile, IComparer<byte[]> comparer, byte[] startkeybytes, byte[] endkeybytes, bool loweropen, bool upperopen, bool ascending)
        {
            this.treefile = treefile;
            this.startKeyBytes = startkeybytes;
            this.endKeyBytes = endkeybytes;
            this.lowerOpen = loweropen;
            this.upperOpen = upperopen;
            this.comparer = comparer;
            this.ascending = ascending;
        }

        public IEnumerator<long> GetEnumerator()
        {
            return new Enumerator(this.treefile, this.comparer, this.startKeyBytes, this.endKeyBytes, this.lowerOpen, this.upperOpen, this.ascending);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<long> IEnumerable<long>.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        public class Enumerator : IEnumerator<long>
        {
            private TreeFile treefile;
            private byte[] startKeyBytes;
            private byte[] endKeyBytes;
            private bool lowerOpen;
            private bool upperOpen;
            private IComparer<byte[]> comparer;

            private bool ascending;

            public Enumerator(TreeFile treefile, IComparer<byte[]> comparer, byte[] startkeybytes, byte[] endkeybytes, bool loweropen, bool upperopen, bool ascending)
            {
                this.treefile = treefile;
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

            private MemoryTreeNode startnode;
            private MemoryTreeNode endnode;
            private MemoryTreeNode currentnode;


            private bool isCurrentStartNode;
            private bool isCurrentEndNode;

            private int recordIndex;
            private List<NodePointer> recordlist = new List<NodePointer>();
            private int recordlistcount;

            private long currentValue;

            private void init()
            {
                if (ascending)
                {
                    this.startnode = MemoryTreeNodeManager.FindLeafByKey(this.treefile, this.treefile.RootNode, startKeyBytes);
                    this.endnode = MemoryTreeNodeManager.FindLeafByKey(this.treefile, this.treefile.RootNode, endKeyBytes);
                }
                else
                {
                    //load the last key as startnode, firstkey as endnode.
                    this.endnode = MemoryTreeNodeManager.FindLeafByKey(this.treefile, this.treefile.RootNode, startKeyBytes);
                    this.startnode = MemoryTreeNodeManager.FindLeafByKey(this.treefile, this.treefile.RootNode, endKeyBytes);
                }

                this.currentnode = startnode;
                if (currentnode == null)
                {
                    return;
                }

                isCurrentStartNode = true;

                if (currentnode.TreeNode.DiskPosition == endnode.TreeNode.DiskPosition)
                {
                    isCurrentEndNode = true;
                }
                recordIndex = -1;
                recordlistcount = 0;

                if (ascending)
                {
                    loadNodeRecord(currentnode);
                }
                else
                {
                    loadNodeRecordDESC(currentnode);
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
                                var pointer = NodePointer.FromBytes(item.Value);
                                recordlist.Add(pointer);
                            }
                        }
                        else
                        {
                            // the start node.  
                            if (this.comparer.Compare(startKeyBytes, item.Key) < 0 || (this.comparer.Compare(startKeyBytes, item.Key) == 0 && !this.lowerOpen))
                            {
                                var pointer = NodePointer.FromBytes(item.Value);
                                recordlist.Add(pointer);
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
                                var pointer = NodePointer.FromBytes(item.Value);
                                recordlist.Add(pointer);
                            }

                        }
                        else
                        {
                            //not start, not end, the middle one, insert everything. 
                            var pointer = NodePointer.FromBytes(item.Value);
                            recordlist.Add(pointer);

                        }

                    }

                }

                recordlistcount = recordlist.Count;
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
                                var pointer = NodePointer.FromBytes(item.Value);
                                recordlist.Add(pointer);
                            }
                        }
                        else
                        {
                            // the start node. 

                            if (this.comparer.Compare(endKeyBytes, item.Key) > 0 || (this.comparer.Compare(endKeyBytes, item.Key) == 0 && !this.upperOpen))
                            {
                                var pointer = NodePointer.FromBytes(item.Value);
                                recordlist.Add(pointer);
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
                                var pointer = NodePointer.FromBytes(item.Value);
                                recordlist.Add(pointer);
                            }

                        }
                        else
                        {
                            //not start, not end, the middle one, insert everything.  
                            var pointer = NodePointer.FromBytes(item.Value);
                            recordlist.Add(pointer);

                        }

                    }

                }

                recordlistcount = recordlist.Count;
            }


            private void getnextnode()
            {
                recordIndex = -1;
                recordlistcount = 0;
                recordlist.Clear();

                if (this.currentnode.TreeNode.DiskPosition == this.endnode.TreeNode.DiskPosition)
                {
                    return;
                }

                if (ascending)
                {
                    this.currentnode = MemoryTreeNodeManager.FindNextLeaf(this.treefile, currentnode);
                }
                else
                {
                    this.currentnode = MemoryTreeNodeManager.FindPreviousLeaf(this.treefile, currentnode);
                }

                if (this.currentnode == null)
                {
                    return;
                }

                if (this.currentnode.TreeNode.DiskPosition == this.startnode.TreeNode.DiskPosition)
                {
                    this.isCurrentStartNode = true;
                }
                else
                {
                    this.isCurrentStartNode = false;
                }

                if (this.currentnode.TreeNode.DiskPosition == this.endnode.TreeNode.DiskPosition)
                {
                    this.isCurrentEndNode = true;
                }
                else
                {
                    this.isCurrentEndNode = false;
                }

                if (ascending)
                {
                    loadNodeRecord(currentnode);
                }
                else
                {
                    loadNodeRecordDESC(currentnode);
                }

            }

            private NodePointer getnextpointer()
            {
                if (recordlistcount == 0)
                {
                    return null;
                }

                if (recordIndex < (recordlistcount - 1))
                {
                    recordIndex += 1;
                    return recordlist[recordIndex];
                }
                else
                {
                    getnextnode();
                    return getnextpointer();
                }
            }

            public long Current
            {
                get { return currentValue; }
            }

            public void Dispose()
            {
                //release the reference. 
                this.treefile = null;
                this.startnode = null;
                this.endnode = null;
                this.currentnode = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            private bool inDuplicateMode = false;
            private BTreeIndexDuplicateReader duplicatereader;

            public bool MoveNext()
            {

                ///if in duplicate mode.
                if (inDuplicateMode)
                {
                    Int64 nextdup = duplicatereader.ReadNext();
                    if (nextdup > 0)
                    {
                        this.currentValue = nextdup;
                        return true;
                    }
                    else
                    {
                        // end of duplicate. 
                        this.inDuplicateMode = false;
                        this.duplicatereader = null;
                        // cotinue with the getnextpointer;
                    }
                }

                NodePointer pointer = getnextpointer();

                if (pointer == null)
                {
                    return false;
                }

                /// else
                /// 
                if (pointer.Indicator == EnumValues.TypeIndicator.block)
                {
                    this.currentValue = pointer.PositionPointer;
                    return true;
                }
                else if (pointer.Indicator == EnumValues.TypeIndicator.duplicate)
                {
                    duplicatereader = this.treefile.duplicate.getReader(pointer.PositionPointer);
                    Int64 nextdup = duplicatereader.ReadNext();

                    if (nextdup > 0)
                    {
                        this.currentValue = nextdup;
                        this.inDuplicateMode = true;
                        return true;
                    }
                    else
                    {
                        // there is no item in the duplicate, should not be possible, just to make sure. 
                        this.duplicatereader = null;
                        this.inDuplicateMode = false;
                        // go to the next record again. 
                        return MoveNext();
                    }

                }
                else
                {
                    /// should not be possible. 
                    return false;
                }
            }

            public void Reset()
            {
                init();
            }

        }

    }
}
