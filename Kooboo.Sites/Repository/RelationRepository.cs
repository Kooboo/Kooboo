//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.Repository
{
    public class RelationRepository : SiteRepositoryBase<ObjectRelation>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<ObjectRelation>(o => o.ConstTypeX);
                paras.AddColumn<ObjectRelation>(o => o.ConstTypeY);
                paras.AddColumn<ObjectRelation>(o => o.RouteDestinationType);
                paras.AddIndex<ObjectRelation>(o => o.objectXId);
                paras.AddIndex<ObjectRelation>(o => o.objectYId);
                paras.AddColumn<ObjectRelation>(o => o.objectXId);
                paras.AddColumn<ObjectRelation>(o => o.objectYId);
                paras.SetPrimaryKeyField<ObjectRelation>(o => o.Id);
                return paras;
            }
        }

        /// <summary>
        /// add a new relation. will abort when the relation already exists. 
        /// </summary>
        /// <param name="objectXId"></param>
        /// <param name="objectYId"></param>
        /// <param name="ConstTypeX"></param>
        /// <param name="ConstTypeY"></param>
        /// <param name="RouteDestionType"></param>
        public void AddOrUpdate(Guid objectXId, Guid objectYId, byte ConstTypeX, byte ConstTypeY, byte RouteDestionType = 0)
        {
            ObjectRelation relation = new ObjectRelation();
            relation.objectXId = objectXId;
            relation.objectYId = objectYId;
            relation.ConstTypeX = ConstTypeX;
            relation.ConstTypeY = ConstTypeY;
            relation.RouteDestinationType = RouteDestionType;
            AddOrUpdate(relation);
        }

        /// <summary>
        /// get a list of guid that represent objects that links from current object. 
        /// </summary>
        /// <param name="objectXId"></param>
        /// <param name="RelationObjectConstType"></param>
        /// <returns></returns>
        public List<ObjectRelation> GetRelations(Guid objectXId, byte RelationObjectConstType = 0)
        {
            //TODO: if the relation is a route, should get the next destination object.. 
            if (RelationObjectConstType == 0)
            {
                return Query.Where(o => o.objectXId == objectXId).SelectAll();
            }
            else
            {
                return Query.Where(o => o.objectXId == objectXId && o.ConstTypeY == RelationObjectConstType).SelectAll();
            }
        }

        public ObjectRelation GetRelation(Guid objectXId, Guid objectYId)
        {
            Guid relationid = Kooboo.Data.IDGenerator.GetRelationId(objectXId, objectYId);
            return Get(relationid);
        }

        /// <summary>
        /// Whether current object is being used by other object or not. 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="ObjectId"></param>
        /// <returns></returns>
        public bool IsBeingUsed(Guid ObjectId)
        {
            var relations = GetReferredByRelations(ObjectId);

            return relations != null && relations.Count > 0;
        }

        /// <summary>
        /// Clean the relation of this object, including in and out relation. This usually happen when deleting an object. 
        /// </summary>
        /// <param name="ObjectId"></param>
        public void CleanObjectRelation(Guid ObjectId)
        {
            foreach (var item in GetRelations(ObjectId))
            {
                Delete(item.Id);
            }

            foreach (var item in GetReferredByRelations(ObjectId))
            {
                Delete(item.Id);
            }
        }

        /// <summary>
        /// get the relations of object x based on routing and destination object type. 
        /// this is used when one object refer to another object via routings. 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="objectXId"></param>
        /// <returns></returns>
        public List<ObjectRelation> GetRelationViaRoutes(Guid objectXId, byte constDestinationObjectType = 0)
        {
            List<ObjectRelation> relations = new List<ObjectRelation>();

            var currentrelations = GetRelations(objectXId, ConstObjectType.Route);

            foreach (var item in currentrelations)
            {
                //Route route = SiteDb.Routes.Get(item.objectYId);
                //if (route != null)
                //{
                if (constDestinationObjectType == 0 || item.RouteDestinationType == constDestinationObjectType)
                {
                    relations.Add(item);
                }
                //} 
            }
            return relations;
        }

        /// <summary>
        /// Get the relations the current object is being referenced. 
        /// </summary>
        /// <param name="objectYId"></param>
        /// <param name="RelationObjectConstTypeX"></param>
        /// <returns></returns>
        public List<ObjectRelation> GetReferredByRelations(Guid objectYId, byte RelationObjectConstTypeX = 0)
        {
            if (RelationObjectConstTypeX == 0)
            {
                return Query.Where(o => o.objectYId == objectYId).SelectAll();
            }
            else
            {
                return Query.Where(o => o.objectYId == objectYId && o.ConstTypeX == RelationObjectConstTypeX).SelectAll();
            }
        }

        private List<ObjectRelation> GetReferredByRelationViaRoutes(Guid objectYId, byte siteObjectType = 0)
        {
            List<ObjectRelation> relations = new List<ObjectRelation>();
            if (objectYId != default(Guid))
            {
                var route = SiteDb.Routes.GetByObjectId(objectYId);
                if (route != null)
                {
                    foreach (var relationitem in GetReferredByRelations(route.Id, siteObjectType))
                    {
                        if (relations.Find(o => o.Id == relationitem.Id) == null)
                        {
                            relations.Add(relationitem);
                        }
                    }
                }
            }
            return relations;
        }

        public List<ObjectRelation> GetReferredBy(SiteObject SiteObject, byte ConstTypeX = 0)
        {
            if (Attributes.AttributeHelper.IsRoutable(SiteObject))
            {
                return GetReferredByRelationViaRoutes(SiteObject.Id, ConstTypeX);
            }
            else
            {
                return GetReferredByRelations(SiteObject.Id, ConstTypeX);
            }
        }

        public List<ObjectRelation> GetReferredBy(Type siteObjectType, Guid ObjectId, byte ConstTypeX = 0)
        {
            if (Attributes.AttributeHelper.IsRoutable(siteObjectType))
            {
                return GetReferredByRelationViaRoutes(ObjectId, ConstTypeX);
            }
            else
            {
                return GetReferredByRelations(ObjectId, ConstTypeX);
            }
        }

        public List<ObjectRelation> GetExternalRelations(Guid objectXId, byte DestinationObjectType)
        {
            List<ObjectRelation> objectRelations = new List<ObjectRelation>();

            var relations = GetRelations(objectXId, ConstObjectType.ExternalResource);

            foreach (var item in relations)
            {
                if (DestinationObjectType == 0)
                {
                    objectRelations.Add(item);
                }
                else
                {
                    if (item.RouteDestinationType == DestinationObjectType)
                    {
                        objectRelations.Add(item);
                    }
                    else
                    {
                        ExternalResource resource = SiteDb.ExternalResource.Get(item.objectYId);
                        if (resource != null && resource.DestinationObjectType == DestinationObjectType)
                        {
                            objectRelations.Add(item);
                        }
                    }
                }
            }

            return objectRelations;
        }

    }
}
