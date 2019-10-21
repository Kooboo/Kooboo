//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Btree;
using System;

namespace Kooboo.IndexedDB.Indexs
{
    public class IndexBase<TValue, TKey> : IIndex<TValue>
    {
        public string FieldName
        {
            get;
            set;
        }

        public Type keyType
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public Btree.BtreeIndex<TKey> index;

        private Func<TValue, TKey> GetValue;

        public IndexBase(string fieldOrPropertyName, string fullIndexFileName, bool unique, int keylength, int maxCacheLevel)
        {
            this.keyType = typeof(TKey);
            this.FieldName = fieldOrPropertyName;

            this.GetValue = Helper.ObjectHelper.GetGetValue<TValue, TKey>(FieldName);
            index = new Btree.BtreeIndex<TKey>(this.FieldName, unique, keylength, fullIndexFileName, maxCacheLevel);

            this.Length = index.keylength;
        }

        public bool Add(TValue input, long blockPosition)
        {
            TKey fieldvalue = GetValue(input);
            return index.Add(fieldvalue, blockPosition);
        }

        public void Update(TValue oldRecord, TValue newRecord, long oldBlockPosition, long newBlockPosition)
        {
            TKey oldvalue = GetValue(oldRecord);
            TKey newvalue = GetValue(newRecord);
            this.index.Update(oldvalue, newvalue, oldBlockPosition, newBlockPosition);
        }

        public bool Del(TValue record, long blockPosition)
        {
            TKey value = GetValue(record);
            return this.index.Del(value, blockPosition);
        }

        public int Count(bool distinct)
        {
            return this.index.Count(distinct);
        }

        public ItemCollection AllItems(bool ascending)
        {
            return this.index.allItemCollection(ascending);
        }

        public ItemCollection GetCollection(byte[] startBytes, byte[] endBytes, bool lowerOpen, bool upperOpen, bool ascending)
        {
            return this.index.getCollection(startBytes, endBytes, lowerOpen, upperOpen, ascending);
        }

        public KeyBytesCollection AllKeys(bool ascending)
        {
            return this.index.AllKeyBytesCollection(ascending);
        }

        public void Close()
        {
            this.index.Close();
        }

        public void Flush()
        {
            this.index.Flush();
        }

        public void DelSelf()
        {
            this.index.DelSelf();
        }
    }
}