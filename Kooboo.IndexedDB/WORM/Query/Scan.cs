using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.WORM.Query
{

    public class Scan<TValue>
    {
        private Predicate<TValue> Predicate { get; set; }
        private WormDb<TValue> Db { get; set; }
        private bool Ascending { get; set; }
        private string OrderByFieldName { get; set; }
        private int skip { get; set; }
        private long LowBound { get; set; } = -1;
        private long HighBound { get; set; } = long.MaxValue;

        public Scan(WormDb<TValue> store, Predicate<TValue> predicate)
        {
            this.Db = store;
            this.Predicate = predicate;
            this.Ascending = true;
            this.skip = 0;
        }

        public Scan(WormDb<TValue> store)
        {
            this.Db = store;
            this.Ascending = true;
            this.skip = 0;
        }

        /// <summary>
        /// The where predicate<T> condition, only one predicate can be used. 
        /// For multiple conditions, use || && inside the predicate.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="predicate"></param>
        public Scan<TValue> Where(Predicate<TValue> predicate)
        {
            this.Predicate = predicate;
            return this;
        }

        /// <summary>
        /// Order by the primary key.
        /// </summary>
        public Scan<TValue> OrderByAscending()
        {
            this.Ascending = true;
            return this;
        }

        public Scan<TValue> OrderByDescending()
        {
            this.Ascending = false;

            return this;
        }

        public Scan<TValue> Skip(int count)
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
            List<TValue> valueList = new List<TValue>();

            if (count == 0)
            {
                return valueList;
            }

            int skipped = 0;
            int taken = 0;

            // if there is not an order by field, use the primary key.  
            foreach (TValue item in Db.Query.Range(this.LowBound, this.HighBound, this.Ascending))
            {
                if (this.Predicate(item))
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

        public List<TValue> SelectAll()
        {
            return Take(99999);
        }
    }


}
