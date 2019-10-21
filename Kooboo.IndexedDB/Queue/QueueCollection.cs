//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Queue
{
    public class QueueCollection<TValue> : IEnumerable<TValue>
    {
        private Queue<TValue> _queue;
        private bool _dequeue;

        public QueueCollection(Queue<TValue> queue, bool dequeue)
        {
            this._queue = queue;
            this._dequeue = dequeue;
        }

        private IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator<TValue>(_queue, _dequeue);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public class Enumerator<TEValue> : IEnumerator<TEValue>
        {
            private Queue<TEValue> _queue;
            private bool _dequeue;

            private int _currentDayInt;
            private int _currentcounter;  // the current reading or dequeue counter of current queue file.
            private int _currentTotalCount;

            public Enumerator(Queue<TEValue> queue, bool dequeue)
            {
                this._queue = queue;
                this._dequeue = dequeue;
                this._currentDayInt = this._queue._dequeueDateInt;
                this._currentcounter = this._queue._dequeueDateCounter;
                this._currentTotalCount = this._queue.GetQueueListFile(_currentDayInt).TotalCount();
            }

            public TEValue Current
            {
                get
                {
                    Int64 blockposition = this._queue.GetQueueListFile(_currentDayInt).GetBlockPosition(_currentcounter);
                    return this._queue.GetQueueContentFile(_currentDayInt).Get(blockposition);
                }
            }

            public void Dispose()
            {
                this._queue = null;
            }

            public bool MoveNext()
            {
                if (this._currentcounter >= this._currentTotalCount)
                {
                    // check one more time to refresh the update...
                    this._currentTotalCount = this._queue.GetQueueListFile(this._currentDayInt).TotalCount();
                }
                if (this._currentcounter < this._currentTotalCount)
                {
                    _currentcounter += 1;
                    if (this._dequeue)
                    {
                        this._queue.GetQueueListFile(this._currentDayInt).SetCounter(_currentcounter);
                    }
                    return true;
                }
                else
                {
                    return DateTime.Now.DayToInt() > this._currentDayInt && MoveOneDay();
                }
            }

            private bool MoveOneDay()
            {
                int nextid = this._currentDayInt;
                foreach (int item in this._queue.queueFileIdList)
                {
                    if (item > nextid)
                    {
                        nextid = item;
                        break;
                    }
                }

                if (nextid > _currentDayInt)
                {
                    this._currentDayInt = nextid;
                    this._currentcounter = this._queue.GetQueueListFile(nextid).GetCounter();
                    this._currentTotalCount = this._queue.GetQueueListFile(nextid).TotalCount();

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