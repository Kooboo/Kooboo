//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class FormRepository : IEmbeddableRepositoryBase<Form>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var storeParams = new ObjectStoreParameters();
                storeParams.AddColumn<Form>(x => x.OwnerObjectId);
                storeParams.AddColumn<Form>(x => x.OwnerConstType);
                storeParams.AddColumn<Form>(x => x.KoobooId);
                storeParams.AddColumn<Form>(x => x.BodyHash);
                storeParams.AddColumn<Form>(x => x.Name);
                storeParams.SetPrimaryKeyField<Form>(o => o.Id);
                return storeParams;
            }
        }

        public List<Form> ListByObjectId(Guid objectId, byte constType = 0)
        {
            List<Form> list;
            if (constType == 0)
            {
                list = this.Query.Where(o => o.OwnerObjectId == objectId).SelectAll();
            }
            else
            {
                list = this.Query.Where(o => o.OwnerConstType == constType && o.OwnerObjectId == objectId).SelectAll();
            }
            return list;
        }

        public override List<UsedByRelation> GetUsedBy(Guid objectId)
        {
            var form = Get(objectId);
            return _GetUsedBy(form);
        }

        private List<UsedByRelation> _GetUsedBy(Form form)
        {
            List<UsedByRelation> result = new List<UsedByRelation>();
            if (form == null)
            { return result; }

            // for embedded style, this is belong to one parent object.
            if (form.IsEmbedded)
            {
                var sameform = this.GetSameEmbedded(form.BodyHash);

                foreach (var item in sameform)
                {
                    UsedByRelation relation = new UsedByRelation
                    {
                        ObjectId = item.OwnerObjectId, ConstType = item.OwnerConstType
                    };
                    Sites.Helper.RelationHelper.SetNameUrl(SiteDb, relation);
                    relation.Remark = item.KoobooOpenTag;
                    result.Add(relation);
                }

                return result;
            }

            var relations = this.SiteDb.Relations.GetReferredBy(form);

            foreach (var item in relations)
            {
                UsedByRelation relation = new UsedByRelation {ObjectId = item.objectXId, ConstType = item.ConstTypeX};
                Sites.Helper.RelationHelper.SetNameUrl(SiteDb, relation);
                result.Add(relation);
            }
            return result;
        }
    }
}