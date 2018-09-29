//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Diagnosis
{
    public class SessionStatus
    {
        public List<Message> Messages { get; set; }

        public bool IsFinished { get; set; }

        public string CurrentGroup { get; set; }

        public string CurrentName { get; set; }

        public string HeadLine { get; set; }

        public int CriticalCount { get; set; }

        public int WarningCount { get; set; }

        public int InfoCount { get; set; }

        public bool IsCancel { get; set; }

    }
}
