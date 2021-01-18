using Kooboo.Sites.FrontEvent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable]
    public class Authentication : CoreObject
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MatcherType Matcher { get; set; }

        public List<Condition> Conditions { get; set; } = new List<Condition>();

        [JsonConverter(typeof(StringEnumConverter))]
        public AuthenticationAction Action { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FailedAction FailedAction { get; set; }

        public string CustomCodeName { get; set; }

        public string Url { get; set; }

        public int HttpCode { get; set; }


        public override int GetHashCode()
        {
            var un = Name;
            un += Matcher.ToString();
            un += string.Join("", Conditions.Select(s => s.Left + s.Right + s.Right));
            un += Online.ToString();
            un += Action.ToString();
            un += FailedAction.ToString();
            un += CustomCodeName;
            un += Url;
            un += HttpCode;

            return Lib.Security.Hash.ComputeIntCaseSensitive(un);
        }
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
        JwtAuth,
        CustomCode
    }

    public enum FailedAction
    {
        None,
        ResultCode,
        Redirect
    }
}
