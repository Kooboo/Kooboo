//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System.Linq;
using System.Text.RegularExpressions;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Models;
using Kooboo.Sites.Scripting.Interfaces;
using Kooboo.Web.ViewModel;
using KScript;

namespace Kooboo.Web.Api.Implementation.Database.Sqlite;

public class SqliteApi : DatabaseApi<SqliteCommands>
{

    private static readonly Regex ColumnNameRegex = new Regex("\"(.+?)\"", RegexOptions.Compiled);

    public override string ModelName => "Sqlite";



    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.VIEW)]
    public override List<string> Tables(ApiCall call)
    {
        return base.Tables(call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.VIEW)]
    public override List<DbTableColumn> Columns(string table, ApiCall call)
    {
        return base.Columns(table, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.VIEW)]
    public override PagedListViewModelWithPrimaryKey<List<DataValue>> Data(string table, ApiCall call)
    {
        return base.Data(table, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.EDIT)]
    public override string AddData(string tablename, List<DatabaseItemEdit> values, ApiCall call)
    {
        return base.AddData(tablename, values, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.EDIT)]
    public override void CreateTable(string name, ApiCall call)
    {
        base.CreateTable(name, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.VIEW)]
    public override List<DatabaseItemEdit> GetEdit(string tablename, string id, ApiCall call)
    {
        return base.GetEdit(tablename, id, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.DELETE)]
    public override void DeleteData(string tablename, List<string> values, ApiCall call)
    {
        base.DeleteData(tablename, values, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.DELETE)]
    public override void DeleteTables(string names, ApiCall call)
    {
        base.DeleteTables(names, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.VIEW)]
    public override void SyncSchema(ApiCall call)
    {
        base.SyncSchema(call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.EDIT)]
    public override void UpdateColumn(string tablename, List<DbTableColumn> columns, ApiCall call)
    {
        base.UpdateColumn(tablename, columns, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.VIEW)]
    public override bool IsUniqueTableName(string name, ApiCall call)
    {
        return base.IsUniqueTableName(name, call);
    }

    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.EDIT)]
    public override string UpdateData(string tablename, string id, List<DatabaseItemEdit> values, ApiCall call)
    {
        return base.UpdateData(tablename, id, values, call);
    }

    protected override IRelationalDatabase GetDatabase(ApiCall call)
    {
        return new k(call.Context).Sqlite;
    }

    protected override void UpdateTable(IRelationalDatabase db, string tablename, List<DbTableColumn> columns,
        List<DbTableColumn> originalColumns)
    {
        var sb = new System.Text.StringBuilder();
        var sqls = db.Query(Cmd.GetSql(tablename))
            .Select(x => new
            {
                Type = (string)x.Values["type"],
                Name = (string)x.Values["name"],
                Sql = (string)x.Values["sql"]
            })
            .OrderByDescending(x => x.Type)
            .ToArray();

        var oldTable = $"_old_{tablename}_{DateTime.Now:yyyyMMddHHmmss}";
        // drop old index
        foreach (var index in sqls.Where(x => x.Type == "index"))
        {
            sb.AppendLine($"DROP INDEX {index.Name};");
        }

        // rename table
        sb.AppendLine($"ALTER TABLE {tablename} RENAME TO {oldTable};");

        // create new table and index
        foreach (var sql in sqls)
        {
            if (sql.Type == "table")
            {
                sb.AppendLine(GetCreateTableSql(tablename, sql.Sql, columns));
            }
            //else if (sql.Type == "index")
            //{
            //    sb.AppendLine(GetCreateIndexSql(sql.Sql, columns));
            //}
        }

        // copy data
        var intersect = originalColumns.Select(x => x.Name).Intersect(columns.Select(x => x.Name)).ToArray();
        if (intersect.Any())
        {
            var cols = string.Join("\",\"", intersect);
            sb.AppendLine($"INSERT INTO {tablename} (\"{cols}\") SELECT \"{cols}\" FROM {oldTable};");
        }

        // drop old table
        sb.AppendLine($"DROP TABLE {oldTable};");

        db.Execute(sb.ToString());

        foreach (var item in columns.Where(w => w.IsIndex && !w.IsPrimaryKey))
        {
            db.GetTable(tablename).createIndex(item.Name);
        }
    }


    private string GetCreateTableSql(string tablename, string oldCreateTableSql, List<DbTableColumn> newColumns)
    {
        var sb = new System.Text.StringBuilder();
        var sql = oldCreateTableSql.Replace("\r", "").Replace("\n", "");
        var oldColumns = new List<string>();
        string primaryKey = null;
        if (sql.Contains("_id TEXT PRIMARY KEY"))
        {
            primaryKey = $"PRIMARY KEY (\"_id\")";
        }
        else
        {
            var primaryKeyMatch = Regex.Match(sql, "(PRIMARY *KEY *\\([ \"_a-z,]+?\\))", RegexOptions.IgnoreCase);
            if (primaryKeyMatch.Success)
            {
                var columns = ColumnNameRegex.Matches(primaryKeyMatch.Groups[1].Value).Cast<Match>()
                    .Select(x => x.Value)
                    .Except(newColumns.Select(x => x.Name), StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                if (columns.Any())
                {
                    primaryKey = $"PRIMARY KEY ({string.Join(", ", columns)})";
                }
            }
            else
            {
                sql.Insert(sql.LastIndexOf(")"), ",");
            }
        }

        // add old column
        sb.AppendLine($"CREATE TABLE \"{tablename}\" (");
        foreach (var column in newColumns)
        {
            sb.AppendLine(GenerateColumnDefine(sql, column));
        }

        // add primary key
        if (primaryKey != null)
        {
            sb.AppendLine(primaryKey);
        }
        else
        {
            sb.Remove(sb.Length - Environment.NewLine.Length - 1, Environment.NewLine.Length + 1);
        }

        sb.AppendLine(");");
        return sb.ToString();
    }

    protected override List<List<DataValue>> ConvertDataValue(IDynamicTableObject[] data, List<DbTableColumn> columns)
    {
        var bools = columns.Where(c => c.DataType.ToLower() == "bool").ToArray();
        var datetimes = columns.Where(c => c.DataType.ToLower() == "datetime").ToArray();
        return data
            .Select(x => x.Values.Select(kv =>
            {
                if (kv.Value == null) return new DataValue { key = kv.Key, value = null };
                object value = null;
                if (bools.Any(b => b.Name == kv.Key))
                {
                    value = Convert.ChangeType(kv.Value, typeof(bool));
                }
                else if (datetimes.Any(d => d.Name == kv.Key))
                {
                    if (DateTime.TryParse(kv.Value.ToString(), out var time))
                    {
                        value = time;
                    }
                }

                return new DataValue { key = kv.Key, value = value ?? kv.Value };
            }).ToList())
            .ToList();
    }

    protected override Type GetClrType(DatabaseItemEdit column)
    {
        switch (column.DataType.ToLower())
        {
            case "number":
                return typeof(double);
            case "bool":
                return typeof(bool);
            case "datetime":
            case "string":
            default:
                return typeof(string);
        }
    }

    private string GenerateColumnDefine(string sql, DbTableColumn column)
    {
        string dataType;
        switch (column.DataType.ToLower())
        {
            case "number":
                dataType = "REAL";
                break;
            case "bool":
                dataType = "INTEGER";
                break;
            case "string":
            case "datetime":
            default:
                dataType = "TEXT";
                break;
        }

        var length = column.Length > 0 ? $"({column.Length})" : "";
        return $"\"{column.Name}\" {dataType}{length},";
    }

    protected override string[] GetIndexColumns(IRelationalDatabase db, string table)
    {
        var columns = new List<string>();
        var indexs = db.Query($"SELECT name from pragma_index_list('{table}')").Select(s => s.obj["name"]);

        foreach (var item in indexs)
        {
            var cols = db.Query($"SELECT name from pragma_index_info('{item}')").Select(s => s.obj["name"])
                .Cast<string>();
            columns.AddRange(cols);
        }

        return columns.ToArray();
    }

    protected override void UpdateIndex(IRelationalDatabase db, string tablename, List<DbTableColumn> columns)
    {
        // not need implement
        throw new NotImplementedException();
    }
}