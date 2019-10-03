using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.Renderers.ExternalCache
{
    public class CacheObject
    {
        public CacheObject()
        {

        }

        public CacheObject(string fullurl, string name, string contenttype, int interval)
        {
            this.FullFileUrl = fullurl;
            this.name = name;
            this.ContentType = contenttype;
            this.interval = interval;

            if (this.interval <= 0)
            {
                this.interval = 60 * 60; // default one hour. 
            }

            // this.Expiration = DateTime.Now.AddSeconds(this.interval);
        }

        // seconds. 
        public int interval { get; set; }

        public DateTime Expiration { get; set; }


        private string _contenttype;

        public string ContentType
        {
            get; set;
        }


        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (FullFileUrl != null)
                    {
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(FullFileUrl);
                    }
                }
                return _id;
            }
        }

        public string FullFileUrl { get; set; }

        private string _name;
        public string name
        {
            get
            {
                if (_name == null && FullFileUrl != null)
                {
                    return Lib.Helper.UrlHelper.GetNameFromUrl(FullFileUrl);
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }

    }
}
