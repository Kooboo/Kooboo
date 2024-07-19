//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System.Collections.Generic;

namespace Kooboo.Api
{
    public class ApiCommand
    {
        public string ObjectType { get; set; }

        public string Method { get; set; }

        public string Value { get; set; }

        public string Version { get; set; } = ApiVersion.V1;

        public string HttpMethod { get; set; }

        private List<string> _parameters;

        public List<string> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new List<string>();
                }

                return _parameters;
            }
            set { _parameters = value; }
        }
    }
}