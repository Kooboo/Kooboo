//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class RouteItemViewModel
    {

        public Guid Id { get; set; }

        public string Name
        {
            get; set;
        }

        public DateTime LastModified { get; set; }

        public string FullUrl
        {
            get
            {
                return this.Name;
            }
        }

        public string ResourceType { get; set; }

        public Dictionary<string, int> Relations { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public bool HasObject
        {
            get
            {
                return this.ObjectId != default(Guid);
            }
        }

        public Guid ObjectId { get; set; }

        public string PreviewUrl { get; set; }

    }
}
