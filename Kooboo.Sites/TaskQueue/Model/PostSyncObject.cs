using System;

namespace Kooboo.Sites.TaskQueue.Model
{
  public  class PostSyncObject
    {
        public string RemoteUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public Guid RemoteSiteId { get; set; }

        public Sync.SyncObject SyncObject { get; set; }
    }
}
