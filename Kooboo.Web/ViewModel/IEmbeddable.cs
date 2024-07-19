//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;

namespace Kooboo.Web.ViewModel
{

    public class IEmbeddableItemListViewModel
    {
        public IEmbeddableItemListViewModel()
        {

        }
        public IEmbeddableItemListViewModel(SiteDb siteDb, IEmbeddable embeddedobject)
        {
            string baseurl = siteDb.WebSite.BaseUrl();
            if (embeddedobject == null)
            {
                throw new ArgumentNullException("embedded object");
            }

            Id = embeddedobject.Id;

            if (embeddedobject.IsEmbedded)
            {
                Name = StringHelper.GetSummary(embeddedobject.Body);
                var info = ObjectService.GetObjectInfo(siteDb, embeddedobject as SiteObject);
                FullUrl = info?.Url;
                RouteName = Name;
                IsEmbedded = true;
            }
            else
            {
                var route = siteDb.Routes.GetByObjectId(embeddedobject.Id);
                if (route != null)
                {
                    //  Name = route.Name;
                    FullUrl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, route.Name);
                    RouteName = route.Name;
                    RouteId = route.Id;
                }

                this.Name = embeddedobject.Name;
                if (string.IsNullOrEmpty(this.Name))
                {
                    if (route != null)
                    {
                        this.Name = route.Name;
                    }
                }
                if (string.IsNullOrEmpty(this.Name))
                {
                    this.Name = StringHelper.GetSummary(embeddedobject.Body);
                }

            }

            LastModified = embeddedobject.LastModified;
            OwnerObjectId = embeddedobject.OwnerObjectId;


            List<UsedByRelation> usedby = null;

            if (embeddedobject.ConstType == ConstObjectType.Style)
            {
                usedby = siteDb.Styles.GetUsedBy(embeddedobject.Id);
            }
            else if (embeddedobject.ConstType == ConstObjectType.Script)
            {
                usedby = siteDb.Scripts.GetUsedBy(embeddedobject.Id);
            }
            else if (embeddedobject.ConstType == ConstObjectType.Form)
            {
                usedby = siteDb.Forms.GetUsedBy(embeddedobject.Id);
            }
            else if (embeddedobject.ConstType == ConstObjectType.Code)
            {
                usedby = siteDb.Code.GetUsedBy(embeddedobject.Id);
            }

            References = Sites.Helper.RelationHelper.Sum(usedby);

        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime LastModified { get; set; }

        public string FullUrl { get; set; }

        public string RouteName { get; set; }

        public long Size { get; set; }

        public string Source { get; set; }

        public Guid RouteId { get; set; }

        public Dictionary<string, int> References { get; set; } = new Dictionary<string, int>();

        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }
        public Guid OwnerObjectId { get; set; }
        public bool IsEmbedded { get; set; }

    }

    public class InlineItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OwnerName { get; set; }

        public DateTime LastModified { get; set; }

        public string OwnerType { get; set; }

        public string DateModifiedString
        {
            get { return String.Concat(LastModified.ToShortDateString(), " ", LastModified.ToShortTimeString()); }
        }

        public string Source { get; set; }
    }

    public class ResourceGroupViewModel
    {
        public ResourceGroupViewModel()
        {
            this.Children = new List<ResourceGroupItem>();
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Target source type
        /// </summary>
        public byte Type { get; set; }

        public string TypeName { get; set; }

        public int ChildrenCount { get; set; }

        public List<ResourceGroupItem> Children { get; set; }

        public DateTime LastModified { get; set; }

        public string PreviewUrl { get; set; }

        public string RelativeUrl { get; set; }

        public Dictionary<string, int> References = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    }

    [Serializable]
    public class ResourceGroupItem
    {
        public string Name { get; set; }

        /// <summary>
        /// Target source id
        /// </summary>
        public Guid RouteId { get; set; }

        public int Order { get; set; }
    }

}
