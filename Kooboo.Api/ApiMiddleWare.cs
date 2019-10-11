//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Context;
using Kooboo.Data.Server;
using System;
using System.Threading.Tasks;

namespace Kooboo.Api
{
    public class ApiMiddleware : IKoobooMiddleWare
    {
        public ApiMiddleware(IApiProvider apiprovider)
        {
            ApiProvider = apiprovider;
            if (!string.IsNullOrEmpty(ApiProvider.ApiPrefix))
            {
                Prefix = ApiProvider.ApiPrefix;
            }
        }

        public string Prefix { get; set; }

        private string _beforeapi;

        internal string BeforeApi
        {
            get
            {
                if (string.IsNullOrEmpty(_beforeapi) && !string.IsNullOrEmpty(Prefix))
                {
                    _beforeapi = Prefix.StartsWith("/") ? Prefix.Substring(1) : Prefix;
                }

                if (string.IsNullOrEmpty(_beforeapi))
                {
                    _beforeapi = "_api";
                }
                return _beforeapi;
            }
            set => _beforeapi = value;
        }

        public Action<RenderContext, IResponse> Log { get; set; }

        public IApiProvider ApiProvider { get; set; }

        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            context.Request.Channel = RequestChannel.API;

            if (!string.IsNullOrEmpty(Prefix) && !context.Request.RawRelativeUrl.ToLower().StartsWith(Prefix))
            {
                await Next.Invoke(context); return;
            }

            if (!ApiRoute.ApiRouting.TryParseCommand(context.Request.RelativeUrl, context.Request.Method, out ApiCommand command, BeforeApi))
            {
                context.Response.StatusCode = 500;
                context.Response.Body = System.Text.Encoding.UTF8.GetBytes("Invalid Api command");

                Log?.Invoke(context, null);

                return;
            }

            var apirequest = new ApiCall { Command = command, Context = context };

            var response = ApiManager.Execute(apirequest, ApiProvider);

            Log?.Invoke(context, response);

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

            switch (response)
            {
                case BinaryResponse binaryResponse:
                    RenderBinary(context, binaryResponse);
                    break;
                case NoResponse _:
                    // do nothing.
                    break;
                case PlainResponse plainResponse:
                    RenderPlainResponse(context, plainResponse);
                    break;
                default:
                {
                    MultilingualService.EnsureLangText(response, context);
                    var jsonbody = Lib.Helper.JsonHelper.Serialize(response);
                    context.Response.ContentType = "application/json";
                    context.Response.Body = System.Text.Encoding.UTF8.GetBytes(jsonbody);

                    context.Response.StatusCode = 200;
                    break;
                }
            }

            context.Response.Headers.Add("Cache-Control", "private, no-cache, no-store, proxy-revalidate, no-transform");

            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            context.Response.Headers.Add("Pragma", "no-cache");
        }

        public void RenderBinary(RenderContext context, BinaryResponse resposne)
        {
            if (resposne == null) return;
            context.Response.ContentType = resposne.ContentType;
            context.Response.StatusCode = 200;

            foreach (var item in resposne.Headers)
            {
                string value = string.Join("", item.Value);
                context.Response.Headers.Add(item.Key, value);
            }

            context.Response.Body = resposne.BinaryBytes;
        }

        public void RenderPlainResponse(RenderContext context, PlainResponse resposne)
        {
            if (resposne == null) return;
            context.Response.ContentType = resposne.ContentType;
            context.Response.StatusCode = resposne.StatusCode;

            context.Response.Body = System.Text.Encoding.UTF8.GetBytes(resposne.Content);
        }
    }
}