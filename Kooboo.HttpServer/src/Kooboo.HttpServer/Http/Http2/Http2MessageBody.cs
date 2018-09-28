// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Kooboo.HttpServer.Http.Http2
{
    public abstract class Http2MessageBody : MessageBody
    {
        private readonly Http2Stream _context;

        protected Http2MessageBody(Http2Stream context)
            : base(context)
        {
            _context = context;
        }

        protected override void OnReadStarted()
        {
            // Produce 100-continue if no request body data for the stream has arrived yet.
            if (!_context.RequestBodyStarted)
            {
                TryProduceContinue();
            }
        }

        protected override Task OnConsumeAsync() => TaskHelpers.CompletedTask();

        public override Task StopAsync()
        {
            _context.RequestBodyPipe.Reader.Complete();
            _context.RequestBodyPipe.Writer.Complete();
            return TaskHelpers.CompletedTask();
        }

        public static MessageBody For(
            HttpRequestHeaders headers,
            Http2Stream context)
        {
            if (context.EndStreamReceived)
            {
                return ZeroContentLengthClose;
            }

            return new ForHttp2(context);
        }

        private class ForHttp2 : Http2MessageBody
        {
            public ForHttp2(Http2Stream context)
                : base(context)
            {
            }
        }
    }
}
