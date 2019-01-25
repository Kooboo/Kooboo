//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data;

namespace Kooboo.Sites.Models
{

    /// <summary>
    /// The cache of image thumbnail. 
    /// </summary>
    public class Thumbnail : SiteObject
    {

        public Thumbnail()
        {
            ConstType = ConstObjectType.Thumbnail;
        }

        private Guid _id;
        public override Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = Kooboo.Data.IDGenerator.GetImageThumbNailId(ImageId, Height, Width);

                }
                return _id;
            }
        }

        public Guid ImageId { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public byte[] ContentBytes { get; set; }

        public string Extension { get; set; }

        public string ContentType { get; set; }


    }
}
