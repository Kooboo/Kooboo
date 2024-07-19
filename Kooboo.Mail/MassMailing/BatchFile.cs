using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Kooboo.Mail.MassMailing
{
    public class BatchFile
    {
        // Rule: use batch ID, to ensure that Destination only receive one time. 
        // Rule:When a batch is finished, write a BatchId.finish file, with lock as the text. 
        // Rule:when a batch is done, write a BatchId.done file, and remove all files,last one is .done.
        public BatchFile(string RootFolder)
        {
            this.Folder = RootFolder;
        }

        private string _folder;
        private string Folder
        {
            get
            {
                if (_folder == null)
                {
                    var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
                    root = Path.Combine(root, "BatchFiles");
                    Lib.Helper.IOHelper.EnsureDirectoryExists(root);
                    _folder = root;
                }
                return _folder;
            }
            set
            {
                _folder = value;
                if (_folder != null)
                {
                    Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(_folder);
                }
            }
        }

        public string destinationFolder(string Destination)
        {
            var path = System.IO.Path.Combine(Folder, Destination);
            Lib.Helper.IOHelper.EnsureDirectoryExists(path);
            return path;
        }

        public string CreateBatch(string Destination)
        {
            var batchId = DateTime.Now.Ticks.ToString(); //it is not possible to create two batch at the same time, using tick is better than Guid.
            var folder = batchFolder(Destination, batchId);
            if (System.IO.Directory.Exists(folder))
            {
                return CreateBatch(Destination);
            }
            return batchId;
        }

        public string CreateBatch(string Destination, string meta)
        {
            var batchId = DateTime.Now.Ticks.ToString(); //it is not possible to create two batch at the same time, using tick is better than Guid.
            var folder = batchFolder(Destination, batchId);
            if (System.IO.Directory.Exists(folder))
            {
                return CreateBatch(Destination, meta);
            }
            else
            {
                string metaFile = System.IO.Path.Combine(folder, "meta.meta");
                Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
                System.IO.File.WriteAllText(metaFile, meta);
            }
            return batchId;
        }

        public string GetMeta(string destination, string BatchId)
        {
            var folder = batchFolder(destination, BatchId);
            string metaFile = System.IO.Path.Combine(folder, "meta.meta");

            if (System.IO.File.Exists(metaFile))
            {
                return System.IO.File.ReadAllText(metaFile);
            }
            return null;
        }

        public string batchFolder(string destination, string batchId)
        {
            var folder = destinationFolder(destination);
            return System.IO.Path.Combine(folder, batchId);
        }

        private string getFileName(string destination, string batchId, bool IsBinary)
        {
            var folder = batchFolder(destination, batchId);
            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
            var namePart = System.Guid.NewGuid().ToString();
            string fileName = IsBinary ? namePart + ".binary" : namePart + ".json";
            return System.IO.Path.Combine(folder, fileName);
        }

        public void Add(string destination, string batchId, byte[] fileContent)
        {
            var fileName = getFileName(destination, batchId, true);
            System.IO.File.WriteAllBytes(fileName, fileContent);
        }
        public void Add(string destination, string batchId, string Json)
        {
            var fileName = getFileName(destination, batchId, false);
            System.IO.File.WriteAllText(fileName, Json);
        }

        public void AddRange(string destination, string BatchId, List<byte[]> Values)
        {
            var folder = batchFolder(destination, BatchId);
            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
            foreach (var item in Values)
            {
                var fileName = System.Guid.NewGuid().ToString() + ".binary";
                fileName = System.IO.Path.Combine(folder, fileName);
                System.IO.File.WriteAllBytes(fileName, item);
            }
        }

        public void AddRange(string destination, string BatchId, List<string> Values)
        {
            var folder = batchFolder(destination, BatchId);
            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
            foreach (var item in Values)
            {
                var fileName = System.Guid.NewGuid().ToString() + ".json";
                fileName = System.IO.Path.Combine(folder, fileName);
                System.IO.File.WriteAllText(fileName, item);
            }
        }

        private string SingleFile(string destination, string batchId, SingleFileName type)
        {
            var folder = destinationFolder(destination);
            if (type == SingleFileName.Finish)
            {
                return System.IO.Path.Combine(folder, batchId + ".completed");
            }
            else if (type == SingleFileName.Done)
            {
                return System.IO.Path.Combine(folder, batchId + ".done");
            }
            else if (type == SingleFileName.Zip)
            {
                return System.IO.Path.Combine(folder, batchId + ".zip");
            }
            else
            {
                return null;
            }
        }

        private byte[] _fakeBytes;
        private byte[] FakeBytes
        {
            get
            {
                if (_fakeBytes == null)
                {
                    byte[] EmptyBytes = new byte[4];
                    EmptyBytes[0] = 1;
                    EmptyBytes[3] = 1;
                    _fakeBytes = EmptyBytes;
                }
                return _fakeBytes;
            }
        }

        public void CompleteBatch(string destination, string batchID)
        {
            var fileName = SingleFile(destination, batchID, SingleFileName.Finish);
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.WriteAllBytes(fileName, FakeBytes);
            }
        }

        public void DoneBatch(string destination, string batchId)
        {
            var fileName = SingleFile(destination, batchId, SingleFileName.Done);
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.WriteAllBytes(fileName, FakeBytes);
            }

            DeleteBatch(destination, batchId);
        }


        public string CreateZipFile(string destination, string batchId)
        {
            CompleteBatch(destination, batchId);

            var folder = batchFolder(destination, batchId);

            if (!System.IO.Directory.Exists(folder))
            {
                return null;
            }

            string zipFile = SingleFile(destination, batchId, SingleFileName.Zip);

            var files = System.IO.Directory.GetFiles(folder, "*.binary", SearchOption.AllDirectories);
            var jsonFiles = System.IO.Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories);

            if (jsonFiles != null && jsonFiles.Any())
            {
                var jsonList = jsonFiles.ToList();
                if (files != null)
                {
                    jsonList.AddRange(files);
                }
                files = jsonList.ToArray();
            }

            if (files != null && files.Any())
            {
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

        public static List<T> UnPack<T>(byte[] ZipFile)
        {
            System.IO.MemoryStream mo = new MemoryStream(ZipFile);

            return UnPack<T>(mo);
        }

        public static List<T> UnPack<T>(Stream zipFile)
        {
            Kooboo.IndexedDB.ByteConverter.KoobooSimpleConverter<T> Converter = new IndexedDB.ByteConverter.KoobooSimpleConverter<T>();

            List<T> result = new List<T>();

            using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Read, false, System.Text.Encoding.UTF8))
            {
                foreach (var item in archive.Entries)
                {
                    MemoryStream memory = new MemoryStream();
                    item.Open().CopyTo(memory);
                    var bytes = memory.ToArray();

                    if (item.Name.EndsWith(".json"))
                    {
                        string Json = System.Text.Encoding.UTF8.GetString(bytes);

                        var obj = System.Text.Json.JsonSerializer.Deserialize<T>(Json);
                        if (obj != null)
                        {
                            result.Add(obj);
                        }
                    }
                    else if (item.Name.EndsWith(".binary"))
                    {
                        var obj = Converter.FromByte(bytes);
                        if (obj != null)
                        {
                            result.Add(obj);
                        }
                    }

                }

            }

            return result;

        }


        private bool IsBatchFinish(string destination, string BatchId)
        {
            return File.Exists(SingleFile(destination, BatchId, SingleFileName.Finish));
        }

        private bool IsBatchDone(string destination, string BatchId)
        {
            return System.IO.File.Exists(SingleFile(destination, BatchId, SingleFileName.Done));
        }

        private bool IsBatchOutdate(string destination, string BatchId)
        {
            var folder = batchFolder(destination, BatchId);

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

        private void DeleteBatch(string destination, string BatchId)
        {
            var folder = batchFolder(destination, BatchId);

            try
            {
                System.IO.Directory.Delete(folder, true);
            }
            catch (Exception)
            {

            }

            try
            {
                System.IO.Directory.Delete(folder, true);
            }
            catch (Exception)
            {

            }

            var fileName = SingleFile(destination, BatchId, SingleFileName.Finish);
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
            fileName = SingleFile(destination, BatchId, SingleFileName.Zip);
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
            fileName = SingleFile(destination, BatchId, SingleFileName.Done);
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

        }


        public List<FileBatch> FetchList()
        {
            var dirs = System.IO.Directory.GetDirectories(this.Folder);

            List<FileBatch> result = new List<FileBatch>();

            foreach (var item in dirs)
            {
                var dir = new System.IO.DirectoryInfo(item);

                if (dir.Exists)
                {
                    string destination = dir.Name;

                    var subBatchIds = System.IO.Directory.GetDirectories(dir.FullName);

                    foreach (var subDir in subBatchIds)
                    {
                        var dirInfo = new System.IO.DirectoryInfo(subDir);

                        if (dirInfo != null && dirInfo.Exists)
                        {
                            var dirList = _FetchFromDir(dirInfo);
                            if (dirList != null)
                            {
                                result.AddRange(dirList);
                            }
                        }
                    }
                }

            }
            return result;
        }

        public List<FileBatch> FetchList(string Destination, bool CountFile = false)
        {
            var folder = destinationFolder(Destination);

            var dir = new System.IO.DirectoryInfo(folder);

            return _FetchFromDir(dir, CountFile);
        }

        private List<FileBatch> _FetchFromDir(DirectoryInfo dir, bool countFiles = false)
        {
            List<FileBatch> result = new List<FileBatch>();

            if (dir.Exists)
            {
                string destination = dir.Name;

                var subBatchIds = System.IO.Directory.GetDirectories(dir.FullName);

                foreach (var subDir in subBatchIds)
                {
                    var subDirInfo = new System.IO.DirectoryInfo(subDir);

                    if (subDirInfo == null || !subDirInfo.Exists)
                    {
                        continue;
                    }

                    var batchId = subDirInfo.Name;

                    if (IsBatchDone(destination, batchId))
                    {
                        DeleteBatch(destination, batchId);
                    }

                    FileBatch batch = new FileBatch();
                    batch.Destination = destination;
                    batch.BatchId = batchId;
                    batch.IsFinish = IsBatchFinish(destination, batchId);

                    var zipFile = SingleFile(destination, batchId, SingleFileName.Zip);

                    if (System.IO.File.Exists(zipFile))
                    {
                        batch.ZipFile = zipFile;
                    }

                    if (countFiles)
                    {
                        var batchDir = batchFolder(destination, batchId);
                        if (System.IO.Directory.Exists(batchDir))
                        {
                            var files = System.IO.Directory.GetFiles(batchDir);
                            if (files != null)
                            {
                                batch.ItemCount = files.Count();
                            }

                        }

                    }

                    result.Add(batch);

                }


            }

            return result;
        }
    }

    public enum SingleFileName
    {
        Finish = 0,
        Zip = 1,
        Done = 2,
    }


    public class FileBatch
    {

        public string Destination { get; set; }

        public string BatchId { get; set; }

        public bool IsFinish { get; set; }

        public int ItemCount { get; set; }

        public string ZipFile { get; set; }
    }


}
