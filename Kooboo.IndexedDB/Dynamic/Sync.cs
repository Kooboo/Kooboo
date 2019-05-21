//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Dynamic
{
    public class Sync
    { 
        public static TableSetting GetTableSetting(Database db)
        {
            var list = db.GetTables();

            list.RemoveAll(o => o.StartsWith("_sys_"));
            list.RemoveAll(o => o.StartsWith("_koobootemp_")); 
  
            TableSetting setting = new TableSetting();

            foreach (var item in list)
            {
                var table = db.GetOrCreateTable(item);
                setting.tables.Add(item, table.Setting.Columns);
            }
            return setting;
        }

        public static void SetTableSetting(Database db, TableSetting setting)
        {
            if (setting != null && setting.tables != null && setting.tables.Count > 0)
            { 
                foreach (var item in setting.tables)
                {
                    var tablesetting = new Setting() { Columns = item.Value };

                    var cloned = Dynamic.SettingHelper.Clone(tablesetting);

                    var table = db.GetOrCreateTable(item.Key, tablesetting);
                    table.UpdateSetting(cloned); 
                } 
            }
        }

        public static List<IDictionary<string, object>> GetTableData(Database db, string tableName)
        {
            var table = db.GetOrCreateTable(tableName);
            return table.All().ToList(); 
        }

        public static List<IDictionary<string, object>> GetTableData(Database db, Table table)
        { 
            return table.All().ToList();
        }

        public static void SetTableData(Database db, string tableName, List<IDictionary<string, object>> data)
        {
            var table = db.GetOrCreateTable(tableName);
            foreach (var item in data)
            {
                table.UpdateOrAdd(item); 
            }
        }

        public static void SetTableData(Database db, Table table, List<IDictionary<string, object>> data)
        { 
            foreach (var item in data)
            {
                table.UpdateOrAdd(item);
            }
        }
    }

    public class TableData
    {
        public Guid Id { get; set; }

        public string TableName { get; set; }

        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); 
    }

    public class TableSetting
    {
        public Dictionary<string, HashSet<Kooboo.IndexedDB.Dynamic.TableColumn>> tables { get; set; } = new Dictionary<string, HashSet<TableColumn>>(StringComparer.OrdinalIgnoreCase); 
    }
}
