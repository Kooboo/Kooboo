using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Lighthouse
{
    public class Setting
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ControlType ControlType { get; set; }
    }

    public enum ControlType
    {
        Text,
        Switch,
        Number
    }
}