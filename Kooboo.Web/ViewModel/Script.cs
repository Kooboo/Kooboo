namespace Kooboo.Web.ViewModel
{
    public class ScriptEditViewModel : IDiffChecker
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string Extension { get; set; }
        public long Version { get; set; }
        public bool? EnableDiffChecker { get; set; }
    }
}

