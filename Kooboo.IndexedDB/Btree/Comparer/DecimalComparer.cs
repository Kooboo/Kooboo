//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Btree.Comparer
{
    /// there is decimal comparer because all decimal will be converted to double. 
    /// why someone would needs a decimal as the index? 
    /// Decimal is 128bit number which is not even available in .NET BitConverter class.

    //class DecimalComparer
    //{
    //}

}
