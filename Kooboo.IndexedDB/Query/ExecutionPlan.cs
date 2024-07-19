//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System.Collections.Generic;
using Kooboo.IndexedDB.Condition.ColumnScan;

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
            HasStartCollection = false;
            IndexRanges = new Dictionary<string, List<Range<byte[]>>>();
        }


        /// <summary>
        /// The collection that will start the search. 
        /// </summary>
        public IEnumerable<long> StartCollection { get; set; }

        /// <summary>
        /// The addition range query,that will be executed all List of Int64 out, and used for verification. 
        /// </summary>
        public Dictionary<string, List<Range<byte[]>>> IndexRanges { get; set; }

        public bool HasStartCollection { get; set; }

        /// <summary>
        /// the order by field has been settle in thhe start collection or it has to order by at the end of the result collection. 
        /// </summary>
        //\ public bool OrderBySettled { get; set; }

        public Node ColumnScanner { get; set; }

        public bool RequireOrderBy { get; set; }
    }
}