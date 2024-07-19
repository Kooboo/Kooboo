//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class BindingInfo
    {
        public Guid Id
        {
            get; set;
        }

        public Guid OrganizationId { get; set; }

        /// <summary>
        ///  the name key of the website. used as a foreign key. 
        /// </summary>
        // public string websiteName;
        public Guid WebSiteId;

        public string SiteName { get; set; }

        /// <summary>
        /// Subdomain, includes www or others. 
        /// </summary>
        public string SubDomain { get; set; }

        public Guid DomainId { get; set; }

        /// <summary>
        /// The full domain record including sub domain information
        /// </summary>
        public string FullName
        {
            get; set;
        }

        /// <summary>
        /// Device is the user agent. It used a contains to match the user agent.
        /// blank = match all. 
        /// </summary>
        public string Device { get; set; }

        public int Port { get; set; }
    }
}
