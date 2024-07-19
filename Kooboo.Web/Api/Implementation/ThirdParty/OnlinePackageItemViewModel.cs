namespace Kooboo.Web.Api.Implementation.ThirdParty
{
    public class OnlinePackageItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string PackageName { get; set; }

        public string FullUrl { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }

        public bool Installed { get; set; }
    }
}
