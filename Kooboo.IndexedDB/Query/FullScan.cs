//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Indexs;
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Query
{
    /// <summary>
    /// restore every record back to the original class and test the condition.
    /// This is easy to use but bad performance.
    /// </summary>
    public class FullScan<TKey, TValue>
    {
        private Predicate<TValue> _predicate;

        private ObjectStore<TKey, TValue> _store;

        private bool _ascending;
        private string _orderByFieldName;
        private int _skip;

        public FullScan(ObjectStore<TKey, TValue> store, Predicate<TValue> predicate)
        {
            this._store = store;
            this._predicate = predicate;
            this._ascending = true;
            this._skip = 0;
        }

        public FullScan(ObjectStore<TKey, TValue> store)
        {
            this._store = store;
            this._ascending = true;
            this._skip = 0;
        }

        /// <summary>
        /// The where predicate<T> condition, only one predicate can be used.
        /// For multiple conditions, use || && inside the predicate.
        /// </summary>
        /// <param name="predicate"></param>
        public FullScan<TKey, TValue> Where(Predicate<TValue> predicate)
        {
            this._predicate = predicate;
            return this;
        }

        /// <summary>
        /// Order by the primary key.
        /// </summary>
        public FullScan<TKey, TValue> OrderByAscending()
        {
            this._ascending = true;
            return this;
        }

        /// <summary>
        /// Order by a field or property. This field should have an index on it.
        /// Order by a non-indexed field will have very bad performance.
        /// </summary>
        public FullScan<TKey, TValue> OrderByAscending(string fieldOrPropertyName)
        {
            this._ascending = true;
            this._orderByFieldName = fieldOrPropertyName;

            return this;
        }

        /// <summary>
        /// Order by descending based on the primary key.
        /// </summary>
        public FullScan<TKey, TValue> OrderByDescending()
        {
            this._ascending = false;

            return this;
        }

        /// <summary>
        /// Order by descending on a field or property. This field should have an index on it.
        /// </summary>
        public FullScan<TKey, TValue> OrderByDescending(string fieldOrPropertyName)
        {
            this._ascending = false;
            this._orderByFieldName = fieldOrPropertyName;
            return this;
        }

        public FullScan<TKey, TValue> Skip(int count)
        {
            this._skip = count;
            return this;
        }

        public TValue FirstOrDefault()
        {
            List<TValue> list = Take(1);
            return list.Count == 0 ? default(TValue) : list[0];
        }

        public List<TValue> Take(int count)
        {
            lock (this._store._Locker)
            {
                List<TValue> valuelist = new List<TValue>();

                if (count == 0)
                {
                    return valuelist;
                }

                int skipped = 0;
                int taken = 0;

                if (string.IsNullOrEmpty(this._orderByFieldName))
                {
                    // if there is not an order by field, use the primary key.
                    foreach (TValue item in _store.ItemCollection(this._ascending))
                    {
                        if (this._predicate(item))
                        {
                            if (skipped < this._skip)
                            {
                                skipped += 1;
                                continue;
                            }

                            valuelist.Add(item);
                            taken += 1;
                            if (taken >= count)
                            {
                                return valuelist;
                            }
                        }
                    }
                    return valuelist;
                }
                else
                {
                    // there is an order by field.
                    // check whether this is an index or not.
                    if (_store.Indexes.HasIndex(this._orderByFieldName))
                    {
                        IIndex<TValue> index = _store.Indexes.getIndex(this._orderByFieldName);

                        foreach (Int64 blockposition in index.AllItems(this._ascending))
                        {
                            TValue record = _store.getValue(blockposition);

                            if (this._predicate(record))
                            {
                                if (skipped < this._skip)
                                {
                                    skipped += 1;
                                    continue;
                                }

                                valuelist.Add(record);
                                taken += 1;
                                if (taken >= count)
                                {
                                    return valuelist;
                                }
                            }
                        }

                        return valuelist;
                    }
                    else
                    {
                        // the most expensive and brutal way.

                        throw new NotImplementedException("order by non-index field is not support yet");
                    }
                }
            }
        }

        public List<TValue> SelectAll()
        {
            return Take(99999);
        }
    }
}