//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Kooboo.IndexedDB.Helper.StringHelper;

namespace Kooboo.IndexedDB.Dynamic
{
    public class SettingHelper
    {
        private static Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<Setting> converter = new Serializer.Simple.SimpleConverter<Setting>();

        public static Setting GetOrSetTableSetting(string SettingFile, Setting setting)
        {
            var old = ReadSetting(SettingFile);

            if (setting == null && old != null)
            {
                return old;
            }

            if (old != null)
            {
                if (HasNew(old, setting))
                {
                    setting = CombineSetting(old, setting);
                    var allbytes = converter.ToBytes(setting);
                    File.WriteAllBytes(SettingFile, allbytes);
                    return setting;
                }
                else
                {
                    return old;
                }
            }
            else
            {
                if (setting == null)
                {
                    setting = new Setting();
                }

                var allbytes = converter.ToBytes(setting);
                File.WriteAllBytes(SettingFile, allbytes);
                return setting;
            }
        }

        public static void WriteSetting(string SettingFile, Setting setting)
        {
            var allbytes = converter.ToBytes(setting);
            File.WriteAllBytes(SettingFile, allbytes);
        }

        public static Setting CombineSetting(Setting exists, Setting newsetting)
        {
            if (exists == null)
            {
                return newsetting;
            }

            foreach (var item in newsetting.Columns)
            {
                var find = exists.Columns.FirstOrDefault(o => o.Name.ToLower() == item.Name.ToLower());

                if (find != null)
                {
                    find.DataType = item.DataType;
                    find.Length = item.Length;
                    find.isComplex = item.isComplex;
                    // TODO: if change datatype from fixed len = 0, we need to reorganize. 
                }
                else
                {
                    var type = Helper.TypeHelper.GetType(item.DataType);
                    if (type != null)
                    {
                        exists.AppendColumn(item.Name, type, item.Length);
                    }
                }
            }


            return exists;
        }

        private static Setting ReadSetting(string settingfile)
        {
            if (File.Exists(settingfile))
            {
                var allbytes = File.ReadAllBytes(settingfile);
                return converter.FromBytes(allbytes);
            }
            return null;
        }

        public static bool RequireRebuild(Setting oldsetting, Setting newsetting)
        {
            // on the condition that column can not be append to the old setting. a rebuild is required.

            if (newsetting.Columns.Count() == 0)
            {
                return false;
            }

            // change collength.... 
            if (ChangeColLen(oldsetting, newsetting))
            {
                return true;
            }

            bool hasnewfixedcol = false;

            foreach (var item in newsetting.Columns)
            {
                if (item.Length != int.MaxValue)
                {
                    hasnewfixedcol = true;
                    break;
                }
            }

            if (!hasnewfixedcol)
            {
                // only variable col, just append to the end.
                return false;
            }

            // only when oldsetting has mixed, require rebuild. 
            return ColHasMixFixed(oldsetting);
        }

