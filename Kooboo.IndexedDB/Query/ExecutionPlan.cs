//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Query
{

    /// <summary>
    /// Execution plan determine how we are going to execute the query. 
    /// 
    /// </summary>
    public class ExecutionPlan
    {
        public ExecutionPlan()
        {
            this.hasStartCollection = false;
            this.indexRanges = new Dictionary<string, Range<byte[]>>();
        }


        /// <summary>
        /// The collection that will start the search. 
        /// </summary>
        public Btree.ItemCollection startCollection { get; set; }

        /// <summary>
        /// The addition range query,that will be executed all List of Int64 out, and used for verification. 
        /// </summary>
        public Dictionary<string, Range<byte[]>> indexRanges;

        public bool hasStartCollection { get; set; }

        /// <summary>
        /// the order by field has been settle in thhe start collection or it has to order by at the end of the result collection. 
        /// </summary>
        //\ public bool OrderBySettled { get; set; }

        public List<ColumnScan> scanColumns = new List<ColumnScan>();

        public bool RequireOrderBy { get; set; }

    }
}
