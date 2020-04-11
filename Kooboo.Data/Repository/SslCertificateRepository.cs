//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
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

            if (certificate.Subject.Contains("*"))
            {
                iswildcard = true;

                string cn = GetCommonName(certificate.Subject);
                if (!string.IsNullOrWhiteSpace(cn))
                {
                    domain = cn;
                }
            }

            SslCertificate ssl = new SslCertificate();
            ssl.Domain = domain;
            ssl.IsWildCard = iswildcard;
            ssl.Expiration = certificate.NotAfter;
            ssl.OrganizationId = Organizationid;
            ssl.Content = cert;
            this.AddOrUpdate(ssl);
        }

        public string GetCommonName(string subject)
        {
            int index = subject.IndexOf("=");
            if (index > 0)
            {
                string name = subject.Substring(index + 1).Trim();
                if (name.StartsWith("*."))
                {
                    return name.Substring(2);
                }
            }
            return null;
        }

        public SslCertificate GetByDomain(string fulldomain)
        {
            if (fulldomain == null)
            {
                return null;
            }
            fulldomain = fulldomain.Trim();

            var id = Lib.Security.Hash.ComputeGuidIgnoreCase(fulldomain);

            var item = this.Get(id);
            if (item != null)
            {
                return item;
            }

            var index = fulldomain.IndexOf(".");
            while (index > -1)
            {
                fulldomain = fulldomain.Substring(index + 1);
                var domainid = Lib.Security.Hash.ComputeGuidIgnoreCase(fulldomain);
                var ssl = this.Get(domainid);
                if (ssl != null && ssl.IsWildCard)
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

        public List<SslCertificate> GetAllInUsed()
        {
            var all = this.All(); 
            List<SslCertificate> result = new List<SslCertificate>();

            foreach (var item in all)
            {
                if (item.IsWildCard)
                {
                    //wildcard always renew...
                    result.Add(item);
                }
                else
                {
                    var binding = GlobalDb.Bindings.GetByFullDomain(item.Domain);
                    if (!binding.Any())
                    {
                        binding = GlobalDb.Bindings.GetByDomain(item.Domain);  
                    }
                    if (binding.Any())
                    {
                        result.Add(item);
                    } 
                }
            } 
            return result;
        }
    }

}



