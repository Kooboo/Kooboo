using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Kooboo.Sites.Payment.Methods.XMLCommon;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class BraintreeService : HttpService
    {
        public string ApiVersion = "5";

        public BraintreeSetting setting;

        public BraintreeService(BraintreeSetting setting) : base()
        {
            this.setting = setting;
        }

        public XmlNode Get(string URL)
        {
            return GetXmlResponse(URL, "GET", null);
        }

        public Task<XmlNode> GetAsync(string URL)
        {
            return GetXmlResponseAsync(URL, "GET", null);
        }

        internal XmlNode Delete(string URL)
        {
            return GetXmlResponse(URL, "DELETE", null);
        }

        internal Task<XmlNode> DeleteAsync(string URL)
        {
            return GetXmlResponseAsync(URL, "DELETE", null);
        }

        public XmlNode Post(string URL, Request requestBody)
        {
            return GetXmlResponse(URL, "POST", requestBody);
        }

        public Task<XmlNode> PostAsync(string URL, Request requestBody)
        {
            return GetXmlResponseAsync(URL, "POST", requestBody);
        }

        internal XmlNode Post(string URL)
        {
            return Post(URL, null);
        }

        internal Task<XmlNode> PostAsync(string URL)
        {
            return PostAsync(URL, null);
        }

        public XmlNode PostMultipart(string URL, Request requestBody)
        {
            return GetXmlResponse(URL, "POST", requestBody);
        }

        public Task<XmlNode> PostMultipartAsync(string URL, Request requestBody)
        {
            return GetXmlResponseAsync(URL, "POST", requestBody);
        }

        public XmlNode Put(string URL)
        {
            return Put(URL, null);
        }

        public Task<XmlNode> PutAsync(string URL)
        {
            return PutAsync(URL, null);
        }

        internal XmlNode Put(string URL, Request requestBody)
        {
            return GetXmlResponse(URL, "PUT", requestBody);
        }

        internal Task<XmlNode> PutAsync(string URL, Request requestBody)
        {
            return GetXmlResponseAsync(URL, "PUT", requestBody);
        }

        public override void SetRequestHeaders(HttpWebRequest request)
        {
            base.SetRequestHeaders(request);
            request.Headers.Add("X-ApiVersion", ApiVersion);
        }

        private XmlNode GetXmlResponse(string url, string method, Request requestBody)
        {
            var request = GetHttpRequest(url, method, null, setting.PrivateKey, setting.PublicKey);

            if (requestBody != null)
            {
                var xmlPrefix = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                byte[] buffer = encoding.GetBytes(xmlPrefix + requestBody.ToXml());
                request.ContentLength = buffer.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(buffer, 0, buffer.Length);
                }
            }

            var response = GetHttpResponse(request);

            return StringToXmlNode(response);

        }

        private async Task<XmlNode> GetXmlResponseAsync(string url, string method, Request requestBody)
        {
            var request = GetHttpRequest(url, method, null, setting.PrivateKey, setting.PublicKey);

            if (requestBody != null)
            {
                var xmlPrefix = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                byte[] buffer = encoding.GetBytes(xmlPrefix + requestBody.ToXml());
                
                request.ContentLength = buffer.Length;
                using (Stream requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    await requestStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }

            var response = await GetHttpResponseAsync(request);

            return StringToXmlNode(response);
        }

        private XmlNode ParseResponseStreamAsXml(Stream stream)
        {
            string body = ParseResponseStream(stream);

            return StringToXmlNode(body);
        }

        internal XmlNode StringToXmlNode(string xml)
        {
            if (xml.Trim() == "")
            {
                return new XmlDocument();
            }
            else
            {
                var doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.LoadXml(xml);
                if (doc.ChildNodes.Count == 1) return doc.ChildNodes[0];
                return doc.ChildNodes[1];
            }
        }
    }
}
