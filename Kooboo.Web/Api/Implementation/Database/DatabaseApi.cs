using System.Linq;
using Csv;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Attributes;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Scripting.Interfaces;
using Kooboo.Web.Api.Implementation.Database.SchemaStore;
using Kooboo.Web.ViewModel;
using KScript;

namespace Kooboo.Web.Api.Implementation.Database;

public abstract class DatabaseApi<TCommand> : IApi where TCommand : RelationalDatabaseRawCommands
{
    private const string DefaultIdFieldName = IndexedDB.Dynamic.Constants.DefaultIdFieldName;
    protected static readonly TCommand Cmd = Activator.CreateInstance<TCommand>();


    public virtual string ModelName { get; set; }

    public virtual bool RequireSite => true;

    public virtual bool RequireUser => false;


    public virtual List<string> Tables(ApiCall call)
    {
        var db = GetDatabase(call);
        SyncSchema(db, GetSchemaMappingRepository(call));
        return ListTables(db);
    }

    public virtual void CreateTable(string name, ApiCall call)
    {
        if (!IndexedDB.Helper.CharHelper.IsValidTableName(name))
        {
            throw new Exception(
                Kooboo.Data.Language.Hardcoded.GetValue("Only Alphanumeric are allowed to use as a table",
                    call.Context));
        }

        var db = GetDatabase(call);

        // create table and schema
        db.GetTable(name).all();
        GetSchemaMappingRepository(call).AddOrUpdateSchema(ModelName, name, Cmd.GetDefaultColumns());
    }

    public virtual void DeleteTables(string names, ApiCall call)
    {
        var db = GetDatabase(call);
        var tables = JsonHelper.Deserialize<string[]>(names);
        if (tables.Length <= 0)
        {
            return;
        }

        db.Execute(Cmd.DeleteTables(tables));
        GetSchemaMappingRepository(call).DeleteTableSchemas(ModelName, tables);
    }

    public virtual bool IsUniqueTableName(string name, ApiCall call)
    {
        var db = GetDatabase(call);
        return !IsExistTable(db, name);
    }

    public virtual List<DbTableColumn> Columns(string table, ApiCall call)
    {
        var db = GetDatabase(call);
        var schemaRepository = GetSchemaMappingRepository(call);
        SyncSchema(db, schemaRepository);
        var columns = schemaRepository.GetColumns(ModelName, table);
        return columns.Where(x => x.Name != DefaultIdFieldName).ToList();
    }

    public virtual void UpdateColumn(string tablename, List<DbTableColumn> columns, ApiCall call)
    {
        var db = GetDatabase(call);
        var schemaRepository = GetSchemaMappingRepository(call);
        var originalColumns = schemaRepository.GetColumns(ModelName, tablename);
        // table not exists, create
        if (originalColumns.Count <= 0)
        {
            db.GetTable(tablename).all();
            originalColumns = Cmd.GetDefaultColumns();
            schemaRepository.AddOrUpdateSchema(ModelName, tablename, originalColumns);
        }

        // update table
        var defaultIdFiled = originalColumns.FirstOrDefault(c => c.Name == DefaultIdFieldName);
        if (defaultIdFiled != null && columns.All(c => c.Name != DefaultIdFieldName))
        {
            columns.Insert(0, defaultIdFiled);
        }

        CompareColumnDifferences(originalColumns, columns, out var shouldUpdateTable, out var shouldUpdateSchema);

        if (shouldUpdateTable)
        {
            UpdateTable(db, tablename, columns, originalColumns);
        }

        if (shouldUpdateSchema)
        {
            schemaRepository.AddOrUpdateSchema(ModelName, tablename, columns);
        }
    }

