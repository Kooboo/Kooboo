//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Kooboo.IndexedDB.Columns;

namespace Kooboo.IndexedDB.Helper
{
    public static class SettingHelper
    {
        public static ObjectStoreSetting GetOldSetting<TValue, TKey>(ObjectStoreParameters para)
        {
            var newsettings = new ObjectStoreSetting();
            newsettings.headerRemark = "store setting in binary form, do not modify";
            newsettings.primaryKeyType = typeof(TKey);
            newsettings.MaxCacheLevel = para.MaxCacheLevel;

            newsettings.primaryKeyLength = Helper.KeyHelper.GetKeyLen(typeof(TKey), para.PrimaryKeyLength);
            newsettings.ValueType = typeof(TValue);
            newsettings.UseDefaultNETBinaryFormater = para.UseDefaultNETBinaryFormater;
            newsettings.EnableLog = para.EnableLog;
            newsettings.EnableVersion = para.EnableVersion;

            newsettings.primaryKeyFieldName = TryGuessPrimaryKey<TValue, TKey>(para.PrimaryKey);

            newsettings.IndexList = Helper.IndexHelper.GenerateIndexsFromParameters(newsettings.ValueType, para);

            newsettings.ColumnList = Helper.SettingHelper.GetColumnSetting(newsettings.ValueType, para);

            return newsettings;
        }

        public static string TryGuessPrimaryKey<TValue, TKey>(string key = null)
        {
            var keytype = typeof(TKey);

            HashSet<string> probelist = new HashSet<string>();
            if (key != null)
            {
                probelist.Add(key);
            }
            probelist.Add("_Id");
            probelist.Add("Id");
            probelist.Add("_key");
            foreach (var item in probelist)
            {
                var fieldtype = TypeHelper.GetFieldType<TValue>(item);
                if (fieldtype != null && fieldtype == keytype)
                {
                    return item;
                }
            }
            return key;
        }

        public static List<ColumnSetting> GetColumnSetting(Type valuetype, ObjectStoreParameters parameters)
        {
            List<ColumnSetting> columnList = new List<ColumnSetting>();

            int relativePosition = 0;

            if (parameters.ColumnList.Count > 0)
            {

                foreach (var item in parameters.ColumnList)
                {
                    ColumnSetting column = new ColumnSetting();
                    column.FieldName = item.Key;

                    FieldInfo fieldinfo = valuetype.GetField(item.Key);

                    if (fieldinfo != null)
                    {
                        column.KeyType = fieldinfo.FieldType;
                        column.isField = true;
                    }
                    else
                    {
                        //try get property. 
                        PropertyInfo propertyinfo = valuetype.GetProperty(item.Key);
                        if (propertyinfo != null)
                        {
                            column.KeyType = propertyinfo.PropertyType;
                            column.isProperty = true;
                        }
                        else
                        {
                            throw new Exception("field or property does not exists " + item);
                        }
                    }

                    column.length = Helper.KeyHelper.GetKeyLen(column.KeyType, item.Value);
                    column.relativePosition = relativePosition;
                    relativePosition += column.length;
                    columnList.Add(column);
                }
            }
            return columnList;
        }

