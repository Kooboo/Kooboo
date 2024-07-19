//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.Queue;

namespace Kooboo.IndexedDB
{
    /// <summary>
    /// queue is a fast storage.
    /// queue only has in and out, sequence write to disk , and sequence get out. 
    /// </summary>
    public class Queue<TValue> : IQueue
    {

        private static object _object = new object();

        private Dictionary<int, QueueContent<TValue>> _queueContentDictionary;
        private Dictionary<int, QueueList> _queueListDictionary;

        /// <summary>
        /// the Int date representation of current dequeue reading file.
        /// </summary>
        internal int _dequeueDateInt;
        internal int _dequeueDateCounter;

        private string queueFolder;

        private string queueContentExtension = ".content";
        private string queueListExtension = ".list";

        private string queuename;

        public SortedSet<int> queueFileIdList;

        public Queue(string queuename)
        {
            this.queuename = queuename;

            string globalqueuefolder = System.IO.Path.Combine(GlobalSettings.RootPath, GlobalSettings.QueuePath);

            this.queueFolder = System.IO.Path.Combine(globalqueuefolder, queuename.ToValidPath());

            if (!System.IO.Directory.Exists(this.queueFolder))
            {
                System.IO.Directory.CreateDirectory(this.queueFolder);
            }

            this._queueContentDictionary = new Dictionary<int, QueueContent<TValue>>();
            this._queueListDictionary = new Dictionary<int, QueueList>();


            this.queueFileIdList = new SortedSet<int>();

            // check for the last reading. 
            _init();
        }


        /// <summary>
        /// check, remove old files and set the start point for next dequeue. 
        /// </summary>
        internal void _init()
        {
            // get list of all files in queue. 

            foreach (var item in System.IO.Directory.GetFiles(this.queueFolder))
            {
                if (item.EndsWith(this.queueListExtension))
                {
                    int lastslash = Helper.PathHelper.GetLastSlash(item);
                    if (lastslash > 0)
                    {
                        string filename = item.Substring(lastslash);

                        filename = filename.Replace(this.queueListExtension, "");

                        filename = filename.Replace("\\", "").Replace("/", "");

                        this.queueFileIdList.Add(Convert.ToInt32(filename));
                    }
                    else
                    {
                        lastslash = item.LastIndexOf('/');
                        if (lastslash > 0)
                        {
                            string filename = item.Substring(lastslash);
                            filename.Replace(this.queueListExtension, "");

                            filename.Replace("/", "");

                            this.queueFileIdList.Add(Convert.ToInt32(filename));
                        }
                    }

                }
            }

            foreach (var item in queueFileIdList)
            {

                QueueList list = GetQueueListFile(item);

                if (list.isDequeueFinished() && item < DateTime.Now.DayToInt())
                {
                    list.close();
                    this._queueListDictionary.Remove(item);

                    System.IO.File.Delete(list.FullFileName);

                    QueueContent<TValue> queuecontent = GetQueueContentFile(item);

                    queuecontent.close();

                    this._queueContentDictionary.Remove(item);

                    System.IO.File.Delete(queuecontent.FullFileName);

                    list = null;
                    queuecontent = null;
                }

                else
                {
                    this._dequeueDateInt = item;
                    this._dequeueDateCounter = list.GetCounter();
                    return;
                }

            }


        }

        private string GetFileName(int dateint, string extension)
        {
            return System.IO.Path.Combine(this.queueFolder, dateint.ToString() + extension);
        }

        /// <summary>
        /// Add one item into queue. 
        /// </summary>
        /// <param name="?"></param>
        public void Add(TValue T)
        {
            int intday = DateTime.Now.DayToInt();
            Int64 blockposition = GetQueueContentFile(intday).Add(T);
            GetQueueListFile(intday).Add(blockposition);

        }

        /// <summary>
        /// Enqueue one item.
        /// </summary>
        /// <param name="T"></param>
        public void EnQueue(TValue T)
        {
            Add(T);
        }


        /// <summary>
        ///   get the current queue  content file to insert the queue item. 
        /// </summary>
        /// <returns></returns>
        internal QueueContent<TValue> GetQueueContentFile(int intday)
        {
            if (!_queueContentDictionary.ContainsKey(intday))
            {
                lock (_object)
                {
                    if (!_queueContentDictionary.ContainsKey(intday))
                    {
                        string contentFileName = GetFileName(intday, this.queueContentExtension);
                        QueueContent<TValue> file = new QueueContent<TValue>(contentFileName);
                        _queueContentDictionary.Add(intday, file);
                    }
                }
            }

            return _queueContentDictionary[intday];
        }

        internal QueueList GetQueueListFile(int intday)
        {
            if (!_queueListDictionary.ContainsKey(intday))
            {
                lock (_object)
                {
                    if (!_queueListDictionary.ContainsKey(intday))
                    {
                        string FileName = GetFileName(intday, this.queueListExtension);

                        QueueList file = new QueueList(FileName);

                        _queueListDictionary.Add(intday, file);

                        this.queueFileIdList.Add(intday);
                    }
                }
            }

            return _queueListDictionary[intday];
        }


        /// <summary>
        /// Read and remove items from queue. 
        /// </summary>
        /// <returns></returns>
        public QueueCollection<TValue> DeQueue()
        {
            this._init();
            QueueCollection<TValue> items = new QueueCollection<TValue>(this, true);
            return items;
        }

        /// <summary>
        /// Read the queue items without removing them from queue. 
        /// </summary>
        /// <returns></returns>
        public QueueCollection<TValue> ReadQueue()
        {
            this._init();
            return new QueueCollection<TValue>(this, false);
        }

        /// <summary>
        /// count total items in the queue.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            int totalcount = 0;
            _init();
            foreach (var item in this.queueFileIdList)
            {
                if (item >= this._dequeueDateInt)
                {
                    int currentcount = this.GetQueueListFile(item).GetCounter();
                    int currenttotal = this.GetQueueListFile(item).TotalCount();

                    totalcount += (currenttotal - currentcount);
                }
            }

            return totalcount;
        }

        public void Close()
        {
            foreach (var item in _queueContentDictionary)
            {
                item.Value.close();
            }

            foreach (var item in this._queueListDictionary)
            {
                item.Value.close();
            }
            this._queueContentDictionary.Clear();
            this._queueListDictionary.Clear();
        }

        /// <summary>
        /// delete itself. 
        /// </summary>
        public void DelSelf()
        {
            this.Close();

            // del all files in this directory. 

            foreach (var item in System.IO.Directory.GetFiles(this.queueFolder))
            {
                System.IO.File.Delete(item);
            }

            System.IO.Directory.Delete(this.queueFolder);
        }

    }
}
