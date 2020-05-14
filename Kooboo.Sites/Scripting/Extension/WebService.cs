//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Data.Interface;
using Kooboo.Data.Context;
using System.Xml;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Kooboo.Data.Attributes;

namespace Kooboo.Sites.Scripting.Extension
{
    public class WebService: IkScript
    {
        [Attributes.SummaryIgnore]
        public string Name
        {
            get
            {
                return "webService";
            }
        }

        [Attributes.SummaryIgnore]
        [KIgnore]
        public RenderContext context { get; set; }

        public string call(string url, string methodName,object data,string ns = "http://tempuri.org/")
        {
            return call(url, methodName, string.Empty, string.Empty, data);
        }

        public string call(string url,string methodName,string userName,string password, object data, string ns= "http://tempuri.org/")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if (data is System.Dynamic.ExpandoObject)
            {
                IDictionary<String, Object> value = data as IDictionary<String, Object>;
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        if (item.Value != null)
                        {
                            parameters.Add(item.Key, item.Value.ToString());
                        }
                        else
                        {
                            parameters.Add(item.Key, string.Empty);
                        }
                    }
                }

            }
            var header = GetSoapHeader(userName, password,ns);
            XmlDocument xmlDocument = CreateSoapEnvelope(ns, methodName,parameters, header);

            HttpWebRequest webRequest = CreateWebRequest(url, ns, methodName);
            InsertSoapEnvelopeIntoWebRequest(xmlDocument,webRequest);
            //webRequest.Credentials = new NetworkCredential(userName,password);

            string result = "";
            try
            {
                string soapResult = "";
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }

                }
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(soapResult);
                result = JsonConvert.SerializeXmlNode(doc);
            }
            catch (Exception ex)
            {

            }
            
            
            return result;
        }

        private HttpWebRequest CreateWebRequest(string url, string ns,string methodName)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            var action = string.Format("{0}{1}", ns, methodName);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private string GetSoapHeader(string userName,string password,string ns)
        {
            return string.Format(@"<SOAP-ENV:Header>
                             <Authentication xmlns=""{2}"">
                              <User>{0}</User>
                              <Password>{1}</Password>
                             </Authentication>
                            </SOAP-ENV:Header>", userName, password,ns);

        }
        private XmlDocument CreateSoapEnvelope(string ns,string methodName,Dictionary<string, string> parameters,string soapHeader)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();

            StringBuilder builder = new StringBuilder();
            if(parameters!=null && parameters.Count>0)
            {
                foreach (var keyPair in parameters)
                {
                    builder.AppendFormat("  <{0}>{1}</{0}>\r\n", keyPair.Key, keyPair.Value);
                }
            }
            

            //soapEnvelopeDocument.LoadXml(@"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/1999/XMLSchema""><SOAP-ENV:Body><HelloWorld xmlns=""http://tempuri.org/"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><int1 xsi:type=""xsd:integer"">12</int1><int2 xsi:type=""xsd:integer"">32</int2></HelloWorld></SOAP-ENV:Body></SOAP-ENV:Envelope>");
            soapEnvelopeDocument.LoadXml(string.Format(@"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">
                                                {3}
                                                <SOAP-ENV:Body>
                                                    <{0} xmlns=""{1}"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                                                        {2}
                                                    </{0}>
                                            </SOAP-ENV:Body>
                                           </SOAP-ENV:Envelope>",methodName,ns,builder.ToString(), soapHeader));
            return soapEnvelopeDocument;
        }
        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}
