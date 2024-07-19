//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.IndexedDB.Helper;


namespace Kooboo.IndexedDB.Query
{
    public class ColumnEvaluator
    {
        private Type DataType { get; set; }
        private Comparer compareType { get; set; }
        private IComparer<byte[]> byteCompare { get; set; }
        private byte[] ValueBytes { get; set; }

        private bool IsString { get; set; }

        private bool IsDateTime { get; set; }
        private string TargetString
        {
            get; set;
        }

        private DateTime TargetDate
        {
            get; set;
        }

        private int columnLength; // the actually column length. Maybe adjusted by different compare type. 
        private int maxColumnLength;   // the setting defined column length. 

        public ColumnEvaluator(Type dataType, Comparer comparerType, IComparer<byte[]> byteCompare, byte[] valueBytes, int columnLength, int maxcolumnLength)
        {
            this.DataType = dataType;
            this.compareType = comparerType;
            this.byteCompare = byteCompare;
            this.ValueBytes = valueBytes;
            this.columnLength = columnLength;
            this.maxColumnLength = maxcolumnLength;

            if (this.DataType == typeof(DateTime))
            {
                this.TargetDate = DateTimeUtcHelper.ToDateTime(this.ValueBytes);
                this.IsDateTime = true;
            }

            if (this.DataType == typeof(string))
            {
                this.TargetString = System.Text.Encoding.UTF8.GetString(this.ValueBytes);
                if (this.TargetString != null)
                {
                    this.TargetString = this.TargetString.Trim('\0');
                    this.TargetString = this.TargetString.Trim();
                }

                this.IsString = true;
            }
        }

        public ColumnEvaluator()
        {

        }

        /// <summary>
        /// test column value match the condition. 
        /// </summary>
        /// <param name="columnBytes"></param>
        /// <returns></returns>
        public virtual bool isMatch(byte[] columnBytes)
        {
            if (columnBytes == null)
            {
                return false;
            }
            if (this.IsDateTime)
            {
                var colTime = DateTimeUtcHelper.ToDateTime(columnBytes);
                return DateTimeUtcHelper.Compare(compareType, colTime, this.TargetDate);
            }
            else if (this.IsString)
            {
                var currentValue = System.Text.Encoding.UTF8.GetString(columnBytes);
                if (currentValue != null)
                {
                    currentValue = currentValue.Trim('\0');
                    currentValue = currentValue.Trim();
                }
                return Helper.StringHelper.Compare(this.compareType, currentValue, this.TargetString);
            }
            else
            {
                switch (compareType)
                {
                    case Comparer.EqualTo:
                        return this.byteCompare.Compare(columnBytes, this.ValueBytes) == 0;

                    case Comparer.GreaterThan:
                        return this.byteCompare.Compare(columnBytes, this.ValueBytes) > 0;

                    case Comparer.GreaterThanOrEqual:
                        return this.byteCompare.Compare(columnBytes, this.ValueBytes) >= 0;

                    case Comparer.LessThan:
                        return this.byteCompare.Compare(columnBytes, this.ValueBytes) < 0;

                    case Comparer.LessThanOrEqual:
                        return this.byteCompare.Compare(columnBytes, this.ValueBytes) <= 0;

                    case Comparer.NotEqualTo:

                        return !BTree.Comparer.ByteEqualComparer.isEqual(columnBytes, this.ValueBytes, columnLength);

                    case Comparer.StartWith:
                        return BTree.Comparer.MoreComparer.StartWith(columnBytes, this.ValueBytes, this.columnLength);

                    case Comparer.Contains:
                        return BTree.Comparer.MoreComparer.Contains(columnBytes, this.ValueBytes, this.columnLength, this.maxColumnLength);

                    default:
                        return false;
                }
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
