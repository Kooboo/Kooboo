using System.Linq;
using System.Text.RegularExpressions;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.Implementation.Database;

public abstract class RelationalDatabaseRawCommands
{
    public abstract char QuotationLeft { get; }

    public abstract char QuotationRight { get; }

    public abstract string ListTables();

    public abstract string IsExistTable(string table, out object param);

    public virtual string DeleteTables(string[] tables)
    {
        return string.Join("", tables.Select(table => $"DROP TABLE {Quote(table)};"));
    }

    public abstract string UpdateTable(string table, List<DbTableColumn> originalColumns, List<DbTableColumn> columns);

    public virtual string GetTotalCount(string table)
    {
        return $"SELECT COUNT(1) AS total FROM {Quote(table)};";
    }

    public abstract string GetPagedData(string table, int totalSkip, int pageSize, string sortField, bool desc);

    public virtual string DeleteData(string table, string primaryKey, List<string> ids)
    {
        var idString = string.Join("', '", ids);
        return $"DELETE FROM {Quote(table)} WHERE {Quote(primaryKey)} IN ('{idString}');";
    }

    public abstract string DbTypeToDataType(string type);

    public abstract string DbTypeToControlType(string type);

    public List<DbTableColumn> GetDefaultColumns()
    {
        return new List<DbTableColumn>
        {
            new DbTableColumn
            {
                Name = "_id",
                IsPrimaryKey = true,
                IsIndex = true,
                IsUnique = true,
                DataType = "String",
                IsSystem = true,
                Length = 64
            }
        };
    }

    protected string Quote(string name)
    {
        return $"{QuotationLeft}{name}{QuotationRight}";
    }

    protected string RemoveDbTypeLengthAndToLower(string dataType)
    {
        return Regex.Replace(dataType?.ToLower() ?? "", @"\(.+\)", "");
    }

}