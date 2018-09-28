// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Buffers;
using System.IO.Pipelines;
using System.Net;

namespace Kooboo.HttpServer.Http.Http2
{
    public class Http2StreamContext : IHttpProtocolContext
    {
        public string ConnectionId { get; set; }
        public int StreamId { get; set; }
        public ServiceContext ServiceContext { get; set; }
        public MemoryPool MemoryPool { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }
        public IHttp2StreamLifetimeHandler StreamLifetimeHandler { get; set; }
        public IHttp2FrameWriter FrameWriter { get; set; }
        public bool IsHttps { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public HttpFeatures Features { get; set; }
    }
}
