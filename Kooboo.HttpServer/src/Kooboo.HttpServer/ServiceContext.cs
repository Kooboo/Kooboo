// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using Kooboo.HttpServer.Http;

namespace Kooboo.HttpServer
{
    public class ServiceContext
    {
        public HttpServerOptions ServerOptions { get; set; }

        public ILogger Log { get; set; } = new DefaultLogger();

        public KoobooThreadPool ThreadPool { get; set; }

        public IHttpParser<Http1ParsingHandler> HttpParser { get; set; }

        public ISystemClock SystemClock { get; set; }

        public DateHeaderValueManager DateHeaderValueManager { get; set; }

        public HttpConnectionManager ConnectionManager { get; set; }

        public MemoryPool MemoryPool { get; set; }

        public IHttpApplication<HttpContext> Application { get; set; }

    }
}
