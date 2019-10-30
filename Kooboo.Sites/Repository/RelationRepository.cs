//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using System;
using System.Collections.Generic;

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
        /// <param name="constTypeX"></param>
        /// <param name="constTypeY"></param>
        /// <param name="routeDestionType"></param>
        public void AddOrUpdate(Guid objectXId, Guid objectYId, byte constTypeX, byte constTypeY, byte routeDestionType = 0)
        {
            ObjectRelation relation = new ObjectRelation
            {
                objectXId = objectXId,
                objectYId = objectYId,
                ConstTypeX = constTypeX,
                ConstTypeY = constTypeY,
                RouteDestinationType = routeDestionType
            };
            AddOrUpdate(relation);
        }

        /// <summary>
        /// get a list of guid that represent objects that links from current object.
        /// </summary>
        /// <param name="objectXId"></param>
        /// <param name="relationObjectConstType"></param>
        /// <returns></returns>
        public List<ObjectRelation> GetRelations(Guid objectXId, byte relationObjectConstType = 0)
        {
            //TODO: if the relation is a route, should get the next destination object..
            if (relationObjectConstType == 0)
            {
                return Query.Where(o => o.objectXId == objectXId).SelectAll();
            }
            else
            {
                return Query.Where(o => o.objectXId == objectXId && o.ConstTypeY == relationObjectConstType).SelectAll();
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
        /// <param name="objectId"></param>
        /// <returns></returns>
        public bool IsBeingUsed(Guid objectId)
        {
            var relations = GetReferredByRelations(objectId);

            return relations != null && relations.Count > 0;
        }

        /// <summary>
        /// Clean the relation of this object, including in and out relation. This usually happen when deleting an object.
        /// </summary>
        /// <param name="objectId"></param>
        public void CleanObjectRelation(Guid objectId)
        {
            foreach (var item in GetRelations(objectId))
            {
                Delete(item.Id);
            }

            foreach (var item in GetReferredByRelations(objectId))
            {
                Delete(item.Id);
            }
        }

        /// <summary>
        /// get the relations of object x based on routing and destination object type.
        /// this is used when one object refer to another object via routings.
        /// </summary>
        /// <param name="objectXId"></param>
        /// <param name="constDestinationObjectType"></param>
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
        /// <param name="relationObjectConstTypeX"></param>
        /// <returns></returns>
        public List<ObjectRelation> GetReferredByRelations(Guid objectYId, byte relationObjectConstTypeX = 0)
        {
            if (relationObjectConstTypeX == 0)
            {
                return Query.Where(o => o.objectYId == objectYId).SelectAll();
            }
            else
            {
                return Query.Where(o => o.objectYId == objectYId && o.ConstTypeX == relationObjectConstTypeX).SelectAll();
            }
        }

        private List<ObjectRelation> GetReferredByRelationViaRoutes(Guid objectYId, byte siteObjectType = 0)
        {
            var route = SiteDb.Routes.GetByObjectId(objectYId);
            List<ObjectRelation> relations = new List<ObjectRelation>();
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
            return relations;
        }

        public List<ObjectRelation> GetReferredBy(SiteObject siteObject, byte constTypeX = 0)
        {
            if (Attributes.AttributeHelper.IsRoutable(siteObject))
            {
                return GetReferredByRelationViaRoutes(siteObject.Id, constTypeX);
            }
            else
            {
                return GetReferredByRelations(siteObject.Id, constTypeX);
            }
        }

        public List<ObjectRelation> GetReferredBy(Type siteObjectType, Guid objectId, byte constTypeX = 0)
        {
            if (Attributes.AttributeHelper.IsRoutable(siteObjectType))
            {
                return GetReferredByRelationViaRoutes(objectId, constTypeX);
            }
            else
            {
                return GetReferredByRelations(objectId, constTypeX);
            }
        }

        public List<ObjectRelation> GetExternalRelations(Guid objectXId, byte destinationObjectType)
        {
            List<ObjectRelation> objectRelations = new List<ObjectRelation>();

            var relations = GetRelations(objectXId, ConstObjectType.ExternalResource);

            foreach (var item in relations)
            {
                if (destinationObjectType == 0)
                {
                    objectRelations.Add(item);
                }
                else
                {
                    if (item.RouteDestinationType == destinationObjectType)
                    {
                        objectRelations.Add(item);
                    }
                    else
                    {
                        ExternalResource resource = SiteDb.ExternalResource.Get(item.objectYId);
                        if (resource != null && resource.DestinationObjectType == destinationObjectType)
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