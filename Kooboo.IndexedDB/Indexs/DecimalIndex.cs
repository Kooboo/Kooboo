//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Kooboo.IndexedDB.Btree;

namespace Kooboo.IndexedDB.Indexs
{
    public class DecimalIndex<TValue> : IIndex<TValue>
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

        public Btree.BtreeIndex<double> index;

        Func<TValue, decimal> getValue;


        public DecimalIndex(string FieldOrPropertyName, string FullIndexFileName, bool unique, int keylength, int MaxCacheLevel)
        {
            this.keyType = typeof(decimal);

            this.FieldName = FieldOrPropertyName;


            getValue = Helper.ObjectHelper.GetGetValue<TValue, decimal>(FieldName);
            index = new Btree.BtreeIndex<double>(this.FieldName, unique, keylength, FullIndexFileName, MaxCacheLevel);

            this.Length = index.keylength;
        }


        public bool Add(TValue input, long blockPosition)
        {
            decimal fieldvalue = getValue(input);
            double doublevalue = Convert.ToDouble(fieldvalue);
            return index.Add(doublevalue, blockPosition);
        }

        public void Update(TValue oldRecord, TValue NewRecord, long oldBlockPosition, long newBlockPosition)
        {
            decimal oldvalue = getValue(oldRecord);
            decimal newvalue = getValue(NewRecord);

            double doldvalue = Convert.ToDouble(oldvalue);
            double dnewvalue = Convert.ToDouble(newvalue);

            this.index.Update(doldvalue, dnewvalue, oldBlockPosition, newBlockPosition);

        }

        public bool Del(TValue record, long blockPosition)
        {
            decimal value = getValue(record);
            double doublevalue = Convert.ToDouble(value);
            return this.index.Del(doublevalue, blockPosition);
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

        public KeyBytesCollection AllKeys(bool ascending)
        {
            return this.index.AllKeyBytesCollection(ascending); 
        }
    }
}
