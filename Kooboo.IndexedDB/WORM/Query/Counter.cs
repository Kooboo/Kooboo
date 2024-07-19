using System;
using System.Linq;

namespace Kooboo.IndexedDB.WORM.Query
{


    public class Counter<TValue>
    {
        WormDb<TValue> db { get; set; }
        LeafResult startLeaf { get; set; }
        LeafResult endLeaf { get; set; }
        bool ascending { get; set; }

        public Counter(WormDb<TValue> db, LeafResult startLeaf, LeafResult endLeaf, bool ascending)
        {
            this.db = db;
            this.ascending = ascending;
            this.startLeaf = startLeaf;
            this.endLeaf = endLeaf;
        }

        public long Count()
        {
            if (startLeaf == null || endLeaf == null)
            {
                return 0;
            }

            var startNode = startLeaf.Node;
            var endNode = endLeaf.Node;

            var MinId = startLeaf.FindKey;
            var MaxId = endLeaf.FindKey;

            if (MinId > MaxId)
            {
                (MaxId, MinId) = (MinId, MaxId);
            }

            Node workingNode;
            if (this.ascending)
            {
                workingNode = startNode;
            }
            else
            {
                workingNode = endNode;
            }

            long counter = 0;

            while (workingNode != null)
            {
                var currentCount = this.CountNode(workingNode, MaxId, MinId);
                counter += currentCount;
                workingNode = GetNextNode(workingNode, startNode, endNode);
            }

            return counter;

        }



        private Node GetNextNode(Node current, Node startNode, Node endNode)
        {
            if (current == null)
            {
                return null;
            }

            Node result = null;
            if (this.ascending)
            {
                if (current.DiskPosition == endNode.DiskPosition)
                {
                    return null;
                }

                if (current.NextNode > 0)
                {
                    result = this.db.LoadNode(current.NextNode);
                }
            }
            else
            {
                if (current.DiskPosition == startNode.DiskPosition)
                {
                    return null;
                }

                if (current.PreviousNode > 0)
                {
                    result = this.db.LoadNode(current.PreviousNode);
                }
            }
            return result;

        }


        private long CountNode(Node node, long MaxId, long MinId)
        {
            if (node == null)
            {
                return 0;
            }

            return node.Pointer.Where(o => o != null && o.IsDeleted == false && o.Id >= MinId && o.Id <= MaxId).Count();

        }


    }



}
