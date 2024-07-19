using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Mail.MassMailing
{
    public class BatchDelivery<T>
    {
        //Rule: use batch ID, to ensure that Destination only receive one time.

        // Rule:When a batch is finished, write a BatchId.finish file, with lock as the text. 
        // Rule:when a batch is done, upload to remote, write a BatchId.done file.
        // when finish is there, and .done is not there, 

        Kooboo.IndexedDB.ByteConverter.KoobooSimpleConverter<T> Converter { get; set; } = new IndexedDB.ByteConverter.KoobooSimpleConverter<T>();

        // API Path much be like /api/in, start with /
        public BatchDelivery(string RootFolder, string RemoteApiPath)
        {
            this.Folder = RootFolder;
            this.ApiPath = RemoteApiPath;
        }

        private string _folder;
        private string Folder
        {
            get
            {
                if (_folder == null)
                {
                    var root = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
                    root = System.IO.Path.Combine(root, "BatchDelivery");
                    Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(root);
                    _folder = root;
                }
                return _folder;
            }
            set
            {
                _folder = value;
            }
        }

        private string _apiPath;

        private string ApiPath
        {
            get
            {
                return _apiPath;
            }
            set
            {
                _apiPath = value;
                if (!_apiPath.StartsWith("/"))
                {
                    _apiPath = "/" + _apiPath;
                }
            }
        }

        public string IPFolder(string IPAddress)
        {
            var path = System.IO.Path.Combine(Folder, IPAddress);
            Lib.Helper.IOHelper.EnsureDirectoryExists(path);
            return path;
        }


        public string CreateBatch(string destinationIP)
        {
            var batchId = DateTime.Now.Ticks.ToString(); //it is not possible to create two batch at the same time, using tick is better than Guid.
            var folder = batchFolder(destinationIP, batchId);
            if (System.IO.Directory.Exists(folder))
            {
                return CreateBatch(destinationIP);
            }
            return batchId;
        }

        private string batchFolder(string ToIP, string batchId)
        {
            var folder = IPFolder(ToIP);
            return System.IO.Path.Combine(folder, batchId);
        }


        public void Add(string ToIpAddress, string batchId, T value)
        {
            var folder = batchFolder(ToIpAddress, batchId);

            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(folder);

            var bytes = Converter.ToByte(value);

            var fileName = System.Guid.NewGuid().ToString() + ".binary";

            fileName = System.IO.Path.Combine(folder, fileName);
            System.IO.File.WriteAllBytes(fileName, bytes);
        }

        public void AddRange(string ToIPAddress, string BatchId, List<T> Values)
        {
            var folder = batchFolder(ToIPAddress, BatchId);

            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
            foreach (var item in Values)
            {
                var bytes = Converter.ToByte(item);
                var fileName = System.Guid.NewGuid().ToString() + ".binary";
                fileName = System.IO.Path.Combine(folder, fileName);
                System.IO.File.WriteAllBytes(fileName, bytes);
            }
        }


        public void FinishBatch(string ToIP, string batchID)
        {
            var IpFolder = IPFolder(ToIP);
            var finishFileName = System.IO.Path.Combine(IpFolder, batchID + ".finish");

            if (!System.IO.File.Exists(finishFileName))
            {
                byte[] EmptyBytes = new byte[4];
                EmptyBytes[0] = 1;
                EmptyBytes[3] = 1;

                System.IO.File.WriteAllBytes(finishFileName, EmptyBytes);
            }

            _ = Delivery();
        }

        private int beingUsed = 0;
        public async Task Delivery()
        {
            if (0 == Interlocked.Exchange(ref beingUsed, 1))
            {
                try
                {
                    var DeliveryList = FetchList();

                    foreach (var item in DeliveryList)
                    {
                        var folder = batchFolder(item.IPAddress, item.BatchId);
                        var batchFiles = System.IO.Directory.GetFiles(folder);

                        if (batchFiles != null && batchFiles.Any())
                        {
                            var zipFileName = CreateZipFile(item.IPAddress, item.BatchId);

                            if (zipFileName != null)
                            {
                                var submitOK = await PostToRemote(zipFileName, item.IPAddress, item.BatchId);

                                if (submitOK)
                                {
                                    DeleteBatch(item.IPAddress, item.BatchId);
                                }

                            }


                        }

                    }

                }
                catch (Exception)
                {

                }


                Interlocked.Exchange(ref beingUsed, 0);
            }
        }


        private async Task<bool> PostToRemote(string zipFile, string RemoteIP, string batchId)
        {
            if (System.IO.File.Exists(zipFile))
            {
                var bytes = System.IO.File.ReadAllBytes(zipFile);

                var URL = "http://" + RemoteIP + ApiPath + "?batchid=" + batchId;

                return Kooboo.Lib.Helper.HttpHelper.PostData(URL, null, bytes);

            }
            return false;
        }

        private string CreateZipFile(string IpAddress, string batchId)
        {
            var folder = batchFolder(IpAddress, batchId);

            if (!System.IO.Directory.Exists(folder))
            {
                return null;
            }

            var files = System.IO.Directory.GetFiles(folder, "*.binary", SearchOption.AllDirectories);

            if (files != null && files.Any())
            {
                var ipFolder = IPFolder(IpAddress);
                var zipFile = System.IO.Path.Combine(ipFolder, batchId + ".zip");

                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }

                var stream = new FileStream(zipFile, FileMode.OpenOrCreate);
                var archive = new ZipArchive(stream, ZipArchiveMode.Create, false);

                foreach (var path in files)
                {
                    archive.CreateEntryFromFile(path, path.Replace(folder, "").Trim('\\').Trim('/'));
                }

                archive.Dispose();
                stream.Dispose();
                return zipFile;

            }

            return null;


        }


        private bool IsBatchFinish(string IPAddress, string BatchId)
        {
            var folder = IPFolder(IPAddress);
            var fileName = System.IO.Path.Combine(folder, BatchId + ".finish");
            return System.IO.File.Exists(fileName);
        }

        private bool IsBatchDone(string IPAddress, string BatchId)
        {
            var folder = IPFolder(IPAddress);
            var fileName = System.IO.Path.Combine(folder, BatchId + ".done");
            return System.IO.File.Exists(fileName);
        }

        private bool IsBatchOutdate(string IPAddress, string BatchId)
        {
            var folder = batchFolder(IPAddress, BatchId);

            if (System.IO.Directory.Exists(folder))
            {
                var info = new System.IO.DirectoryInfo(folder);

                if (info.LastWriteTime < DateTime.Now.AddHours(-1))
                {
                    return true;
                }
            }

            return false;
        }

        private void DeleteBatch(string IPAddress, string BatchId)
        {
            var folder = batchFolder(IPAddress, BatchId);

            try
            {
                System.IO.Directory.Delete(folder, true);
            }
            catch (Exception)
            {

            }

            var ipFolder = IPFolder(IPAddress);

            var fileName = System.IO.Path.Combine(ipFolder, BatchId + ".zip");

            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            fileName = System.IO.Path.Combine(folder, BatchId + ".finish");
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
            fileName = System.IO.Path.Combine(folder, BatchId + ".done");
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

        }


        private List<ReadyBatch> FetchList()
        {
            var dirs = System.IO.Directory.GetDirectories(this.Folder);

            List<ReadyBatch> ToBeSend = new List<ReadyBatch>();

            foreach (var item in dirs)
            {
                var dir = new System.IO.DirectoryInfo(item);

                if (dir.Exists)
                {
                    var ip = dir.Name;
                    if (System.Net.IPAddress.TryParse(ip, out var ToIP))
                    {
                        var subBatchIds = System.IO.Directory.GetDirectories(dir.FullName);

                        foreach (var batchId in subBatchIds)
                        {
                            if (IsBatchDone(ip, batchId))
                            {
                                DeleteBatch(ip, batchId);
                            }
                            else if (IsBatchFinish(ip, batchId))
                            {
                                ToBeSend.Add(new ReadyBatch() { IPAddress = ip, BatchId = batchId });
                            }
                            else if (IsBatchOutdate(ip, batchId))
                            {
                                FinishBatch(ip, batchId);
                                ToBeSend.Add(new ReadyBatch() { IPAddress = ip, BatchId = batchId });
                            }
                        }

                    }
                }

            }
            return ToBeSend;
        }

    }

    public record ReadyBatch
    {
        public string IPAddress { get; set; }

        public string BatchId { get; set; }
    }
}
