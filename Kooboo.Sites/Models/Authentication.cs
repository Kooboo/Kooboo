using Kooboo.Sites.FrontEvent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable]
    public class Authentication : CoreObject
    {
        public MatcherType Matcher { get; set; }

        public string MatcherDetail { get; set; }

        public SucceedAction SucceedAction { get; set; }

        public string SucceedCodeName { get; set; }

        public FailedAction FailedAction { get; set; }

        public string FailedCodeName { get; set; }

    }

    public enum MatcherType
    {
        Condition,
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
