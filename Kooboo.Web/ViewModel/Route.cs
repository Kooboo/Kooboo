//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

namespace Kooboo.Web.ViewModel
{
    public class RouteResolvedViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public Dictionary<string, object> Params { get; set; }
    }
}
