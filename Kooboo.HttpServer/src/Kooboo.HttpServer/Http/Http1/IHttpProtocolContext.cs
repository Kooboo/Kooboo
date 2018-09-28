// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Buffers;
using System.IO.Pipelines;
using System.Net;

namespace Kooboo.HttpServer.Http
{
    public interface IHttpProtocolContext
    {
        string ConnectionId { get;  }
        ServiceContext ServiceContext { get;  }
        HttpFeatures Features { get; }
        MemoryPool MemoryPool { get; }
        IPEndPoint RemoteEndPoint { get; }
        IPEndPoint LocalEndPoint { get; }
        bool IsHttps { get; }
    }
}
