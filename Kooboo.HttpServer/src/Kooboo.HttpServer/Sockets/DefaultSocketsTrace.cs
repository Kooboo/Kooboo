// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Kooboo.HttpServer.Sockets
{
    public class DefaultSocketsTrace : ISocketsTrace
    {
        public void ConnectionError(string connectionId, Exception ex)
        {
        }

        public void ConnectionPause(string connectionId)
        {
        }

        public void ConnectionReadFin(string connectionId)
        {
        }

        public void ConnectionReset(string connectionId)
        {
        }

        public void ConnectionResume(string connectionId)
        {
        }

        public void ConnectionWriteFin(string connectionId)
        {
        }

        public void LogError(string message, Exception ex)
        {
        }
    }
}
