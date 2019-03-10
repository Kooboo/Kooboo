//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Relation
{

    /// <summary>
    /// The relation map of two objects.
    /// This relation are mostly embedded from the Html source. 
    /// </summary>
    public class ObjectRelation : Kooboo.Sites.Models.SiteObject
    {
        public ObjectRelation()
        {
            this.ConstType = ConstObjectType.ObjectRelation; 
        }

        private Guid _id;
        public override Guid Id
        {
            set { _id = value; }
            get
            {

                if (_id == default(Guid))
                {
                    _id = Kooboo.Data.IDGenerator.GetRelationId(this.objectXId, this.objectYId);
                }

                return _id;
            }
        }

        public Guid objectXId { get; set; }
        public Guid objectYId { get; set; }

        public byte ConstTypeX { get; set; }
        public byte ConstTypeY { get; set; }

        /// <summary>
        /// when ConstTypeY is a route, this is the route destination type. 
        /// </summary>
        public byte RouteDestinationType { get; set; }

        public override int GetHashCode()
        {
            string unique = this.objectXId.ToString() + this.objectYId.ToString() + this.ConstTypeX.ToString() + this.ConstTypeY.ToString() + this.RouteDestinationType.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
     
     
}
