using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.WORM.Query
{

    public class WhereQuery<T>
    {
        private WormDb<T> db { get; set; }

        private string OrderByFieldName;
        private int skip;
        private bool Ascending { get; set; }

        private bool metaOnly { get; set; }

        private long LowBound { get; set; } = -1;

        private long HighBound { get; set; } = long.MaxValue;

        private List<Predicate<T>> Filters { get; set; }

        public WhereQuery(WormDb<T> store, bool metaData)
        {
            this.db = store;
            this.metaOnly = metaData;
            this.skip = 0;
        }

        public WhereQuery<T> Where(Predicate<T> condition)
        {
            if (this.Filters == null)
            {
                this.Filters = new List<Predicate<T>>();
            }

            this.Filters.Add(condition);

            return this;
        }

        public WhereQuery<T> SearchRange(long lowBound, long HighBound)
        {
            this.LowBound = lowBound;
            this.HighBound = HighBound;
            return this;
        }

        public bool Exists()
        {
            var item = this.FirstOrDefault();
            return item != null;
        }

        /// <summary>
        /// Order by the Auto ID.
        /// </summary>
        /// <returns></returns>
        public WhereQuery<T> OrderByAscending()
        {
            this.Ascending = true;
            return this;
        }

        /// <summary>
        /// Order by descending based on the primary key.
        /// </summary>
        public WhereQuery<T> OrderByDescending()
        {
            this.Ascending = false;
            return this;
        }

        /// <summary>
        /// use column data to fill in the return TValue object. 
        /// The TValue object must have a parameterless constructor. 
        /// </summary>
        public WhereQuery<T> UseMeta()
        {
            this.metaOnly = true;
            return this;
        }

        public WhereQuery<T> Skip(int count)
        {
            this.skip = count;
            return this;
        }

        public T FirstOrDefault()
        {
            var item = Take(1);
            if (item == null)
            {
                return default(T);
            }
            return item.FirstOrDefault();
        }


        public List<T> SelectAll()
        {
            return Take(99999);
        }

        public List<T> Take(int count)
        {
            int skipped = 0;
            int taken = 0;

            bool HasFilter = this.Filters != null && this.Filters.Any();
            int filterCounter = 0;
            if (HasFilter)
            {
                filterCounter = this.Filters.Count;
            }

            List<T> result = new List<T>();

            foreach (var item in this.Collection())
            {
                bool match = true;
                for (int i = 0; i < filterCounter; i++)
                {
                    if (!this.Filters[i](item))
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                {
                    continue;
                }

                if (skipped < this.skip)
                {
                    skipped += 1;
                    continue;
                }

                result.Add(item);

                taken += 1;

                if (taken >= count)
                {
                    return result;
                }
            }

            return result;
        }

        public IEnumerable<T> GetCollection()
        {
            bool HasFilter = this.Filters != null && this.Filters.Any();
            int filterCounter = 0;
            if (HasFilter)
            {
                filterCounter = this.Filters.Count;
            }

            List<T> result = new List<T>();

            foreach (var item in this.Collection())
            {
                bool match = true;
                for (int i = 0; i < filterCounter; i++)
                {
                    if (!this.Filters[i](item))
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                {
                    continue;
                }

                yield return item;
            }

        }

        public int Count()
        {
            int counter = 0;
            int skipped = 0;

            bool HasFilter = this.Filters != null && this.Filters.Any();
            int filterCounter = 0;
            if (HasFilter)
            {
                filterCounter = this.Filters.Count;
            }

            List<T> result = new List<T>();

            foreach (var item in this.Collection())
            {
                bool match = true;
                for (int i = 0; i < filterCounter; i++)
                {
                    if (!this.Filters[i](item))
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                {
                    continue;
                }

                if (skipped < this.skip)
                {
                    skipped += 1;
                    continue;
                }

                counter += 1;
            }

            return counter;
        }


        private IEnumerable<T> Collection()
        {
            if (this.metaOnly)
            {
                return this.db.MetaQuery.Range(this.LowBound, this.HighBound, this.Ascending);
            }
            else
            {
                return this.db.Query.Range(this.LowBound, this.HighBound, Ascending);
            }
        }

    }



}
