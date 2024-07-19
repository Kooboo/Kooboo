using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.WORM.Query
{

    public class MetaQuery<TValue>
    {
        public WormDb<TValue> Db { get; set; }

        public MetaQuery(WormDb<TValue> database)
        {
            this.Db = database;
        }

        public MetaCollection<TValue> All(bool Ascending = false)
        {
            var start = FindLeaf(this.Db.RootNode, 0);
            var end = FindLeaf(this.Db.RootNode, long.MaxValue);

            return new MetaCollection<TValue>(this.Db, start, end, Ascending);
        }


        public MetaCollection<TValue> Range(long LowKey, long HighKey, bool Ascending = false)
        {
            var start = FindLeaf(this.Db.RootNode, LowKey);
            var end = FindLeaf(this.Db.RootNode, HighKey);
            return new MetaCollection<TValue>(this.Db, start, end, Ascending);
        }

        public long Count(long LowKey, long HighKey)
        {
            var start = FindLeaf(this.Db.RootNode, LowKey);
            var end = FindLeaf(this.Db.RootNode, HighKey);

            Counter<TValue> counter = new Counter<TValue>(this.Db, start, end, true);

            return counter.Count();

        }


        public TValue Find(Predicate<TValue> condition)
        {
            foreach (var item in this.All())
            {
                if (condition(item))
                {
                    return item;
                }
            }
            return default;
        }

        public TValue Find(Predicate<TValue> condition, bool Ascending, long MinId = -1, long MaxId = long.MaxValue)
        {
            foreach (var item in this.Range(MinId, MaxId, Ascending))
            {
                if (condition(item))
                {
                    return item;
                }
            }
            return default;
        }

        public List<TValue> FindAll(Predicate<TValue> condition)
        {
            List<TValue> result = new List<TValue>();

            foreach (var item in this.All())
            {
                if (condition(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public List<TValue> FindAll(Predicate<TValue> condition, bool Ascending, long MinId = -1, long MaxId = long.MaxValue)
        {
            List<TValue> result = new List<TValue>();

            foreach (var item in this.Range(MinId, MaxId, Ascending))
            {
                if (condition(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public int Count(Predicate<TValue> condition, int MaxCount, bool Ascending, long MinId = -1, long MaxId = long.MaxValue)
        {
            int counter = 0;

            foreach (var item in this.Range(MinId, MaxId, Ascending))
            {
                if (condition(item))
                {
                    counter += 1;
                    if (counter >= MaxCount)
                    {
                        return counter;
                    }
                }
            }
            return counter;
        }

        internal LeafResult FindLeaf(Node node, long id)
        {
            if (node == null)
            {
                return null;
            }

            int foundIndex = -1;

            for (int i = 0; i < WormSetting.NodePointerCount; i++)
            {
                var item = node.Pointer[i];
                if (item == null || item.Id > id)
                {
                    // found bigger. 
                    foundIndex = i - 1;
                    if (foundIndex < 0)
                    {
                        foundIndex = 0;
                    }
                    break;
                }
                else if (item.Id == id)
                {
                    foundIndex = i;
                    break;
                }
            }

            if (foundIndex < 0)
            {
                // not found, the biggest key.
                foundIndex = WormSetting.NodePointerCount - 1;
            }

            if (node.Isleaf)
            {
                return new LeafResult() { Node = node, PointerIndex = foundIndex, FindKey = id };
            }
            else
            {
                var pointer = node.Pointer[foundIndex];
                if (pointer == null || pointer.Position <= 0)
                {
                    return null;
                }

                var subNode = this.Db.LoadNode(pointer.Position);

                return FindLeaf(subNode, id);
            }

        }

        public WhereQuery<TValue> Where(Predicate<TValue> query)
        {
            var where = new WhereQuery<TValue>(this.Db, true);
            where.Where(query);
            return where;
        }

        public WhereQuery<TValue> Where()
        {
            var where = new WhereQuery<TValue>(this.Db, true);
            return where;
        }
    }


}
