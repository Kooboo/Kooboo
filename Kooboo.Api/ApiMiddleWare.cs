//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Context;
using Kooboo.Data.Server;

namespace Kooboo.Api
{
    public class ApiMiddleware : IKoobooMiddleWare
    {
        public ApiMiddleware(IApiProvider apiprovider)
        {
            this.ApiProvider = apiprovider; 
            if (!string.IsNullOrEmpty(this.ApiProvider.ApiPrefix))
            {
                this.Prefix = this.ApiProvider.ApiPrefix; 
            }
        }

        private string Prefix { get; set; }

        private string _beforeapi; 
       internal string BeforeApi
        {
            get
            {
                if (string.IsNullOrEmpty(_beforeapi))
                {
                    if (!string.IsNullOrEmpty(this.Prefix))
                    {
                        if (this.Prefix.StartsWith("/"))
                        {
                            _beforeapi = this.Prefix.Substring(1); 
                        }
                        else
                        {
                            _beforeapi = this.Prefix; 
                        }
                    } 
                }

                if (string.IsNullOrEmpty(_beforeapi))
                {
                    _beforeapi = "_api"; 
                }
                return _beforeapi; 
            } 
            set
            { _beforeapi = value;    }
        }
           
        public IApiProvider ApiProvider { get; set;  }

        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            context.Request.Channel =  RequestChannel.API;

            if (!string.IsNullOrEmpty(this.Prefix) && !context.Request.RawRelativeUrl.ToLower().StartsWith(this.Prefix))
            {
                await Next.Invoke(context); return;
            }
            ApiCommand command;

            if (!ApiRoute.ApiRouting.TryParseCommand(context.Request.RelativeUrl, context.Request.Method, out command,this.BeforeApi))
            {
                context.Response.StatusCode = 500;
                context.Response.Body = System.Text.Encoding.UTF8.GetBytes("Invalid Api command");
                return;
            }

            ApiCall apirequest = new ApiCall() { Command = command, Context = context };

            var response = ApiManager.Execute(apirequest, this.ApiProvider);

            if (response is MetaResponse)
            {
                var dataresponse = response as MetaResponse;
                foreach (var item in dataresponse.Headers)
                {
                    context.Response.Headers.Add(item.Key, string.Join(null, item.Value));
                     
                } 

                foreach (var item in dataresponse.DeletedCookieNames)
                {
                    context.Response.DeleteCookie(item);
                } 
                foreach (var item in dataresponse.AppendedCookies)
                {
                    context.Response.AddCookie(item);  
                } 
                dataresponse.DeletedCookieNames.Clear();
                dataresponse.AppendedCookies.Clear();
                dataresponse.Headers.Clear();
                               
                context.Response.StatusCode = dataresponse.StatusCode;
                if (!string.IsNullOrEmpty(dataresponse.RedirectUrl))
                {
                    context.Response.Redirect(302, dataresponse.RedirectUrl);
                    return;
                }
            }

            if (response is BinaryResponse)
            {
                  RenderBinary(context, response as BinaryResponse);
            }
            else if (response is NoResponse)
            {
                // do nothing. 
            }
            else
            {         
                MultilingualService.EnsureLangText(response, context);   
                var jsonbody = Lib.Helper.JsonHelper.Serialize(response); 
                context.Response.ContentType = "application/json";
                context.Response.Body = System.Text.Encoding.UTF8.GetBytes(jsonbody);
                  
                context.Response.StatusCode = 200;
            } 

            context.Response.Headers.Add("Cache-Control", "private, no-cache, no-store, proxy-revalidate, no-transform");

            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
  
            context.Response.Headers.Add("Pragma", "no-cache"); 
        }

        public void RenderBinary(RenderContext context, BinaryResponse resposne)
        {
            if (resposne == null)
            {
                return;
            }
            context.Response.ContentType = resposne.ContentType;
            context.Response.StatusCode = 200;

            foreach (var item in resposne.Headers)
            {
                string value = string.Join("", item.Value); 
                context.Response.Headers.Add(item.Key, value);
            } 
            context.Response.Body= resposne.BinaryBytes;
        }
    }
} 