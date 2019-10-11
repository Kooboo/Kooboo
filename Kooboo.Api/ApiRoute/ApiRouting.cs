//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Api.ApiRoute
{
    public static class ApiRouting
    {
        public static bool TryParseCommand(string relativeUrl, string httpMethod, out ApiCommand command, string beforeApi = null)
        {
            if (string.IsNullOrEmpty(beforeApi))
            {
                beforeApi = "_api";
            }
            command = new ApiCommand
            {
                HttpMethod = httpMethod
            };
            if (string.IsNullOrEmpty(relativeUrl))
            {
                command = null;
                return false;
            }
            var questionMarkIndex = relativeUrl.IndexOf("?");
            if (questionMarkIndex > -1)
            {
                if (questionMarkIndex == 0)
                {
                    return false;
                }

                relativeUrl = relativeUrl.Substring(0, questionMarkIndex);
            }
            relativeUrl = relativeUrl.Replace("\\", "/");

            var state = RoutingState.BeforeApi;

            var segments = relativeUrl.Split('/');

            foreach (var item in segments)
            {
                if (string.IsNullOrEmpty(item))
                {
                    if (state == RoutingState.BeforeApi)
                    {
                        continue;
                    }

                    break;
                }

                switch (state)
                {
                    case RoutingState.BeforeApi:
                    {
                        if (item.ToLower() == beforeApi)
                        {
                            state = RoutingState.BeforeObject;
                            continue;
                        }

                        return false;
                    }
                    case RoutingState.BeforeObject:

                        command.ObjectType = item;
                        state = RoutingState.AfterObject;
                        break;

                    case RoutingState.AfterObject:
                        command.Method = item;
                        state = RoutingState.AfterCommand;
                        break;

                    case RoutingState.AfterCommand:
                        command.Value = item;
                        state = RoutingState.AfterValue;
                        break;

                    case RoutingState.AfterValue:

                        if (command.Parameters.Count == 0)
                        {
                            command.Parameters.Add(command.Value);
                        }
                        command.Parameters.Add(item);
                        // return false;
                        break;
                }
            }

            if (state == RoutingState.BeforeObject || state == RoutingState.BeforeApi)
            {
                return false;
            }

            if (string.IsNullOrEmpty(command.Method))
            {
                command.Method = httpMethod;
            }
            return true;
        }
    }

    public enum RoutingState
    {
        BeforeApi = 0,
        BeforeObject = 1,
        AfterObject = 2,
        AfterCommand = 3,
        AfterValue = 4
    }
}