// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Kooboo.HttpServer.Http.Http2
{
    public partial class Http2Stream : HttpProtocol
    {
        private readonly Http2StreamContext _context;

        public Http2Stream(Http2StreamContext context)
            : base(context)
        {
            _context = context;

            Output = new Http2OutputProducer(StreamId, _context.FrameWriter);
        }

        public int StreamId => _context.StreamId;

        public bool RequestBodyStarted { get; private set; }
        public bool EndStreamReceived { get; private set; }

        protected IHttp2StreamLifetimeHandler StreamLifetimeHandler => _context.StreamLifetimeHandler;

        public override bool IsUpgradableRequest => false;

        protected override void OnReset()
        {
        }

        protected override void OnRequestProcessingEnded()
        {
            StreamLifetimeHandler.OnStreamCompleted(StreamId);
        }

        protected override string CreateRequestId()
            => StringUtilities.ConcatAsHexSuffix(ConnectionId, ':', (uint)StreamId);

        protected override MessageBody CreateMessageBody()
            => Http2MessageBody.For(HttpRequestHeaders, this);

        protected override bool TryParseRequest(ReadResult result, out bool endConnection)
        {
            // We don't need any of the parameters because we don't implement BeginRead to actually
            // do the reading from a pipeline, nor do we use endConnection to report connection-level errors.

            // Todo: consider header api and why :method
            HttpRequestFeature.Method = HttpRequestFeature.Headers[":method"];
            HttpRequestFeature.Scheme = HttpRequestFeature.Headers[":scheme"];
            _httpVersion = Http.HttpVersion.Http2;

            var path = HttpRequestFeature.Headers[":path"].ToString();
            var queryIndex = path.IndexOf('?');

            HttpRequestFeature.Path = queryIndex == -1 ? path : path.Substring(0, queryIndex);
            HttpRequestFeature.QueryString = queryIndex == -1 ? string.Empty : path.Substring(queryIndex);
            HttpRequestFeature.RawTarget = path;

            HttpRequestFeature.Headers["Host"] = HttpRequestFeature.Headers[":authority"];

            endConnection = false;
            return true;
        }

        public async Task OnDataAsync(ArraySegment<byte> data, bool endStream)
        {
            // TODO: content-length accounting
            // TODO: flow-control

            try
            {
                if (data.Count > 0)
                {
                    try
                    {
                        RequestBodyPipe.Writer.Write(data);
                    }
                    finally
                    {
                        RequestBodyPipe.Writer.Commit();
                    }

                    RequestBodyStarted = true;
                    await RequestBodyPipe.Writer.FlushAsync();
                }

                if (endStream)
                {
                    EndStreamReceived = true;
                    RequestBodyPipe.Writer.Complete();
                }
            }
            catch (Exception ex)
            {
                RequestBodyPipe.Writer.Complete(ex);
            }
        }
    }
}
