//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.Query
{
    public class ColumnEvaluator
    {
        private Type _dataType;
        private Comparer _compareType;
        private IComparer<byte[]> _byteCompare;
        private byte[] _valueBytes;

        private int _columnLength; // the actually column length. Maybe adjusted by different compare type.
        private int _maxColumnLength;   // the setting defined column length.

        public ColumnEvaluator(Type datatype, Comparer comparertype, IComparer<byte[]> bytecompare, byte[] valuebytes, int columnlength, int maxcolumnlength)
        {
            this._dataType = datatype;
            this._compareType = comparertype;
            this._byteCompare = bytecompare;
            this._valueBytes = valuebytes;
            this._columnLength = columnlength;
            this._maxColumnLength = maxcolumnlength;
        }

        public ColumnEvaluator()
        { }

        /// <summary>
        /// test column value match the condition.
        /// </summary>
        /// <param name="columnbytes"></param>
        /// <returns></returns>
        public virtual bool isMatch(byte[] columnbytes)
        {
            if (this._dataType == typeof(DateTime))
            {
                columnbytes = ConvertDatetimeBytes(columnbytes);
            }

            switch (_compareType)
            {
                case Comparer.EqualTo:
                    return this._byteCompare.Compare(columnbytes, this._valueBytes) == 0;

                case Comparer.GreaterThan:
                    return this._byteCompare.Compare(columnbytes, this._valueBytes) > 0;

                case Comparer.GreaterThanOrEqual:
                    return this._byteCompare.Compare(columnbytes, this._valueBytes) >= 0;

                case Comparer.LessThan:
                    return this._byteCompare.Compare(columnbytes, this._valueBytes) < 0;

                case Comparer.LessThanOrEqual:
                    return this._byteCompare.Compare(columnbytes, this._valueBytes) <= 0;

                case Comparer.NotEqualTo:

                    return !Btree.Comparer.ByteEqualComparer.isEqual(columnbytes, this._valueBytes, _columnLength);

                case Comparer.StartWith:
                    return Btree.Comparer.MoreComparer.StartWith(columnbytes, this._valueBytes, this._columnLength);

                case Comparer.Contains:
                    return Btree.Comparer.MoreComparer.Contains(columnbytes, this._valueBytes, this._columnLength, this._maxColumnLength);

                default:
                    return false;
            }
        }

        /// <summary>
        /// If column's type is datetime,the block data will store datetime.ticks in byte array
        /// but the compare value(this.ValueBytes) store (datetime-DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)) in byte array
        /// so we need change datetime to the same byte array format.
        /// </summary>
        /// <param name="columnbytes"></param>
        /// <returns></returns>
        private static byte[] ConvertDatetimeBytes(byte[] columnbytes)
        {
            var datetime = Helper.DateTimeUtcHelper.ToDateTime(columnbytes);
            var converter = ObjectContainer.GetConverter<DateTime>();
            columnbytes = converter.ToByte(datetime);
            return columnbytes;
        }

        public static ColumnEvaluator GetEvaluator(Type datatype, Comparer comparetype, byte[] valueBytes, int columnLength)
        {
            int orginalLength = columnLength;

            //StartWith, Contains & ... are exception, not need the apppend space.
            if (comparetype == Comparer.Contains || comparetype == Comparer.StartWith)
            {
                columnLength = valueBytes.Length;
            }

            IComparer<byte[]> compare = ObjectContainer.getComparer(datatype, columnLength);

            byte[] fixedbytes;

            if (datatype == typeof(string))
            {
                int currentbytecount = valueBytes.Count();

                fixedbytes = columnLength != int.MaxValue ? new byte[columnLength] : new byte[currentbytecount];

                if (currentbytecount > columnLength)
                {
                    System.Buffer.BlockCopy(valueBytes, 0, fixedbytes, 0, columnLength);
                }
                else if (currentbytecount < columnLength)
                {
                    System.Buffer.BlockCopy(valueBytes, 0, fixedbytes, 0, currentbytecount);
                }
                else
                {
                    fixedbytes = valueBytes;
                }
            }
            else
            {
                fixedbytes = valueBytes;
            }

            return new ColumnEvaluator(datatype, comparetype, compare, fixedbytes, columnLength, orginalLength);
        }
    }
}