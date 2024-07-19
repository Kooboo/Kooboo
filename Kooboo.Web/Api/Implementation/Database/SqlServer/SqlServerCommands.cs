using System.Linq;
using System.Text;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.Implementation.Database.SqlServer;

public class SqlServerCommands : RelationalDatabaseRawCommands
{
    public override char QuotationLeft => '[';

    public override char QuotationRight => ']';

    public override string ListTables()
    {
        return "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";
    }

    public override string IsExistTable(string table, out object param)
    {
        param = new { table };
        return
            "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = @table;";
    }

    public override string DbTypeToDataType(string type)
    {
        switch (RemoveDbTypeLengthAndToLower(type))
        {
            case "bigint":
            case "decimal":
            case "float":
            case "int":
            case "money":
            case "numeric":
            case "real":
            case "smallint":
            case "smallmoney":
            case "tinyint":
                return "Number";
            case "bit":
                return "Bool";
            case "date":
            case "datetime":
            case "datetime2":
            case "datetimeoffset":
            case "smalldatetime":
            case "time":
            case "timestamp":
                return "DateTime";
            case "binary":
            case "char":
            case "geography":
            case "geometry":
            case "hierarchyid":
            case "image":
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "sql_variant":
            case "sysname":
            case "text":
            case "uniqueidentifier":
            case "varbinary":
            case "varchar":
            case "xml":
                return "String";
            default:
                return "String";
        }
    }

    public override string DbTypeToControlType(string type)
    {
        switch (RemoveDbTypeLengthAndToLower(type))
        {
            case "bigint":
            case "decimal":
            case "float":
            case "int":
            case "money":
            case "numeric":
            case "real":
            case "smallint":
            case "smallmoney":
            case "tinyint":
                return "Number";
            case "bit":
                return "Boolean";
            case "date":
            case "datetime":
            case "datetime2":
            case "datetimeoffset":
            case "smalldatetime":
            case "time":
            case "timestamp":
                return "DateTime";
            case "binary":
            case "geography":
            case "geometry":
            case "hierarchyid":
            case "image":
            case "sql_variant":
            case "sysname":
            case "xml":
                return "TextArea";
            case "char":
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "uniqueidentifier":
            case "text":
            case "varbinary":
            case "varchar":
                return "TextBox";
            default:
                return "TextBox";
        }
    }


    public override string GetPagedData(string table, int totalSkip, int pageSize, string sortField, bool desc)
    {
        var descString = desc ? "DESC" : string.Empty;
        var orderByDesc = string.IsNullOrWhiteSpace(sortField) ? "" : $"ORDER BY {Quote(sortField)} {descString}";
        return "SELECT * FROM " +
               $"( SELECT ROW_NUMBER () OVER ( {orderByDesc} ) AS RowNum, * FROM {Quote(table)} ) AS RowConstrainedResult " +
               $"WHERE RowNum > {totalSkip} AND RowNum <= {totalSkip + pageSize}" +
               "ORDER BY RowNum";
    }

    public override string UpdateTable(
        string table,
        List<DbTableColumn> originalColumns,
        List<DbTableColumn> columns)
    {
        var sb = new StringBuilder();

        // add column and update index
        foreach (var column in columns)
        {
            var ori = originalColumns.FirstOrDefault(x => x.Name == column.Name);
            if (ori == null)
            {
                sb.AppendLine($"ALTER TABLE {Quote(table)} ADD {GenerateColumnDefine(column)};");
            }
            else if (ori.Length != column.Length && column.DataType.ToLower() == "string")
            {
                sb.AppendLine($"ALTER TABLE {Quote(table)} ALTER COLUMN {GenerateColumnDefine(column)};");
            }
        }

        // remove column
        foreach (var column in originalColumns)
        {
            var keep = columns.FirstOrDefault(x => x.Name == column.Name);
            if (keep != null)
            {
                continue;
            }

            sb.AppendLine($"ALTER TABLE {Quote(table)} DROP COLUMN {Quote(column.Name)};");
        }

        return sb.ToString();
    }

    private string GenerateColumnDefine(DbTableColumn column)
    {
        string dataType;
        switch (column.DataType.ToLower())
        {
            case "number":
                dataType = "float";
                break;
            case "bool":
                dataType = "bit";
                break;
            case "datetime":
                dataType = "datetime";
                break;
            case "string":
            default:
                dataType = "nvarchar";
                break;
        }

        if (dataType != "nvarchar")
        {
            return $"{Quote(column.Name)} {dataType}";
        }

        var length = column.Length > 0 && column.Length <= 4000 ? $"({column.Length})" : "(max)";
        var notNull = column.IsPrimaryKey ? " NOT NULL" : "";
        return $"{Quote(column.Name)} {dataType}{length}{notNull}";
    }
}