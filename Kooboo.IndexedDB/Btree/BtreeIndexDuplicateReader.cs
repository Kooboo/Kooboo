//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Btree
{

    /// <summary>
    /// duplcate items sequence reader. 
    /// </summary>
    public class BtreeIndexDuplicateReader
    {
        private BtreeIndexDuplicate duplicate;
        private JumpRecord start;
        private JumpRecord current;

        /// <summary>
        ///
        /// </summary>
        /// <param name="duplicate"></param>
        /// <param name="start"> the start jumprecord in the most bottom</param>
        public BtreeIndexDuplicateReader(BtreeIndexDuplicate duplicate, JumpRecord start)
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
