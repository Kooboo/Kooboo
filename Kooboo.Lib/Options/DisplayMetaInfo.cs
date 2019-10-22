//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Extensions;

namespace Kooboo
{
    public class DisplayMetaInfo
    {
        private string _id;

        public string Id
        {
            get { return _id ?? (_id = Name.ToHashGuid().ToString()); }
        }

        public string ShortName { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public string Prompt { get; set; }

        public int Order { get; set; }

        public string GetDisplayName()
        {
            return DisplayName ?? Name;
        }
    }
}