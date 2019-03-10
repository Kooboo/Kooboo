//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions; 

namespace Kooboo.Sites.Contents.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    public class ContentCategory : Kooboo.Sites.Models.CoreObject
    {
        public ContentCategory()
        {
            this.ConstType = ConstObjectType.ContentCategory;
        }
         

        private Guid _id;
        [Kooboo.Attributes.SummaryIgnore]
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string unique = string.Concat(this.ConstType, this.CategoryFolder.ToString(), this.CategoryId, this.ContentId);
                    _id = Kooboo.Data.IDGenerator.GetId(unique);
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public Guid CategoryFolder { get; set; }

        public Guid CategoryId { get; set; }

        public Guid ContentId { get; set; }

        public override int GetHashCode()
        {
            string unique = this.CategoryFolder.ToString() + this.CategoryId.ToString() + this.ContentId.ToString();
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
}
