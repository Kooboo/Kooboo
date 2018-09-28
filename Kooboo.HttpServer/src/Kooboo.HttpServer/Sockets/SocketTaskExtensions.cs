// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
    public static class SocketTaskExtensions
    {
        public static Task<Socket> AcceptAsync(this Socket socket)
        {
            return Task<Socket>.Factory.FromAsync(
                (callback, state) => ((Socket)state).BeginAccept(callback, state),
                asyncResult => ((Socket)asyncResult.AsyncState).EndAccept(asyncResult),
                state: socket);
        }
    }
}
