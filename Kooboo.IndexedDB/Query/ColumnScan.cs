//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Query
{
   public class ColumnScan
    {

       /// <summary>
       /// The field or property name of this column.
       /// </summary>
       public string ColumnName;

       /// <summary>
       /// the relative start position within the entire column data bytes. 
       /// </summary>
       public int relativeStartPosition;

       /// <summary>
       /// The byte length of this column in the column data. 
       /// </summary>
       public int length;

       public ColumnEvaluator Evaluator;

    }
}
