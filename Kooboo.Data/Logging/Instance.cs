//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Text;

namespace Kooboo.Data.Log
{

    public static class Instance
    {
        private static object _locker = new object();

        private static LogWriter _http; 
        public static LogWriter Http
        {
            get
            {
                if (_http == null)
                {
                    lock(_locker)
                    {
                        if (_http == null)
                        {
                            _http = new LogWriter("http"); 
                        }
                    }
                }
                return _http; 
            }
        }


        private static LogWriter _exception; 

        public static LogWriter Exception
        {
            get
            {
                if (_exception== null)
                {
                    lock(_locker)
                    {
                        if (_exception== null)
                        {
                            _exception = new LogWriter("exception"); 
                        }
                    }
                }
                return _exception; 
            }
        }
    }
 
}
