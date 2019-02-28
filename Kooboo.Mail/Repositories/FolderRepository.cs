//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using System.Collections.Generic;
using Kooboo.IndexedDB;

namespace Kooboo.Mail.Repositories
{
    public  class FolderRepository : RepositoryBase<Folder>
    {
        private MailDb maildb { get; set; }

        public FolderRepository(MailDb db) 
            : base(db.Db)
        {
            this.maildb = db; 
        }

        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters(); 
                paras.SetPrimaryKeyField<Folder>(o => o.Id);
                return paras;
            }
        }

        public Folder Get(string name)
        {  
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

        public override Folder Get(int id)
        {
            if (Folder.ReservedFolder.ContainsKey(id))
            {
                return new Folder(Folder.ReservedFolder[id]);
            }
            return base.Get(id);
        } 

        public void Rename(Folder folder, string newName, bool Recursive= true)
        {
            /// create new folder, move all message there.  
            var newfolder = new Folder(newName); 

            if (Get(newfolder.Id) !=null)
            {
                return; // folder already exists. 
            } 
            this.AddOrUpdate(newfolder); 
            var allmessage = this.maildb.Messages.Query().Where(o => o.FolderId == folder.Id).SelectAll();
            foreach (var item in allmessage)
            {
                item.FolderId = newfolder.Id;
                this.maildb.Messages.AddOrUpdate(item);  
            } 
            if (!Folder.ReservedFolder.ContainsKey(folder.Id))
            {
                // not reserverd folder, remove it. 
                this.Delete(folder.Id); 
            }
             
            // get all children folder, and rename. 
            if (Recursive)
            { 
                foreach (var item in this.All())
                {
                    if (item.Name.StartsWith(folder.Name + "/"))
                    {
                        string newname = newfolder.Name + "/" + item.Name.Substring(folder.Name.Length + 2);
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
            Delete(folder.Id);
        }

        public void Add(string name)
        {
            var folder = new Folder
            {
                Name = name 
            };
            Add(folder);
        }
         
        public void Subscribe(Folder folder)
        {
            folder.Subscribed = true;
            Update(folder);
        }

        public void Unsubscribe(Folder folder)
        {
            folder.Subscribed = false;
            Update(folder);
        }
          
        public List<Folder> AllFolders()
        {
            List<Folder> result = this.All(); 
            foreach (var item in Folder.ReservedFolder)
            {
                result.Add(new Folder(item.Value));
            } 
            return result;  
        }
    }
}
