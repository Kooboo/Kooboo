//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.Query
{
    public class ColumnInEvaluator : ColumnEvaluator
    {
        public List<byte[]> InValues;

        public IEqualityComparer<byte[]> EqualCompare;

        public ColumnInEvaluator()
        {
        }

        /// <summary>
        /// test column value match the condition. 
        /// </summary>
        /// <param name="columnbytes"></param>
        /// <returns></returns>
        public override bool isMatch(byte[] columnbytes)
        {
            return InValues.Contains(columnbytes, EqualCompare);
        }

        public static ColumnInEvaluator GetInEvaluator(Type datatype, List<byte[]> ValueBytes, int columnLength)
        {
            IComparer<byte[]> compare = ObjectContainer.getComparer(datatype, columnLength);

            byteEquality bytecompare = new byteEquality(compare);

            ColumnInEvaluator Evaluator = new ColumnInEvaluator();
            Evaluator.EqualCompare = bytecompare;
            Evaluator.InValues = ValueBytes;
            return Evaluator;
        }

    }

    public class byteEquality : IEqualityComparer<byte[]>
    {
        public byteEquality(IComparer<byte[]> compare)
        {
            this._compare = compare;
        }

        private IComparer<byte[]> _compare;

        public bool Equals(byte[] x, byte[] y)
        {
            if (x == null || y == null)
            { return false; }

            if (x.Length != y.Length)
            {
                return false;
            }
            return _compare.Compare(x, y) == 0;
        }

        public int GetHashCode(byte[] obj)
        {
            return obj.GetHashCode();
        }
    }
}
