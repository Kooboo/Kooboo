using Kooboo.IndexedDB;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class DatabaseTableRepository : SiteRepositoryBase<DatabaseTable>
    {
        private Database KDB
        {
            get
            {
                return Kooboo.Data.DB.GetKDatabase(this.WebSite);
            }
        }

        public override bool AddOrUpdate(DatabaseTable value)
        {
            return AddOrUpdate(value, default(Guid));
        }

        public override bool AddOrUpdate(DatabaseTable value, Guid UserId)
        {
            VerifyData(value.Columns);

            var ok = base.AddOrUpdate(value, UserId);

            if (ok)
            {
                UpdateColumns(value.Name, value.Columns);
            }

            return ok;
        }

        internal void UpdateColumns(string tablename, List<DbTableColumn> columns)
        {
            var table = Data.DB.GetOrCreateTable(KDB, tablename);

            var setting = Lib.Serializer.Copy.DeepCopy<IndexedDB.Dynamic.Setting>(table.Setting);

            // deleted items. 
            setting.Columns.RemoveWhere(o => columns.Find(m => o.Name.ToLower() == m.Name.ToLower()) == null && o.Name != IndexedDB.Dynamic.Constants.DefaultIdFieldName);

            // update items or new added items. 

            foreach (var item in columns)
            {
                if (item.Name == Kooboo.IndexedDB.Dynamic.Constants.DefaultIdFieldName)
                {
                    continue;
                }

                var find = setting.Columns.FirstOrDefault(o => o.Name.ToLower() == item.Name.ToLower());
                if (find == null)
                {
                    Type datatype = DatabaseColumnHelper.ToClrType(item.DataType);

                    int length = 0;

                    if (datatype == typeof(string) && item.ControlType != null)
                    {
                        if (item.Length > 0)
                        {
                            length = item.Length;
                        }
                        else
                        {
                            if (item.ControlType.ToLower() != "textbox")
                            {
                                length = int.MaxValue;
                            }
                            else
                            {
                                length = 256;
                            }
                        }

                    }

                    setting.AppendColumn(item.Name, datatype, length);

                    var col = setting.Columns.FirstOrDefault(o => o.Name == item.Name);
                    col.Setting = item.Setting;
                    col.ControlType = item.ControlType;
                    col.IsIncremental = item.IsIncremental;
                    col.Seed = item.Seed;
                    col.Increment = item.Scale;
                    col.IsIndex = item.IsIndex;
                    col.IsPrimaryKey = item.IsPrimaryKey;
                    col.IsUnique = item.IsUnique;
                }
                else
                {
                    find.Setting = item.Setting;

                    // check string change the controltype from textbox to textarea. 
                    if (find.ClrType == typeof(string))
                    {
                        if (find.Length != item.Length && item.Length > 0)
                        {
                            find.Length = item.Length;
                        }
                    }

                    find.ControlType = item.ControlType;
                    find.IsIncremental = item.IsIncremental;
                    find.Seed = item.Seed;
                    find.Increment = item.Scale;
                    find.IsIndex = item.IsIndex;
                    find.IsPrimaryKey = item.IsPrimaryKey;
                    find.IsUnique = item.IsUnique;
                }
            }

            setting.EnsurePrimaryKey("");

            table.UpdateSetting(setting);

            table.Close();
        }

        public override void Delete(Guid id)
        {
            this.Delete(id, default(Guid));
        }

        public override void Delete(Guid id, Guid UserId)
        {
            var old = Get(id);

            if (old != null)
            {
                base.Delete(id, UserId);
                KDB.DeleteTable(old.Name);
            }
        }

        public void DeleteTable(List<string> nameorids, Guid userid)
        {
            if (nameorids != null && nameorids.Count() > 0)
            {
                foreach (var item in nameorids)
                {
                    DatabaseTable table = new DatabaseTable() { Name = item };

                    var old = Get(table.Id);

                    if (old != null)
                    {
                        base.Delete(old.Id, userid);
                    }
                    KDB.DeleteTable(item);
                }
            }
        }

        public bool isUniqueName(string name)
        {
            return this.Get(name) == null;
        }

        public void VerifyData(List<DbTableColumn> columns)
        {
            var finds = columns.FindAll(o => o.IsPrimaryKey || o.IsIndex);

            if (finds != null)
            {
                foreach (var item in finds)
                {
                    if (item.DataType != null && item.DataType.ToLower().Contains("date"))
                    {
                        throw new Exception(Data.Language.Hardcoded.GetValue("Does not support index on date time column"));
                    }
                }
            }



        }

    }
}
