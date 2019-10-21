//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.Indexs
{
    public class IndexInstanceList<TValue>
    {
        public List<IIndex<TValue>> items = new List<IIndex<TValue>>();

        public void AddIndex(IIndex<TValue> index)
        {
            items.Add(index);
        }

        public void ParseSetting(Dictionary<string, int> indexs, string objectFolder, int maxCacheLevel)
        {
            if (indexs != null && indexs.Count() > 0)
            {
                foreach (var item in indexs)
                {
                    var index = GetIndexInstance(objectFolder, item.Key, item.Value, maxCacheLevel);
                    if (index != null)
                    {
                        this.AddIndex(index);
                    }
                }
            }
        }

        internal static IIndex<TValue> GetIndexInstance(string objectFolder, string fieldName, int keyLength, int maxCacheLevel)
        {
            var keyType = Helper.TypeHelper.GetFieldType<TValue>(fieldName);
            if (keyType == null)
            {
                return null;
            }

            if (keyType == typeof(Int32))
            {
                return new IndexBase<TValue, Int32>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(Int64))
            {
                return new IndexBase<TValue, Int64>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(Int16))
            {
                return new IndexBase<TValue, Int16>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(DateTime))
            {
                return new IndexBase<TValue, DateTime>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(byte))
            {
                return new IndexBase<TValue, byte>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(float))
            {
                return new IndexBase<TValue, float>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(string))
            {
                return new IndexBase<TValue, string>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(double))
            {
                return new IndexBase<TValue, double>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(Decimal))
            {
                return new DecimalIndex<TValue>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else if (keyType == typeof(Guid))
            {
                return new IndexBase<TValue, Guid>(fieldName, Helper.IndexHelper.GetIndexFileName(objectFolder, fieldName), false, keyLength, maxCacheLevel);
            }
            else
            {
                if (keyType == typeof(bool))
                {
                    throw new Exception("you better place a bool data type in column instead of index.");
                }
                else
                {
                    throw new Exception("index data type is not supported");
                }
            }
        }

        public bool Add(TValue record, Int64 blockPosition)
        {
            for (int i = 0; i < items.Count; i++)
            {
                bool ok = items[i].Add(record, blockPosition);

                if (!ok)
                {
                    // add failed, rollback, and return false.
                    for (int j = 0; j < i; j++)
                    {
                        bool delok = items[j].Del(record, blockPosition);
                        if (!delok)
                        {
                            // if del failed, which in theory should never happen, then we have real issue.
                            // should warning and fire to rebuild index.
                            // TODO: TO Be implemented.
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        public void Update(TValue oldrecord, TValue newrecord, Int64 oldblockposition, Int64 newblockposition)
        {
            // update is more complicate to roll back.
            // Rollback to be implemented.
            foreach (var item in items)
            {
                item.Update(oldrecord, newrecord, oldblockposition, newblockposition);
            }
        }

        public bool HasIndex(string fieldName)
        {
            foreach (var item in items)
            {
                if (item.FieldName.ToLower() == fieldName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public bool Del(TValue record, Int64 blockposition)
        {
            for (int i = 0; i < items.Count; i++)
            {
                bool ok = items[i].Del(record, blockposition);

                if (!ok)
                {
                    // del failed, rollback, and return false.
                    for (int j = 0; j < i; j++)
                    {
                        bool addok = items[j].Add(record, blockposition);
                        if (!addok)
                        {
                            // if del failed, which in theory should never happen, then we have real issue.
                            // should warning and fire to rebuild index.
                            // TODO: TO Be implemented.
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        public IIndex<TValue> getIndex(string fieldOrPropertyName)
        {
            foreach (var item in items)
            {
                if (item.FieldName.ToLower() == fieldOrPropertyName.ToLower())
                {
                    return item;
                }
            }
            return null;
        }

        public void CloseAll()
        {
            foreach (var item in items)
            {
                item.Close();
            }
        }

        public void FlushAll()
        {
            foreach (var item in items)
            {
                item.Flush();
            }
        }
    }
}