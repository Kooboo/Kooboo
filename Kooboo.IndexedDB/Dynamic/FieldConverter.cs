//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using Kooboo.IndexedDB.Columns;

namespace Kooboo.IndexedDB.Dynamic
{
    public class FieldConverter : IColumn
    {
        public int FieldNameHash { get; set; }

        public string FieldName { get; set; }

        // int.max for non-fixed length. 
        public int Length { get; set; }

        public int RelativePosition { get; set; }

        public bool IsComplex { get; set; }

        public bool IsIncremental { get; set; }

        public Type DataType { get; set; }

        public bool SkipDefaultValue { get; set; }

        // dynamic object... 
        public Func<object, byte[]> ToBytes { get; set; }

        // for dynamic object.. 
        public Func<byte[], object> FromBytes { get; set; }
    }
}