        // keep...
        public static Dictionary<string, int> GetColumns<TValue>(Dictionary<string, int> columns, string primarykey, int primarykeylen)
        {
            if (columns == null || columns.Count() == 0)
            {
                return new Dictionary<string, int>();
            }

            Dictionary<string, int> result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (columns.Count > 0)
            {
                foreach (var item in columns)
                {
                    var coltype = TypeHelper.GetFieldType<TValue>(item.Key);

                    if (coltype != null)
                    {
                        int len = KeyHelper.GetKeyLen(coltype, item.Value);
                        result[item.Key] = len;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(primarykey))
            {
                var keytype = TypeHelper.GetFieldType<TValue>(primarykey);
                if (keytype != null)
                {
                    int len = KeyHelper.GetKeyLen(keytype, primarykeylen);
                    result[primarykey] = len;
                }
            }
            return result;
        }

        #region NewSettingApi

        public static StoreSetting GetOrSetSetting<TValue, TKey>(string SettingFile, ObjectStoreParameters para)
        {
            Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<StoreSetting> converter = new Serializer.Simple.SimpleConverter<StoreSetting>();

            if (File.Exists(SettingFile))
            {
                var allbytes = File.ReadAllBytes(SettingFile);
                return converter.FromBytes(allbytes);
            }
            else
            {
                var setting = GetSetting<TValue>(para);
                var allbytes = converter.ToBytes(setting);
                File.WriteAllBytes(SettingFile, allbytes);
                return setting;
            }
        }

        public static StoreSetting ReadSetting(string SettingFile)
        {
            Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<StoreSetting> converter = new Serializer.Simple.SimpleConverter<StoreSetting>();

            if (File.Exists(SettingFile))
            {
                var allbytes = File.ReadAllBytes(SettingFile);
                return converter.FromBytes(allbytes);
            }
            return null;
        }

        public static bool IsSettingChange<TValue>(string SettingFile, ObjectStoreParameters para)
        {
            if (File.Exists(SettingFile))
            {
                var allbytes = File.ReadAllBytes(SettingFile);
                Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<StoreSetting> converter = new Serializer.Simple.SimpleConverter<StoreSetting>();
                var oldsetting = converter.FromBytes(allbytes);
                var newsetting = GetSetting<TValue>(para);
                if (!IsSameSetting(oldsetting, newsetting))
                {
                    return true;
                }
            }
            return false;
        }


        internal static bool IsSameSetting(StoreSetting settings, StoreSetting newsetting)
        {
            if (settings.EnableLog != newsetting.EnableLog || settings.EnableVersion != newsetting.EnableVersion || settings.UseDefaultNETBinaryFormater != newsetting.UseDefaultNETBinaryFormater || settings.PrimaryKey != newsetting.PrimaryKey || settings.PrimaryKeyLen != newsetting.PrimaryKeyLen)
            {
                return false;
            }

            string oldindex = string.Empty;
            string newindex = string.Empty;
            if (settings.Indexs != null)
            {
                foreach (var item in settings.Indexs)
                {
                    oldindex += item.Key + item.Value.ToString();
                }
            }

            if (newsetting.Indexs != null)
            {
                foreach (var item in newsetting.Indexs)
                {
                    newindex += item.Key + item.Value.ToString();
                }
            }

            if (settings.Columns != null)
            {
                foreach (var item in settings.Columns)
                {
                    oldindex += item.Key + item.Value.ToString();
                }
            }

            if (newsetting.Columns != null)
            {
                foreach (var item in newsetting.Columns)
                {
                    newindex += item.Key + item.Value.ToString();
                }
            }

            return (oldindex == newindex);
        }

        public static void UpdateSetting(string SettingFile, StoreSetting setting)
        {
            Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<StoreSetting> converter = new Serializer.Simple.SimpleConverter<StoreSetting>();

            var allbytes = converter.ToBytes(setting);
            File.WriteAllBytes(SettingFile, allbytes);

        }

        public static StoreSetting GetSetting<TValue>(ObjectStoreParameters para)
        {
            var setting = new StoreSetting();
            setting.PrimaryKey = para.PrimaryKey;
            var keytype = Helper.TypeHelper.GetFieldType<TValue>(setting.PrimaryKey);
            if (keytype != null)
            {
                setting.PrimaryKeyLen = Helper.KeyHelper.GetKeyLen(keytype, para.PrimaryKeyLength);
            }
            else
            {
                setting.PrimaryKeyLen = para.PrimaryKeyLength;
            }

            setting.MaxCacheLevel = para.MaxCacheLevel;

            setting.ValueTypeFullName = typeof(TValue).FullName;

            setting.UseDefaultNETBinaryFormater = para.UseDefaultNETBinaryFormater;
            setting.EnableLog = para.EnableLog;
            setting.EnableVersion = para.EnableVersion;

            setting.Columns = GetColumns<TValue>(para.ColumnList, setting.PrimaryKey, setting.PrimaryKeyLen);

            setting.Indexs = para.IndexList;

            return setting;
        }

        #endregion

    }
}
