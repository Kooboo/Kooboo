using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.WORM.Query
{

    public class DbCollection<TValue> : IEnumerable<TValue>
    {
        WormDb<TValue> Db { get; set; }
        LeafResult StartLeaf { get; set; }
        LeafResult EndLeaf { get; set; }
        bool ascending { get; set; }

        public DbCollection(WormDb<TValue> db, LeafResult startLeaf, LeafResult endLeaf, bool ascending)
        {
            this.Db = db;
            this.ascending = ascending;
            this.StartLeaf = startLeaf;
            this.EndLeaf = endLeaf;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator(this.Db, this.StartLeaf, this.EndLeaf, this.ascending);

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        public class Enumerator : IEnumerator<TValue>
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


            public Enumerator(WormDb<TValue> db, LeafResult startLeaf, LeafResult endLeaf, bool ascending)
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
                        return list.OrderBy(o => o.Id).ToList();
                    }
                    else
                    {
                        return list.OrderByDescending(o => o.Id).ToList();
                    }
                }
            }


            private TValue currentValue;


            public TValue Current
            {
                get { return currentValue; }
            }


            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            TValue IEnumerator<TValue>.Current => this.currentValue;

            public bool MoveNext()
            {
                if (this.readIndex <= this.itemCount - 1)
                {
                    var item = this.itemList[this.readIndex];
                    if (item == null || item.IsDeleted)
                    {
                        this.readIndex += 1;
                        return MoveNext();
                    }

                    var pos = this.itemList[this.readIndex];
                    this.currentValue = this.Db.LoadValue(pos);
                    this.readIndex += 1;
                    return true;
                }

                // load next node.  
                this.WorkingNode = GetNextNode(this.WorkingNode);

                if (this.WorkingNode == null)
                {
                    return false;
                }

                var list = this.LoadNodePointer(this.WorkingNode);

                while (list == null || !list.Any())
                {
                    this.WorkingNode = GetNextNode(this.WorkingNode);

                    if (this.WorkingNode == null)
                    {
                        return false;
                    }
                    list = this.LoadNodePointer(this.WorkingNode);
                }


                this.itemCount = list.Count;
                this.readIndex = 0;
                this.itemList = list;
                return MoveNext();
            }

            public void Reset()
            {

            }

            public void Dispose()
            {
                this.itemList = null;
                this.currentValue = default(TValue);
                this.WorkingNode = null;
                this.StartNode = null;
                this.EndNode = null;
            }
        }

    }

}
