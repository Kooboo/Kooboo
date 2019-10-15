using System.Collections.Generic;

namespace Kooboo.Data.Ensurance.Model
{
    // this method must return bool.
    public class HttpGetBool : IQueueTask
    {
        public string FullUrl { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}