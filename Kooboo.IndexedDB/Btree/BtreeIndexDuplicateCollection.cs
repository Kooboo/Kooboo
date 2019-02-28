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
    /// this implement the enumerable interface. 
    /// </summary>
    public class BtreeIndexDuplicateCollection : IEnumerable<Int64>
    {

        private BtreeIndexDuplicate duplicate;
        private JumpRecord start;

        public BtreeIndexDuplicateCollection(BtreeIndexDuplicate duplicate, JumpRecord start)
        {
            this.duplicate = duplicate;
            this.start = start;

           
        }

        IEnumerator<long> GetEnumerator()
        {
            return new Enumerator(duplicate, start);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<long> IEnumerable<long>.GetEnumerator()
        {
            return this.GetEnumerator();
        }



        public class Enumerator : IEnumerator<long>
        {
            private BtreeIndexDuplicate duplicate;
            private JumpRecord start;
            private JumpRecord current;

            public Enumerator(BtreeIndexDuplicate duplicate, JumpRecord start)
            {
                this.duplicate = duplicate;
                this.start = start;
                this.current = start;
            }

            public long Current
            {
                get
                {
                    return this.current.BlockPosition;
                }
            }

            public void Dispose()
            {
                this.duplicate = null;
                this.start = null;
                this.current = null;
            }


            public bool MoveNext()
            {
                if (this.current.Next > 0)
                {
                    current = this.duplicate.getJumpRecord(current.Next);
                    return true;
                }
                else
                {
                    return false;
                }

            }

            public void Reset()
            {
                this.current = this.start;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }
        }


    }



}
