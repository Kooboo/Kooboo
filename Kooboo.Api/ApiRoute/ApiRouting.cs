//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

namespace Kooboo.Api.ApiRoute
{
    public static class ApiRouting
    {
        public static bool TryParseCommand(string RelativeUrl, string HttpMethod, out ApiCommand command,
            string beforeapi = null)
        {
            if (string.IsNullOrEmpty(beforeapi))
            {
                beforeapi = "_api";
            }

            command = new ApiCommand();
            command.HttpMethod = HttpMethod;
            if (string.IsNullOrEmpty(RelativeUrl))
            {
                command = null;
                return false;
            }

            int questionMarkIndex = RelativeUrl.IndexOf("?");
            if (questionMarkIndex > -1)
            {
                if (questionMarkIndex == 0)
                {
                    return false;
                }
                else
                {
                    RelativeUrl = RelativeUrl.Substring(0, questionMarkIndex);
                }
            }

            RelativeUrl = RelativeUrl.Replace("\\", "/");

            RoutingState state = RoutingState.BeforeApi;

            string[] segments = RelativeUrl.Split('/');

            foreach (var item in segments)
            {
                if (string.IsNullOrEmpty(item))
                {
                    if (state == RoutingState.BeforeApi)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (state == RoutingState.BeforeObject && ApiVersion.IsVersion(item))
                    {
                        state = RoutingState.BeforeVersion;
                    }

                    switch (state)
                    {
                        case RoutingState.BeforeApi:
                            {
                                if (item.ToLower() == beforeapi)
                                {
                                    state = RoutingState.BeforeObject;
                                    continue;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        case RoutingState.BeforeObject:
                            command.ObjectType = item;
                            state = RoutingState.AfterObject;
                            break;

                        case RoutingState.BeforeVersion:
                            command.Version = ApiVersion.GetVersion(item);
                            state = RoutingState.BeforeObject;
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
                        default:
                            break;
                    }
                }
            }

            if (state == RoutingState.BeforeObject || state == RoutingState.BeforeApi)
            {
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(command.Method))
                {
                    command.Method = HttpMethod;
                }

                return true;
            }
        }
    }

    public enum RoutingState
    {
        BeforeApi = 0,
        BeforeObject = 1,
        AfterObject = 2,
        AfterCommand = 3,
        AfterValue = 4,
        BeforeVersion = 5
    }
}