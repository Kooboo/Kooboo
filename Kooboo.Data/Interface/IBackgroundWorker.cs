//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Interface
{
    public interface IBackgroundWorker
    {
        /// <summary>
        /// Interval in seconds.
        /// </summary>
        int Interval { get; }

        DateTime LastExecute { get; set; }

        void Execute();
    }
}