//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.BTree;

namespace Kooboo.IndexedDB.Dynamic
{
    public interface ITableIndex
    {
        /// <summary>
        ///  the key length of this index. 
        /// </summary>
        int Length { get; set; }

        string FieldName { get; set; }

        bool IsIncremental { get; set; }

        long Seed { get; set; }

        bool IsUnique { get; set; }

        long Increment { get; set; }

        long NextIncrement();

        Type keyType { get; set; }

        bool IsPrimaryKey { get; set; }

        bool IsSystem { get; set; }

        bool Add(object key, Int64 blockPosition);

        void Update(object oldKey, Int64 oldBlockPosition, Int64 newBlockPosition);

        void Update(object oldKey, object newkey, long oldBlockPosition, long newBlockPosition);

        long Get(object key);

        List<long> List(object key);

        bool Del(object key, Int64 blockPosition);

        List<Int64> Del(object key);

        /// <summary>
        /// count of the total records number.
        /// </summary>
        /// <returns></returns>
        int Count(bool distinct);

        ItemCollection AllItems(bool ascending);

        ItemCollection GetCollection(byte[] startBytes, byte[] endBytes, bool lowerOpen, bool upperOpen, bool ascending);

        KeyBytesCollection AllKeys(bool ascending);


        void Close();

        void Flush();

        void DelSelf();
    }

}
