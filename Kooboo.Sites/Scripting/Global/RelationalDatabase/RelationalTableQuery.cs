using KScript;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public class RelationalTableQuery<TExecuter, TSchema> : ITableQuery where TExecuter : SqlExecuter where TSchema : RelationalSchema
    {
        public bool Ascending { get; set; }
        public string OrderByField { get; set; }
        public string SearchCondition { get; set; }
        public int skipcount { get; set; }
        public RelationalTable<TExecuter, TSchema> _table { get; set; }

        public RelationalTableQuery(RelationalTable<TExecuter, TSchema> table)
        {
            _table = table;
        }

        public int count()
        {
            return _table.Database.SqlExecuter.Count(_table.Name, SearchCondition, null, skipcount);
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

        public ITableQuery skip(int skip)
        {
            this.skipcount = skip;
            return this;
        }

        public IDynamicTableObject[] take(int count)
        {
            var desc = Ascending ? string.Empty : "DESC";
            var orderBy = OrderByField == null ? null : $"{OrderByField} {desc}";
            var data = _table.Database.SqlExecuter.QueryData(_table.Name, SearchCondition, count, skipcount, orderBy);
            return RelationalDynamicTableObject<TExecuter, TSchema>.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), _table);
        }

        public ITableQuery Where(string searchCondition)
        {
            this.SearchCondition = searchCondition;
            return this;
        }
    }
}
