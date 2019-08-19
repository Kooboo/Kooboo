//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
  
#if !NETSTANDARD2_0
 
using Kooboo.HttpServer; 
using System.Collections.Generic;

namespace Kooboo.Data.Server
{   
    public class WebServer : IWebServer
    {
        private int port = 80;
        private List<IKoobooMiddleWare> MiddleWares = new List<IKoobooMiddleWare>();
        private IKoobooMiddleWare StartWare = null;
         
        public WebServer(int Port)
        {
            this.port = Port;

            bool https = false;

            if (Port == 443)
            {
                https = true;
            }
            var options = new HttpServerOptions()
            {
                HttpHandler = new ServerHandler(r => this.StartWare.Invoke(r)),
                SelectCertificate = Kooboo.Data.Server.SslCertificateProvider.SelectCertificate,  
                IsHttps = https
            };

            this.Server = new Kooboo.HttpServer.HttpServer(new System.Net.IPEndPoint(System.Net.IPAddress.Any, this.port), options);
        }

        public WebServer(int Port, bool IsHttps)
        {
            this.port = Port;

            bool https = IsHttps;
             
            var options = new HttpServerOptions()
            {
                HttpHandler = new ServerHandler(r => this.StartWare.Invoke(r)),
                SelectCertificate = Kooboo.Data.Server.SslCertificateProvider.SelectCertificate, 
                IsHttps = https
            };

            this.Server = new Kooboo.HttpServer.HttpServer(new System.Net.IPEndPoint(System.Net.IPAddress.Any, this.port), options);
        }

        public void SetMiddleWares(List<IKoobooMiddleWare> middlewares)
        {
            this.MiddleWares = middlewares;
            OrganizeChain();
        }

        private void OrganizeChain()
        {
            bool HasEnd = false;
            foreach (var item in this.MiddleWares)
            {
                if (item.GetType() == typeof(EndMiddleWare))
                {
                    HasEnd = true;
                }
            }
            if (!HasEnd)
            {
                this.MiddleWares.Add(new EndMiddleWare());
            }

            int count = this.MiddleWares.Count;
            for (int i = 0; i < count; i++)
            {
                if (i < count - 1)
                {
                    this.MiddleWares[i].Next = this.MiddleWares[i + 1];
                }
            }
            this.StartWare = this.MiddleWares[0];
        }
           
        public Kooboo.HttpServer.HttpServer Server { get; set; }
         
        /// <summary>
        /// Starts the listener and request processing threads.
        /// </summary>
        public void Start()
        {
            this.Server.Start();  
        } 

        public void Stop()
        { 
           
           
            // TODO:  I also need a stop option... 
        }
        
         
    }
      
}

#endif