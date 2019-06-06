//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Api.ApiResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Language;
using Kooboo.Data.Context;

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

            if (apimethod == null && apiProvider.GetMethod!=null)
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
            if(apiobject!=null)
            {
                if (!ValidateRequirement(call.Command, call.Context, apiProvider))
                {
                    var result = new JsonResponse() { Success = false };
                    result.Messages.Add(Hardcoded.GetValue("User or website not valid", call.Context));
                    return result;
                }
            }
            
            List<string> errors = new List<string>();
            if (!ValideAssignModel(apimethod, call, errors.Add))
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


            // ValidateKModel()


            try
            {
                return ExecuteMethod(call, apimethod);
            }
            catch (Exception ex)
            {
                var result = new JsonResponse() { Success = false };
                result.Messages.Add(ex.Message);

                Kooboo.Data.Log.ExceptionLog.Write(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);

                return result;
            } 
        }

        private static IResponse ExecuteMethod(ApiCall call, ApiMethod apimethod)
        {
            var response = Methods.ApiMethodManager.Execute(apimethod, call);

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

            var apiobject = apiProvider.Get(command.ObjectType);
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
                if (context.WebSite.OrganizationId != context.User.CurrentOrgId)
                {
                    return false;
                }
            }

            return true;
        }
                             

        public static bool ValideAssignModel(ApiMethod method, ApiCall call, Action<string> callback)
        {
            bool IsSuccess = true;
            if (method.RequireModelType != null)
            {
                string json = call.Context.Request.Body;

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
                        callback.Invoke(Hardcoded.GetValue("required model type not provided", call.Context) + ": " + method.RequireModelType.Name);
                        IsSuccess = false;
                    }
                    else
                    {
                        string dictjson = Lib.Helper.JsonHelper.Serialize(values);
                        try
                        {
                            call.Context.Request.Model = Lib.Helper.JsonHelper.Deserialize(dictjson, method.RequireModelType);
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
                        callback.Invoke(Hardcoded.GetValue("Require parameter not found", call.Context) + ": " + item);
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
                        callback.Invoke(Hardcoded.GetValue("Require parameter not found", call.Context) + ": " + item.Name);
                    }
                }
            }

            return IsSuccess;
        }
    }
}
