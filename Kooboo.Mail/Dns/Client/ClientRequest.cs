using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using DNS.Protocol;
using DNS.Client.RequestResolver;
using System.Threading.Tasks;

namespace DNS.Client {
    public class ClientRequest : IRequest {
        private const int DEFAULT_PORT = 53;

        private IPEndPoint _dns;
        private IRequestResolver _resolver;
        private IRequest _request;
        public IPEndPoint RemoteEndPoint { get; set; }

        public ClientRequest(IPEndPoint dns, IRequest request = null, IRequestResolver resolver = null) {
            this._dns = dns;
            this._request = request == null ? new Request() : new Request(request);
            this._resolver = resolver ?? new UdpRequestResolver();
        }

        public ClientRequest(IPAddress ip, int port = DEFAULT_PORT, IRequest request = null, IRequestResolver resolver = null) :
            this(new IPEndPoint(ip, port), request, resolver) { }

        public ClientRequest(string ip, int port = DEFAULT_PORT, IRequest request = null, IRequestResolver resolver = null) :
            this(IPAddress.Parse(ip), port, request, resolver) { }

        public int Id {
            get { return _request.Id; }
            set { _request.Id = value; }
        }

        public OperationCode OperationCode {
            get { return _request.OperationCode; }
            set { _request.OperationCode = value; }
        }

        public bool RecursionDesired {
            get { return _request.RecursionDesired; }
            set { _request.RecursionDesired = value; }
        }

        public IList<Question> Questions {
            get { return _request.Questions; }
        }

        public int Size {
            get { return _request.Size; }
        }

        public byte[] ToArray() {
            return _request.ToArray();
        }

        public override string ToString() {
            return _request.ToString();
        }

        public IPEndPoint Dns {
            get { return _dns; }
            set { _dns = value; }
        }

        /// <summary>
        /// Resolves this request into a response using the provided DNS information. The given
        /// request strategy is used to retrieve the response.
        /// </summary>
        /// <exception cref="ResponseException">Throw if a malformed response is received from the server</exception>
        /// <exception cref="IOException">Thrown if a IO error occurs</exception>
        /// <exception cref="SocketException">Thrown if the reading or writing to the socket fails</exception>
        /// <exception cref="OperationCanceledException">Thrown if reading or writing to the socket timeouts</exception>
        /// <returns>The response received from server</returns>
        public async Task<ClientResponse> Resolve() {
            try {
                ClientResponse response = await _resolver.Request(this);

                if (response.Id != this.Id) {
                    throw new ResponseException(response, "Mismatching request/response IDs");
                }
                if (response.ResponseCode != ResponseCode.NoError) {
                    throw new ResponseException(response);
                }

                return response;
            } catch (ArgumentException e) {
                throw new ResponseException("Invalid response", e);
            }
        }
    }
}
