//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Context;

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

        public string Prefix { get; set; }

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
            set { _beforeapi = value; }
        }

        public Action<Kooboo.Data.Context.RenderContext, IResponse> Log { get; set; }

        public IApiProvider ApiProvider { get; set; }

        public IKoobooMiddleWare Next { get; set; }

        public async Task Invoke(RenderContext context)
        {
            context.Request.Channel = RequestChannel.API;

            if (context.IsOptionsRequest())
            {
                context.EnableCORS();
                context.Response.StatusCode = 200;
                return;
            }

            if (!string.IsNullOrEmpty(this.Prefix) && !context.Request.RawRelativeUrl.ToLower().StartsWith(this.Prefix))
            {
                await Next.Invoke(context);
                return;
            }

            if (!ApiRoute.ApiRouting.TryParseCommand(context.Request.RelativeUrl, context.Request.Method,
                    out var command, BeforeApi))
            {
                context.Response.StatusCode = 500;
                context.Response.Body = System.Text.Encoding.UTF8.GetBytes("Invalid Api command");

                if (Log != null)
                {
                    Log(context, null);
                }

                return;
            }

            ApiCall apiCall = new ApiCall() { Command = command, Context = context };

            var response = ApiManager.Execute(apiCall, this.ApiProvider);

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

            if (response is BinaryResponse)
            {
                RenderBinary(context, response as BinaryResponse);
            }
            else if (response is NoResponse)
            {
                // do nothing. 
            }
            else if (response is PlainResponse)
            {
                RenderPlainResponse(context, response as PlainResponse);
            }
            else if (response is JsonResponse jsonResponse && command.Version == ApiVersion.V2)
            {
                if (response.Success)
                {
                    var jsonbody = Lib.Helper.JsonHelper.Serialize(response.Model);
                    context.Response.ContentType = "application/json";

                    //context.Response.Body = System.Text.Encoding.UTF8.GetBytes(jsonbody);
                    SetTextResponseBytes(context, System.Text.Encoding.UTF8.GetBytes(jsonbody));

                    context.Response.StatusCode = jsonResponse.HttpCode ?? 200;
                }
                else
                {
                    var jsonbody = Lib.Helper.JsonHelper.Serialize(response.Messages);
                    context.Response.ContentType = "application/json";
                    SetTextResponseBytes(context, System.Text.Encoding.UTF8.GetBytes(jsonbody));
                    context.Response.StatusCode = jsonResponse.HttpCode ?? 400;
                }
            }

            else if (response is JsonTextResponse JsonTextRes)
            {
                if (response.Success)
                {
                    var jsonbody = JsonTextRes.Model.ToString();
                    context.Response.ContentType = "application/json";
                    SetTextResponseBytes(context, System.Text.Encoding.UTF8.GetBytes(jsonbody));
                    context.Response.StatusCode = JsonTextRes.HttpCode ?? 200;
                }
                else
                {
                    var jsonbody = Lib.Helper.JsonHelper.Serialize(response.Messages);
                    context.Response.ContentType = "application/json";
                    SetTextResponseBytes(context, System.Text.Encoding.UTF8.GetBytes(jsonbody));
                    context.Response.StatusCode = JsonTextRes.HttpCode ?? 400;
                }
            }
            else
            {
                MultilingualService.EnsureLangText(response, context);
                var jsonbody = Lib.Helper.JsonHelper.Serialize(response);
                context.Response.ContentType = "application/json";

                SetTextResponseBytes(context, System.Text.Encoding.UTF8.GetBytes(jsonbody));

                if (context.Response.StatusCode < 200)
                {
                    context.Response.StatusCode = 200;
                }

            }

            foreach (var item in DefaultHeaders)
            {
                if (!context.Response.Headers.ContainsKey(item.Key))
                {
                    context.Response.Headers.Add(item.Key, item.Value);
                }
            }
        }


        private object _locker = new object();
        private Dictionary<string, string> _defaultheaders;

        public Dictionary<string, string> DefaultHeaders
        {
            get
            {
                if (_defaultheaders == null)
                {
                    lock (_locker)
                    {
                        if (_defaultheaders == null)
                        {
                            var header = new Dictionary<string, string>();
                            header.Add("Access-Control-Allow-Origin", "*");
                            header.Add("Access-Control-Allow-Headers", "*");
                            //  header.Add("Pragma", "no-cache");  //why we need this?
                            header.Add("Cache-Control", "private, no-cache, no-store, proxy-revalidate, no-transform");

                            _defaultheaders = header;
                        }
                    }


                }

                return _defaultheaders;
            }
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

            if (resposne.Stream != null)
            {
                context.Response.Stream = resposne.Stream;
            }
            else
            {
                context.Response.Body = resposne.BinaryBytes;
            }
        }

        public void RenderPlainResponse(RenderContext context, PlainResponse resposne)
        {
            if (resposne == null)
            {
                return;
            }

            context.Response.ContentType = resposne.ContentType;
            context.Response.StatusCode = resposne.statusCode;

            // context.Response.Body = System.Text.Encoding.UTF8.GetBytes(resposne.Content); 
            SetTextResponseBytes(context, System.Text.Encoding.UTF8.GetBytes(resposne.Content ?? string.Empty));
        }

        public void SetTextResponseBytes(RenderContext context, byte[] BinaryBytes)
        {
            //Check if client support GZIP. 
            bool gzipSupported = false;
            var acceptEncoding = context.Request.Headers.Get("Accept-Encoding");
            if (acceptEncoding != null)
            {
                gzipSupported = acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate");
            }

            int ByteLen = BinaryBytes.Length;

            if (!gzipSupported || ByteLen < 4096)
            {
                context.Response.Body = BinaryBytes;
            }
            else
            {
                // handle GZIP. 
                byte[] gzipBytes;
                using (var stream = new System.IO.MemoryStream())
                {
                    using (System.IO.Compression.GZipStream gZipStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionLevel.Fastest))
                    {
                        gZipStream.Write(BinaryBytes, 0, BinaryBytes.Length);
                    }
                    gzipBytes = stream.ToArray();
                }
                context.Response.Body = gzipBytes;
                context.Response.Headers.Add("Content-encoding", "gzip");
            }
        }
    }
}