    public virtual PagedListViewModelWithPrimaryKey<List<DataValue>> Data(string table, ApiCall call)
    {
        var db = GetDatabase(call);
        var sortField = call.GetValue("sort", "orderby", "order");
        var desc = call.GetBoolValue("desc");
        // verify sortField. 
        var columns = GetSchemaMappingRepository(call).GetColumns(ModelName, table);
        if (sortField != null)
        {
            var col = columns.FirstOrDefault(o => o.Name == sortField);
            if (col == null)
            {
                sortField = null;
            }
        }

        if (sortField == null)
        {
            var primaryCol = columns.FirstOrDefault(o => o.IsPrimaryKey) ?? columns.FirstOrDefault();
            if (primaryCol != null)
            {
                sortField = primaryCol.Name;
            }
        }

        var pager = ApiHelper.GetPager(call, 30);

        var result = new PagedListViewModelWithPrimaryKey<List<DataValue>>(columns)
        {
        };

        var total = db.Query(Cmd.GetTotalCount(table))[0].Values.First().Value;
        var totalCount = (int)Convert.ChangeType(total, typeof(int));

        result.TotalCount = totalCount;
        result.TotalPages = ApiHelper.GetPageCount(totalCount, pager.PageSize);
        result.PageNr = pager.PageNr;
        result.PageSize = pager.PageSize;

        var totalSkip = 0;
        if (pager.PageNr > 1)
        {
            totalSkip = (pager.PageNr - 1) * pager.PageSize;
        }

        var data = db.Query(Cmd.GetPagedData(table, totalSkip, pager.PageSize, sortField, desc));

        if (data.Any())
        {

            result.List = ConvertDataValue(data, columns);
        }

        return result;
    }

    public BinaryResponse ExportData(string table, ApiCall call)
    {
        var result = Data(table, call);
        var header = result.Columns.Where(w => w.Name != "_id").Select(s => s.Name).ToArray();
        var body = result.List.Select(s => s.Where(w => w.key != "_id").Select(s =>
        {
            if (s.value is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return s.value?.ToString() ?? string.Empty;
        }).ToArray());
        var csv = CsvWriter.WriteToText(header, body);

        var response = new BinaryResponse
        {
            ContentType = "application/octet-stream"
        };

        response.Headers.Add("Content-Disposition", $"attachment;filename={table}.csv");
        response.BinaryBytes = System.Text.Encoding.UTF8.GetBytes(csv);
        return response;
    }

    [RequireModel(typeof(ImportDatabaseDataViewModel))]
    [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.EDIT)]
    public virtual void ImportData(string table, ApiCall call)
    {
        if (call.Context.Request.Model is not ImportDatabaseDataViewModel model)
        {
            throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("invalid data", call.Context));
        }
        var dbTable = GetDatabase(call).GetTable(table);
        var columns = GetAllColumnsForItemEdit(call, table);
        var validFields = model.Fields.Where(it => !string.IsNullOrEmpty(it.CsvFieldName) && columns.Any(c => c.Name == it.DbFieldName)).ToList();
        var columnsMap = columns.ToDictionary(it => it.Name, it => it);
        var toAdds = new List<List<DatabaseItemEdit>>();
        var toUpdates = new Dictionary<string, List<DatabaseItemEdit>>();
        var uniqueFields = validFields.Where(it => it.Unique).ToList();


        foreach (var row in model.Records)
        {
            var cells = new Dictionary<string, string>(row, StringComparer.OrdinalIgnoreCase);
            var values = new List<DatabaseItemEdit>();
            var isValidRow = true;
            foreach (var field in validFields)
            {
                if (!columnsMap.TryGetValue(field.DbFieldName, out var column))
                {
                    continue;
                }

                var csvValue = cells.GetValueOrDefault(field.CsvFieldName);
                if (field.Required && string.IsNullOrWhiteSpace(csvValue))
                {
                    isValidRow = false;
                    break;
                }
                var toAdd = new DatabaseItemEdit
                {
                    Name = column.Name,
                    IsIncremental = column.IsIncremental,
                    IsUnique = column.IsUnique,
                    IsIndex = column.IsIndex,
                    IsPrimaryKey = column.IsPrimaryKey,
                    Seed = column.Seed,
                    Scale = column.Scale,
                    IsSystem = column.IsSystem,
                    ControlType = column.ControlType,
                    DataType = column.DataType,
                    Value = GetFieldValue(column.DataType, csvValue),
                };
                values.Add(toAdd);
            }

            if (!isValidRow)
            {
                continue;
            }

            if (uniqueFields.Count > 0)
            {
                var criteria = new Dictionary<string, object>();
                foreach (var uf in uniqueFields)
                {
                    var searchValue = values.FirstOrDefault(it => it.Name == uf.DbFieldName && it.Value != null)?.Value;
                    if (searchValue != null)
                    {
                        criteria[uf.DbFieldName] = searchValue;
                    }
                }

                var existsId = dbTable.find(criteria)?.GetValue("_id")?.ToString();
                if (!string.IsNullOrEmpty(existsId))
                {
                    if (model.OverwriteExisting)
                    {
                        toUpdates[existsId] = values;
                    }
                    continue;
                }
            }

            toAdds.Add(values);
        }

