using System.Linq;
using System.Text;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.Implementation.Database.MySql;

public class MySqlCommands : RelationalDatabaseRawCommands
{
    public override char QuotationLeft => '`';
    public override char QuotationRight => '`';

    public override string ListTables()
    {
        return "SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_SCHEMA='{0}' AND TABLE_TYPE = 'BASE TABLE';";
    }

    public override string IsExistTable(string table, out object param)
    {
        param = new { table };
        return "SELECT EXISTS(SELECT 1 FROM information_schema.tables WHERE TABLE_SCHEMA='{0}' AND TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = @table);";
    }

    public override string DbTypeToDataType(string type)
    {
        switch (RemoveDbTypeLengthAndToLower(type))
        {
            case "bigint":
            case "decimal":
            case "double":
            case "float":
            case "int":
            case "integer":
            case "mediumint":
            case "numaric":
            case "real":
            case "smallint":
            case "tinyint":
                return "Number";
            case "bit":
                return "Bool";
            case "date":
            case "datetime":
            case "time":
            case "timestamp":
            case "year":
                return "DateTime";
            case "binary":
            case "blob":
            case "char":
            case "enum":
            case "geometry":
            case "geometrycollection":
            case "json":
            case "linestring":
            case "longblob":
            case "longtext":
            case "mediumblob":
            case "mediumtext":
            case "multilinestring":
            case "multipoint":
            case "multipolygon":
            case "point":
            case "polygon":
            case "set":
            case "text":
            case "tinyblob":
            case "tinytext":
            case "varbinary":
            case "varchar":
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
            case "double":
            case "float":
            case "int":
            case "integer":
            case "mediumint":
            case "numaric":
            case "real":
            case "smallint":
            case "tinyint":
                return "Number";
            case "bit":
                return "Boolean";
            case "date":
            case "datetime":
            case "time":
            case "timestamp":
            case "year":
                return "DateTime";
            case "binary":
            case "blob":
            case "enum":
            case "geometry":
            case "geometrycollection":
            case "json":
            case "linestring":
            case "longblob":
            case "mediumblob":
            case "mediumtext":
            case "multilinestring":
            case "multipoint":
            case "multipolygon":
            case "point":
            case "polygon":
            case "tinyblob":
            case "set":
            case "longtext":
            case "varbinary":
                return "TextArea";
            case "char":
            case "text":
            case "tinytext":
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
        return $"SELECT * FROM {Quote(table)} {orderByDesc} LIMIT {totalSkip},{pageSize};";
    }

    public override string UpdateTable(
        string table,
        List<DbTableColumn> originalColumns,
        List<DbTableColumn> columns)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"ALTER TABLE {Quote(table)}");
        var startLength = sb.Length;

        // add column
        foreach (var column in columns)
        {
            var ori = originalColumns.FirstOrDefault(x => x.Name == column.Name);
            if (ori == null)
            {
                sb.AppendLine($"ADD {GenerateColumnDefine(column)},");
            }
            else if (ori.Length != column.Length && column.DataType.ToLower() == "string")
            {
                sb.AppendLine($"MODIFY {GenerateColumnDefine(column)},");
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

            sb.AppendLine($"DROP COLUMN {Quote(column.Name)},");
        }

        if (sb.Length == startLength) return string.Empty;
        sb.Remove(sb.Length - Environment.NewLine.Length - 1, Environment.NewLine.Length + 1);
        sb.AppendLine(";");
        return sb.ToString();
    }

    private string GenerateColumnDefine(DbTableColumn column)
    {
        string dataType;
        switch (column.DataType.ToLower())
        {
            case "number":
                dataType = "double";
                break;
            case "bool":
                dataType = "bit";
                break;
            case "datetime":
                dataType = "datetime";
                break;
            case "string":
            default:
                dataType = "varchar";
                break;
        }

        if (dataType != "varchar")
        {
            return $"{Quote(column.Name)} {dataType}";
        }

        if (column.IsPrimaryKey && (column.Length > 512 || column.Length <= 0))
        {
            column.Length = 512;
        }

        var length = column.Length > 0 ? $"({column.Length})" : "(10240)";
        return $"{Quote(column.Name)} {dataType}{length}";
    }

}