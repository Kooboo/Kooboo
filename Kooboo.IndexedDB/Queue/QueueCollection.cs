//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Queue
{
    public class QueueCollection<TValue> : IEnumerable<TValue>
    {

        private Queue<TValue> queue;
        private bool dequeue;

        public QueueCollection(Queue<TValue> queue, bool dequeue)
        {
            this.queue = queue;
            this.dequeue = dequeue;
        }

        IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator<TValue>(queue, dequeue);
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
            private Queue<EValue> queue;
            private bool dequeue;

            private int CurrentDayInt;
            private int currentcounter;  // the current reading or dequeue counter of current queue file. 
            private int currentTotalCount;

            public Enumerator(Queue<EValue> queue, bool dequeue)
            {
                this.queue = queue;
                this.dequeue = dequeue;
                this.CurrentDayInt = this.queue._dequeueDateInt;
                this.currentcounter = this.queue._dequeueDateCounter;
                this.currentTotalCount = this.queue.GetQueueListFile(CurrentDayInt).TotalCount();

            }

            public EValue Current
            {
                get
                {
                    Int64 blockposition = this.queue.GetQueueListFile(CurrentDayInt).GetBlockPosition(currentcounter);
                    return this.queue.GetQueueContentFile(CurrentDayInt).Get(blockposition);
                }
            }

            public void Dispose()
            {
                this.queue = null;
            }


            public bool MoveNext()
            {
                if (this.currentcounter >= this.currentTotalCount)
                {
                    /// check one more time to refresh the update... 
                    this.currentTotalCount = this.queue.GetQueueListFile(this.CurrentDayInt).TotalCount();
                }
                if (this.currentcounter < this.currentTotalCount)
                {
                    currentcounter += 1;
                    if (this.dequeue)
                    {
                        this.queue.GetQueueListFile(this.CurrentDayInt).SetCounter(currentcounter);
                    }
                    return true;
                }
                else
                {
                    if (DateTime.Now.DayToInt() > this.CurrentDayInt)
                    {
                        return MoveOneDay();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            private bool MoveOneDay()
            {
                int nextid = this.CurrentDayInt;
                foreach (int item in this.queue.queueFileIdList)
                {
                    if (item > nextid)
                    {
                        nextid = item;
                        break;
                    }
                }

                if (nextid > CurrentDayInt)
                {
                    this.CurrentDayInt = nextid;
                    this.currentcounter = this.queue.GetQueueListFile(nextid).GetCounter();
                    this.currentTotalCount = this.queue.GetQueueListFile(nextid).TotalCount();

                    return MoveNext();
                }
                else
                {
                    return false;
                }

            }

            /// <summary>
            /// Sorry,not reset available or needed here. 
            /// </summary>
            public void Reset()
            {
                // this.current = this.start;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }
        }

    }
}
