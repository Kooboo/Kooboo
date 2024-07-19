using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.WORM.Query
{

    public class PositionPointerCollection<TValue>
    {
        WormDb<TValue> Db { get; set; }
        Node StartNode { get; set; }
        Node EndNode { get; set; }
        Node WorkingNode { get; set; }
        bool Ascending { get; set; }

        long MaxId { get; set; }
        long MinId { get; set; }

        List<PositionPointer> itemList = new List<PositionPointer>();
        int itemCount = 0;
        int readIndex = 0;

        public PositionPointerCollection(WormDb<TValue> db, LeafResult startLeaf, LeafResult endLeaf, bool ascending)
        {
            this.Db = db;

            if (startLeaf == null || endLeaf == null)
            {
                return;
            }

            this.Ascending = ascending;
            this.StartNode = startLeaf.Node;
            this.EndNode = endLeaf.Node;

            this.MinId = startLeaf.FindKey;
            this.MaxId = endLeaf.FindKey;

            if (this.MinId > this.MaxId)
            {
                var temp = this.MinId;
                this.MinId = this.MaxId;
                this.MaxId = temp;
            }

            if (this.Ascending)
            {
                this.WorkingNode = StartNode;
            }
            else
            {
                this.WorkingNode = EndNode;
            }


            var list = this.LoadNodePointer(this.WorkingNode);

            while (list == null || !list.Any())
            {
                this.WorkingNode = GetNextNode(this.WorkingNode);

                if (this.WorkingNode == null)
                {
                    break;
                }

                list = this.LoadNodePointer(this.WorkingNode);
            }

            if (list != null && list.Any())
            {
                this.itemCount = list.Count;
                this.readIndex = 0;
                this.itemList = list;

            }
        }

        private Node GetNextNode(Node current)
        {
            if (current == null)
            {
                return null;
            }

            Node result = null;
            if (this.Ascending)
            {
                if (current.DiskPosition == this.EndNode.DiskPosition)
                {
                    return null;
                }

                if (current.NextNode > 0)
                {
                    result = this.Db.LoadNode(current.NextNode);
                }
            }
            else
            {
                if (current.DiskPosition == this.StartNode.DiskPosition)
                {
                    return null;
                }

                if (current.PreviousNode > 0)
                {
                    result = this.Db.LoadNode(current.PreviousNode);
                }
            }
            return result;

        }

        private List<PositionPointer> LoadNodePointer(Node node)
        {
            if (node == null)
            {
                return null;
            }

            var list = node.Pointer.Where(o => o != null && o.Id >= this.MinId && o.Id <= this.MaxId);

            if (list == null || !list.Any())
            {
                return null;
            }
            else
            {
                if (this.Ascending)
                {
                    //return list.OrderBy(o => o.Id).ToList();
                    return node.Pointer.OrderBy(o => o, new NullPointerComparer()).ToList();
                }
                else
                {
                    return node.Pointer.OrderByDescending(o => o, new NullPointerComparer()).ToList();
                }
            }
        }


        public class NullPointerComparer : IComparer<PositionPointer>
        {
            public int Compare(PositionPointer a, PositionPointer b)
            {
                long AInt = a != null ? a.Id : -1;
                long BInt = b != null ? b.Id : -1;

                if (AInt == BInt)
                {
                    return 0;
                }
                return AInt < BInt ? -1 : 1;
            }
        }


        public List<long> DeleteAll()
        {
            List<long> deletedId = new List<long>();
            var item = FetchNext();
            while (item != null && item.Pointer != null)
            {
                if (item.Pointer.Position > 0)
                {
                    //Mark Pointer...
                    item.Pointer.IsDeleted = true;
                    this.Db.UpdatePointer(this.WorkingNode, item.Pointer, item.Index);
                    this.Db.MarkValueAsDelete(item.Pointer);

                    deletedId.Add(item.Pointer.Id);
                }
                item = FetchNext();
            }
            return deletedId;
        }

        public List<PositionPointer> FetchAll()
        {
            List<PositionPointer> Result = new List<PositionPointer>();
            var item = FetchNext();
            while (item != null)
            {
                Result.Add(item.Pointer);
                item = FetchNext();
            }
            return Result;
        }

        public PointerReadResult FetchNext()
        {
            if (this.readIndex <= this.itemCount - 1)
            {
                var item = this.itemList[this.readIndex];

                if (this.Ascending && item.Id > this.MaxId)
                {
                    return null;
                }
                if (!this.Ascending && item.Id < this.MinId)
                {
                    return null;
                }

                if (item.Id < this.MinId || item.Id > this.MaxId)
                {
                    this.readIndex += 1;
                    return FetchNext();
                }

                if (item == null || item.IsDeleted)
                {
                    this.readIndex += 1;
                    return FetchNext();
                }

                var pos = this.itemList[this.readIndex];
                PointerReadResult result = new PointerReadResult() { Pointer = pos, Index = this.readIndex };
                this.readIndex += 1;
                return result;
            }

            // load next node.  
            this.WorkingNode = GetNextNode(this.WorkingNode);

            if (this.WorkingNode == null)
            {
                return null;
            }

            var list = this.LoadNodePointer(this.WorkingNode);

            while (list == null || !list.Any())
            {
                this.WorkingNode = GetNextNode(this.WorkingNode);

                if (this.WorkingNode == null)
                {
                    return null;
                }
                list = this.LoadNodePointer(this.WorkingNode);
            }

            this.itemCount = list.Count;
            this.readIndex = 0;
            this.itemList = list;
            return FetchNext();
        }

        public class PointerReadResult
        {
            public PositionPointer Pointer;
            public int Index;
        }

    }



}
