//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.BTree;

namespace Kooboo.IndexedDB.Indexs
{
    public interface IIndex<TValue>
    {
        string FieldName { get; set; }

        /// <summary>
        ///  the key length of this index. 
        /// </summary>
        int Length { get; set; }

        Type keyType { get; set; }

        bool Add(TValue input, Int64 blockPosition);


        void Update(TValue oldRecord, TValue NewRecord, Int64 oldBlockPosition, Int64 newBlockPosition);

        bool Del(TValue record, Int64 blockPosition);

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
