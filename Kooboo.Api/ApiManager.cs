//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Api
{
    public static class ApiManager
    {
        public static IResponse Execute(ApiCall call, IApiProvider apiProvider)
        {
            ApiMethod apimethod = null;

            var apiobject = apiProvider.Get(call.Command.ObjectType);

            if (apiobject != null)
            {
                apimethod = Methods.ApiMethodManager.Get(apiobject, call.Command.Method);
            }

            if (apimethod == null && apiProvider.GetMethod != null)
            {
                apimethod = apiProvider.GetMethod(call);
            }

            if (apimethod == null)
            {
                var result = new JsonResponse { Success = false };
                result.Messages.Add(Hardcoded.GetValue("Api method Not Found", call.Context));
                return result;
            }

            // check permission
            if (apiProvider.CheckAccess != null)
            {
                if (!apiProvider.CheckAccess(call.Context, apimethod))
                {
                    var result = new JsonResponse() { Success = false };
                    result.Messages.Add(Hardcoded.GetValue("Unauthorized access", call.Context));
                    return result;
                }
            }

            if (call.IsFake)
            {
                var fakedata = Lib.Development.FakeData.GetFakeValue(apimethod.ReturnType);
                return new JsonResponse(fakedata) { Success = true };
            }
            if (apiobject != null)
            {
                if (!ValidateRequirement(call.Command, call.Context, apiProvider))
                {
                    var result = new JsonResponse() { Success = false };
                    result.Messages.Add(Hardcoded.GetValue("User or website not valid", call.Context));
                    return result;
                }
            }

            var errors = new List<string>();
            if (!ValidAssignModel(apimethod, call, errors.Add))
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.AddRange(errors);
                return result;
            }

            if (!ValidParameters(apimethod, call, errors.Add))
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.AddRange(errors);
                return result;
            }

            // ValidateKModel()
            try
            {
                return ExecuteMethod(call, apimethod);
            }
            catch (Exception ex)
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.Add(ex.Message);

                Data.Log.Instance.Exception.Write(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);

                return result;
            }
        }

        private static IResponse ExecuteMethod(ApiCall call, ApiMethod apimethod)
        {
            var response = Methods.ApiMethodManager.Execute(apimethod, call);

            if (apimethod.IsVoid)
            {
                return new JsonResponse { Success = true, DataChange = true };
            }

            if (response != null && response.GetType() == typeof(bool))
            {
                var ok = (bool)response;
                var result = new JsonResponse() { Success = ok };
                if (!ok)
                {
                    result.Messages.Add(Hardcoded.GetValue("Api method define a bool return type and return false", call.Context));
                }
                return result;
            }

            if (response == null)
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.Add(Hardcoded.GetValue("method return null for required object type", call.Context) + " :" + apimethod.ReturnType.ToString());
                return result;
            }

            if (response is IResponse)
            {
                return response as IResponse;
                //TODO: set the response message to multiple lingual.
            }

            return new JsonResponse(response) { Success = true };
        }

        public static bool ValidateRequirement(ApiCommand command, RenderContext context, IApiProvider apiProvider)
        {
            if (command == null)
                return false;

            var apiobject = apiProvider.Get(command.ObjectType);
            if (apiobject == null)
                return false;

            if (apiobject.RequireSite && context.WebSite == null)
                return false;

            if (apiobject.RequireUser && context.User == null)
                return false;

            if (apiobject.RequireSite && apiobject.RequireUser)
                if (context.WebSite.OrganizationId != context.User.CurrentOrgId)
                    return false;

            if (apiobject is ISecureApi)
            {
                var secureobj = apiobject as ISecureApi;
                if (secureobj != null)
                    return secureobj.AccessCheck(command, context);
            }

            return true;
        }

        public static bool ValidAssignModel(ApiMethod method, ApiCall call, Action<string> callback)
        {
            var isSuccess = true;
            if (method.RequireModelType != null)
            {
                var json = call.Context.Request.Body;

                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        call.Context.Request.Model = Lib.Helper.JsonHelper.Deserialize(json, method.RequireModelType);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        callback.Invoke(ex.Message);
                    }
                }
                else
                {
                    var req = call.Context.Request;
                    var values = new Dictionary<string, string>();

                    foreach (var item in req.Forms.AllKeys)
                    {
                        var value = req.Forms[item];
                        values[item] = value;
                    }

                    foreach (var item in req.QueryString.AllKeys)
                    {
                        var value = req.QueryString[item];
                        values[item] = value;
                    }

                    if (values.Count == 0)
                    {
                        callback.Invoke(Hardcoded.GetValue("required model type not provided", call.Context) + ": " + method.RequireModelType.Name);
                        isSuccess = false;
                    }
                    else
                    {
                        var dictjson = Lib.Helper.JsonHelper.Serialize(values);
                        try
                        {
                            call.Context.Request.Model = Lib.Helper.JsonHelper.Deserialize(dictjson, method.RequireModelType);
                        }
                        catch (Exception ex)
                        {
                            isSuccess = false;
                            callback.Invoke(ex.Message);
                        }
                    }
                }
            }
            return isSuccess;
        }

        public static bool ValidParameters(ApiMethod method, ApiCall call, Action<string> callback)
        {
            var isSuccess = true;
            if (method.RequireParas != null && method.RequireParas.Any())
            {
                foreach (var item in method.RequireParas)
                {
                    var value = call.GetValue(item);
                    if (string.IsNullOrEmpty(value))
                    {
                        isSuccess = false;
                        callback.Invoke(Hardcoded.GetValue("Require parameter not found", call.Context) + ": " + item);
                    }
                }
            }

            foreach (var item in method.Parameters)
            {
                if (item.ClrType != typeof(ApiCall) && (item.ClrType.IsValueType || item.ClrType == typeof(string)))
                {
                    var value = call.GetValue(item.Name);
                    if (string.IsNullOrEmpty(value))
                    {
                        isSuccess = false;
                        callback.Invoke(Hardcoded.GetValue("Require parameter not found", call.Context) + ": " + item.Name);
                    }
                }
            }

            return isSuccess;
        }
    }
}