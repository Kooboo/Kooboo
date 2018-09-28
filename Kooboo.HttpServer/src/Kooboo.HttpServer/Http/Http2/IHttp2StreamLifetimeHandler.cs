// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Kooboo.HttpServer.Http.Http2
{
    public interface IHttp2StreamLifetimeHandler
    {
        void OnStreamCompleted(int streamId);
    }
}
