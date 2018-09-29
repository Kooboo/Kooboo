//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Columns
{
    public interface IColumn<TValue>
    {
        string FieldName { get; set; }
          
       
        // fieldname + len. 8 bytes. this is for the fixed len column. 
        byte[] FieldNameLengthBytes { get; set; }

        bool IsString { get; set; }

        int FieldNameHash { get; set; }

        byte[] GetBytes(TValue input);
 
        void SetBytes(TValue input, byte[] bytes);

        /// <summary>
        /// The data type, string/int/decimal, etc of this column.
        /// </summary>
        Type DataType { get; set; }

        /// <summary>
        /// the length of this field in number of bytes.
        /// </summary>
        int Length { get; set; }

        int relativePosition { get; set; }
    }
}
