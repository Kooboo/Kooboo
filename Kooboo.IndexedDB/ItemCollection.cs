//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.IndexedDB
{
    /// <summary>
    /// object store value collection.
    /// </summary>
    public class ValueCollection<TKey, TValue> : IEnumerable<TValue>
    {
        private ObjectStore<TKey, TValue> store;
        private IEnumerator<long> enumerator;

        public ValueCollection(ObjectStore<TKey, TValue> store, IEnumerator<long> enumerator)
        {
            this.store = store;
            this.enumerator = enumerator;
        }

        private IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator(store, enumerator);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public class Enumerator : IEnumerator<TValue>
        {
            private ObjectStore<TKey, TValue> _store;
            private IEnumerator<long> _enumerator;

            public Enumerator(ObjectStore<TKey, TValue> store, IEnumerator<long> enumerator)
            {
                this._store = store;
                this._enumerator = enumerator;
            }

            public TValue Current
            {
                get
                {
                    return _store.getValue(_enumerator.Current);
                }
            }

            public void Dispose()
            {
                this._store = null;
                this._enumerator = null;
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}