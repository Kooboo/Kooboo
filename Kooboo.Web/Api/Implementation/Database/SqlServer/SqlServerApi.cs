//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Models;
using Kooboo.Sites.Scripting.Interfaces;
using Kooboo.Web.ViewModel;
using KScript;

namespace Kooboo.Web.Api.Implementation.Database.SqlServer;

public class SqlServerApi : DatabaseApi<SqlServerCommands>
{

    public override string ModelName => "SqlServer";

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
        return new k(call.Context).SqlServer;
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
                return typeof(DateTime);
            case "string":
            default:
                return typeof(string);
        }
    }

    protected override List<List<DataValue>> ConvertDataValue(IDynamicTableObject[] data, List<DbTableColumn> columns)
    {
        return data.Select(x => x.Values.Where(v => v.Key != "RowNum")
                .Select(kv => new DataValue { key = kv.Key, value = kv.Value }).ToList())
            .ToList();
    }

    protected override string[] GetIndexColumns(IRelationalDatabase db, string table)
    {
        return db.Query($"EXEC Sp_helpindex [{table}]").Select(s => s.obj["index_keys"]).Cast<string>()
            .SelectMany(s => s.Split(',')).ToArray();
    }

    protected override void UpdateIndex(IRelationalDatabase db, string tablename, List<DbTableColumn> columns)
    {
        var cols = columns.Where(w => w.IsIndex).Select(s => s.Name);
        var tableIndexs = db.Query($"EXEC Sp_helpindex [{tablename}]");

        var removed = tableIndexs.Where(w =>
        {
            var keys = w.obj["index_keys"].ToString()?.Split(',');
            return keys.Any(a => !cols.Contains(a));
        });

        foreach (var item in removed)
        {
            try
            {
                db.Execute($"DROP INDEX [{item.obj["index_name"]}] ON [{tablename}]");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        foreach (var item in cols)
        {
            try
            {
                if (tableIndexs.Any(a => a.obj["index_keys"].ToString()!.Split(',').All(aa => aa == item))) continue;
                db.GetTable(tablename).createIndex(item);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}