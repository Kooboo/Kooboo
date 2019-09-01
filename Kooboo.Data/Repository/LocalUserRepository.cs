//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;

namespace Kooboo.Data.Repository
{
    public class LocalUserRepository : RepositoryBase<User>
    {
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<User>(o => o.UserName);
                paras.AddColumn<User>(o => o.PasswordHash);
                paras.SetPrimaryKeyField<User>(o => o.Id);
                return paras;
            }
        }

        public User Get(string nameOrGuid)
        {
            Guid key = Kooboo.Lib.Helper.IDHelper.ParseKey(nameOrGuid);
            User user = this.Get(key);
            if (user != null)
            {
                user.IsAdmin = GlobalDb.Users.IsAdmin(user.CurrentOrgId, user.Id);
            }
            return user;
        }
    }
}
