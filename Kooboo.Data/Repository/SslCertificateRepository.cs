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

        public void AddCert(Guid Organizationid, string domain, byte[] cert, bool IsWildcard = false)
        {
            var certificate = new X509Certificate2(cert, "kooboo");

            SslCertificate ssl = new SslCertificate();
            ssl.Domain = domain;
            ssl.IsWildCard = IsWildcard;
            ssl.Expiration = certificate.NotAfter; 
            ssl.OrganizationId = Organizationid; 
            ssl.Content = cert; 
            this.AddOrUpdate(ssl);
        }

        public SslCertificate GetByDomain(string Domain)
        { 
            var id = Lib.Security.Hash.ComputeGuidIgnoreCase(Domain); 
            // TODO: should request for root domain with wildcard next...
            return this.Get(id);  
        }


        public List<SslCertificate> ListByOrganization(Guid OrganizationId)
        {
            return this.Query.Where(o => o.OrganizationId == OrganizationId).SelectAll();    
        }
    } 

}



