// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using Kooboo.HttpServer;

namespace Kooboo.HttpServer.Http
{
    public class Http1ConnectionContext : IHttpProtocolContext
    {
        public HttpConnectionContext Context { get; set; }

        public ServiceContext ServiceContext
        {
            get
            {
                return Context.ServiceContext;
            }
        }

        public MemoryPool MemoryPool
        {
            get
            {
                return Context.ServiceContext.MemoryPool;
            }
        }

        public string ConnectionId
        {
            get
            {
                return Context.ConnectionId;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return Context.RemoteEndPoint;
            }
        }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                return Context.LocalEndPoint;
            }
        }

        public ITimeoutControl TimeoutControl { get; set; }
        
        public IDuplexPipe ToTransport { get; set; }

        public IDuplexPipe ToApplication { get; set; }

        public bool IsHttps
        {
            get
            {
                return ServiceContext.ServerOptions.IsHttps;
            }
        }

        public HttpFeatures Features => Context.Features;
    }
}
