using Kooboo.Lib.Helper;
using Kooboo.Sites.Engine;
using Kooboo.Sites.FrontEvent;
using Kooboo.Sites.Models;
using Kooboo.Sites.Render;
using Kooboo.Sites.Scripting;
using Kooboo.Sites.Scripting.Global.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kooboo.Sites.Helper
{
    public static class AuthenticationHelper
    {

        public static bool Authentication(FrontContext frontContext)
        {
            var authentication = Match(frontContext);
            if (authentication == null) return true;
            var enableCORS = frontContext?.WebSite?.EnableCORS ?? false;

            if (CorsHelper.IsOptionsRequest(frontContext) && enableCORS)
            {
                CorsHelper.HandleHeaders(frontContext);
                return false;
            }

            switch (authentication.Action)
            {
                case AuthenticationAction.None:
                    return true;
                case AuthenticationAction.JwtAuth:
                    return JwtLogin(frontContext, authentication);
                case AuthenticationAction.CustomCode:
                    return ExecuteCode(frontContext, authentication);
                default:
                    return true;
            }
        }

        private static bool JwtLogin(FrontContext frontContext, Authentication authentication)
        {
            var jwt = new kJwt(frontContext.RenderContext);
            var json = jwt.Decode();
            var jsonParser = new Jint.Native.Json.JsonParser(new Jint.Engine());
            var result = jsonParser.Parse(json);
            var code = result.AsObject().Get("code").AsNumber();
            if (code == 1)
            {
                switch (authentication.FailedAction)
                {
                    case FailedAction.None:
                        frontContext.RenderContext.Response.AppendString(result.AsObject().Get("value").ToString());
                        break;
                    case FailedAction.ResultCode:
                        frontContext.RenderContext.Response.StatusCode = authentication.HttpCode;
                        break;
                    case FailedAction.Redirect:
                        frontContext.RenderContext.Response.Redirect(302, authentication.Url);
                        break;
                    default:
                        break;
                }
                return false;
            }
            else
            {
                frontContext.RenderContext.Items.Add("jwt_payload", result.AsObject().Get("value"));
                return true;
            }
        }

        private static bool ExecuteCode(FrontContext frontContext, Authentication authentication)
        {
            var code = frontContext.SiteDb.Code.GetByNameOrId(authentication.CustomCodeName);
            if (code == null) return true;
            var result = Scripting.Manager.ExecuteCode(frontContext.RenderContext, code.Body, code.Id);
            if (!string.IsNullOrWhiteSpace(result)) frontContext.RenderContext.Response.AppendString(result);
            return frontContext.RenderContext.Response.StatusCode < 300;
        }

        private static Authentication Match(FrontContext frontContext)
        {
            var authentications = frontContext.SiteDb.Authentication.All();
            foreach (var item in authentications)
            {
                switch (item.Matcher)
                {
                    case MatcherType.None:
                        continue;
                    case MatcherType.Any:
                        return item;
                    case MatcherType.Condition:
                        if (MatchCondition(frontContext, item.Conditions)) return item;
                        break;
                    default:
                        continue;
                }
            }

            return null;
        }

        private static bool MatchCondition(FrontContext frontContext, List<Condition> conditions)
        {
            if (conditions == null || conditions.Count == 0) return false;

            foreach (var item in conditions)
            {
                var leftValue = GetLeftValue(item.Left, frontContext);
                if (string.IsNullOrWhiteSpace(leftValue) || string.IsNullOrWhiteSpace(item.Right)) return false;

                leftValue = leftValue.Trim().ToLower();
                var rightValue = item.Right.Trim().ToLower();

                switch (item.Operator)
                {
                    case "=":
                        if (leftValue != rightValue) return false;
                        break;
                    case "!=":
                        if (leftValue == rightValue) return false;
                        break;
                    case "contains":
                        if (!leftValue.Contains(rightValue)) return false;
                        break;
                    case "notcontains":
                        if (leftValue.Contains(rightValue)) return false;
                        break;
                    case "startwith":
                        if (!leftValue.StartsWith(rightValue)) return false;
                        break;
                    case "notstartwith":
                        if (leftValue.StartsWith(rightValue)) return false;
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        private static string GetLeftValue(string left, FrontContext frontContext)
        {
            switch (left)
            {
                case "url":
                    return frontContext.RenderContext.Request.RelativeUrl;
                case "method":
                    return frontContext.RenderContext.Request.Method;
                default:
                    return null; ;
            }
        }
    }
}
