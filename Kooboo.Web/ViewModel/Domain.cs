//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class DomainSummaryViewModel
    {
        public Guid Id { get; set; }

        public string DomainName { get; set; }

        public string Expires { get; set; }

        public int Records { get; set; }

        public int Sites { get; set; }

        public int Emails { get; set; } = 2;

        public bool UseEmail { get { return this.Emails > 0; } }

        public bool EnableDns { get; set; }

        public bool EnableEmail { get; set; }

        public string DataCenter { get; set; }

        public string DataCenterName { get; set; }

        public string NameServer { get; set; }
    }

    public class DomainBindingViewModel
    {
        public Guid Id { get; set; }
        public string SubDomain { get; set; }

        public string WebSiteName { get; set; }
    }


    public class DomainDataCenterViewModel
    {
        public string DataCenter { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsChosen { get; set; }
    }
}
