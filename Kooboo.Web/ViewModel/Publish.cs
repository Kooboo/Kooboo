//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.IndexedDB;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service;

namespace Kooboo.Web.ViewModel
{
    public class ChangeItemViewModel
    {
        public Guid KeyHash { get; private set; }

        public int StoreNameHash { get; private set; }

        public string TableName { get; set; }

        public int TableNameHash { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public string ChangeType { get; set; }

        public string ObjectType { get; set; }

        public string Thumbnail { get; set; }

        public bool Selected { get; set; }

        public DateTime LastModified { get; set; }

        public string Size { get; set; }

        public long Version { get; set; }

        public byte KoobooType { get; set; }

        public Guid SyncItemId { get; set; }

        public long LogId { get; set; }

        public string User { get; set; }

        public string StoreName { get; set; }

        public Guid ObjectId { get; set; }

        public static ChangeItemViewModel FromLog(RenderContext context, LogEntry item)
        {
            var sitedb = context.WebSite.SiteDb();


            ChangeType changeType = Kooboo.ChangeType.Add;

            if (item.EditType == IndexedDB.EditType.Delete)
            {
                changeType = Kooboo.ChangeType.Delete;
            }
            else if (item.EditType == IndexedDB.EditType.Update)
            {
                changeType = Kooboo.ChangeType.Update;
            }

            var kdb = Kooboo.Data.DB.GetKDatabase(context.WebSite);

            var key = Kooboo.IndexedDB.ByteConverter.GuidConverter.ConvertFromByte(item.KeyBytes);

            if (item.IsTable)
            {
                ChangeItemViewModel viewModel = new ChangeItemViewModel()
                {
                    Id = item.Id,
                    ObjectType = Hardcoded.GetValue("Table", context),
                    StoreName = "Table",
                    Name = LogService.GetTableDisplayName(sitedb, item, context),
                    ChangeType = changeType.ToString(),
                    LastModified = item.UpdateTime,
                    LogId = item.Id,
                    User = Data.GlobalDb.Users.GetUserName(item.UserId),
                    ObjectId = key,
                    KeyHash = item.KeyHash,
                    StoreNameHash = item.StoreNameHash,
                    TableName = item.TableName,
                    TableNameHash = item.TableNameHash

                };

                return viewModel;
            }
            else
            {
                var siteObject = ObjectService.GetSiteObject(sitedb, item);
                if (siteObject == null) return null;

                ChangeItemViewModel viewModel = new ChangeItemViewModel()
                {
                    Id = item.Id,
                    ObjectType = Hardcoded.GetValue(ConstTypeService.GetModelType(siteObject.ConstType).Name, context),
                    Name = LogService.GetLogDisplayName(sitedb, item),
                    KoobooType = siteObject.ConstType,
                    ChangeType = changeType.ToString(),
                    LastModified = item.UpdateTime,
                    LogId = item.Id,
                    User = Data.GlobalDb.Users.GetUserName(item.UserId),
                    Size = Lib.Utilities.CalculateUtility.GetSizeString(ObjectService.GetSize(siteObject)),
                    StoreName = item.StoreName,
                    ObjectId = key,
                    KeyHash = item.KeyHash,
                    StoreNameHash = item.StoreNameHash

                };

                if (siteObject.ConstType == ConstObjectType.Image)
                {
                    viewModel.Thumbnail = ThumbnailService.GenerateThumbnailUrl(siteObject.Id, 50, 50, context.WebSite.Id);
                }

                return viewModel;
            }


        }

    }


    public class SyncSettingViewModel
    {
        public Guid Id { get; set; }

        // only for display purpose.... 
        public string RemoteSiteName { get; set; }

        /// <summary>
        /// The server or IP that host the remote website, plus the port if it is not port 80
        /// </summary>
        public string RemoteServerUrl { get; set; }

        [Obsolete]
        public int Difference { get; set; }
        public string ServerName { get; set; }

        public int LocalDifference { get; set; }

        public int RemoteDifference { get; set; }

    }

    public class PullResult
    {
        public bool IsFinish { get; set; }

        public long SiteLogId { get; set; }

        public long SenderVersion { get; set; }
    }

    public class PushLog
    {
        public bool IsFinish { get; set; }

        public long SiteLogId { get; set; }

        public LogEntry Log { get; set; }
    }

    public class PushFeedBack
    {
        public bool Success
        {
            get
            {
                return !this.HasConflict;
            }
        }

        public bool IsFinish { get; set; }

        public bool IsImage { get; set; }

        public long SiteLogId { get; set; }

        public long RemoteVersion { get; set; }

        public bool HasConflict { get; set; }

        public string RemoteUserName { get; set; }
        public string LocalUserName { get; set; }

        public DateTime RemoteTime { get; set; }
        public DateTime LocalTime { get; set; }

        public string RemoteBody { get; set; }

        public string LocalBody { get; set; }
    }


    public class PullFeedBack
    {
        public bool Success
        {
            get
            {
                return !this.HasConflict;
            }
        }

        public bool IsFinish { get; set; }

        public bool IsImage { get; set; }

        public long SenderVersion { get; set; }

        public long CurrentSenderVersion { get; set; }

        public long LocalLogId { get; set; }

        public bool HasConflict { get; set; }

        public string RemoteUserName { get; set; }
        public string LocalUserName { get; set; }

        public DateTime RemoteTime { get; set; }
        public DateTime LocalTime { get; set; }

        public string RemoteBody { get; set; }

        public string LocalBody { get; set; }
    }


    public class SyncProgress
    {
        public long LocalProgress
        {
            get; set;
        }

        public long LocalLastId { get; set; }


        public long RemoteProgress
        {
            get; set;

        }

        public long RemoteLastId { get; set; }
    }

}
