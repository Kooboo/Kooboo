using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.Implementation.Database.Sqlite;

public class SqliteCommands : RelationalDatabaseRawCommands
{
    public override char QuotationLeft => '"';
    public override char QuotationRight => '"';

    public override string ListTables()
    {
        return "SELECT name FROM sqlite_master WHERE type='table';";
    }

    public override string IsExistTable(string table, out object param)
    {
        param = new { table };
        return $"SELECT name FROM sqlite_master WHERE type='table' and name=@table LIMIT 1";
    }

    public override string DbTypeToDataType(string type)
    {
        if (type.ToLower().StartsWith("real") || type.ToLower().StartsWith("integer"))
        {
            return "number";
        }

        return "string";
    }

    public override string DbTypeToControlType(string type)
    {
        if (type.ToLower().StartsWith("real") || type.ToLower().StartsWith("integer"))
        {
            return "Number";
        }

        return "TextBox";
    }

    public string GetSql(string tableName)
    {
        return $"SELECT type, name, sql FROM SQLite_master WHERE sql IS NOT NULL AND tbl_name = '{tableName}';";
    }


    public override string GetPagedData(string table, int totalSkip, int pageSize, string sortfield, bool desc)
    {
        var descString = desc ? "DESC" : string.Empty;
        var orderByDesc = string.IsNullOrWhiteSpace(sortfield) ? "" : $"ORDER BY {Quote(sortfield)} {descString}";
        return $"SELECT * FROM {Quote(table)} {orderByDesc} LIMIT {totalSkip},{pageSize};";
    }

    public override string UpdateTable(string table, List<DbTableColumn> originalColumns, List<DbTableColumn> columns)
    {
        // see SqliteApi.UpdateTable
        throw new NotImplementedException();
    }
}