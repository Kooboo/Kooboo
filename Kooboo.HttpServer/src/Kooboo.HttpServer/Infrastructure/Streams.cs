// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Kooboo.HttpServer.Http;
using System.Collections.Generic;
using System.Collections;

namespace Kooboo.HttpServer
{
    public class Streams
    {
        private static readonly ThrowingWriteOnlyStream _throwingResponseStream
            = new ThrowingWriteOnlyStream(new InvalidOperationException(CoreStrings.ResponseStreamWasUpgraded));
        private readonly HttpResponseStream _response;
        private readonly HttpRequestStream _request;
        private readonly WrappingStream _upgradeableResponse;
        private readonly HttpRequestStream _emptyRequest;
        private readonly Stream _upgradeStream;

        public Streams( bool AllowSynchronousIO, IHttpResponseControl httpResponseControl)
        {
            _request = new HttpRequestStream(AllowSynchronousIO);
            _emptyRequest = new HttpRequestStream(AllowSynchronousIO);
            _response = new HttpResponseStream(AllowSynchronousIO, httpResponseControl);
            _upgradeableResponse = new WrappingStream(_response);
            _upgradeStream = new HttpUpgradeStream(_request, _response);
        }

        public Stream Upgrade()
        {
            // causes writes to context.Response.Body to throw
            _upgradeableResponse.SetInnerStream(_throwingResponseStream);
            // _upgradeStream always uses _response
            return _upgradeStream;
        }

        public KeyValuePair<Stream, Stream>Start(MessageBody body)
        {
            _request.StartAcceptingReads(body);
            _emptyRequest.StartAcceptingReads(MessageBody.ZeroContentLengthClose);
            _response.StartAcceptingWrites();

            if (body.RequestUpgrade)
            {
                // until Upgrade() is called, context.Response.Body should use the normal output stream
                _upgradeableResponse.SetInnerStream(_response);
                // upgradeable requests should never have a request body
                return new KeyValuePair<Stream, Stream>(_emptyRequest, _upgradeableResponse);
               
            }
            else
            {
                return new KeyValuePair<Stream, Stream>(_request, _response);
            }
        }

        public void Pause()
        {
            _request.PauseAcceptingReads();
            _emptyRequest.PauseAcceptingReads();
            _response.PauseAcceptingWrites();
        }

        public void Stop()
        {
            _request.StopAcceptingReads();
            _emptyRequest.StopAcceptingReads();
            _response.StopAcceptingWrites();
        }

        public void Abort(Exception error)
        {
            _request.Abort(error);
            _emptyRequest.Abort(error);
            _response.Abort();
        }
    }
}
