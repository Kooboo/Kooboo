//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public interface IWrappedCollection : IList
    {
        object UnderlyingCollection { get; }
    }

    public class CollectionWrapper<T> : ICollection<T>, IWrappedCollection
    {
        private readonly IList _list;
        private readonly ICollection<T> _genericCollection;
        private object _syncRoot;

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
            else
            {
                _list.Add(item);
            }
        }

        public virtual void Clear()
        {
            if (_genericCollection != null)
            {
                _genericCollection.Clear();
            }
            else
            {
                _list.Clear();
            }
        }

        public virtual bool Contains(T item)
        {
            return _genericCollection?.Contains(item) ?? _list.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (_genericCollection != null)
            {
                _genericCollection.CopyTo(array, arrayIndex);
            }
            else
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public virtual int Count
        {
            get { return _genericCollection?.Count ?? _list.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return _genericCollection?.IsReadOnly ?? _list.IsReadOnly; }
        }

        public virtual bool Remove(T item)
        {
            if (_genericCollection != null)
            {
                return _genericCollection.Remove(item);
            }
            else
            {
                bool contains = _list.Contains(item);

                if (contains)
                {
                    _list.Remove(item);
                }

                return contains;
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _genericCollection != null ? _genericCollection.GetEnumerator() : _list.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _genericCollection?.GetEnumerator() ?? _list.GetEnumerator();
        }

        int IList.Add(object value)
        {
            VerifyValueType(value);
            Add((T)value);

            return (Count - 1);
        }

        bool IList.Contains(object value)
        {
            return IsCompatibleObject(value) && Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            if (_genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> does not support IndexOf.");
            }

            if (IsCompatibleObject(value))
            {
                return _list.IndexOf((T)value);
            }

            return -1;
        }

        void IList.RemoveAt(int index)
        {
            if (_genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> does not support RemoveAt.");
            }

            _list.RemoveAt(index);
        }

        void IList.Insert(int index, object value)
        {
            if (_genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> does not support Insert.");
            }

            VerifyValueType(value);
            _list.Insert(index, (T)value);
        }

        bool IList.IsFixedSize
        {
            get
            {
                // ICollection<T> only has IsReadOnly
                return _genericCollection?.IsReadOnly ?? _list.IsFixedSize;
            }
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                if (_genericCollection != null)
                {
                    throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
                }

                return _list[index];
            }
            set
            {
                if (_genericCollection != null)
                {
                    throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
                }

                VerifyValueType(value);
                _list[index] = (T)value;
            }
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

        private static void VerifyValueType(object value)
        {
            if (!IsCompatibleObject(value))
            {
                // throw new ArgumentException("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.".FormatWith(CultureInfo.InvariantCulture, value, typeof(T)), nameof(value));
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            return value is T || (value == null && (!typeof(T).IsValueType));
        }

        public object UnderlyingCollection
        {
            get
            {
                if (_genericCollection != null)
                {
                    return _genericCollection;
                }
                else
                {
                    return _list;
                }
            }
        }
    }
}