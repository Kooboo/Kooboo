namespace Kooboo.IndexedDB.WORM
{
    public static class WormSetting
    {

        public static int NodePointerCount { get; set; } = 100; // the size of record pointers...

        //public static int  NodeIndexLen { get; set; } = 24;  // fixed defined, can not change. 

        public static int ConnectedNodeCount { get; set; } = 50;

        //0NUT(null)  空字符
        //1	SOH(start of headline) 标题开始
        //2	STX(start of text) 正文开始
        //3	ETX(end of text)   正文结束
        //4	EOT(end of transmission)   传输结束
        //5	ENQ(enquiry)   请求
        //6	ACK(acknowledge)   收到通知


        public static string RestoreFolder
        {
            get
            {
                var folder = "_worm_restore";
                return System.IO.Path.Combine(GlobalSettings.RootPath, folder);
            }
        }

    }
}
