using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using System.Collections.Generic;
using System.Data;

namespace Kooboo.Sites.Scripting.Interfaces
{
    public interface ISqlExecuter
    {
        char QuotationLeft { get; }
        char QuotationRight { get; }
        RelationalSchema GetSchema(string name);
        void UpgradeSchema(string name, IEnumerable<RelationalSchema.Item> items);
        void CreateTable(string name);
        object Insert(string name, object data, RelationalSchema schema, bool returnId = false);
        object Append(string name, object data, RelationalSchema schema, bool returnId = false);
        void CreateIndex(string name, string fieldname);
        void Delete(string name, string primaryKey, object id);
        void UpdateData(string name, string primaryKey, object id, object data);
        RelationModel GetRelation(string name, string relation);
        object[] QueryData(string name, string where = null, long? limit = null, long? offset = null, string orderBy = null, object @params = null);
        int Count(string name, string where = null, long? limit = null, long? offset = null);
        IDbConnection CreateConnection();
    }
}