//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Kooboo.Reflection
{
    public interface IWrappedCollection : IList
    {
        object UnderlyingCollection { get; }
    }

    public class CollectionWrapper<T> : ICollection<T>, IWrappedCollection
    {
        private readonly ICollection<T> _genericCollection;
        private object _syncRoot;

        //public CollectionWrapper(IList list)
        //{
        //    if (list is ICollection<T>)
        //    {
        //        _genericCollection = (ICollection<T>)list;
        //    }
        //    else
        //    {
        //        _list = list;
        //    }
        //}

        public CollectionWrapper(ICollection<T> list)
        {
            _genericCollection = list;
        }

        public virtual void Add(T item)
        {
            if (_genericCollection != null)
            {
                _genericCollection.Add(item);
            }
        }

        public virtual void Clear()
        {
            _genericCollection.Clear();

        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _genericCollection.CopyTo(array, arrayIndex);

        }

        public virtual int Count
        {
            get
            {
                return _genericCollection.Count;
            }
        }

        public virtual bool Remove(T item)
        {

            return _genericCollection.Remove(item);

        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _genericCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _genericCollection.GetEnumerator();
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return (Count - 1);
        }


        bool IList.IsFixedSize
        {
            get
            {

                return _genericCollection.IsReadOnly;

            }
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }


        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            CopyTo((T[])array, arrayIndex);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }


        public bool Contains(T item)
        {
            return _genericCollection.Contains(item);
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object UnderlyingCollection
        {
            get
            {
                return _genericCollection;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
