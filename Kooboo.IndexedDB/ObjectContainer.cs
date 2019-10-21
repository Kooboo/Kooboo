//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Btree.Comparer;
using Kooboo.IndexedDB.ByteConverter;
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB
{
    public static class ObjectContainer
    {
        private static GuidConverter _guidConverter;

        public static GuidConverter GuidConverter
        {
            get { return _guidConverter ?? (_guidConverter = new GuidConverter()); }
        }

        private static Dictionary<Type, object> _byteConverterList;

        private static Dictionary<Type, object> Byteconverterlist()
        {
            return _byteConverterList ?? (_byteConverterList = new Dictionary<Type, object>
            {
                {typeof(bool), new BoolConverter()},
                {typeof(byte), new IndexedDB.ByteConverter.ByteConverter()},
                {typeof(byte[]), new BytesConverter()},
                {typeof(DateTime), new DateTimeConverter()},
                {typeof(decimal), new DecimalConverter()},
                {typeof(double), new DoubleConverter()},
                {typeof(float), new FloatConverter()},
                {typeof(Guid), new GuidConverter()},
                {typeof(short), new Int16Converter()},
                {typeof(int), new Int32Converter()},
                {typeof(long), new Int64Converter()},
                {typeof(string), new StringConverter()}
            });
        }

        public static IByteConverter<T> GetConverter<T>(Dictionary<string, int> columns = null)
        {
            Dictionary<Type, object> list = Byteconverterlist();

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

        private static Dictionary<Type, IComparer<byte[]>> Comparelist
        {
            get
            {
                if (_comparelist == null)
                {
                    _comparelist = new Dictionary<Type, IComparer<byte[]>>
                    {
                        {typeof(Int32), new IntComparer(IntType.Int32)},
                        {typeof(Int64), new IntComparer(IntType.Int64)},
                        {typeof(Int16), new IntComparer(IntType.Int16)},
                        {typeof(decimal), new DoubleComparer()},
                        {typeof(double), new DoubleComparer()},
                        {typeof(float), new FloatComparer()},
                        {typeof(DateTime), new DateTimeComparer()},
                        {typeof(byte), new ByteComparer()},
                        {typeof(Guid), new GuidComparer()}
                    };
                }

                return _comparelist;
            }
        }

        public static IComparer<byte[]> getComparer(Type keytype, int keylength)
        {
            if (Comparelist.ContainsKey(keytype))
            {
                return Comparelist[keytype];
            }
            else
            {
                return new Kooboo.IndexedDB.Btree.Comparer.StringComparer(keylength, GlobalSettings.DefaultEncoding);
            }
        }
    }
}