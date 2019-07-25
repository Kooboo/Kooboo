//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;

namespace Kooboo.Jobs
{
    public static class JobContainer
    {
        static JobContainer()
        {
            JobList = new List<IJob>();

            var alltypes = AssemblyLoader.LoadTypeByInterface(typeof(IJob));

            foreach (var item in alltypes)
            {
                var jobinstance = (IJob)Activator.CreateInstance(item);
                JobList.Add(jobinstance);
            }


            BackgroundWorkers = new List<IBackgroundWorker>();

            var allbacktypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IBackgroundWorker));

            foreach (var item in allbacktypes)
            {
                var backinstance = (IBackgroundWorker)Activator.CreateInstance(item);
                BackgroundWorkers.Add(backinstance);
            }

        }


        private static object _locker = new object();

        public static IJob GetJob(string name)
        {
            var find = JobList.Find(o => o.Name == name);
            if (find != null)
            {
                return find;
            }
            return null;
        }

        public static List<IJob> JobList
        {
            get; set;
        } 

        public static List<IBackgroundWorker> BackgroundWorkers
        {
            get; set;
        }
    }
     
}
