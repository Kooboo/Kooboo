using System.Linq;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.Implementation.Database.SchemaStore;

public class TableSchemaMapping : CoreObject
{
    public TableSchemaMapping()
    {
        ConstType = ConstObjectType.TableSchemaMapping;
    }

    private Guid _id;

    public override Guid Id
    {
        set => _id = value;
        get
        {
            if (_id != default(Guid)) return _id;
            if (string.IsNullOrEmpty(this.DbType) || string.IsNullOrWhiteSpace(this.Name)) return _id;
            string unique = ConstType + DbType + Name;
            _id = Data.IDGenerator.GetId(unique);
            return _id;
        }
    }

    public string DbType { get; init; }

    public override string Name { get; set; }

    public List<DbTableColumn> Columns { get; init; }

    public static Guid GetId(string dbType, string tableName)
    {
        string unique = 84.ToString() + dbType + tableName;

        return Data.IDGenerator.GetId(unique);
    }


    public override int GetHashCode()
    {
        string unique = "";
        if (Columns == null) return Lib.Security.Hash.ComputeInt(unique);
        unique = Columns.Aggregate(unique, (current, item) => current + item.GetHashCode().ToString());

        return Lib.Security.Hash.ComputeInt(unique);
    }
}