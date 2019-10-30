//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Linq;

namespace Kooboo.Sites.Scripting.Global
{
    public class TableQuery
    {
        public TableQuery(KTable table)
        {
            this.ktable = table;
        }

        [Attributes.SummaryIgnore]
        public KTable ktable { get; set; }

        public int skipcount { get; set; }

        public bool Ascending { get; set; }

        public string OrderByField { get; set; }

        public string SearchCondition { get; set; }

        public TableQuery skip(int skip)
        {
            this.skipcount = skip;
            return this;
        }

        public TableQuery Where(string searchCondition)
        {
            this.SearchCondition = searchCondition;
            return this;
        }

        public TableQuery OrderBy(string fieldname)
        {
            this.OrderByField = fieldname;
            this.Ascending = true;
            return this;
        }

        public TableQuery OrderByDescending(string fieldname)
        {
            this.OrderByField = fieldname;
            this.Ascending = false;
            return this;
        }

        public DynamicTableObject[] take(int count)
        {
            var query = new IndexedDB.Dynamic.Query(this.ktable.table);

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

        public int count()
        {
            // TODO: improve performance.
            var all = take(99999);
            return all == null ? 0 : all.Count();
        }
    }
}