//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.Dynamic
{
    public static class IndexHelper
    {
        public static List<ITableIndex> CreatIndexs(Setting Setting, string tableFolder)
        {
            List<ITableIndex> result = new List<ITableIndex>();

            foreach (var item in Setting.Columns.Where(o => o.IsIndex || o.IsPrimaryKey || o.IsSystem || o.IsIncremental || o.IsUnique))
            {
                string IndexFile = GetIndexFile(tableFolder, item.Name);

                var type = Helper.TypeHelper.GetType(item.DataType);

                if (type != null)
                {
                    var index = CreateIndex(item.Name, type, IndexFile, item.IsUnique, item.Length);
                    if (index != null)
                    {
                        index.IsPrimaryKey = item.IsPrimaryKey;
                        index.IsSystem = item.IsSystem;

                        if (item.IsIncremental)
                        {
                            index.IsIncremental = true;
                            index.Seed = item.Seed;
                            index.Increment = item.Increment;
                        }
                        result.Add(index);
                    }
                }
            }
            return result;
        }

        public static ITableIndex CreateIndex(string FieldName, Type keytype, string indexfile, bool isunique, int keylen)
        {
            // can create an generic constructor.... 
            ITableIndex index = null;

            if (keytype == typeof(string))
            {
                index = new TableIndexBase<string>(FieldName, indexfile, isunique, keylen);
            }
            else if (keytype == typeof(Int32))
            {
                index = new TableIndexBase<Int32>(FieldName, indexfile, isunique, keylen);
            }
            else if (keytype == typeof(long))
            {
                index = new TableIndexBase<long>(FieldName, indexfile, isunique, keylen);
            }

            else if (keytype == typeof(Int16))
            {
                index = new TableIndexBase<Int16>(FieldName, indexfile, isunique, keylen);
            }

            else if (keytype == typeof(byte))
            {
                index = new TableIndexBase<byte>(FieldName, indexfile, isunique, keylen);
            }
            else if (keytype == typeof(Guid))
            {
                index = new TableIndexBase<Guid>(FieldName, indexfile, isunique, keylen);
            }

            else if (keytype == typeof(float))
            {
                index = new TableIndexBase<float>(FieldName, indexfile, isunique, keylen);
            }
            else if (keytype == typeof(double))
            {
                index = new TableIndexBase<double>(FieldName, indexfile, isunique, keylen);
            }
            // TODO: add more here...  
            else
            {
                throw new Exception(keytype.FullName + " index key type not supported");
            }

            return index;
        }


        public static string GetIndexFile(string objectfolder, string fieldname)
        {
            return System.IO.Path.Combine(objectfolder, fieldname + ".index");
        }

        public static object DefaultValue(Type clrtype)
        {
            if (clrtype == typeof(string))
            {
                return "";
            }
            else if (clrtype == typeof(byte) || clrtype == typeof(int) || clrtype == typeof(Int16) || clrtype == typeof(long) || clrtype == typeof(decimal) || clrtype == typeof(double) || clrtype == typeof(float))
            {
                return Convert.ChangeType(0, clrtype);
            }
            else if (clrtype == typeof(Guid))
            {
                return default(Guid);
            }

            else if (clrtype == typeof(bool))
            {
                return false;
            }
            else if (clrtype == typeof(DateTime))
            {
                return DateTime.Now;
            }
            return null;
        }


        public static bool IsDefaultValue(object value, Type clrtype)
        {
            if (value == null)
            {
                return true;
            }

            var defaultvalue = DefaultValue(clrtype);

            if (value == defaultvalue)
            {
                return true;
            }
            return false;
        }

    }
}
