//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.BTree
{
    public class EnumValues
    {
        public enum TypeIndicator : byte
        {
            undefined = 0,
            leaf = 1,
            node = 2,
            root = 3,
            block = 4,   // Pointer to block position.
            duplicate = 6   // this is a duplicate item, pointer to duplicate section start location. 
        }

    }
}
