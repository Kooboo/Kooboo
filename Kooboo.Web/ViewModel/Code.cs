//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{

    public class CodeEditViewModel : IDiffChecker
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        // Js code block.
        public string Body { get; set; }

        public string Config { get; set; }

        public string EventType { get; set; }

        public string CodeType { get; set; }
        public string ScriptType { get; set; }

        public List<string> AvailableCodeType { get; set; }

        public List<string> AvailableEventType { get; set; }

        public string Url { get; set; }
        public long Version { get; set; }
        public bool? EnableDiffChecker { get; set; }
        public bool isEmbedded { get; set; }
        public bool IsDecrypted { get; set; }
    }


    public class CodeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CodeType { get; set; }
        public string ScriptType { get; set; }

        public string EventType { get; set; }

        public string Url { get; set; }

        public string PreviewUrl { get; set; }

        public DateTime LastModified { get; set; }

        public Dictionary<string, int> References { get; set; } = new Dictionary<string, int>();

        public bool IsEmbedded { get; set; }

        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }
        public long Version { get; set; }

    }


}
