//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Sequence
{
    public class SequenceCollection<TValue> : IEnumerable<TValue>
    {
        private Sequence<TValue> _sequence;
        private bool _ascending;
        private Int64 _start;
        private Int64 _end;

        public SequenceCollection(Sequence<TValue> sequence, bool @ascending, Int64 start, Int64 end)
        {
            this._sequence = sequence;
            this._ascending = @ascending;

            //make sure start smaller than end.

            if (start > end)
            {
                this._start = end;
                this._end = start;
            }
            else
            {
                this._start = start;
                this._end = end;
            }
        }

        private IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator<TValue>(_sequence, _ascending, _start, _end);
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
            private Sequence<TEValue> _sequence;
            private bool _ascending;
            private Int64 _start;
            private Int64 _end;

            private long _currentindex = 0;
            private int _currentcount = 0;

            public Enumerator(Sequence<TEValue> sequence, bool Ascending, Int64 start, Int64 end)
            {
                this._sequence = sequence;
                this._ascending = Ascending;
                this._start = start;
                this._end = end;
            }

            public TEValue Current
            {
                get
                {
                    long startindex = 0;

                    if (this._ascending)
                    {
                        startindex = this._currentindex;
                    }
                    else
                    {
                        startindex = this._currentindex - 12 - this._currentcount;
                    }

                    return this._sequence.Get(startindex, this._currentcount);
                }
            }

            public void Dispose()
            {
                this._sequence = null;
            }

            public bool MoveNext()
            {
                if (this._start == this._end)
                {
                    return false;
                }

                if (this._ascending)
                {
                    if (_currentindex == 0)
                    {
                        _currentindex = this._start;
                        _currentcount = this._sequence.GetValueBytesCount(_currentindex, this._ascending);
                        return true;
                    }
                    else
                    {
                        if ((_currentindex + _currentcount + 12 + 6) >= this._end)
                        {
                            return false;
                        }
                        else
                        {
                            _currentindex = _currentindex + 12 + _currentcount;
                            _currentcount = this._sequence.GetValueBytesCount(_currentindex, this._ascending);
                            return true;
                        }
                    }
                }
                else
                {
                    //DESC
                    if (_currentindex == 0)
                    {
                        _currentindex = this._end;
                        _currentcount = this._sequence.GetValueBytesCount(_currentindex, this._ascending);
                        return true;
                    }
                    else
                    {
                        if ((_currentindex - _currentcount - 12 - 6) <= this._start)
                        {
                            return false;
                        }
                        else
                        {
                            _currentindex = _currentindex - 12 - _currentcount;

                            // get the next current count.
                            _currentcount = this._sequence.GetValueBytesCount(_currentindex, this._ascending);

                            return (_currentindex - _currentcount - 12) >= this._start;
                        }
                    }
                }
            }

            /// <summary>
            /// Sorry,not reset available or needed here.
            /// </summary>
            public void Reset()
            {
                this._currentindex = 0;
                this._currentcount = 0;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}