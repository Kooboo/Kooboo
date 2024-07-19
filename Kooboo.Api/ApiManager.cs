//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Lib.Exceptions;

namespace Kooboo.Api
{
    public static class ApiManager
    {
        public static IResponse Execute(ApiCall call, IApiProvider apiProvider)
        {
            ApiMethod apimethod = null;

            var apiobject = apiProvider.Get(call.Command);

            if (apiobject != null)
            {
                apimethod = Methods.ApiMethodManager.Get(apiobject, call.Command);
            }

            if (apimethod == null && apiProvider.GetMethod != null)
            {
                apimethod = apiProvider.GetMethod(call);
            }

            if (apimethod == null)
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.Add(Hardcoded.GetValue("Api method Not Found", call.Context));
                return result;
            }

            // check permission
            if (apiProvider.CheckAccess != null)
            {
                if (!apiProvider.CheckAccess(call.Context, apimethod))
                {
                    var result = new JsonResponse() { Success = false, HttpCode = 403 };
                    result.Messages.Add(Hardcoded.GetValue("Access denied", call.Context));
                    return result;
                }
            }

            if (call.IsFake)
            {
                var fakeData = Lib.Development.FakeData.GetFakeValue(apimethod.ReturnType);
                return new JsonResponse(fakeData) { Success = true };
            }

            if (apiobject != null)
            {
                if (!ValidateRequirement(call.Command, call.Context, apiProvider))
                {
                    var result = new JsonResponse() { Success = false, HttpCode = 401 };
                    result.Messages.Add(Hardcoded.GetValue("User or website not valid", call.Context));
                    return result;
                }
            }

            List<string> errors = new List<string>();
            if (!ValidAssignModel(apimethod, call, errors.Add))
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.AddRange(errors);
                return result;
            }

            if (!ValideParameters(apimethod, call, errors.Add))
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.AddRange(errors);
                return result;
            }

            try
            {
                return ExecuteMethod(call, apimethod);
            }
            catch (DiffException ex)
            {
                var result = new JsonResponse() { Success = true };

                result.Model = new
                {
                    ex.Version,
                    ex.Body
                };

                result.HttpCode = 409;
                return result;
            }
            catch (TranslationLanguageException ex)
            {
                var value = Hardcoded.GetValue(ex.Message, call.Context);
                var result = new JsonResponse() { Success = false };
                result.Messages.Add(value);

                Data.Log.Instance.Exception.WriteException(ex);

                return result;
            }
            catch (Exception ex)
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.Add(ex.Message);

                Data.Log.Instance.Exception.WriteException(ex);

                return result;
            }
        }

        private static IResponse ExecuteMethod(ApiCall call, ApiMethod apimethod)
        {
            object response = null;

            if (apimethod.ClassInstance is Api)
            {
                var instance = Activator.CreateInstance(apimethod.DeclareType) as Api;

                try
                {
                    response = instance.OnActionExecuting(call);
                    if (response == null)
                    {
                        response = Methods.ApiMethodManager.Execute(apimethod, call);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    instance.OnActionExecuted(call);
                }
            }
            else
            {
                response = Methods.ApiMethodManager.Execute(apimethod, call);
            }


            if (apimethod.IsVoid)
            {
                return new JsonResponse() { Success = true, DataChange = true };
            }

            if (response != null && response.GetType() == typeof(bool))
            {
                var ok = (bool)response;
                var result = new JsonResponse() { Success = ok };
                if (!ok)
                {
                    result.Messages.Add(Hardcoded.GetValue("Api method define a bool return type and return false",
                        call.Context));
                }

                return result;
            }

            if (response == null)
            {
                return new JsonResponse(null);
            }

            if (response is IResponse)
            {
                return response as IResponse;
            }
            else
            {
                return new JsonResponse(response) { Success = true };
            }
        }


        public static bool ValidateRequirement(ApiCommand command, RenderContext context, IApiProvider apiProvider)
        {
            if (command == null)
            {
                return false;
            }

            var apiobject = apiProvider.Get(command);
            if (apiobject == null)
            {
                return false;
            }

            if (apiobject.RequireSite && context.WebSite == null)
            {
                return false;
            }

            if (apiobject.RequireUser && context.User == null)
            {
                return false;
            }

            if (apiobject.RequireSite && apiobject.RequireUser)
            {
                if (context.WebSite.OrganizationId != context.User.CurrentOrgId && context.WebSite.OrganizationId != context.User.Id)
                {
                    return false;
                }
            }


            if (apiobject is ISecureApi)
            {
                var secureobj = apiobject as ISecureApi;
                if (secureobj != null)
                {
                    return secureobj.AccessCheck(command, context);
                }
            }

            return true;
        }

        public static bool ValidAssignModel(ApiMethod method, ApiCall call, Action<string> callback)
        {
            bool IsSuccess = true;
            if (method.RequireModelType != null)
            {
                // if has more parameters. 
                bool HasMoreParaMeter = false;
                string ParaName = null;
                foreach (var item in method.Parameters)
                {
                    if (item.ClrType == method.RequireModelType)
                    {
                        ParaName = item.Name;
                    }
                    else if (item.ClrType != typeof(ApiCall))
                    {
                        HasMoreParaMeter = true;
                    }
                }
                string json = null;
                if (HasMoreParaMeter && ParaName != null)
                {
                    json = call.GetValue(ParaName);
                }
                else
                {
                    json = call.Context.Request.Body;
                }


                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        call.Context.Request.Model = Lib.Helper.JsonHelper.Deserialize(json, method.RequireModelType);
                    }
                    catch (Exception ex)
                    {
                        IsSuccess = false;
                        callback.Invoke(ex.Message);
                    }
                }
                else
                {
                    var req = call.Context.Request;
                    Dictionary<string, string> values = new Dictionary<string, string>();

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

                    if (values.Count() == 0)
                    {
                        callback.Invoke(Hardcoded.GetValue("required model type not provided", call.Context) + ": " +
                                        method.RequireModelType.Name);
                        IsSuccess = false;
                    }
                    else
                    {
                        string dictjson = Lib.Helper.JsonHelper.Serialize(values);
                        try
                        {
                            call.Context.Request.Model =
                                Lib.Helper.JsonHelper.Deserialize(dictjson, method.RequireModelType);
                        }
                        catch (Exception ex)
                        {
                            IsSuccess = false;
                            callback.Invoke(ex.Message);
                        }
                    }
                }
            }

            return IsSuccess;
        }

        public static bool ValideParameters(ApiMethod method, ApiCall call, Action<string> callback)
        {
            bool IsSuccess = true;
            if (method.RequireParas != null && method.RequireParas.Count() > 0)
            {
                foreach (var item in method.RequireParas)
                {
                    string value = call.GetValue(item);
                    if (string.IsNullOrEmpty(value))
                    {
                        IsSuccess = false;
                        callback.Invoke(Hardcoded.GetValue("Parameter not found", call.Context) + ": " + item);
                    }
                }
            }

            foreach (var item in method.Parameters)
            {
                if (item.ClrType != typeof(ApiCall) && (item.ClrType.IsValueType || item.ClrType == typeof(string)))
                {
                    string value = call.GetValue(item.Name);
                    if (string.IsNullOrEmpty(value))
                    {
                        IsSuccess = false;
                        callback.Invoke(Hardcoded.GetValue("Parameter not found", call.Context) + ": " +
                                        item.Name);
                    }
                }
            }

            return IsSuccess;
        }
    }
}