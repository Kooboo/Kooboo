//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Data.Log;

namespace Kooboo.Mail.Smtp
{
    public static class Log
    {
        #region Info

        private static bool EnableDebugInfo = Kooboo.Lib.AppSettingsUtility.GetBool("smtpLog");

        public static LogWriter InfoWriter { get; set; } = new LogWriter("SmtpServerInfo");

        public static LogWriter ExceptionWriter { get; set; } = new LogWriter("SmtpException");

        public static void LogInfo(string message)
        {
            try
            {
                if (!EnableDebugInfo)
                    return;

                InfoWriter.Write(DateTime.UtcNow.ToString("MM-dd HH:mm:ss.fff") + " " + message);
            }
            catch
            {
            }
        }

        #endregion

        #region Exception

        public static void LogError(Exception ex)
        {
            ExceptionWriter.WriteException(ex);
        }
        #endregion
    }
}