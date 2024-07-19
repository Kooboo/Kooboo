using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Web.Api.Implementation.Database.SchemaStore;

public class TableSchemaMappingRepository : SiteRepositoryBase<TableSchemaMapping>
{
    public override ObjectStoreParameters StoreParameters
    {
        get
        {
            var storeParameters = new ObjectStoreParameters();
            storeParameters.SetPrimaryKeyField<TableSchemaMapping>(x => x.Id);
            storeParameters.AddIndex<TableSchemaMapping>(x => x.DbType, 30);
            storeParameters.AddIndex<TableSchemaMapping>(x => x.Name, 120);
            return storeParameters;
        }
    }


    public virtual List<DbTableColumn> GetColumns(string dbType, string table)
    {
        return Get(TableSchemaMapping.GetId(dbType, table))?.Columns ?? new List<DbTableColumn>();
    }

    public virtual void AddOrUpdateSchema(string dbType, string tableName, List<DbTableColumn> columns)
    {
        var value = new TableSchemaMapping { DbType = dbType, Name = tableName, Columns = columns };
        AddOrUpdate(value);
    }

    public virtual void DeleteTableSchemas(string dbType, string[] tables)
    {
        foreach (var table in tables)
        {
            var id = new TableSchemaMapping { DbType = dbType, Name = table }.Id;
            Delete(id);
        }
    }

    public virtual List<TableSchemaMapping> SelectAll(string dbType)
    {
        return Query.Where(x => x.DbType == dbType).SelectAll();
    }
}