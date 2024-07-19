using System.Collections.Generic;
using Kooboo.Mail.Models;

namespace Kooboo.Mail.Repositories.Sqlite
{
    public class DmarcReportRepo
    {

        private string TableName
        {
            get
            {
                return "DmarcReport";
            }
        }

        private OrgDb orgdb { get; set; }

        public DmarcReportRepo(OrgDb db)
        {
            this.orgdb = db;
        }

        public IEnumerable<DmarcReport> Last()
        {
            string sql = "SELECT * FROM DmarcReport Order By Id DESC  limit 0, 100;";
            return this.orgdb.SqliteHelper.Query<DmarcReport>(sql);
        }

        public bool Add(DmarcReport value)
        {
            this.orgdb.SqliteHelper.AddOrUpdate(value, this.TableName, nameof(DmarcReport.Id));
            return true;
        }

    }
}






