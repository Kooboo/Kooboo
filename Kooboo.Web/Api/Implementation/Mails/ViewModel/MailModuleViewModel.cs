namespace Kooboo.Web.Api.Implementation
{
    public class MailModuleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Settings { get; set; }
        public string TaskJs { get; set; }
        public bool Online { get; set; }
        public string BackendViewUrl { get; set; }
    }
}