        foreach (var toImport in toAdds)
        {
            AddData(table, toImport, call);
        }

        foreach (var item in toUpdates)
        {
            UpdateData(table, item.Key, item.Value, call);
        }
    }

    public virtual List<DatabaseItemEdit> GetEdit(string tablename, string id, ApiCall call)
    {
        var db = GetDatabase(call);
        var result = new List<DatabaseItemEdit>();
        var obj = id == Guid.Empty.ToString() ? null : db.GetTable(tablename).get(id);
        var cloumns = GetAllColumnsForItemEdit(call, tablename);

        foreach (var model in cloumns)
        {
            // get value
            if (obj != null && obj.Values.ContainsKey(model.Name))
            {
                var value = obj.Values[model.Name];
                if (value != null)
                {
                    switch (model.DataType.ToLower())
                    {
                        case "bool":
                            model.Value = Convert.ChangeType(value, typeof(bool));
                            break;
                        case "datetime" when value is not DateTime:
                            {
                                if (DateTime.TryParse(value.ToString(), out var time))
                                {
                                    model.Value = time;
                                }

                                break;
                            }
                    }

                    model.Value ??= value;
                }
            }

            result.Add(model);
        }

        return result;
    }

    public virtual string UpdateData(string tablename, string id, List<DatabaseItemEdit> values, ApiCall call)
    {
        var db = GetDatabase(call);
        var dbTable = db.GetTable(tablename);
        var columns = GetAllColumnsForItemEdit(call, tablename);

        // edit
        if (string.IsNullOrWhiteSpace(id) || id == Guid.Empty.ToString()) return AddData(dbTable, values, columns);
        var obj = dbTable.get(id)?.Values;
        if (obj == null) return "";

        foreach (var item in columns.Where(o => !o.IsSystem))
        {
            var value = values.Find(o => o.Name.ToLower() == item.Name.ToLower());
            if (value == null)
            {
                obj.Remove(item.Name);
            }
            else
            {
                obj[item.Name] = JsonHelper.Deserialize(value.RawValue, GetClrType(item));
            }
        }

        dbTable.update(id, obj);
        return id;

        // add
    }

    public virtual string AddData(string tablename, List<DatabaseItemEdit> values, ApiCall call)
    {
        var db = GetDatabase(call);
        var dbTable = db.GetTable(tablename);
        var columns = GetAllColumnsForItemEdit(call, tablename);

        return AddData(dbTable, values, columns);
    }


    public virtual void DeleteData(string tablename, List<string> values, ApiCall call)
    {
        var db = GetDatabase(call);
        var primaryKey = db.SqlExecuter.GetSchema(tablename)?.PrimaryKey ?? DefaultIdFieldName;
        db.Execute(Cmd.DeleteData(tablename, primaryKey, values));
    }

    public virtual void SyncSchema(ApiCall call)
    {
        var repo = GetSchemaMappingRepository(call);

        var db = GetDatabase(call);
        SyncSchema(db, repo);
    }

    protected virtual void UpdateTable(IRelationalDatabase db, string tablename, List<DbTableColumn> columns,
        List<DbTableColumn> originalColumns)
    {
        var sql = Cmd.UpdateTable(tablename, originalColumns, columns);
        if (!string.IsNullOrWhiteSpace(sql)) db.Execute(sql);
        UpdateIndex(db, tablename, columns);
    }

    protected virtual List<string> ListTables(IRelationalDatabase db)
    {
        var tables = db.Query(Cmd.ListTables());
        return tables.Select(x => (string)x.Values.First().Value).OrderBy(it => it).ToList();
    }

    protected virtual bool IsExistTable(IRelationalDatabase db, string name)
    {
        var exist = db.Query(Cmd.IsExistTable(name, out var param), param);
        return exist.Any(x => x.Values.Count > 0);
    }

    protected virtual List<List<DataValue>> ConvertDataValue(IDynamicTableObject[] data,
        List<DbTableColumn> columns)
    {
        return data
            .Select(x => x.Values.Select(kv => new DataValue { key = kv.Key, value = kv.Value }).ToList())
            .ToList();
    }

    protected virtual Dictionary<string, List<DbTableColumn>> SyncSchema(IRelationalDatabase db,
        TableSchemaMappingRepository schemaRepository)
    {
        var newCloumnFromDb = new Dictionary<string, List<DbTableColumn>>();
        var koobooSchemas = schemaRepository.SelectAll(ModelName).Where(x => x != null)
            .ToDictionary(x => x.Name, x => x.Columns);
        var allTables = ListTables(db);

        var deletedTables = koobooSchemas.Keys.Except(allTables).ToArray();
        if (deletedTables.Length > 0)
        {
            schemaRepository.DeleteTableSchemas(ModelName, deletedTables);
        }

        foreach (var table in allTables)
        {
            var dbSchema = db.SqlExecuter.GetSchema(table);
            koobooSchemas.TryGetValue(table, out var koobooSchema);
            var indexColumns = GetIndexColumns(db, table).Select(s => s.Trim()).ToArray();
            if (koobooSchema == null)
            {
                // add
                koobooSchema = dbSchema.Items.Select(s => new DbTableColumn
                {
                    IsSystem = s.Name == DefaultIdFieldName,
                    IsUnique = s.Name == DefaultIdFieldName,
                    IsIndex = indexColumns.Contains(s.Name),
                    Name = s.Name,
                    DataType = Cmd.DbTypeToDataType(s.Type),
                    IsPrimaryKey = s.IsPrimaryKey,
                    ControlType = Cmd.DbTypeToControlType(s.Type)
                }).ToList();

                schemaRepository.AddOrUpdateSchema(ModelName, table, koobooSchema);
                newCloumnFromDb.Add(table, koobooSchema);
            }
            else
            {
                // update
                var columns = koobooSchema.Select(x => x.Name).ToArray();
                var newSchema = JsonHelper.Deserialize<List<DbTableColumn>>(JsonHelper.Serialize(koobooSchema));

                foreach (var item in newSchema)
                {
                    item.IsIndex = indexColumns.Contains(item.Name);
                }

                // remove columns that no longer exists in db
                newSchema.RemoveAll(x => dbSchema.Items.All(c => c.Name != x.Name));
                // new columns added in db
                var dbNewColumn = dbSchema.Items.Where(x => !columns.Contains(x.Name))
                    .Select(s =>
                        new DbTableColumn
                        {
                            IsSystem = s.Name == DefaultIdFieldName,
                            IsUnique = s.Name == DefaultIdFieldName,
                            IsIndex = indexColumns.Contains(s.Name),
                            Name = s.Name,
                            DataType = Cmd.DbTypeToDataType(s.Type),
                            IsPrimaryKey = s.IsPrimaryKey,
                            ControlType = Cmd.DbTypeToControlType(s.Type)
                        })
                    .ToList();
                newSchema.AddRange(dbNewColumn);

                // upData schema
                CompareColumnDifferences(koobooSchema, newSchema, out _, out var shouldUpdateSchema);

                if (!shouldUpdateSchema) continue;
                newCloumnFromDb.Add(table, dbNewColumn);
                schemaRepository.AddOrUpdateSchema(ModelName, table, newSchema);
            }
        }

        return newCloumnFromDb;
    }

    private object GetFieldValue(string dataType, object value)
    {
        if (value == null) return null;

        switch (dataType.ToLower())
        {
            case "bool":
                return Convert.ChangeType(value, typeof(bool));
            case "datetime" when value is not DateTime:
                {
                    if (DateTime.TryParse(value.ToString(), out var time))
                    {
                        return time;
                    }

                    break;
                }
        }

        return value;
    }

    private List<DatabaseItemEdit> GetAllColumnsForItemEdit(ApiCall call, string table)
    {
        var schemaRepository = GetSchemaMappingRepository(call);

        return schemaRepository.GetColumns(ModelName, table)
            .Select(x => new DatabaseItemEdit
            {
                ControlType = x.ControlType,
                DataType = x.DataType,
                Name = x.Name,
                Setting = x.Setting,
                IsIncremental = x.IsIncremental,
                IsIndex = x.IsIndex,
                IsPrimaryKey = x.IsPrimaryKey,
                IsSystem = x.IsSystem,
                IsUnique = x.IsUnique,
                Scale = x.Scale,
                Seed = x.Seed
            })
            .ToList();
    }

    private static void CompareColumnDifferences(
        IEnumerable<DbTableColumn> originalColumns,
        IEnumerable<DbTableColumn> newColumns,
        out bool shouldUpdateTable,
        out bool shouldUpdateSchema)
    {
        shouldUpdateTable = false;
        shouldUpdateSchema = false;
        var oriCols = originalColumns.Where(x => x.Name != DefaultIdFieldName)
            .OrderBy(x => x.Name).ToArray();
        var newCols = newColumns.Where(x => x.Name != DefaultIdFieldName)
            .OrderBy(x => x.Name).ToArray();
        if (oriCols.Length != newCols.Length)
        {
            shouldUpdateTable = true;
            shouldUpdateSchema = true;
            return;
        }

        for (var i = 0; i < oriCols.Length; i++)
        {
            var oriCol = oriCols[i];
            var newCol = newCols[i];
            if (oriCol.Name != newCol.Name || oriCol.IsIndex != newCol.IsIndex || oriCol.Length != newCol.Length)
            {
                shouldUpdateTable = true;
                shouldUpdateSchema = true;
                return;
            }

            if (oriCol.ControlType != newCol.ControlType || oriCol.Setting != newCol.Setting)
            {
                shouldUpdateSchema = true;
            }
        }
    }

    protected virtual string AddData(ITable dbTable, List<DatabaseItemEdit> values, List<DatabaseItemEdit> columns)
    {
        var add = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in columns.Where(o => !o.IsSystem))
        {
            if (item.IsIncremental) continue;
            var value = values.Find(o => o.Name.ToLower() == item.Name.ToLower());
            if (value == null)
            {
                add.Remove(item.Name);
                continue;
            }

            if (value.Value == null || (item.IsPrimaryKey && value.Value is string &&
                                        string.IsNullOrWhiteSpace(value.Value.ToString())))
            {
                add[item.Name] = null;
                continue;
            }

            if (item.IsPrimaryKey && value.Value is string s && s == " ")
            {
                add[item.Name] = null;
            }
            else
            {
                add[item.Name] = JsonHelper.Deserialize(value.RawValue, GetClrType(item));
            }
        }

        return dbTable.add(add)?.ToString() ?? "";
    }

    protected virtual TableSchemaMappingRepository GetSchemaMappingRepository(ApiCall call)
    {
        return StoreService.GetMappingStore(call.WebSite);
    }

    protected abstract void UpdateIndex(IRelationalDatabase db, string tablename, List<DbTableColumn> columns);

    protected abstract IRelationalDatabase GetDatabase(ApiCall call);

    protected abstract Type GetClrType(DatabaseItemEdit column);

    protected abstract IEnumerable<string> GetIndexColumns(IRelationalDatabase db, string table);
}