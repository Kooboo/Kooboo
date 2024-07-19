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

        IEnumerator<TValue> GetEnumerator()
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

            private ObjectStore<TKey, TValue> store;
            private IEnumerator<long> enumerator;

            public Enumerator(ObjectStore<TKey, TValue> store, IEnumerator<long> enumerator)
            {
                this.store = store;
                this.enumerator = enumerator;
            }

            public TValue Current
            {
                get
                {
                    return store.getValue(enumerator.Current);
                }
            }

            public void Dispose()
            {
                this.store = null;
                this.enumerator = null;
            }


            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

        }


    }
}
