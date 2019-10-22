using System.Collections.Generic;
using System.Collections.ObjectModel;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;

namespace DNS.Client {
    public class ClientResponse : IResponse {
        private Response _response;
        private byte[] _message;

        public static ClientResponse FromArray(ClientRequest request, byte[] message) {
            Response response = Response.FromArray(message);
            return new ClientResponse(request, response, message);
        }

        internal ClientResponse(ClientRequest request, Response response, byte[] message) {
            Request = request;

            this._message = message;
            this._response = response;
        }

        internal ClientResponse(ClientRequest request, Response response) {
            Request = request;

            this._message = response.ToArray();
            this._response = response;
        }

        public ClientRequest Request {
            get;
            private set;
        }

        public int Id {
            get { return _response.Id; }
            set { }
        }

        public IList<IResourceRecord> AnswerRecords {
            get { return _response.AnswerRecords; }
        }

        public IList<IResourceRecord> AuthorityRecords {
            get { return new ReadOnlyCollection<IResourceRecord>(_response.AuthorityRecords); }
        }

        public IList<IResourceRecord> AdditionalRecords {
            get { return new ReadOnlyCollection<IResourceRecord>(_response.AdditionalRecords); }
        }

        public bool RecursionAvailable {
            get { return _response.RecursionAvailable; }
            set { }
        }

        public bool AuthorativeServer {
            get { return _response.AuthorativeServer; }
            set { }
        }

        public bool Truncated {
            get { return _response.Truncated; }
            set { }
        }

        public OperationCode OperationCode {
            get { return _response.OperationCode; }
            set { }
        }

        public ResponseCode ResponseCode {
            get { return _response.ResponseCode; }
            set { }
        }

        public IList<Question> Questions {
            get { return new ReadOnlyCollection<Question>(_response.Questions); }
        }

        public int Size {
            get { return _message.Length; }
        }

        public byte[] ToArray() {
            return _message;
        }

        public override string ToString() {
            return _response.ToString();
        }
    }
}
