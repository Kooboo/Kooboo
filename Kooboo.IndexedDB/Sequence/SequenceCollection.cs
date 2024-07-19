//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Sequence
{
    public class SequenceCollection<TValue> : IEnumerable<TValue>
    {
        private Sequence<TValue> sequence;
        private bool ascending;
        private Int64 start;
        private Int64 end;

        public SequenceCollection(Sequence<TValue> sequence, bool Ascending, Int64 start, Int64 end)
        {
            this.sequence = sequence;
            this.ascending = Ascending;

            ///make sure start smaller than end. 

            if (start > end)
            {
                this.start = end;
                this.end = start;
            }
            else
            {
                this.start = start;
                this.end = end;
            }

        }

        IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator<TValue>(sequence, ascending, start, end);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public class Enumerator<EValue> : IEnumerator<EValue>
        {
            private Sequence<EValue> sequence;
            private bool ascending;
            private Int64 start;
            private Int64 end;

            private long currentindex = 0;
            private int currentcount = 0;

            public Enumerator(Sequence<EValue> sequence, bool Ascending, Int64 start, Int64 end)
            {
                this.sequence = sequence;
                this.ascending = Ascending;
                this.start = start;
                this.end = end;
            }

            public EValue Current
            {
                get
                {
                    long startindex = 0;

                    if (this.ascending)
                    {
                        startindex = this.currentindex;
                    }
                    else
                    {
                        startindex = this.currentindex - 12 - this.currentcount;
                    }

                    return this.sequence.Get(startindex, this.currentcount);

                }
            }

            public void Dispose()
            {
                this.sequence = null;
            }

            public bool MoveNext()
            {
                if (this.start == this.end)
                {
                    return false;
                }

                if (this.ascending)
                {
                    if (currentindex == 0)
                    {
                        currentindex = this.start;
                        currentcount = this.sequence.GetValueBytesCount(currentindex, this.ascending);
                        return true;
                    }
                    else
                    {
                        if ((currentindex + currentcount + 12 + 6) >= this.end)
                        {
                            return false;
                        }
                        else
                        {
                            currentindex = currentindex + 12 + currentcount;
                            currentcount = this.sequence.GetValueBytesCount(currentindex, this.ascending);
                            return true;
                        }
                    }
                }
                else
                {
                    ///DESC
                    if (currentindex == 0)
                    {
                        currentindex = this.end;
                        currentcount = this.sequence.GetValueBytesCount(currentindex, this.ascending);
                        return true;
                    }
                    else
                    {
                        if ((currentindex - currentcount - 12 - 6) <= this.start)
                        {
                            return false;
                        }
                        else
                        {
                            currentindex = currentindex - 12 - currentcount;

                            /// get the next current count. 
                            currentcount = this.sequence.GetValueBytesCount(currentindex, this.ascending);

                            if ((currentindex - currentcount - 12) < this.start)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }

                }

            }


            /// <summary>
            /// Sorry,not reset available or needed here. 
            /// </summary>
            public void Reset()
            {
                this.currentindex = 0;
                this.currentcount = 0;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }
        }


    }
}
