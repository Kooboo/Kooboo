//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using Kooboo.Data.Repository;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Schedule;

namespace Kooboo.Data
{
    public static class GlobalDb
    {
        private static object _lock = new object();

        private static BindingRepository _bindings;
        public static BindingRepository Bindings
        {
            get
            {
                return EnsureRepository<BindingRepository, Binding>(ref _bindings);
            }
        }

        private static OrganizationRepository _organization;

        public static OrganizationRepository Organization
        {
            get
            {
                if (_organization == null)
                {
                    lock (_lock)
                    {
                        if (_organization == null)
                        {
                            _organization = new OrganizationRepository();
                        }
                    }
                }
                return _organization;
            }
        }

        private static TaskQueueRepository _queue;

        public static TaskQueueRepository TaskQueue
        {
            get
            {
                return EnsureRepository<TaskQueueRepository, Queue>(ref _queue);
            }
        }


        private static IDomainRepository _domains;

        public static IDomainRepository Domains
        {
            get
            {
                if (_domains == null)
                {
                    _domains = Lib.IOC.Service.GetSingleTon<IDomainRepository>(typeof(LocalDomainRepository)); 
                }
                return _domains;
            }
            set
            {
                _domains = value; 
            }
        }

        private static WebSiteRepository _Website;

        public static WebSiteRepository WebSites
        {
            get
            {
                return EnsureRepository<WebSiteRepository, WebSite>(ref _Website);
            }
        }

        private static UserRepository _users;

        public static UserRepository Users
        {
            get
            {
                if (_users == null)
                {
                    lock (_lock)
                    {
                        if (_users == null)
                        {
                            _users = new UserRepository();
                        }
                    }
                }
                return _users;
            }
        }
         
        private static LocalUserRepository _localuser;
        public static LocalUserRepository LocalUser
        {
            get
            {
                if (_localuser == null)
                {
                    lock (_lock)
                    {
                        if (_localuser == null)
                        {
                            _localuser = new LocalUserRepository();
                        }
                    }
                }
                return _localuser;
            }
        }

        private static DllRepository _dlls;

        public static DllRepository Dlls
        {
            get
            {
                return EnsureRepository<DllRepository, Dll>(ref _dlls);
            }
        }

        private static DataMethodRepository _datamethods;

        public static DataMethodRepository DataMethodSettings
        {
            get
            {
                return EnsureRepository<DataMethodRepository, DataMethodSetting>(ref _datamethods);
            }
        }

        private static GlobalSettingRepository _GlobalSetting;
        public static GlobalSettingRepository GlobalSetting
        {
            get
            {
                return EnsureRepository<GlobalSettingRepository, GlobalSetting>(ref _GlobalSetting);
            }
        }

        private static ClusterRepository _cluster;

        public static ClusterRepository Cluster
        {
            get
            {
                return EnsureRepository<ClusterRepository, Cluster>(ref _cluster);
            }
        }


        private static SslCertificateRepository _sslcert;

        public static SslCertificateRepository SslCertificate
        {
            get
            {
                return EnsureRepository<SslCertificateRepository, SslCertificate>(ref _sslcert);
            }
        }

        private static Schedule<Job> _schedule;

        public static Schedule<Job> ScheduleJob()
        {
            if (_schedule == null)
            {
                lock (_lock)
                {
                    if (_schedule == null)
                    {
                        _schedule = new Schedule<Job>("ScheduleTask");
                    }
                }
            }
            return _schedule;
        }

        private static RepeatTask<Job> _repeating;
        public static RepeatTask<Job> RepeatingJob()
        {
            if (_repeating == null)
            {
                lock (_lock)
                {
                    if (_repeating == null)
                    {
                        _repeating = new IndexedDB.Schedule.RepeatTask<Job>("RepeatTask");
                    }
                }
            }
            return _repeating;
        }

        private static Sequence<JobLog> _joblog;

        public static Sequence<JobLog> JobLog()
        {
            string storename = typeof(JobLog).Name;

            if (_joblog == null)
            {
                lock (_lock)
                {
                    if (_joblog == null)
                    {
                        _joblog = new Sequence<JobLog>(storename);
                    }
                }
            }

            return _joblog;
        }

        private static NotificationRepository _notification;

        public static NotificationRepository Notification
        {
            get
            {
                return EnsureRepository<NotificationRepository, Notificationnew>(ref _notification);
            }
        }

        private static TRepository EnsureRepository<TRepository, TGlobalObject>(ref TRepository repository)
          where TRepository : RepositoryBase<TGlobalObject>
          where TGlobalObject : IGolbalObject
        {
            if (repository == null)
            {
                lock (_lock)
                {
                    if (repository == null)
                    {
                        repository = Activator.CreateInstance<TRepository>();
                    }
                }
            }
            return repository;
        }

    }
}
