//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.Columns
{

    /// <summary>
    /// Save some extra column data of one record, in order to search into it quickly. 
    /// This is only used in the store setting, but not the real instance.
    /// </summary>
    [Serializable]
    public class ColumnSetting
    {
        /// <summary>
        /// The public field or property name.
        /// </summary>
        public string FieldName;
        [Obsolete]
        public Type KeyType;

        /// <summary>
        /// this is a public field.
        /// </summary>
        public bool isField { get; set; }

        /// <summary>
        /// this is a public property.
        /// </summary>
        public bool isProperty;

        /// <summary>
        /// The fixed. byte[] length of this column.For non-string data, the length will be automatically caculated, int32=4 bytes, datetime = 8 bytes, bool = 1 byte. For string data, you must specify a length, otherwise it can not be columnized. using -1 for uncertain length.
        /// </summary>
        public int length;

        /// <summary>
        /// The relative start position of this column within the entire columnized data bytes. 
        /// </summary>
        public int relativePosition { get; set; }
    }

}
