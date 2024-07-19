using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Query
{
    public class RangeScan<TKey, TValue>
    {
        Predicate<TValue> predicate;

        ObjectStore<TKey, TValue> store;

        private bool ascending;
        private int skip;
        private TKey LowerKey;
        private TKey HightKey;
        private bool LowKeyOpen;
        private bool HighKeyOpen;

        public RangeScan(ObjectStore<TKey, TValue> store, Predicate<TValue> predicate)
        {
            this.store = store;
            this.predicate = predicate;
            this.ascending = true;
            this.skip = 0;
        }

        public RangeScan(ObjectStore<TKey, TValue> store)
        {
            this.store = store;
            this.ascending = true;
            this.skip = 0;
        }


        public RangeScan<TKey, TValue> Where(Predicate<TValue> predicate)
        {
            this.predicate = predicate;
            return this;
        }

        public RangeScan<TKey, TValue> Range(TKey lowBound, TKey HighBound, bool LowBoundOpen = false, bool HighBoundOpen = false)
        {
            this.LowerKey = lowBound;
            this.HightKey = HighBound;
            this.LowKeyOpen = LowBoundOpen;
            this.HighKeyOpen = HighBoundOpen;
            return this;
        }

        public RangeScan<TKey, TValue> Range(TKey lowBound)
        {
            this.LowerKey = lowBound;
            this.HightKey = this.store.primaryIndex.LastKey;
            return this;
        }


        public RangeScan<TKey, TValue> OrderByAscending()
        {
            this.ascending = true;
            return this;
        }

        public RangeScan<TKey, TValue> OrderByDescending()
        {
            this.ascending = false;
            return this;

        }

        public RangeScan<TKey, TValue> Skip(int count)
        {
            this.skip = count;
            return this;
        }

        public TValue FirstOrDefault()
        {
            List<TValue> list = Take(1);
            if (list.Count == 0)
            {
                return default(TValue);
            }
            else
            {
                return list[0];
            }
        }

        public List<TValue> Take(int count)
        {
            lock (this.store._Locker)
            {
                List<TValue> valueList = new List<TValue>();

                if (count == 0)
                {
                    return valueList;
                }

                int skipped = 0;
                int taken = 0;

                var col = store.primaryIndex.getCollection(new Range<TKey>() { lower = this.LowerKey, upper = this.HightKey, lowerOpen = this.LowKeyOpen, upperOpen = this.HighKeyOpen }, this.ascending);


                foreach (var pos in col)
                {
                    TValue item = store.getValue(pos);
                    if (this.predicate == null || this.predicate(item))
                    {
                        if (skipped < this.skip)
                        {
                            skipped += 1;
                            continue;
                        }

                        valueList.Add(item);
                        taken += 1;
                        if (taken >= count)
                        {
                            return valueList;
                        }
                    }
                }
                return valueList;


            }
        }

        public IEnumerable<TValue> ToEnumerable()
        {

            var col = store.primaryIndex.getCollection(new Range<TKey>() { lower = this.LowerKey, upper = this.HightKey, lowerOpen = this.LowKeyOpen, upperOpen = this.HighKeyOpen }, this.ascending);

            foreach (var pos in col)
            {
                TValue item = store.getValue(pos);
                if (this.predicate == null || this.predicate(item))
                {
                    yield return item;
                }
            }

        }

    }





}
