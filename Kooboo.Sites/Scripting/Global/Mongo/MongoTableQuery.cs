using System.Collections.Generic;
using System.Linq;
using KScript;
using MongoDB.Driver;

namespace KScript
{
    public class MongoTableQuery : ITableQuery
    {
        private MongoTable mongoTable;

        public MongoTableQuery(MongoTable mongoTable)
        {
            this.mongoTable = mongoTable;
        }

        public bool Ascending { get; set; }
        public string OrderByField { get; set; }
        public string SearchCondition { get; set; }
        public int skipcount { get; set; }

        public int count()
        {
            var pipline = GetPipline();
            var results = mongoTable.MongoCollection.Aggregate(pipline.Count()).ToList();
            if (results.Any()) return (int)results.First().Count;
            return 0;
        }

        private PipelineDefinition<object, object> GetPipline()
        {
            PipelineDefinition<object, object> pipline = new EmptyPipelineDefinition<object>();

            if (!string.IsNullOrWhiteSpace(SearchCondition))
            {
                pipline = pipline.Match(MongoTable.QueryToFilter(SearchCondition));
            }

            if (!string.IsNullOrWhiteSpace(OrderByField))
            {
                var sortBuilder = new SortDefinitionBuilder<object>();
                var sort = Ascending ? sortBuilder.Ascending(OrderByField) : sortBuilder.Descending(OrderByField);
                pipline = pipline.Sort(sort);
            }

            if (skipcount > 0)
            {
                pipline = pipline.Skip(skipcount);
            }

            return pipline;
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
            var pipline = GetPipline();
            var data = mongoTable.MongoCollection.Aggregate(pipline.Limit(count)).ToList();
            var list = data.Select(s => s as IDictionary<string, object>).ToArray();
            return MongoDynamicTableObject.CreateList(list, mongoTable.MongoCollection.CollectionNamespace.CollectionName);
        }

        public ITableQuery Where(string searchCondition)
        {
            this.SearchCondition = searchCondition;
            return this;
        }
    }
}