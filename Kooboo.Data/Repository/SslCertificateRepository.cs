//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Kooboo.Data.Repository
{ 

    public class SslCertificateRepository : RepositoryBase<SslCertificate>
    {
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.SetPrimaryKeyField<SslCertificate>(o => o.Id);
                paras.AddColumn<SslCertificate>(o => o.OrganizationId); 
                return paras;
            }
        }

        public void AddCert(Guid Organizationid, string domain, byte[] cert)
        {
            bool iswildcard = false; 
            if (domain.StartsWith("*."))
            {
                domain = domain.Substring(2);
                iswildcard = true; 
            }
             
            var certificate = new X509Certificate2(cert, "kooboo");

            SslCertificate ssl = new SslCertificate();
            ssl.Domain = domain;
            ssl.IsWildCard = iswildcard;
            ssl.Expiration = certificate.NotAfter; 
            ssl.OrganizationId = Organizationid; 
            ssl.Content = cert; 
            this.AddOrUpdate(ssl);
        }

        public SslCertificate GetByDomain(string fulldomain)
        { 
            if (fulldomain == null)
            {
                return null; 
            } 
            fulldomain = fulldomain.Trim(); 

            var id = Lib.Security.Hash.ComputeGuidIgnoreCase(fulldomain); 
 
            var item =  this.Get(id);  
            if (item !=null)
            {
                return item; 
            }

            var index = fulldomain.IndexOf("."); 
            while(index >-1)
            {
                fulldomain = fulldomain.Substring(index + 1);
                var domainid = Lib.Security.Hash.ComputeGuidIgnoreCase(fulldomain);
                var ssl = this.Get(domainid); 
                if (ssl !=null && ssl.IsWildCard)
                {
                    return ssl; 
                }
                index = fulldomain.IndexOf("."); 
            }

            return null; 
        }
         
        public List<SslCertificate> ListByOrganization(Guid OrganizationId)
        {
            return this.Query.Where(o => o.OrganizationId == OrganizationId).SelectAll();    
        }
    } 

}



