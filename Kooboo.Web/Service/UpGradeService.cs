namespace Kooboo.Web.Service
{
    public static class UpGradeService
    {

        public static void UpgradeFix()
        {
            FixOLDSSL();
            MigrateConfigFile();
        }

        public static void FixOLDSSL()
        {
            System.Threading.Tasks.Task.Run(RenewSSL);
        }

        private static void RenewSSL()
        {
            // RENEW SSL..
            var OldSsl = Kooboo.Data.GlobalDb.OldSslCertificate.Store.Filter.SelectAll();

            foreach (var ssl in OldSsl)
            {
                if (ssl.Expiration > DateTime.Now)
                {
                    Kooboo.Data.GlobalDb.SslCertificate.AddCert(ssl.Content);
                }
            }

        }

        private static void MigrateConfigFile()
        {
            Kooboo.Data.Config.MigrateOld.Migration.Execute();
        }

    }
}
