//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.Query
{
    public class ColumnEvaluator
    {
        private Type DataType;
        private Comparer compareType;
        private IComparer<byte[]> byteCompare;
        private byte[] ValueBytes;

        private int columnLength; // the actually column length. Maybe adjusted by different compare type.
        private int maxColumnLength;   // the setting defined column length.

        public ColumnEvaluator(Type datatype, Comparer comparertype, IComparer<byte[]> bytecompare, byte[] valuebytes, int columnlength, int maxcolumnlength)
        {
            this.DataType = datatype;
            this.compareType = comparertype;
            this.byteCompare = bytecompare;
            this.ValueBytes = valuebytes;
            this.columnLength = columnlength;
            this.maxColumnLength = maxcolumnlength;
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
            if (this.DataType == typeof(DateTime))
            {
                columnbytes = ConvertDatetimeBytes(columnbytes);
            }

            switch (compareType)
            {
                case Comparer.EqualTo:
                    return this.byteCompare.Compare(columnbytes, this.ValueBytes) == 0;

                case Comparer.GreaterThan:
                    return this.byteCompare.Compare(columnbytes, this.ValueBytes) > 0;

                case Comparer.GreaterThanOrEqual:
                    return this.byteCompare.Compare(columnbytes, this.ValueBytes) >= 0;

                case Comparer.LessThan:
                    return this.byteCompare.Compare(columnbytes, this.ValueBytes) < 0;

                case Comparer.LessThanOrEqual:
                    return this.byteCompare.Compare(columnbytes, this.ValueBytes) <= 0;

                case Comparer.NotEqualTo:

                    return !Btree.Comparer.ByteEqualComparer.isEqual(columnbytes, this.ValueBytes, columnLength);

                case Comparer.StartWith:
                    return Btree.Comparer.MoreComparer.StartWith(columnbytes, this.ValueBytes, this.columnLength);

                case Comparer.Contains:
                    return Btree.Comparer.MoreComparer.Contains(columnbytes, this.ValueBytes, this.columnLength, this.maxColumnLength);

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

        public static ColumnEvaluator GetEvaluator(Type datatype, Comparer comparetype, byte[] ValueBytes, int columnLength)
        {
            int orginalLength = columnLength;

            ///StartWith, Contains & ... are exception, not need the apppend space.
            if (comparetype == Comparer.Contains || comparetype == Comparer.StartWith)
            {
                columnLength = ValueBytes.Length;
            }

            IComparer<byte[]> compare = ObjectContainer.getComparer(datatype, columnLength);

            byte[] fixedbytes;

            if (datatype == typeof(string))
            {
                int currentbytecount = ValueBytes.Count();

                if (columnLength != int.MaxValue)
                {
                    fixedbytes = new byte[columnLength];
                }
                else
                {
                    fixedbytes = new byte[currentbytecount];
                }

                if (currentbytecount > columnLength)
                {
                    System.Buffer.BlockCopy(ValueBytes, 0, fixedbytes, 0, columnLength);
                }
                else if (currentbytecount < columnLength)
                {
                    System.Buffer.BlockCopy(ValueBytes, 0, fixedbytes, 0, currentbytecount);
                }
                else
                {
                    fixedbytes = ValueBytes;
                }
            }
            else
            {
                fixedbytes = ValueBytes;
            }

            return new ColumnEvaluator(datatype, comparetype, compare, fixedbytes, columnLength, orginalLength);
        }
    }
}