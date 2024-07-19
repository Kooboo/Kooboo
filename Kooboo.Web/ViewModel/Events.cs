//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class AvailableActivityViewModel
    {
        public Guid ActivityId { get; set; }

        public string Name { get; set; }

    }

    public class RuleMeta
    {
        public string RuleType { get; set; }

        public List<FieldInfo> Fields { get; set; }

    }

    public class FieldInfo
    {
        public string FieldName { get; set; }

        public string Type { get; set; }
    }
}