        private static bool ColHasMixFixed(Setting oldsetting)
        {
            bool hasfix = false;
            bool hasnotfix = false;
            foreach (var item in oldsetting.Columns)
            {
                if (item.Length == int.MaxValue)
                {
                    hasnotfix = true;
                }
                else
                {
                    hasfix = true;
                }

                if (hasfix && hasnotfix)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ChangeColLen(Setting oldsetting, Setting newsetting)
        {
            foreach (var item in newsetting.Columns)
            {
                var find = oldsetting.Columns.FirstOrDefault(o => o.Name.ToLower() == item.Name.ToLower());
                if (find != null)
                {
                    if (find.DataType != item.DataType || find.Length != item.Length)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // has new setting from columns that require a rebuild. 
        // if only has new index, does not 
        public static bool HasNew(Setting exists, Setting newsetting)
        {
            foreach (var item in newsetting.Columns)
            {
                var find = exists.Columns.FirstOrDefault(o => o.Name.ToLower() == item.Name.ToLower());
                if (find != null)
                {
                    if (find.DataType != item.DataType || find.Length != item.Length || find.IsIndex != item.IsIndex || find.IsPrimaryKey != item.IsPrimaryKey || find.IsUnique != item.IsUnique)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetColumnLen(Type colType, int len = 0)
        {
            if (colType == typeof(string))
            {
                if (len == 0)
                {
                    return Constants.DefaultColLen;
                }
                else
                {
                    return len;
                }
            }
            else if (colType == typeof(Int32))
            {
                return 4;
            }
            else if (colType == typeof(Int64))
            {
                return 8;
            }
            else if (colType == typeof(Int16))
            {
                return 2;
            }
            else if (colType == typeof(decimal))
            {
                ///decimal is not available, will be converted to double directly on byteconverter.
                return 8;
            }
            else if (colType == typeof(double))
            {
                return 8;
            }
            else if (colType == typeof(float))
            {
                return 4;
            }
            else if (colType == typeof(DateTime))
            {
                return 8;
            }
            else if (colType == typeof(Guid))
            {
                return 16;
            }
            else if (colType == typeof(byte))
            {
                return 1;
            }
            else if (colType == typeof(bool))
            {
                return 1;
            }
            else if (colType.IsEnum)
            {
                return 4;
            }
            else
            {
                throw new Exception(colType.Name + " data type not supported");
            }
        }

        public static int GetKeyLen(Type colType, int len = 0)
        {
            if (colType == typeof(string))
            {
                if (len == 0)
                {
                    return 256;
                }
                else
                {
                    return len;
                }
            }
            else if (colType == typeof(Int32))
            {
                return 4;
            }
            else if (colType == typeof(Int64))
            {
                return 8;
            }
            else if (colType == typeof(Int16))
            {
                return 2;
            }
            else if (colType == typeof(decimal))
            {
                ///decimal is not available, will be converted to double directly on byteconverter.
                return 8;
            }
            else if (colType == typeof(double))
            {
                return 8;
            }
            else if (colType == typeof(float))
            {
                return 4;
            }
            else if (colType == typeof(DateTime))
            {
                return 8;
            }
            else if (colType == typeof(Guid))
            {
                return 16;
            }
            else if (colType == typeof(byte))
            {
                return 1;
            }
            else if (colType == typeof(bool))
            {
                return 1;
            }
            else if (colType.IsEnum)
            {
                return 4;
            }
            else
            {
                throw new Exception(colType.Name + " data type not supported for index");
            }
        }

        public static bool IsComplexType(Type datatype)
        {
            if (datatype == typeof(Int32) || datatype == typeof(Int64) || datatype == typeof(Int16) || datatype == typeof(byte))
            {
                return false;
            }

            if (datatype == typeof(decimal) || datatype == typeof(double) || datatype == typeof(float))
            {
                return false;
            }

            if (datatype == typeof(DateTime) || datatype == typeof(Guid) || datatype == typeof(bool))
            {
                return false;
            }

            if (datatype == typeof(string))
            {
                return false;
            }
            return true; // the rest has to be consider as json data type... 
        }

        public static List<TableColumn> GetTypeColumns(object obj)
        {
            var setting = new Setting();

            if (obj is System.Collections.IDictionary)
            {
                var idict = obj as System.Collections.IDictionary;

                foreach (var item in idict.Keys)
                {
                    var value = idict[item];

                    if (value != null)
                    {
                        var valuetype = value.GetType();
                        int len = 0;
                        if (valuetype == typeof(string))
                        {
                            if (value.ToString().Length > (Constants.DefaultColLen - 300))
                            {
                                len = int.MaxValue;
                            }
                            else
                            { len = Constants.DefaultColLen; }
                        }
                        else
                        {
                            len = SettingHelper.GetColumnLen(valuetype, 0);
                        }
                        setting.AppendColumn(item.ToString(), valuetype, len);
                    }
                }
            }

            else if (obj is IDictionary<string, object>)
            {
                var idict = obj as IDictionary<string, object>;

                foreach (var item in idict)
                {
                    if (item.Value != null)
                    {
                        var valuetype = item.Value.GetType();
                        int len = 0;
                        if (valuetype == typeof(string))
                        {
                            if (item.Value.ToString().Length > (Constants.DefaultColLen - 300))
                            {
                                len = int.MaxValue;
                            }
                            else
                            { len = Constants.DefaultColLen; }
                        }
                        else
                        {
                            len = SettingHelper.GetColumnLen(valuetype, 0);
                        }
                        setting.AppendColumn(item.Key, valuetype, len);
                    }
                }
            }

            else
            {
                var type = obj.GetType();

                var AllProperties = Helper.TypeHelper.GetPublicPropertyOrFields(type);

                foreach (var item in AllProperties)
                {
                    setting.AppendColumn(item.Key, item.Value, 0);


                }
            }

            return setting.Columns.ToList();
        }



        private static bool QuickCheckChange(List<TableColumn> newcols, Setting setting)
        {
            if (newcols.Count() != setting.Columns.Count())
            {
                return true;
            }

            foreach (var item in newcols)
            {
                if (item.IsSystem)
                {
                    continue;
                }
                var find = setting.Columns.FirstOrDefault(o => o.Name == item.Name);
                if (find == null)
                {
                    return true;
                }
                else
                {
                    if ( item.DataType != find.DataType || item.IsIndex != find.IsIndex || item.IsIncremental != find.IsIncremental || item.Seed != find.Seed || item.Increment != find.Increment || item.IsUnique != find.IsUnique || item.IsPrimaryKey != find.IsPrimaryKey || item.ControlType != find.ControlType || item.Setting != find.Setting)
                    {
                        return true;
                    }

                    if (item.Length >0 && item.Length != find.Length)
                    {
                        return true; 
                    }

                }
            }

            return false;
        }

        public static CompareResult CompareSetting(object newObject, Setting setting)
        {
            var colums = GetTypeColumns(newObject);
            return CompareColSetting(colums, setting);
        }

        // use for adding data, will only increase col, does not descrease col... 

        public static CompareResult CompareColSetting(List<TableColumn> NewColumns, Setting CurrentSetting)
        {
            CompareResult result = new CompareResult();

            if (!QuickCheckChange(NewColumns, CurrentSetting))
            {
                return result;
            }

            result.NewSetting.Columns = CurrentSetting.Columns;

            foreach (var item in NewColumns)
            {
                var find = result.NewSetting.Columns.FirstOrDefault(o => o.Name == item.Name);
                if (find == null)
                {
                    result.HasChange = true;
                    result.NewSetting.AppendColumn(item.Name, item.ClrType, item.Length);
                    if (item.Name != result.NewSetting.Columns.Last().Name)
                    {
                        result.ShouldRebuild = true;
                    }
                }
                else
                {
                    if (find.DataType == typeof(DateTime).FullName)
                    {
                        if (item.DataType == typeof(double).FullName || item.DataType == typeof(decimal).FullName || item.DataType == typeof(long).FullName)
                        {
                            continue; // does not change the datatime, because JS 
                        }
                    }

                    else
                    {
                        // allow change or datatype or length..... 
                        if (item.Length > find.Length && item.Length >0 && item.Length != Constants.DefaultColLen)
                        {
                            find.Length = item.Length;
                            result.HasChange = true;
                            result.ShouldRebuild = true;
                        }
                        
                        if (item.DataType != find.DataType)
                        {
                            if(!CanConvertType(item.ClrType, find.ClrType))
                            {
                                find.DataType = item.DataType;
                                find.Length = SettingHelper.GetColumnLen(find.ClrType, find.Length);     
                                result.HasChange = true;
                                result.ShouldRebuild = true;
                            }   
                        }
                    }
                }
            }

            return result;
        }



        public static bool CanConvertType(Type input, Type exists, object data = null)
        {
            if (exists == typeof(string))
            {
                return true;
            }

            else if (input == typeof(float) || input == typeof(double) || input == typeof(decimal))
            {
                if (exists == typeof(float) || exists == typeof(double) || input == typeof(decimal))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (input == typeof(byte) || input == typeof(Int16) || input == typeof(Int32) || input == typeof(Int64))
            {
                if (exists == typeof(float) || exists == typeof(double) || input == typeof(decimal))
                {
                    return true;
                }
                else if (exists == typeof(byte) || exists == typeof(Int16) || exists == typeof(Int32) || exists == typeof(Int64))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (input == typeof(Guid))
            {
                if (exists == typeof(Guid) || exists == typeof(string))
                {
                    return true;
                }
                return false;
            }
            else if (input == typeof(bool))
            {
                if (exists == typeof(bool) || exists == typeof(string))
                {
                    return true;
                }
                return false;
            }
            else if (input == typeof(string))
            {
                if (data == null)
                {
                    return true;
                }
                else
                {
                    var rightvalue = Accessor.ChangeType(data, exists); 
                    if (rightvalue == null)
                    {
                        return false; 
                    }
                    else
                    {
                        return true; 
                    }   
                }
            }
            return false;
        }

        // user manually change the setting, remove col is allowed. 
        public static CompareResult UpdateSetting(List<TableColumn> columns, Setting setting)
        {
            CompareResult result = new CompareResult();

            string newprimaryfield = string.Empty;

            if (!QuickCheckChange(columns, setting))
            {
                return result;
            }

            result.NewSetting.Columns = Clone(setting).Columns;

            // remove cols. 
            List<TableColumn> removecol = new List<TableColumn>();

            foreach (var item in result.NewSetting.Columns)
            {
                if (item.IsSystem)
                {
                    continue;
                }
                var found = columns.Find(o => o.Name == item.Name);
                if (found == null)
                {
                    removecol.Add(item);
                }
            }

            foreach (var item in removecol)
            {
                result.NewSetting.Columns.Remove(item);
                result.HasChange = true;
                result.ShouldRebuild = true;
            }


            foreach (var item in columns)
            {
                if (item.IsSystem)
                {
                    continue;
                }
                var find = result.NewSetting.Columns.FirstOrDefault(o => o.Name == item.Name);
                if (find == null)
                {
                    result.HasChange = true;
                    result.NewSetting.AddColumn(item);

                    result.HasChange = true;

                    if (item.Name != result.NewSetting.Columns.Last().Name || item.IsIncremental)
                    {
                        result.ShouldRebuild = true;
                    }
                }
                else
                {
                    // allow change or datatype or length..... 
                    if (item.Length != find.Length && item.Length>0 && item.Length != Constants.DefaultColLen)
                    {
                        find.Length = item.Length;

                        result.ShouldRebuild = true;
                    }
                    if (item.DataType != find.DataType)
                    {
                        find.DataType = item.DataType;
                        result.ShouldRebuild = true;
                    }

                    if (!find.IsIncremental && item.IsIncremental)
                    {
                        throw new Exception("Can not change a column to incremental, you should delete and create a new column");
                    }

                    if (item.IsIncremental != find.IsIncremental || item.Seed != find.Seed || item.Increment != find.Increment)
                    {
                        find.Increment = item.Increment;
                        find.Seed = item.Seed;
                        find.IsIncremental = item.IsIncremental;
                        result.HasChange = true;
                    }

                    if (item.IsIndex != find.IsIndex || item.IsUnique != find.IsUnique || item.IsPrimaryKey != find.IsPrimaryKey)
                    {
                        find.IsIndex = item.IsIndex;
                        find.IsUnique = item.IsUnique;
                        find.IsPrimaryKey = item.IsPrimaryKey;
                        result.HasChange = true;

                        if (find.IsPrimaryKey)
                        {
                            newprimaryfield = find.Name;
                        }
                        result.ShouldRebuild = true; 
                    }

                    if (item.ControlType != find.ControlType || item.Setting != find.Setting)
                    {
                        find.Setting = item.Setting;
                        find.ControlType = item.ControlType;
                        result.HasChange = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(newprimaryfield))
            {
                foreach (var item in result.NewSetting.Columns.Where(o => o.IsPrimaryKey && o.Name != newprimaryfield))
                {
                    item.IsPrimaryKey = false;
                }
            }

            if (!result.NewSetting.Columns.Any(o => o.IsPrimaryKey))
            {
                var find = result.NewSetting.Columns.FirstOrDefault(o => o.Name == Constants.DefaultIdFieldName);
                if (find != null)
                {
                    find.IsPrimaryKey = true;
                }
            }

            return result;
        }


        public static Setting Clone(Setting input)
        {
            var convert = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<Setting>();

            var bytes = convert.ToBytes(input);
            var back = convert.FromBytes(bytes);

            return back;
        }

    }

    public class CompareResult
    {
        public bool ShouldRebuild { get; set; }

        public bool HasChange { get; set; }

        public Setting NewSetting { get; set; } = new Setting();
    }
}
