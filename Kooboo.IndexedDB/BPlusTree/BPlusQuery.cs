using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.IndexedDB.BTree;

namespace Kooboo.IndexedDB.BPlusTree
{
    public class BPlusQuery<TKey, TValue> where TValue : IBPlusTreeObject
    {
        public BPlusStore<TKey, TValue> store { get; set; }

        private bool UseBPlusOnly;

        public BPlusQuery(BPlusStore<TKey, TValue> store, bool UseBplusOnly)
        {
            this.store = store;
            this.UseBPlusOnly = UseBplusOnly;
        }

        public IEnumerable<TValue> All(bool Ascending = false)
        {
            var col = this.store.primaryIndex.allItemCollection(Ascending);
            return col.Select(o => ConvertFromPointer(o));
        }

        public IEnumerable<TValue> Collection(Range<TKey> range, bool Ascending = false)
        {
            var rangeCol = this.store.primaryIndex.getCollection(range, Ascending);
            return rangeCol.Select(o => ConvertFromPointer(o));
        }

        public IEnumerable<TValue> Collection(TKey lowerBound, TKey highBound, bool Ascending = false, bool lowerBoundOpen = false, bool highBoundOpen = false)
        {
            var rang = new Range<TKey>();
            rang.lower = lowerBound;
            rang.upper = highBound;
            rang.lowerOpen = lowerBoundOpen;
            rang.upperOpen = highBoundOpen;
            return Collection(rang, Ascending);
        }

        public IEnumerable<TValue> Collection(Predicate<TValue> condition, Range<TKey> range = null, bool Ascending = false)
        {
            if (range == null)
            {
                range = new Range<TKey>();
                range.lower = this.store.FirstKey;
                range.upper = this.store.LastKey;
            }

            var rangeCol = this.store.primaryIndex.getCollection(range, Ascending);
            return rangeCol.Where(o => condition(ConvertFromPointer(o))).Select(o => ConvertFromPointer(o));
        }

        public TValue Find(Predicate<TValue> condition, Range<TKey> Range = null, bool Ascending = false)
        {
            if (Range == null)
            {
                Range = new Range<TKey>();
                Range.lower = this.store.FirstKey;
                Range.upper = this.store.LastKey;
            }

            foreach (var item in this.store.primaryIndex.getCollection(Range, Ascending))
            {
                var value = this.ConvertFromPointer(item);

                if (value != null && condition(value))
                {
                    return value;
                }
            }

            return default(TValue);
        }

        public List<TValue> FindAll(Predicate<TValue> condition, Range<TKey> Range = null, bool Ascending = false)
        {
            List<TValue> result = new List<TValue>();
            if (Range == null)
            {
                Range = new Range<TKey>();
                Range.lower = this.store.FirstKey;
                Range.upper = this.store.LastKey;
            }
            foreach (var item in this.store.primaryIndex.getCollection(Range, Ascending))
            {
                var value = this.ConvertFromPointer(item);
                if (value != null && condition(value))
                {
                    result.Add(value);
                }
            }

            return result;
        }

        //public List<TValue> FindAll(Predicate<TValue> condition)
        //{
        //    List<TValue> result = new List<TValue>();

        //    foreach (var item in this.All())
        //    {
        //        if (condition(item))
        //        {
        //            result.Add(item);
        //        }
        //    }
        //    return result;
        //}

        private TValue ConvertFromPointer(NodePointer pointer)
        {
            if (UseBPlusOnly || pointer.PositionPointer <= 0)
            {
                var obj = Activator.CreateInstance<TValue>();
                obj.SetBPlusBytes(pointer.BPlusBytes);
                if (this.store.SetKey != null)
                {
                    TKey key = this.store.KeyConverter.FromByte(pointer.KeyToPosition);
                    if (key != null)
                    {
                        this.store.SetKey(obj, key);
                    }
                }
                return obj;
            }
            else
            {
                return this.store.getValue(pointer.PositionPointer);
            }
        }

    }

}
