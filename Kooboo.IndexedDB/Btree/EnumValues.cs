//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Btree
{
    public class EnumValues
    {
        public enum DeleteIndicator : byte
        {
            normal_not_deleted = 1,
            free_deleted = 0,
            //root = 2
        }

        public enum TypeIndicator : byte
        {
            undefined = 0,
            free_deleted = 5,
            leaf = 1,
            node = 2,
            root = 3,
            block = 4,   // Pointer to block position.
            duplicate =6   // this is a duplicate item, pointer to duplicate section start location. 
        }

        /// <summary>
        /// OnlyPrevious = in the previous pointer and the only sub node. parentnode.keyarray.count ==0
        /// largest = the largest key.
        /// InBetween = found key > current key, in order to merge. 
        /// </summary>
        public enum SearchResultType : byte
        {
            OnlyPrevious = 1,
            InBetween = 2,
            Largest = 3
        }

    }
}
