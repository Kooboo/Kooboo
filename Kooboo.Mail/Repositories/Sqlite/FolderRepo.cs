using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Mail.Repositories.Sqlite
{
    public class FolderRepo
    {
        private string TableName
        {
            get
            {
                return "Folder";
            }
        }

        private MailDb maildb { get; set; }

        public FolderRepo(MailDb db)
        {
            this.maildb = db;
        }

        public bool AddOrUpdate(Folder folder)
        {
            if (!Folder.ReservedFolder.ContainsKey(folder.Id))
            {
                return this.maildb.SqliteHelper.AddOrUpdate(folder, this.TableName, nameof(Folder.Id));
            }
            return false;
        }


        public Folder Get(string name, bool isMigration = false)
        {
            // migration check
            if (isMigration) return Get(Folder.ToId(name));
            if (name.Contains("@"))
            {
                var prasedFolder = Utility.FolderUtility.ParseFolder(name);
                return Get(prasedFolder.FolderId);
            }
            else
            {
                return Get(Folder.ToId(name));
            }
        }

        public int GetFolderId(string name)
        {
            if (name.Contains("@"))
            {
                var prasedFolder = Utility.FolderUtility.ParseFolder(name);

                return prasedFolder.FolderId;
            }
            else
            {
                return Folder.ToId(name);
            }
        }

        public Folder Get(int id)
        {
            if (Folder.ReservedFolder.ContainsKey(id))
            {
                return new Folder(Folder.ReservedFolder[id]);
            }
            return this.maildb.SqliteHelper.Get<Folder>(this.TableName, nameof(Folder.Id), id);
        }

        public void Rename(Folder folder, string newName, bool Recursive = true)
        {
            /// create new folder, move all message there.  
            var progress = maildb.MailMigrationProgress.GetActiveProgressByFolder(folder.Id);
            if (progress.Any())
            {
                throw new Exception("One or more associated jobs are running, please stop them first");
            }

            if (newName.Equals(folder.Name, StringComparison.OrdinalIgnoreCase))
            {
                // 可能是大小写改动，只需要更新文件夹，不需要移动相关邮件
                folder.Name = newName;
                AddOrUpdate(folder);

                return;
            }

            var newfolder = new Folder(newName);
            if (Get(newfolder.Id) != null)
            {
                throw new Exception("Folder already exist");
            }

            this.Add(newfolder);

            // var allmessage = this.maildb.Msgstore.MetaQuery.Where(o => o.FolderId == folder.Id).SelectAll();
            var allmessage = this.maildb.Message2.Query.Where(o => o.FolderId == folder.Id).SelectAll();
            foreach (var item in allmessage)
            {
                item.FolderId = newfolder.Id;
                /// this.maildb.Msgstore.UpdateMeta(item);
                this.maildb.Message2.UpdateMeta(item);
            }
            if (!Folder.ReservedFolder.ContainsKey(folder.Id))
            {
                // not reserverd folder, remove it. 
                this.Delete(folder.Id);
            }

            // get all children folder, and rename. 
            if (Recursive)
            {
                List<Folder> AllFolders = this.maildb.SqliteHelper.Query<Folder>("SELECT * FROM " + this.TableName).ToList();

                foreach (var item in AllFolders)
                {
                    if (item.Name.StartsWith(folder.Name + "/"))
                    {
                        string newname = newfolder.Name + "/" + item.Name.Substring(folder.Name.Length + 1);
                        this.Rename(item, newname, false);
                    }
                }
            }
        }

        public void Delete(Folder folder)
        {
            if (Folder.ReservedFolder.ContainsKey(folder.Id))
            {
                return;
            }
            List<Folder> AllFolders = this.maildb.SqliteHelper.Query<Folder>("SELECT * FROM " + this.TableName).ToList();

            foreach (var item in AllFolders)
            {
                if (item.Name.StartsWith(folder.Name + "/"))
                {
                    throw new Exception($"Name \"{folder.Name}\" has inferior hierarchical names");
                }
            }
            Delete(folder.Id);
        }

        public void Delete(int folderId)
        {
            // this.maildb.SqliteHelper.Delete(this.TableName, nameof(Folder.Id), folderId); 
            long msgid = -1;
            var MsgIdQuery = "SELECT MsgId FROM " + this.maildb.Message2.TableName + " WHERE MsgId> " + msgid.ToString() + " AND " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(folderId) + " Order By MsgId limit 0, 1000";

            var allmsgid = this.maildb.SqliteHelper.Query<long>(MsgIdQuery);

            while (allmsgid != null && allmsgid.Any())
            {
                foreach (var item in allmsgid)
                {
                    this.maildb.MsgHandler.DeleteMsg(item);
                }
                msgid = allmsgid.Max();

                MsgIdQuery = "SELECT MsgId FROM " + this.maildb.Message2.TableName + " WHERE MsgId> " + msgid.ToString() + " AND " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(folderId) + " Order By MsgId limit 0, 1000";

                allmsgid = this.maildb.SqliteHelper.Query<long>(MsgIdQuery);
            }

            this.maildb.SqliteHelper.Delete(this.TableName, nameof(Folder.Id), folderId);

            // also delete all message. 
            string sqlDel = "DELETE FROM " + this.maildb.Message2.TableName + " WHERE " + nameof(Message.FolderId) + "=" + this.maildb.SqliteHelper.ObjToString(folderId);

            this.maildb.SqliteHelper.Execute(sqlDel);
        }

        /// <summary>
        /// Move folder
        /// </summary>
        /// <param name="folderId">Origin folder ID</param>
        /// <param name="targetFolder"></param>
        public void Move(int folderId, string targetFolder)
        {
            var folder = Get(folderId);
            if (folder == null)
            {
                return;
            }

            if (folder.Name.Contains("/", StringComparison.OrdinalIgnoreCase))
            {
                var folders = AllFolders();
                foreach (var item in folders)
                {
                    if (item.Name.Contains("/", StringComparison.OrdinalIgnoreCase))
                    {
                        var spans = item.Name.Split('/');
                        foreach (var span in spans)
                        {
                            if (span.Equals(folder.Name))
                            {
                                throw new Exception("Folders that contains sub folders can not be deleted");
                            }
                        }
                    }
                }
            }

            var progress = maildb.MailMigrationProgress.GetActiveProgressByFolder(folderId);
            if (progress.Any())
            {
                throw new Exception("One or more associated jobs are running, please stop them first");
            }

            var target = Get(Folder.ToId(targetFolder));
            if (target == null)
            {
                target = new Folder(targetFolder);
                Add(target);
            }

            var sql = $"UPDATE [{maildb.Message2.TableName}] SET [{nameof(Message.FolderId)}] = {target.Id} WHERE  [{nameof(Message.FolderId)}] = {folder.Id};";
            maildb.SqliteHelper.Execute(sql);
            Delete(folder.Id);
        }

        public void Add(string name)
        {
            if (name.IndexOf('/') != -1)
            {
                var folderPaths = name.Split('/');
                var folderPathBuilder = Path.Combine();
                foreach (var folderPath in folderPaths)
                {
                    folderPathBuilder = Path.Combine(folderPathBuilder, folderPath).Replace("\\", "/");
                    var folder = new Folder
                    {
                        Name = folderPathBuilder
                    };
                    Add(folder);
                }
            }
            else
            {
                var folder = new Folder
                {
                    Name = name
                };
                Add(folder);
            }
        }

        public void Add(Folder folder)
        {
            if (!Folder.ReservedFolder.ContainsKey(folder.Id))
            {
                if (folder.Name.Contains('/'))
                {
                    var folderPaths = folder.Name.Split('/');
                    var folderPathBuilder = Path.Combine();
                    foreach (var folderPath in folderPaths)
                    {
                        folderPathBuilder = Path.Combine(folderPathBuilder, folderPath).Replace("\\", "/");
                        var parentFolder = new Folder
                        {
                            Name = folderPathBuilder
                        };
                        this.maildb.SqliteHelper.AddOrUpdate(folder, this.TableName, nameof(folder.Id));
                    }
                }

                this.maildb.SqliteHelper.AddOrUpdate<Folder>(folder, this.TableName, nameof(folder.Id));
            }
        }

        public void Subscribe(Folder folder)
        {
            Dictionary<string, object> data = new()
            {
                { nameof(folder.Subscribed), true }
            };

            this.maildb.SqliteHelper.Update(this.TableName, nameof(folder.Id), folder.Id, data);
        }

        public void Unsubscribe(Folder folder)
        {
            Dictionary<string, object> data = new()
            {
                { nameof(folder.Subscribed), false }
            };

            this.maildb.SqliteHelper.Update(this.TableName, nameof(folder.Id), folder.Id, data);
        }

        public List<Folder> AllFolders()
        {
            List<Folder> result = this.maildb.SqliteHelper.Query<Folder>("SELECT * FROM " + this.TableName).ToList();
            foreach (var item in Folder.ReservedFolder)
            {
                result.Add(new Folder(item.Value));
            }
            return result;
        }

        public List<Folder> All()
        {
            return this.maildb.SqliteHelper.Query<Folder>("SELECT * FROM " + this.TableName).ToList();
        }
    }
}
