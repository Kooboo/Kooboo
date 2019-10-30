//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using System.Collections.Generic;

namespace Kooboo.Sites.Sync
{
    public class SyncObjectConvertor
    {
        public static Sync.SyncObject ToSyncObject(ISiteObject siteObject)
        {
            if (siteObject == null)
            { return null; }

            SyncObject syncObj = new SyncObject
            {
                ObjectId = siteObject.Id,
                ObjectConstType = siteObject.ConstType,
                JsonData = Lib.Helper.JsonHelper.Serialize(siteObject)
            };


            return syncObj;
        }

        public static ISiteObject FromSyncObject(SyncObject syncObject)
        {
            var modeltype = Service.ConstTypeService.GetModelType(syncObject.ObjectConstType);

            var result = Lib.Helper.JsonHelper.Deserialize(syncObject.JsonData, modeltype);

            //if (result != null && result is IBinaryFile)
            //{
            //    var file = result as IBinaryFile;

            //    byte[] contentbytes = null;

            //    if (!string.IsNullOrEmpty(SyncObject.EncodedByteString))
            //    {
            //        contentbytes = System.Convert.FromBase64String(SyncObject.EncodedByteString);
            //    }

            //    file.ContentBytes = contentbytes;
            //    return file as ISiteObject;
            //}

            return result as ISiteObject;
        }

        public static Sync.SyncObject ToTableSyncObject(string tablename, System.Guid id, string colName, Dictionary<string, object> tableData)
        {
            SyncObject syncObj = new SyncObject
            {
                TableName = tablename,
                TableColName = colName,
                ObjectId = id,
                JsonData = Lib.Helper.JsonHelper.Serialize(tableData)
            };
            return syncObj;
        }

        public static Dictionary<string, object> FromTableSyncObject(SyncObject syncObject)
        {
            return Lib.Helper.JsonHelper.Deserialize<Dictionary<string, object>>(syncObject.JsonData);
        }
    }
}