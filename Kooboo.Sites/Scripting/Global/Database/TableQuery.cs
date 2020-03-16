//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using KScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KScript
{
    public class TableQuery : ITableQuery
    {
        public TableQuery(KTable table)
        {
            this.ktable = table;
        }

        [Kooboo.Attributes.SummaryIgnore]
        public KTable ktable { get; set; }

        public int skipcount { get; set; }

        public bool Ascending { get; set; }

        public string OrderByField { get; set; }

        public string SearchCondition { get; set; }

        public ITableQuery skip(int skip)
        {
            this.skipcount = skip;
            return this;
        }

        public ITableQuery Where(string searchCondition)
        {
            this.SearchCondition = searchCondition;
            return this;
        }

        public ITableQuery OrderBy(string fieldname)
        {
            this.OrderByField = fieldname;
            this.Ascending = true;
            return this;
        }

        public ITableQuery OrderByDescending(string fieldname)
        {
            this.OrderByField = fieldname;
            this.Ascending = false;
            return this;
        }

        public IDynamicTableObject[] take(int count)
        {
            var query = new Kooboo.IndexedDB.Dynamic.Query(this.ktable.table);

            if (!string.IsNullOrEmpty(this.SearchCondition))
            {
                var filter = query.ParserFilter(this.SearchCondition);
                query.items = filter;
            }
            else
            {
                //throw new Exception("You do not any search condition");
            }

            if (!string.IsNullOrEmpty(this.OrderByField))
            {
                if (this.Ascending)
                {
                    query.OrderByAscending(this.OrderByField);
                }
                else
                {
                    query.OrderByDescending(this.OrderByField);
                }
            }

            var result = query.Skip(this.skipcount).Take(count).ToArray();
            return DynamicTableObject.CreateList(result, this.ktable.table, this.ktable.context);

        }

        public IDynamicTableObject[] All()
        {
            return take(999999); 
        }

        public int count()
        {
            // TODO: improve performance.
            var all = take(99999);
            if (all == null)
            {
                return 0;
            }
            else
            {
                return all.Count();
            }
        }
    }
}