using System;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Data.Interface;

namespace Kooboo.Mail.Repositories
{

    public class CleanSpamTask : IBackgroundWorker
    {
        public int Interval
        {

            get
            {
#if DEBUG
                return 30;
#endif

                return 60 * 60 * 10; // every 10 hours.

            }
        }


        public DateTime LastExecute { get; set; }

        public void Execute()
        {
            Task.Factory.StartNew(() => { CleanSpam(); });
        }

        public void CleanSpam()
        {
            try
            {
                var allOpenDb = Kooboo.Mail.Factory.DBFactory.OpenMailDbs();

                var spamfolder = new Folder(Folder.Spam);

                var CompareDateTick = DateTime.Now.AddDays(-30).Ticks;

                foreach (var maildb in allOpenDb)
                {
                    SqlWhere<Message> query = maildb.Message2.Query.Where(o => o.FolderId == spamfolder.Id && o.CreationTimeTick < CompareDateTick);

                    var totalMsgs = query.Take(3000);

                    if (totalMsgs != null && totalMsgs.Any())
                    {
                        foreach (var item in totalMsgs)
                        {
                            maildb.Message2.Delete(item.MsgId);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }
    }

}
