//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class DataBaseApi : IApi
    {
        public string ModelName
        {
            get { return "Database"; }
        }

        public bool RequireSite
        {
            get { return true; }
        }

        public bool RequireUser
        {
            get { return true; }
        }

        public List<string> Tables(ApiCall call)
        {
            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);
            var list = db.GetTables();

            list.RemoveAll(o => o.StartsWith("_sys_"));

            list.RemoveAll(o => o.StartsWith("_koobootemp"));

            return list;
        }

        public PagedListViewModel<List<DataValue>> Data(string table, ApiCall call)
        {
            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);
            var dbtable = Kooboo.Data.DB.GetOrCreateTable(db, table);

            string sortfield = call.GetValue("sort", "orderby", "order");
            // verify sortfield. 

            if (sortfield != null)
            {
                var col = dbtable.Setting.Columns.First(o => o.Name == sortfield);
                if (col == null)
                {
                    sortfield = null;
                }
            }

            if (sortfield == null)
            {
                var primarycol = dbtable.Setting.Columns.First(o => o.IsPrimaryKey);
                if (primarycol != null)
                {
                    sortfield = primarycol.Name;
                }
            }


            var pager = ApiHelper.GetPager(call, 30);

            PagedListViewModel<List<DataValue>> result = new PagedListViewModel<List<DataValue>>(); 

            int totalcount = (int)dbtable.length;

            result.TotalCount = totalcount;
            result.TotalPages = ApiHelper.GetPageCount(totalcount, pager.PageSize);
            result.PageNr = pager.PageNr;
            result.PageSize = pager.PageSize;

            int totalskip = 0;
            if (pager.PageNr > 1)
            {
                totalskip = (pager.PageNr - 1) * pager.PageSize;
            }

            var query = dbtable.Query;

            if (!string.IsNullOrWhiteSpace(sortfield))
            {
                query.OrderByDescending(sortfield);
            }

            var items = query.Skip(totalskip).Take(pager.PageSize).ToList();

            if (items != null && items.Count() > 0)
            {
                result.List = ConvertDataValue(items);
            }

            return result;
        }

        public List<List<DataValue>> ConvertDataValue(List<IDictionary<string, object>> input)
        {
            List<List<DataValue>> result = new List<List<DataValue>>();

            foreach (var dict in input)
            {
                List<DataValue> model = new List<DataValue>();
                foreach (var item in dict)
                {
                    model.Add(new DataValue() { key = item.Key, value = item.Value });
                }

                result.Add(model);
            }
            return result;
        }

        public class DataValue
        {
            public string key { get; set; }

            public object value { get; set; }

        }

        public void CreateTable(string name, ApiCall call)
        {
            if (!Kooboo.IndexedDB.Helper.CharHelper.IsValidTableName(name))
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Only Alphanumeric are allowed to use as a table", call.Context));
            }

            var repo = call.Context.WebSite.SiteDb().GetSiteRepository<DatabaseTableRepository>();
            repo.AddOrUpdate(new DatabaseTable() { Name = name });
            return;
        }

        public void DeleteTables(string names, ApiCall call)
        {
            List<string> ids = Lib.Helper.JsonHelper.Deserialize<List<string>>(names);
            var repo = call.Context.WebSite.SiteDb().GetSiteRepository<DatabaseTableRepository>();
            repo.DeleteTable(ids, call.Context.User.Id);
        }


        public bool IsUniqueTableName(string name, ApiCall call)
        {
            var repo = call.Context.WebSite.SiteDb().GetSiteRepository<DatabaseTableRepository>();

            return repo.isUniqueName(name);

        }

        public List<string> AvailableControlTypes(ApiCall call)
        {
            return Kooboo.Data.Definition.ControlTypes.List;
        }

        public List<DbTableColumn> Columns(string table, ApiCall call)
        {
            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);

            var dbTable = Kooboo.Data.DB.GetTable(db, table);

            List<DbTableColumn> result = new List<DbTableColumn>();

            if (dbTable == null)
            {
                return result;
            }

            foreach (var item in dbTable.Setting.Columns)
            {

                if (item.IsSystem && item.Name == IndexedDB.Dynamic.Constants.DefaultIdFieldName)
                {
                    continue;
                }
                 
                DbTableColumn model = new DbTableColumn() { Name = item.Name, IsIncremental = item.IsIncremental, IsUnique = item.IsUnique, IsIndex = item.IsIndex, IsPrimaryKey = item.IsPrimaryKey, Seed = item.Seed, Scale = item.Increment, IsSystem = item.IsSystem };

                model.DataType = DatabaseColumnHelper.ToFrontEndDataType(item.ClrType);

                model.ControlType = item.ControlType;
                model.Setting = item.Setting;

                model.Length = item.Length;

                result.Add(model);
            }
            return result;
        }

        public List<DatabaseItemEdit> GetEdit(string tablename, string Id, ApiCall call)
        {
            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);

            var dbTable = Kooboo.Data.DB.GetTable(db, tablename);

            List<DatabaseItemEdit> result = new List<DatabaseItemEdit>();

            var obj = dbTable.Get(Id);

            foreach (var item in dbTable.Setting.Columns)
            {
                DatabaseItemEdit model = new DatabaseItemEdit() { Name = item.Name, IsIncremental = item.IsIncremental, IsUnique = item.IsUnique, IsIndex = item.IsIndex, IsPrimaryKey = item.IsPrimaryKey, Seed = item.Seed, Scale = item.Increment, IsSystem = item.IsSystem };

                model.DataType = DatabaseColumnHelper.ToFrontEndDataType(item.ClrType);

                model.ControlType = item.ControlType;
                model.Setting = item.Setting;

                // get value
                if (obj != null && obj.ContainsKey(model.Name))
                {
                    model.Value = obj[model.Name];
                }

                result.Add(model);
            }
            return result;
        }


        public Guid UpdateData(string tablename, Guid id, List<DatabaseItemEdit> Values, ApiCall call)
        {
            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);

            var dbTable = Kooboo.Data.DB.GetOrCreateTable(db, tablename);
            dbTable.CurrentUserId = call.Context.User.Id;

            List<DatabaseItemEdit> result = new List<DatabaseItemEdit>();

            if (id != default(Guid))
            {
                var obj = dbTable.Get(id);
                if (obj == null)
                {
                    return default(Guid);
                }

                foreach (var item in dbTable.Setting.Columns.Where(o => !o.IsSystem))
                {
                    var value = Values.Find(o => o.Name.ToLower() == item.Name.ToLower());
                    if (value == null)
                    {
                        obj.Remove(item.Name);
                    }
                    else
                    {
                        var rightvalue = Lib.Reflection.TypeHelper.ChangeType(value.Value, item.ClrType);
                        obj[item.Name] = rightvalue;
                    }
                }
                dbTable.Update(id, obj);
            }
            else
            {
                var obj = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                foreach (var item in dbTable.Setting.Columns.Where(o => !o.IsSystem))
                {
                    if (!item.IsIncremental)
                    {
                        var value = Values.Find(o => o.Name.ToLower() == item.Name.ToLower());
                        if (value == null)
                        {
                            obj.Remove(item.Name);
                        }
                        else
                        {
                            var rightvalue = Lib.Reflection.TypeHelper.ChangeType(value.Value, item.ClrType);
                            obj[item.Name] = rightvalue;
                        }
                    }
                }
                return dbTable.Add(obj);
            }

            return default(Guid);
        }

        public void DeleteData(string tablename, List<Guid> values, ApiCall call)
        {
            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);

            var dbTable = Kooboo.Data.DB.GetTable(db, tablename);
            dbTable.CurrentUserId = call.Context.User.Id;

            foreach (var item in values)
            {
                dbTable.Delete(item);
            }
        }

        public void UpdateColumn(string tablename, List<DbTableColumn> columns, ApiCall call)
        {
            DatabaseTable table = new DatabaseTable();
            table.Name = tablename;
            table.Columns = columns;
            var sitedb = call.Context.WebSite.SiteDb();
            var repo = sitedb.GetSiteRepository<DatabaseTableRepository>();
            repo.AddOrUpdate(table, call.Context.User.Id);
        }

    }


}
