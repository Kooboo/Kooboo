//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Dom;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class ScriptRepository : IEmbeddableRepositoryBase<Script>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters param = new ObjectStoreParameters();

                param.AddColumn<Script>(it => it.OwnerObjectId);
                param.AddColumn<Script>(it => it.OwnerConstType);
                param.AddColumn<Script>(o => o.BodyHash);
                param.AddColumn<Script>(o => o.Id);
                param.AddColumn<Script>(o => o.IsEmbedded);
                param.AddColumn("Name", 100);
                param.AddColumn<Script>(o => o.LastModified);
                param.SetPrimaryKeyField<Script>(o => o.Id);
                return param;
            }
        }

        //public override List<UsedByRelation> GetUsedBy(Guid ObjectId)
        //{
        //    var script = Get(ObjectId); 

        //    return GetUsedBy(script);
        //}
         
        //public   List<UsedByRelation> GetUsedBy(Script Script)
        //{
        //    List<UsedByRelation> result = new List<UsedByRelation>();
        //    if (Script == null)
        //    { return result; }

        //    /// for embedded style, this is belong to one parent object. 
        //    if (Script.IsEmbedded)
        //    {
        //        var samestyles = this.GetSameEmbedded(Script.BodyHash);

        //        foreach (var item in samestyles)
        //        {
        //            UsedByRelation relation = new UsedByRelation();
        //            relation.ObjectId = item.OwnerObjectId;
        //            relation.ConstType = item.OwnerConstType;
        //            Helper.RelationHelper.SetNameUrl(SiteDb, relation); 
        //            relation.Remark = item.KoobooOpenTag;
        //            result.Add(relation);
        //        }

        //        return result;
        //    }

        //    var relations = this.SiteDb.Relations.GetReferredBy(Script);

        //    foreach (var item in relations)
        //    {
        //        UsedByRelation relation = new UsedByRelation();
        //        relation.ObjectId = item.objectXId;
        //        relation.ConstType = item.ConstTypeX; 
        //        Helper.RelationHelper.SetNameUrl(this.SiteDb, relation); 
        //        result.Add(relation);
        //    }
        //    return result; 
        //}
    }
}
