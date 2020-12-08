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

        public string MatcherDetail { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SucceedAction SucceedAction { get; set; }

        public string SucceedCodeName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FailedAction FailedAction { get; set; }

        public string FailedCodeName { get; set; }

    }

    public enum MatcherType
    {
        None,
        Any,
        Condition,
        Regular,
        CustomCode
    }

    public enum SucceedAction
    {
        None,
        JwtLogin,
        CustomCode
    }

    public enum FailedAction
    {
        None,
        ResultCode,
        Redirect,
        CustomCode
    }
}
