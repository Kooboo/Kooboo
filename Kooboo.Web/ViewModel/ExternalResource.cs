//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class ExternalResourceItemViewModel
    {
        public Guid Id { get; set; }

        public string FullUrl { get; set; }

        public string Name
        {
            get
            { return this.FullUrl; }
            set { this.FullUrl = value; }
        }

        public DateTime LastModified { get; set; }
        public string ResourceType { get; set; }

        public Dictionary<string, int> Relations { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public string PreviewUrl
        {
            get { return this.FullUrl; }
        }
    }
}
