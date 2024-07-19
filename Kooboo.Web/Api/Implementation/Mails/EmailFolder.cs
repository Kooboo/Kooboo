using System.Linq;
using Kooboo.Api;
using Kooboo.Mail;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailFolderApi : IApi
    {
        public string ModelName => "EmailFolder";

        public bool RequireSite => false;

        public bool RequireUser => true;

        public List<FolderViewModel> List(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                return EmailForwardManager.Get<List<FolderViewModel>>(this.ModelName, nameof(EmailFolderApi.List), call.Context.User, null);
            }

            var user = call.Context.User;
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(user.CurrentOrgId);

            var addList = orgDb.Email.ByUser(user.Id);   // orgdb.EmailAddress.Query().Where(o => o.UserId == user.Id).SelectAll();

            List<FolderViewModel> result = new();
             
            var mailDb = Mail.Factory.DBFactory.UserMailDb(user.Id, user.CurrentOrgId);

            var userFolders = mailDb.Folder.All();

            foreach (var item in userFolders)
            {
                var MsgUnread = mailDb.Message2.FolderUnread(item.Id);

                FolderViewModel model = new FolderViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    DisplayName = item.Name,
                    UnRead = MsgUnread.UnRead,
                    Count = MsgUnread.Exists
                };

                result.Add(model);

            }

            return result.OrderBy(o => o.Name).ToList();
        }

        public void Create(string FolderName, ApiCall call)
        {
            var user = call.Context.User;
            var mailDb = Mail.Factory.DBFactory.UserMailDb(user.Id, user.CurrentOrgId);

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    { nameof(FolderName), FolderName },
                };
                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailFolderApi.Create), call.Context.User, dic);
                return;
            }

            if (Folder.ReservedFolder.Any(o => FolderName.Equals(o.Value, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("The folder name cannot be the same as the reserved folder");
            }

            if (mailDb.Folder.Get(FolderName) != null)
            {
                throw new Exception("Folder already exist");
            }

            mailDb.Folder.Add(FolderName);
        }

        public void Rename(string FolderName, string NewName, ApiCall call)
        {
            var mailDB = Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var newFolder = new Folder(NewName);

            if (!FolderName.Equals(NewName, StringComparison.OrdinalIgnoreCase) && mailDB.Folder.Get(newFolder.Id) != null)
            {
                throw new Exception("Folder already exist");
            }

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    { nameof(FolderName), FolderName },
                    { nameof(NewName), NewName }
                };
                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailFolderApi.Rename), call.Context.User, dic);
                return;
            }

            var folder = new Folder(FolderName);
            mailDB.Folder.Rename(folder, NewName);
        }

        /// <summary>
        /// delete folder and all emails. 
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="call"></param>
        public void Delete(int folderId, ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    { "folderId", folderId.ToString() },
                };
                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailFolderApi.Delete), call.Context.User, dic);
                return;
            }

            var mailDb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);
            mailDb.Folder.Move(folderId, Folder.Trash);
        }
    }


    public class FolderViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public string DisplayName { get; set; }

        public int UnRead { get; set; }
    }
}
