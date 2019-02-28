//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Kooboo.Data.Models
{ 
    public class MailQueue : IGolbalObject
    {
        /// <summary>
        /// Full name of task model type.. 
        /// </summary>
        public string TaskModelType { get; set; }

        public string JsonModel { get; set; }

        public byte FailTimes { get; set; } = 0; 

        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid();
                }
                return _id;
            }

            set { _id = value; }
        }
    }
}
