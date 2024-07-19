//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;

namespace Kooboo.Web.ViewModel
{
    public class IpCount
    {
        public string IP { get; set; }

        public int Count { get; set; }
    }


    public class ImageLogItemViewModel
    {
        public string ThumbNail { get; set; }

        public string PreviewUrl { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public long Size { get; set; }
    }

    public class ErrorSummaryViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }

        public string PreviewUrl { get; set; }

        public int Count { get; set; }
    }

    public class VisitorLogOnline : Kooboo.Data.Models.VisitorLog
    {
        public string Country { get; set; }
        public string State { get; set; }
    }

    public class SiteErrorLogViewModel : SiteErrorLog
    {
        public SiteErrorLogViewModel(SiteErrorLog orgErr, string baseUrl)
        {

            this.Id = orgErr.Id;
            this.ClientIP = orgErr.ClientIP;
            this.Message = orgErr.Message;
            this.StartTime = orgErr.StartTime;
            this.StatusCode = orgErr.StatusCode;
            this.Url = orgErr.Url;

            this.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseUrl, this.Url);

        }


        public string PreviewUrl { get; set; }


    }
}
