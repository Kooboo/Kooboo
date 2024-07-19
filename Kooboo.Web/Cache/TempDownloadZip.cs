//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.Cache
{
    public static class TempDownloadZip
    {
        public static Dictionary<Guid, string> DownloadPath = new Dictionary<Guid, string>();
        static TempDownloadZip()
        {

        }


        public static Guid AddPath(string path)
        {
            List<Guid> idsToRemove = new List<Guid>();
            foreach (var item in DownloadPath)
            {
                var time = Lib.Helper.IDHelper.ExtractTimeFromGuid(item.Key);
                if (time < DateTime.Now.AddDays(-1))
                {
                    idsToRemove.Add(item.Key);
                }
            }

            foreach (var item in idsToRemove)
            {
                DownloadPath.Remove(item);
            }

            Guid newid = Lib.Helper.IDHelper.NewTimeGuid(DateTime.Now);

            DownloadPath[newid] = path;

            return newid;
        }

        public static string GetPath(Guid Id)
        {
            if (DownloadPath.ContainsKey(Id))
            {
                return DownloadPath[Id];
            }
            return null;
        }
    }
}
