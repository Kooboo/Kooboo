//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.BTree.Comparer;
using Kooboo.IndexedDB.ByteConverter;

namespace Kooboo.IndexedDB
{
    public static class ObjectContainer
    {

        private static GuidConverter _guidConverter;
        public static GuidConverter GuidConverter
        {
            get
            {
                if (_guidConverter == null)
                {
                    _guidConverter = new GuidConverter();
                }
                return _guidConverter;
            }
        }


        private static Dictionary<Type, object> _ByteConverterList;

        private static Dictionary<Type, object> byteconverterlist()
        {
            if (_ByteConverterList == null)
            {
                _ByteConverterList = new Dictionary<Type, object>();
                _ByteConverterList.Add(typeof(bool), new BoolConverter());
                _ByteConverterList.Add(typeof(byte), new IndexedDB.ByteConverter.ByteConverter());
                _ByteConverterList.Add(typeof(byte[]), new BytesConverter());
                _ByteConverterList.Add(typeof(DateTime), new DateTimeConverter());
                _ByteConverterList.Add(typeof(decimal), new DecimalConverter());
                _ByteConverterList.Add(typeof(double), new DoubleConverter());
                _ByteConverterList.Add(typeof(float), new FloatConverter());
                _ByteConverterList.Add(typeof(Guid), new GuidConverter());
                _ByteConverterList.Add(typeof(short), new Int16Converter());
                _ByteConverterList.Add(typeof(int), new Int32Converter());
                _ByteConverterList.Add(typeof(long), new Int64Converter());
                _ByteConverterList.Add(typeof(string), new StringConverter());
            }

            return _ByteConverterList;
        }

        public static IByteConverter<T> GetConverter<T>(Dictionary<string, int> columns = null)
        {
            Dictionary<Type, object> list = byteconverterlist();

            if (list.ContainsKey(typeof(T)))
            {
                return list[typeof(T)] as IByteConverter<T>;
            }

            IByteConverter<T> converter = null;

            bool supportok = true;

            try
            {
                converter = new KoobooSimpleConverter<T>(columns);
            }
            catch (Exception)
            {
                supportok = false;
            }

            if (!supportok)
            {
                converter = new BinaryConverter<T>();
            }

            if (converter == null)
            {
                return new BinaryConverter<T>();
            }
            else
            {
                return converter;
            }


        }

        /// <summary>
        /// Get the default .NET binary converter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IByteConverter<T> GetBinaryConverter<T>()
        {
            return new BinaryConverter<T>();
        }


        private static Dictionary<Type, IComparer<byte[]>> _comparelist;

        private static Dictionary<Type, IComparer<byte[]>> comparelist
        {
            get
            {
                if (_comparelist == null)
                {
                    _comparelist = new Dictionary<Type, IComparer<byte[]>>();
                    _comparelist.Add(typeof(Int32), new IntComparer(IntType.Int32));
                    _comparelist.Add(typeof(Int64), new IntComparer(IntType.Int64));
                    _comparelist.Add(typeof(Int16), new IntComparer(IntType.Int16));
                    _comparelist.Add(typeof(decimal), new DoubleComparer());
                    _comparelist.Add(typeof(double), new DoubleComparer());
                    _comparelist.Add(typeof(float), new FloatComparer());
                    _comparelist.Add(typeof(DateTime), new DateTimeComparer());
                    _comparelist.Add(typeof(byte), new ByteComparer());
                    _comparelist.Add(typeof(Guid), new GuidComparer());
                    _comparelist.Add(typeof(byte[]), new ByteArrayComparer());
                }

                return _comparelist;
            }
        }

        public static IComparer<byte[]> getComparer(Type keytype, int keylength)
        {
            if (comparelist.ContainsKey(keytype))
            {
                return comparelist[keytype];
            }
            else
            {
                return new Kooboo.IndexedDB.BTree.Comparer.StringComparer(keylength, GlobalSettings.DefaultEncoding);
            }
        }
    }


}
