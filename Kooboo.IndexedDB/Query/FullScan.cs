//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.Indexs;

namespace Kooboo.IndexedDB.Query
{

    /// <summary>
    /// restore every record back to the original class and test the condition. 
    /// This is easy to use but bad performance. 
    /// </summary>
    public class FullScan<TKey, TValue>
    {

        Predicate<TValue> predicate;

        ObjectStore<TKey, TValue> store;

        private bool ascending;
        private string OrderByFieldName;
        private int skip;

        public FullScan(ObjectStore<TKey, TValue> store, Predicate<TValue> predicate)
        {
            this.store = store;
            this.predicate = predicate;
            this.ascending = true;
            this.skip = 0;
        }

        public FullScan(ObjectStore<TKey, TValue> store)
        {
            this.store = store;
            this.ascending = true;
            this.skip = 0;
        }

        /// <summary>
        /// The where predicate<T> condition, only one predicate can be used. 
        /// For multiple conditions, use || && inside the predicate.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="predicate"></param>
        public FullScan<TKey, TValue> Where(Predicate<TValue> predicate)
        {
            this.predicate = predicate;
            return this;
        }

        /// <summary>
        /// Order by the primary key.
        /// </summary>
        public FullScan<TKey, TValue> OrderByAscending()
        {
            this.ascending = true;
            return this;
        }

        /// <summary>
        /// Order by a field or property. This field should have an index on it. 
        /// Order by a non-indexed field will have very bad performance. 
        /// </summary>
        public FullScan<TKey, TValue> OrderByAscending(string FieldOrPropertyName)
        {
            this.ascending = true;
            this.OrderByFieldName = FieldOrPropertyName;

            return this;
        }

        /// <summary>
        /// Order by descending based on the primary key.
        /// </summary>
        public FullScan<TKey, TValue> OrderByDescending()
        {
            this.ascending = false;

            return this;
        }

        /// <summary>
        /// Order by descending on a field or property. This field should have an index on it. 
        /// </summary>
        public FullScan<TKey, TValue> OrderByDescending(string FieldOrPropertyName)
        {
            this.ascending = false;
            this.OrderByFieldName = FieldOrPropertyName;
            return this;
        }

        public FullScan<TKey, TValue> Skip(int count)
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
                List<TValue> valuelist = new List<TValue>();

                if (count == 0)
                {
                    return valuelist;
                }

                int skipped = 0;
                int taken = 0;

                if (string.IsNullOrEmpty(this.OrderByFieldName))
                {
                    // if there is not an order by field, use the primary key.  
                    foreach (TValue item in store.ItemCollection(this.ascending))
                    {
                        if (this.predicate(item))
                        {
                            if (skipped < this.skip)
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
                    if (store.Indexes.HasIndex(this.OrderByFieldName))
                    {
                        IIndex<TValue> index = store.Indexes.getIndex(this.OrderByFieldName);

                        foreach (Int64 blockPosition in index.AllItems(this.ascending))
                        {
                            TValue record = store.getValue(blockPosition);

                            if (this.predicate(record))
                            {
                                if (skipped < this.skip)
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
                        /// the most expensive and brutal way. 

                        throw new NotImplementedException("order by non-index field is not support yet");

                    }

                }

            }
        }

        public List<TValue> SelectAll()
        {
            return Take(99999);
        }


        public int Count()
        {
            lock (this.store._Locker)
            {
                int counter = 0;

                if (string.IsNullOrEmpty(this.OrderByFieldName))
                {
                    // if there is not an order by field, use the primary key.  
                    foreach (TValue item in store.ItemCollection(this.ascending))
                    {
                        if (this.predicate(item))
                        {
                            counter += 1;
                        }
                    }
                    return counter;
                }
                else
                {
                    // there is an order by field. 
                    // check whether this is an index or not. 
                    if (store.Indexes.HasIndex(this.OrderByFieldName))
                    {
                        IIndex<TValue> index = store.Indexes.getIndex(this.OrderByFieldName);

                        foreach (Int64 blockPosition in index.AllItems(this.ascending))
                        {
                            TValue record = store.getValue(blockPosition);

                            if (this.predicate(record))
                            {
                                counter += 1;
                            }
                        }
                        return counter;

                    }

                    else
                    {
                        /// the most expensive and brutal way. 
                        throw new NotImplementedException("order by non-index field is not support yet");

                    }

                }

            }
        }



    }
}
