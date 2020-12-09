using Kooboo.Sites.FrontEvent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable]
    public class Authentication : CoreObject
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MatcherType Matcher { get; set; }

        public List<Condition> Conditions { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AuthenticationAction Action { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FailedAction FailedAction { get; set; }

        public string CustomCodeName { get; set; }

        public string Url { get; set; }

        public int HttpCode { get; set; }

    }

    public enum MatcherType
    {
        None,
        Any,
        Condition
    }

    public enum AuthenticationAction
    {
        None,
        Jwt,
        basic,
        CustomCode
    }

    public enum FailedAction
    {
        None,
        ResultCode,
        Redirect
    }
}
