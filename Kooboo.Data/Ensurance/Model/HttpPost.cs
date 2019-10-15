namespace Kooboo.Data.Ensurance.Model
{
    public class HttpPost : IQueueTask
    {
        public string FullUrl { get; set; }

        public string Json { get; set; }
    }
}