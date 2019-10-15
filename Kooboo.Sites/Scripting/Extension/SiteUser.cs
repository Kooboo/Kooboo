//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Scripting.Extension
{
    public class SiteUser : IkScript
    {
        [Attributes.SummaryIgnore]
        public string Name => "SampleUser";

        [Attributes.SummaryIgnore]
        public RenderContext Context { get; set; }

        public bool Validate(string userName, string password)
        {
            return true;
        }

        public string Get(string username)
        {
            return username + " sample";
        }
    }
}