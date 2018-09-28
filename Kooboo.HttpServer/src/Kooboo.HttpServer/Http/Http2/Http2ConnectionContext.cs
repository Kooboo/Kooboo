// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Buffers;
using System.IO.Pipelines;

namespace Kooboo.HttpServer.Http.Http2
{
    public class Http2ConnectionContext
    {
        public HttpConnectionContext Context { get; set; }
        
        /// <summary>
        /// The application endpoint which process data to transport
        /// </summary>
        public IDuplexPipe ToTransport { get; set; }

        /// <summary>
        /// Transport endpoint which process data to application
        /// </summary>
        public IDuplexPipe ToApplication { get; set; }

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
                return Context.MemoryPool;
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

        public HttpFeatures Features => Context.Features;
    }
}
