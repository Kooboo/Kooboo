//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Sync
{
   public class SyncObjectConvertor
    { 
        public static Sync.SyncObject ToSyncObject(ISiteObject SiteObject)
        {
            if (SiteObject == null)
            { return null;  }

            SyncObject SyncObj = new SyncObject();
            SyncObj.ObjectId = SiteObject.Id;
            SyncObj.ObjectConstType = SiteObject.ConstType;
             
            SyncObj.JsonData = Lib.Helper.JsonHelper.Serialize(SiteObject);

            return SyncObj;
        }

        public static ISiteObject FromSyncObject(SyncObject SyncObject)
        { 
           var modeltype = Service.ConstTypeService.GetModelType(SyncObject.ObjectConstType);

            var result = Lib.Helper.JsonHelper.Deserialize(SyncObject.JsonData, modeltype);

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

            if (result != null)
            {
                return result as ISiteObject;
            }
            return null; 
        }
         
    }
}
