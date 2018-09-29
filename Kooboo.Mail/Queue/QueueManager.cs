//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Mail.Queue.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Mail.Queue
{
    public static class QueueManager
    {
        static QueueManager()
        {
            string root = Data.AppSettings.RootPath;
            string mailingroot = System.IO.Path.Combine(root, "Mailing");
            QueueStore = new DiskQueueStore(mailingroot);   
        }

        public static SemaphoreSlim Locker = new SemaphoreSlim(1, 1);

        public static DiskQueueStore QueueStore { get; set; }

        private static bool IsRunning = false;
          
        public static void AddSendQueue(string mailfrom, string rcptto, string messagebody)
        {
            OutGoing input = new OutGoing() { MailFrom = mailfrom, RcptTo = rcptto, MsgBody = messagebody };
            AddQueue(input);
        }

        public static void AddQueue(object input)
        {
            Kooboo.Data.Models.MailQueue value = new Data.Models.MailQueue();
            value.TaskModelType = input.GetType().FullName;
            value.JsonModel = Lib.Helper.JsonHelper.Serialize(input);

            QueueStore.AddQueue(value);   
        }

        public static void AddGroupMail(GroupMail input)
        {
            AddQueue(input);
        }
         
        public static async Task Execute()
        {
            if (!IsRunning)
            {
                await Locker.WaitAsync();

                try
                { 
                    if (!IsRunning)
                    {
                        IsRunning = true;
                        var queueitems = QueueStore.AllItems();  

                        foreach (var item in queueitems)
                        {
                            var executor = Mail.Queue.Executor.TaskExecutorContainer.GetExecutor(item.TaskModelType); 
                            if (executor != null)
                            { 
                                try
                                { 
                                    var response = await executor.Execute(item.JsonModel);

                                    if (response.Success)
                                    {
                                        QueueStore.SendOk(item); 
                                    }
                                    else if (!response.ShouldRetry)
                                    {       
                                        QueueStore.SendFailNoRetry(item);   
                                        await executor.SendFail(item.JsonModel, response.Message);
                                    }
                                    else
                                    {     
                                        var faileresult = QueueStore.SendFail(item); 

                                        if (faileresult.WillRetry == false)
                                        {
                                            await executor.SendFail(item.JsonModel, response.Message);
                                        }  
                                    } 

                                }
                                catch (Exception ex)
                                {
                                    QueueStore.SendFailNoRetry(item); 
                                    await executor.SendFail(item.JsonModel, ex.Message);   
                                }  
                            }
                             
                        }

                        IsRunning = false;
                    }
                }
                finally
                {
                    Locker.Release();
                }
            }
        }
                
    }
}
