//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.BTree
{

    public class BTreeIndexDuplicateReader
    {
        private BTreeIndexDuplicate duplicate;
        private JumpRecord start;
        private JumpRecord current;

        /// <summary>
        ///
        /// </summary>
        /// <param name="duplicate"></param>
        /// <param name="start"> the start jumpRecord in the most bottom</param>
        public BTreeIndexDuplicateReader(BTreeIndexDuplicate duplicate, JumpRecord start)
        {
            this.duplicate = duplicate;
            this.start = start;
            this.current = start;
        }

        public Int64 ReadNext()
        {
            if (this.current.Next > 0)
            {
                current = this.duplicate.getJumpRecord(current.Next);

                if (current != null)
                {
                    return current.BlockPosition;
                }
            }

            return -1;
        }

        public byte[] ReadNextPointerBytes()
        {

            if (this.current.Next > 0)
            {
                current = this.duplicate.getJumpRecord(current.Next);

                if (current != null)
                {
                    return current.pointerBytes;
                }
            }

            return null;
        }
    }
}
