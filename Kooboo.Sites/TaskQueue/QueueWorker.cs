//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface; 
using System; 

namespace Kooboo.Sites.TaskQueue
{
    public class QueueWorker : IBackgroundWorker
    {
        public int Interval
        {
            get
            {
                return 20;
            }
        }

        public DateTime LastExecute
        {
            get; set;
        }
  
        public void Execute()
        {
            //if (WebSiteId != default(Guid))
            //{
            //    QueueManager.Execute(WebSiteId);
            //}
            //else
            //{
                QueueManager.Execute();
            //} 
        }
    } 
}
