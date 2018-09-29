using Kooboo.Sites.SiteTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using System.Net; 

namespace Kooboo.Sites.Repository
{
   public class TransferTaskRepository : SiteRepositoryBase<TransferTask>
    {
        internal override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<TransferTask>(o => o.done);
                paras.AddColumn<TransferTask>(o => o.CreationDate);
                paras.SetPrimaryKeyField<TransferTask>(o => o.Id); 
                return paras;
            }
        }

        public void SetDone(Guid TaskId)
        {
            var task = this.Get(TaskId); 
            if (task!= null)
            {
                task.done = true;
                this.AddOrUpdate(task); 
            }
        }
          
        public   bool IsInProgress()
        {
            foreach (var item in this.All())
            {
                if (item.done == false  && item.CreationDate > DateTime.Now.AddMinutes(-30))
                {
                    return true; 
                } 
            } 
            return false;   
        }


        public override bool AddOrUpdate(TransferTask value, Guid UserId = default(Guid))
        {
            if (value.Domains.Count() == 0)
            {
                if (!string.IsNullOrEmpty(value.FullStartUrl))
                {
                    value.Domains.Add(UrlHelper.UriHost(value.FullStartUrl, true)); 
                }
            }
            return base.AddOrUpdate(value, UserId);
        }

        /// <summary>
        /// stop all download tasks of this website. 
        /// </summary>
        /// <param name="website"></param>
        public  void CancelDownload()
        { 
            var alltask = this.TableScan.Where(o => o.done == false && o.CreationDate > DateTime.Now.AddMinutes(-30)).SelectAll();  

            foreach (var item in alltask)
            {
                item.done = true;
               this.AddOrUpdate(item); 
            }

            var allpages = this.SiteDb.TransferPages.TableScan.Where(o => o.done == false).SelectAll();

            foreach (var item in allpages)
            {
                item.done = true;
                this.SiteDb.TransferPages.AddOrUpdate(item);
            }
        }

        public   string FirstImportHost()
        {
            var all = this.All();  

            foreach (var item in all.OrderBy(o=>o.CreationDate))
            {
                foreach (var domain in item.Domains)
                {
                    if (!string.IsNullOrEmpty(domain))
                    {
                        if (!domain.ToLower().StartsWith("http://") && !domain.ToLower().StartsWith("https://"))
                        {
                            return "http://" + domain; 
                        }
                        else
                        {
                            return domain; 
                        }
                    }   
                }
            }

            foreach (var item in all.OrderBy(o => o.CreationDate))
            {
                if (!string.IsNullOrEmpty(item.FullStartUrl))
                {
                    return item.FullStartUrl; 
                }
            }

            return string.Empty;
        }

        public HashSet<string> History()
        {
            HashSet<string> result = new HashSet<string>(); 
             
            var all = this.All();

            foreach (var item in all.OrderBy(o => o.CreationDate))
            {
                if (item.Domains.Count()>0)
                {
                    foreach (var domain in item.Domains)
                    {
                        if (!string.IsNullOrEmpty(domain))
                        {
                            if (!domain.ToLower().StartsWith("http://") && !domain.ToLower().StartsWith("https://"))
                            {
                                if (!string.IsNullOrEmpty(item.FullStartUrl))
                                {
                                    if (item.FullStartUrl.ToLower().StartsWith("https"))
                                    {
                                        result.Add("https://" + domain);
                                    }
                                    else
                                    {
                                        result.Add("http://" + domain);
                                    }
                                }
                                else
                                {
                                    result.Add("http://" + domain);
                                }
                            }
                            else
                            {
                                  result.Add(domain);
                            }
                        } 
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.FullStartUrl))
                    {
                        result.Add(item.FullStartUrl); 
                    }
                }
 
            }

            return result;  

        }
        
        public void UpdateCookie(Guid taskid, System.Net.CookieContainer cookiecontainer)
        {
            var task = this.Get(taskid); 
            if (task !=null)
            {
                var uri = new Uri(task.FullStartUrl);
                CookieCollection cookies = cookiecontainer.GetCookies(uri);
          
                if (cookies !=null )
                {
                    bool hascookie = false; 
                    foreach (Cookie item in cookies)
                    {
                        hascookie = true; 
                        if (!item.Expired)
                        {
                            task.cookies[item.Name] = item.Value; 
                        }
                        else
                        {
                            task.cookies.Remove(item.Name); 
                        }
                    } 

                    if (hascookie)
                    {
                        this.AddOrUpdate(task);
                    }
                } 
         
            }
        }
        
        public CookieContainer GetCookieContainer(Guid TaskId)
        { 
            var task = this.Get(TaskId);
            return GetCookieContainer(task);
        }

        private static CookieContainer GetCookieContainer(TransferTask task)
        {
            CookieContainer container = new CookieContainer();

            if (task != null)
            {
                var uri = new Uri(task.FullStartUrl);

                foreach (var item in task.cookies)
                {
                    container.Add(new Cookie() { Name = item.Key, Value = item.Value, Domain = uri.Host });

                }
            }

            return container;
        }

        public CookieContainer GetCookieContainer(string Domain)
        {
            if (string.IsNullOrWhiteSpace(Domain))
            {
                return null; 
            }
        
            var all = this.All();

            foreach (var item in all)
            {
                if (item.Domains.Contains(Domain, StringComparer.OrdinalIgnoreCase))
                {
                    return GetCookieContainer(item); 
                }
            }

            return null; 
        }

        public CookieContainer GetCookieContainerByFullUrl(string fullurl)
        {
            if (!string.IsNullOrWhiteSpace(fullurl) && Uri.IsWellFormedUriString(fullurl, UriKind.Absolute))
            {
                var uri = new Uri(fullurl); 
                if (uri !=null)
                {
                    return GetCookieContainer(uri.Host); 
                }
            } 
            return null; 
        }
    } 
}
