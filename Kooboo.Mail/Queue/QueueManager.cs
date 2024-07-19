//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Kooboo.Mail.Queue.Model;

namespace Kooboo.Mail.Queue
{
    public static class QueueManager
    {
        static QueueManager()
        {
            string root = Data.AppSettings.RootPath;
            string mailingRoot = Path.Combine(root, "Mailing");
            if (!Directory.Exists(mailingRoot))
            {
                mailingRoot = Path.Combine(Data.AppSettings.AppDataFolder, "Mailing");
            }
            QueueStore = new DiskQueueStore(mailingRoot);
        }

        public static SemaphoreSlim Locker = new SemaphoreSlim(1, 1);

        public static DiskQueueStore QueueStore { get; set; }

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
            if (0 == Interlocked.Exchange(ref beingUsed, 1))
            {
                try
                {
                    while (true)
                    {
                        var items = QueueStore.AllItems();
                        if (items.Count == 0) break;
                        var queue = new ConcurrentQueue<Data.Models.MailQueue>(items);

                        while (!queue.IsEmpty)
                        {
                            await ParallelExecuteAsync(queue);
                        }
                    }
                }
                catch
                {

                }

                Interlocked.Exchange(ref beingUsed, 0);
            }
        }

        // Task.Run(() => BackgroundTask("TPL")); 

        public static async Task ParallelExecuteAsync(ConcurrentQueue<Data.Models.MailQueue> queue, int parallelCount = 10)
        {
            var list = new List<Data.Models.MailQueue>();

            for (int i = 0; i < parallelCount; i++)
            {
                if (queue.TryDequeue(out var mailQueue))
                {
                    list.Add(mailQueue);
                }
                else
                {
                    break;
                }
            }

            await Parallel.ForEachAsync(list, async (item, token) =>
            {
                var executor = Executor.TaskExecutorContainer.GetExecutor(item.TaskModelType);
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
                            var failResult = QueueStore.SendFail(item);

                            if (failResult.WillRetry == false)
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
            });
        }

        private static int beingUsed = 0;
    }
}
