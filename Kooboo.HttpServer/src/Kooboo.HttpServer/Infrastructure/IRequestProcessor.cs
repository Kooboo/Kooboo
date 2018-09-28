// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Kooboo.HttpServer
{
    public interface IRequestProcessor
    {
        Task ProcessRequestsAsync<TContext>(IHttpApplication<TContext> application);
        void StopProcessingNextRequest();
        void Abort(Exception ex);
    }
}