//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System.Linq;
using Dapper;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Models;
using Kooboo.Sites.Scripting.Interfaces;
using Kooboo.Web.ViewModel;
using KScript;

namespace Kooboo.Web.Api.Implementation.Database.MySql;

public class MySqlApi : DatabaseApi<MySqlCommands>
{

    public override string ModelName => "MySql";

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
        return new k(call.Context).Mysql;
    }

    protected override bool IsExistTable(IRelationalDatabase db, string name)
    {
        using var conn = db.SqlExecuter.CreateConnection();
        var cmd = Cmd.IsExistTable(name, out var param);
        var dbName = conn.Database;
        var exists = conn.ExecuteScalar<bool>(string.Format(cmd, dbName), param);
        return exists;
    }

    protected override List<string> ListTables(IRelationalDatabase db)
    {
        using var conn = db.SqlExecuter.CreateConnection();
        var cmd = Cmd.ListTables();
        var dbName = conn.Database;
        return conn.Query<string>(string.Format(cmd, dbName)).ToList();
    }

    protected override List<List<DataValue>> ConvertDataValue(IDynamicTableObject[] data,
        List<DbTableColumn> columns)
    {
        var bools = columns.Where(c => c.DataType.ToLower() == "bool").ToArray();
        return data
            .Select(x => x.Values.Select(kv =>
            {
                if (kv.Value == null) return new DataValue { key = kv.Key, value = null };
                var value = bools.Any(b => b.Name == kv.Key)
                    ? Convert.ChangeType(kv.Value, typeof(bool))
                    : kv.Value;
                return new DataValue { key = kv.Key, value = value };
            }).ToList())
            .ToList();
    }

    protected override Type GetClrType(DatabaseItemEdit column)
    {
        return column.DataType.ToLower() switch
        {
            "number" => typeof(double),
            "bool" => typeof(bool),
            "datetime" => typeof(DateTime),
            _ => typeof(string)
        };
    }

    protected override IEnumerable<string> GetIndexColumns(IRelationalDatabase db, string table)
    {
        return db.Query($"show index from `{table}`").Select(s => s.obj["Column_name"]).Cast<string>().ToArray();
    }


    protected override void UpdateIndex(IRelationalDatabase db, string tablename, List<DbTableColumn> columns)
    {
        var cols = columns.Where(w => w.IsIndex).Select(s => s.Name);
        var tableIndexs = db.Query($"show index from `{tablename}`");
        var removed = tableIndexs.Where(w => !cols.Contains(w.obj["Column_name"]));

        foreach (var item in removed)
        {
            try
            {
                db.Execute($"DROP INDEX `{item.obj["Key_name"]}` ON `{tablename}`");
            }
            catch (Exception)
            {
            }
        }

        foreach (var item in cols)
        {
            try
            {
                db.GetTable(tablename).createIndex(item);
            }
            catch (Exception)
            {
                try
                {
                    if (tableIndexs.Any(a => a.obj["Column_name"].ToString() == item)) continue;
                    db.Execute($"create fulltext index `{item}` on `{tablename}`(`{item}`)");
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}