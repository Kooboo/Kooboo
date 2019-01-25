//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;

namespace Kooboo.Sites.Models
{
    /// <summary>
    /// Links to External resource like external url, image, css, font, etc.  
    /// </summary>
    public class ExternalResource : SiteObject
    {
        public ExternalResource()
        {
            this.ConstType = ConstObjectType.ExternalResource;
        }

        public override string Name
        {
            get
            {
                return FullUrl;
            }
        }

        /// <summary>
        /// The const type of destination object type. 
        /// </summary>
        public byte DestinationObjectType { get; set; }

        private string _fullurl;
        public string FullUrl
        {
            get
            { return _fullurl; }
            set
            {
                _fullurl = value;
                UrlHash = _fullurl.ToHashGuid();
            }
        }

        private Guid _urlhash;
        public Guid UrlHash
        {
            get
            {
                if (_urlhash == default(Guid) && !string.IsNullOrEmpty(FullUrl))
                {
                    _urlhash = FullUrl.ToHashGuid();
                }
                return _urlhash;
            }
            set
            { _urlhash = value; }
        }


        public override int GetHashCode()
        {
            string unique = this.FullUrl + this.DestinationObjectType.ToString();
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

    }
}
