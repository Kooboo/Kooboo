//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Executor
{
    public class TransferProgress
    {
        public int counter { get; set; } = 0;
        public Guid TaskId { get; set; }

        public string TransferDomain { get; set; }

        public void SetDomain(string FullStartUrl)
        {
            if (!string.IsNullOrEmpty(FullStartUrl))
            {
                this.TransferDomain = UrlHelper.UriHost(FullStartUrl, true);
            }
        }

        public string FullStartUrl { get; set; }

        private string _baseurl;
        public string BaseUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_baseurl))
                {
                    if (!string.IsNullOrEmpty(this.TransferDomain))
                    {
                        if (!this.TransferDomain.ToLower().StartsWith("http://") && !this.TransferDomain.ToLower().StartsWith("https://"))
                        {
                            _baseurl = "http://" + this.TransferDomain;
                        }
                        else
                        {
                            _baseurl = this.TransferDomain;
                        }
                    }
                    else if (!string.IsNullOrEmpty(this.FullStartUrl))
                    {
                        if (!this.FullStartUrl.ToLower().StartsWith("http://") && !this.FullStartUrl.ToLower().StartsWith("https://"))
                        {
                            _baseurl = "http://" + this.FullStartUrl;
                        }
                        else
                        {
                            _baseurl = this.FullStartUrl;
                        }
                    }
                }
                return _baseurl;
            }
            set
            {
                _baseurl = value;
            }
        }

        public int TotalPages { get; set; }

        public int Levels { get; set; }


    }
}
