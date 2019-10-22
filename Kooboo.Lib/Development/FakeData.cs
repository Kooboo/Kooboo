//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Kooboo.Lib.Development
{
    public class FakeData
    {
        private static Random rnd = new Random();

        public static object GetFakeValue(Type fieldType)
        {
            if (fieldType.IsAbstract || fieldType == typeof(Type))
            {
                return null;
            }

            if (Lib.Reflection.TypeHelper.IsDictionary(fieldType))
            {
                return GetFakeDictionary(fieldType);
            }
            else if (Lib.Reflection.TypeHelper.IsList(fieldType))
            {
                return GetFakeList(fieldType);
            }
            else if (Lib.Reflection.TypeHelper.IsGenericCollection(fieldType))
            {
                return GetFakeCollection(fieldType);
            }
            else if (fieldType == typeof(string))
            {
                return "string_" + rnd.Next(1, 9999).ToString();
            }
            else if (fieldType == typeof(bool))
            {
                return false;
            }
            else if (fieldType == typeof(DateTime))
            {
                return DateTime.Now;
            }
            else if (fieldType == typeof(Guid))
            {
                return System.Guid.NewGuid();
            }
            else if (fieldType == typeof(byte))
            {
                var rand = rnd.Next(1, 100);
                return (byte)rand;
            }
            else if (fieldType == typeof(byte[]))
            {
                var bytes = new byte[10];
                for (int i = 0; i < 10; i++)
                {
                    bytes[i] = (byte)i;
                }
                return bytes;
            }
            else if (fieldType == typeof(decimal))
            {
                return (decimal)rnd.Next(0, 10000);
            }
            else if (fieldType == typeof(double))
            {
                return (double)rnd.Next(0, 10000);
            }
            else if (fieldType == typeof(float))
            {
                return (float)rnd.Next(0, 10000);
            }
            else if (fieldType == typeof(Int16))
            {
                return (Int16)rnd.Next(0, 9999);
            }
            else if (fieldType == typeof(int))
            {
                return rnd.Next(0, 9999);
            }
            else if (fieldType == typeof(Int64))
            {
                return (Int64)rnd.Next(0, 9999);
            }
            else if (fieldType == typeof(System.Net.IPAddress))
            {
                int value = rnd.Next(0, 99999);
                byte[] bytes = BitConverter.GetBytes(value);
                System.Net.IPAddress address = new System.Net.IPAddress(bytes);
                return address.ToString();
            }
            else if (fieldType == typeof(object))
            {
                return "string_object_" + rnd.Next(0, 999).ToString();
            }
            else if (fieldType.IsClass)
            {
                return GetFakeClass(fieldType);
            }
            else if (fieldType.IsEnum)
            {
                return Activator.CreateInstance(fieldType);
            }
            else
            {
                throw new Exception(fieldType.Name + " can not be identified.");
            }
        }

        public static object GetFakeDictionary(Type dictionaryType)
        {
            var result = Activator.CreateInstance(dictionaryType) as System.Collections.IDictionary;

            var keytype = TypeHelper.GetDictionaryKeyType(dictionaryType);
            var valuetype = TypeHelper.GetDictionaryValueType(dictionaryType);

            var key = GetFakeValue(keytype);
            var value = GetFakeValue(valuetype);

            result.Add(key, value);

            for (int i = 0; i < 100; i++)
            {
                key = GetFakeValue(keytype);
                value = GetFakeValue(valuetype);
                if (!result.Contains(key))
                {
                    result.Add(key, value);
                    return result;
                }
            }
            return result;
        }

        public static object GetFakeList(Type listType)
        {
            var list = Activator.CreateInstance(listType) as System.Collections.IList;

            var datatype = Kooboo.Lib.Reflection.TypeHelper.GetEnumberableType(listType);

            var value = GetFakeValue(datatype);

            list.Add(value);

            var another = GetFakeValue(datatype);
            list.Add(another);

            return list;
        }

        public static object GetFakeCollection(Type collectionType)
        {
            var datatype = Lib.Reflection.TypeHelper.GetEnumberableType(collectionType);

            var genericHashSet = typeof(CollectionWrapper<>).MakeGenericType(datatype);

            var originalInstance = Activator.CreateInstance(collectionType);

            var list = Activator.CreateInstance(genericHashSet, originalInstance) as System.Collections.IList;

            var value = GetFakeValue(datatype);

            list.Add(value);

            var another = GetFakeValue(datatype);
            list.Add(another);

            return list;
        }

        public static object GetFakeClass(Type classType)
        {
            var result = Activator.CreateInstance(classType);
            var allfields = TypeHelper.GetPublicMembers(classType);

            foreach (var item in allfields)
            {
                if (item is PropertyInfo)
                {
                    var property = item as PropertyInfo;
                    var ptype = property.PropertyType;

                    if (!IsSelfReference(classType, ptype))
                    {
                        if (ptype.IsClass && ptype != typeof(string))
                        {
                            if (!TypeHelper.IsGenericCollection(ptype) && !TypeHelper.IsDictionary(ptype) && !TypeHelper.IsList(ptype))
                            {
                                continue;
                            }
                        }

                        var value = GetFakeValue(property.PropertyType);
                        property.SetValue(result, value);
                    }
                }
                else if (item is FieldInfo)
                {
                    var field = item as FieldInfo;
                    var ftype = field.FieldType;

                    if (!IsSelfReference(classType, ftype))
                    {
                        if (ftype.IsClass && ftype != typeof(string))
                        {
                            if (!TypeHelper.IsGenericCollection(ftype) && !TypeHelper.IsDictionary(ftype) && !TypeHelper.IsList(ftype))
                            {
                                continue;
                            }
                        }

                        var value = GetFakeValue(field.FieldType);
                        field.SetValue(result, value);
                    }
                }
            }
            return result;
        }

        private static bool IsSelfReference(Type baseType, Type subType)
        {
            if (baseType == subType)
            {
                return true;
            }
            if (TypeHelper.IsDictionary(subType))
            {
                var keytype = TypeHelper.GetDictionaryKeyType(subType);
                var valuetype = TypeHelper.GetDictionaryValueType(subType);
                if (keytype == baseType || valuetype == baseType)
                {
                    return true;
                }
            }
            else if (TypeHelper.IsGenericCollection(subType))
            {
                var keytype = TypeHelper.GetEnumberableType(subType);
                if (keytype == baseType)
                {
                    return true;
                }
            }

            return false;
        }
    }

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
            get { return _genericCollection?.IsReadOnly ?? _list.IsFixedSize; }
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