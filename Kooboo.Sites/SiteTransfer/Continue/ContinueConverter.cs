//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.SiteTransfer.Model
{
    public class ContinueConverter : SiteObject
    {
        public ContinueConverter()
        {
            this.ConstType = ConstObjectType.ContinueConverter; 
        }

        private Guid _id;
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.OriginalPageId.ToString() + this.KoobooId;
                    _id = unique.ToHashGuid();
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string ConvertType { get; set; }

        public Guid OriginalPageId { get; set; }

        public string ConvertedTag { get; set; }

        public string ObjectNameOrId { get; set; }

        public string KoobooId { get; set; }

        public List<string> ElementPaths { get; set; }

        public string ElementTag { get; set; }

        public Guid Hash { get; set; }

    }
}
