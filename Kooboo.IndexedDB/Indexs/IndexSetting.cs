//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB
{
    [Serializable]
    public class IndexSetting
    {
        /// <summary>
        /// The property or field name of your object. 
        /// </summary>
        public string FieldName;

        public bool unique;

        /// <summary>
        /// only used for string key. the other datatype has fixed key length. 
        /// </summary>
        public int keyLength;

        public Type KeyType;

    }
}
