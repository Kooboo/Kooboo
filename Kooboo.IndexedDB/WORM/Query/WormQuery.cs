using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.WORM.Query
{
    public class WormQuery<TValue>
    {
        public WormDb<TValue> db { get; set; }

        public WormQuery(WormDb<TValue> database)
        {
            this.db = database;
        }

        public DbCollection<TValue> All(bool Ascending = false)
        {
            var start = FindLeaf(this.db.RootNode, 0);
            var end = FindLeaf(this.db.RootNode, long.MaxValue);

            return new DbCollection<TValue>(this.db, start, end, Ascending);
        }


        public DbCollection<TValue> Range(long LowKey, long HighKey, bool Ascending = false)
        {
            var start = FindLeaf(this.db.RootNode, LowKey);
            var end = FindLeaf(this.db.RootNode, HighKey);

            return new DbCollection<TValue>(this.db, start, end, Ascending);
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
            return default(TValue);
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
            return default(TValue);
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

        public Scan<TValue> FullScan
        {
            get
            {
                return new Scan<TValue>(this.db);
            }
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

                if (item == null)
                {
                    if (NextAllNull(i))
                    {
                        foundIndex = i - 1;
                        if (foundIndex < 0)
                        {
                            foundIndex = 0;
                        }
                        if (node.Pointer[foundIndex] == null)
                        {
                            foundIndex = i;
                        }
                        break;
                    }
                    else
                    {
                        // move null ahead...
                        while (i < WormSetting.NodePointerCount - 1 && node.Pointer[i + 1] == null)
                        {
                            i++;
                        }
                        continue;
                    }
                }

                else if (item.Id > id)
                {
                    //found bigger. 
                    foundIndex = i - 1;
                    if (foundIndex < 0)
                    {
                        foundIndex = 0;
                    }
                    if (node.Pointer[foundIndex] == null)
                    {
                        foundIndex = i;
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

                var subNode = this.db.LoadNode(pointer.Position);

                return FindLeaf(subNode, id);
            }


            bool NextAllNull(int currentIndex)
            {
                for (int i = currentIndex; i < WormSetting.NodePointerCount; i++)
                {
                    if (node.Pointer[i] != null)
                    {
                        return false;
                    }
                }
                return true;
            }

        }


        public WhereQuery<TValue> Where(Predicate<TValue> query)
        {
            var where = new WhereQuery<TValue>(this.db, false);
            where.Where(query);
            return where;
        }

        public WhereQuery<TValue> Where()
        {
            var where = new WhereQuery<TValue>(this.db, false);
            return where;
        }

    }

}
