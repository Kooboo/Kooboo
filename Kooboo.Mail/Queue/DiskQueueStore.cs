//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Mail.Queue
{
    public class DiskQueueStore
    {

        public HashSet<Guid> Retrys { get; set; }

        public DiskQueueStore(string rootpath)
        {
            this.Root = rootpath;

            this.IncomingFolder = System.IO.Path.Combine(this.Root, "Incoming");
            Lib.Helper.IOHelper.EnsureDirectoryExists(this.IncomingFolder);

            this.SentFolder = System.IO.Path.Combine(this.Root, "Sent");
            Lib.Helper.IOHelper.EnsureDirectoryExists(this.SentFolder);

            this.BadMailFolder = System.IO.Path.Combine(this.Root, "BadMail");
            Lib.Helper.IOHelper.EnsureDirectoryExists(this.BadMailFolder);

            this.ErrorFolder = System.IO.Path.Combine(this.Root, "Error");
            Lib.Helper.IOHelper.EnsureDirectoryExists(this.ErrorFolder);

            this.Retrys = new HashSet<Guid>();
        }

        public string Root { get; set; }

        public string SentFolder { get; set; }

        public string ErrorFolder { get; set; }

        private string IncomingFolder { get; set; }

        public string BadMailFolder { get; set; }

        public void AddQueue(Kooboo.Data.Models.MailQueue model)
        {
            string fullname = GetWriteFileName(model);
            string data = Lib.Helper.JsonHelper.Serialize(model);
            System.IO.File.WriteAllText(fullname, data);
        }

        private string GetWriteFileName(Data.Models.MailQueue model)
        {
            string filename = model.Id.ToString() + ".mail";
            string fullname = System.IO.Path.Combine(this.IncomingFolder, filename);
            if (System.IO.File.Exists(fullname))
            {
                model.Id = Guid.NewGuid();
                return GetWriteFileName(model);
            }
            return fullname;
        }

        public List<Data.Models.MailQueue> AllItems()
        {
            var files = System.IO.Directory.GetFiles(this.IncomingFolder);

            List<Data.Models.MailQueue> result = new List<Data.Models.MailQueue>();

            if (files != null)
            {
                foreach (var item in files)
                {
                    var text = System.IO.File.ReadAllText(item);

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        System.IO.File.Delete(item);
                    }

                    bool haserror = false;

                    try
                    {
                        var model = Lib.Helper.JsonHelper.Deserialize<Data.Models.MailQueue>(text);
                        if (model != null)
                        {
                            result.Add(model);
                        }
                    }
                    catch (Exception)
                    {
                        haserror = true;
                    }

                    if (haserror)
                    {
                        System.IO.File.Delete(item);
                        string name = Guid.NewGuid().ToString() + ".log";
                        name = System.IO.Path.Combine(this.ErrorFolder, name);
                        System.IO.File.WriteAllText(name, text);
                    }

                }
            }
            return result;
        }


        public void SendOk(Data.Models.MailQueue model)
        {
            var file = GetIncomingFileName(model);

            if (System.IO.File.Exists(file))
            {
                System.IO.File.Move(file, GetSentFileName(model));
            }

            this.Retrys.Remove(model.Id);
        }

        public SendFailResponse SendFail(Data.Models.MailQueue model)
        {
            if (this.Retrys.Contains(model.Id))
            {
                this.Retrys.Remove(model.Id);

                var file = GetIncomingFileName(model);

                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Move(file, GetBadMailFileName(model));
                }
                return new SendFailResponse() { WillRetry = false, LogOk = true };
            }
            else
            {
                this.Retrys.Add(model.Id);
                return new SendFailResponse() { WillRetry = true, LogOk = true };
            }

        }

        public void SendFailNoRetry(Data.Models.MailQueue model)
        {
            if (this.Retrys.Contains(model.Id))
            {
                this.Retrys.Remove(model.Id);
            }

            var file = GetIncomingFileName(model);

            if (System.IO.File.Exists(file))
            {
                System.IO.File.Move(file, GetBadMailFileName(model));
            }
        }


        public string GetIncomingFileName(Data.Models.MailQueue model)
        {
            string filename = model.Id.ToString() + ".mail";
            string fullname = System.IO.Path.Combine(this.IncomingFolder, filename);
            if (System.IO.File.Exists(fullname))
            {
                return fullname;
            }
            return null;
        }

        public string GetSentFileName(Data.Models.MailQueue model)
        {
            string filename = model.Id.ToString() + ".mail";
            string path = System.IO.Path.Combine(this.SentFolder, filename);
            if (!System.IO.File.Exists(path))
            {
                return path;
            }
            else
            {
                filename = System.Guid.NewGuid().ToString() + ".mail";
                path = System.IO.Path.Combine(this.SentFolder, filename);
                return path;
            }
        }

        public string GetBadMailFileName(Data.Models.MailQueue model)
        {
            string filename = model.Id.ToString() + ".mail";
            string path = System.IO.Path.Combine(this.BadMailFolder, filename);
            if (!System.IO.File.Exists(path))
            {
                return path;
            }
            else
            {
                filename = System.Guid.NewGuid().ToString() + ".mail";
                path = System.IO.Path.Combine(this.SentFolder, filename);
                return path;
            }
        }

    }

    public class SendFailResponse
    {
        public bool LogOk { get; set; } = true;

        public bool WillRetry { get; set; } = false;
    }
}
