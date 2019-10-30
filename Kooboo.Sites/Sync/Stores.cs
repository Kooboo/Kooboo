//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Sync.Cluster;
using System;

namespace Kooboo.Sites.Sync
{
    public static class Stores
    {
        public static ObjectStore<Guid, ClusterNode> ClusterNodes(SiteDb siteDb)
        {
            string storename = typeof(ClusterNode).Name;

            ObjectStoreParameters paras = new ObjectStoreParameters();

            return siteDb.DatabaseDb.GetOrCreateObjectStore<Guid, ClusterNode>(storename, paras);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ObjectStore<Guid, NodeProgress> NodeProgress(SiteDb db)
        {
            string storename = typeof(NodeProgress).Name;

            ObjectStoreParameters paras = new ObjectStoreParameters();

            return db.DatabaseDb.GetOrCreateObjectStore<Guid, NodeProgress>(storename, paras);
        }

        public static ObjectStore<Guid, NodeUpdate> ClusterUpdateHistory(SiteDb db)
        {
            string storename = typeof(NodeUpdate).Name;

            ObjectStoreParameters paras = new ObjectStoreParameters();
            paras.AddIndex<NodeUpdate>(o => o.ObjectId);
            paras.AddIndex<NodeUpdate>(o => o.LocalVersion);

            return db.DatabaseDb.GetOrCreateObjectStore<Guid, NodeUpdate>(storename, paras);
        }
    }
}