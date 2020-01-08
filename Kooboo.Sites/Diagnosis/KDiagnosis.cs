//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Diagnosis
{
  public  class KDiagnosis
    {

        [KIgnore]
        public DiagnosisSession session { get; set; }

        [KIgnore]
        public string headline
        {
            get
            {
                return this.session.Headline; 
            }
            set
            {
                this.session.Headline = value; 
            }
        }

       public KDiagnosis(DiagnosisSession session)
        {
            this.session = session; 
        }

        [KIgnore]
        public void onError(string message)
        {
            this.session.AddMessage(new Message() { body = message, Type = MessageType.Critical }); 
        }
        [KIgnore]
        public void  error(string message)
        {
            onError(message); 
        }
        public void addError(string message)
        {
            onError(message);
        }
        [KIgnore]
        public void onInfo(string message)
        {
            this.session.AddMessage(new Message() { body = message, Type = MessageType.Information }); 
        }
        [KIgnore]
        public void info(string message)
        {
            onInfo(message); 
        }

        public void addInfo(string message)
        {
            onInfo(message);
        }
        [KIgnore]
        public void onWarning(string message)
        {
            this.session.AddMessage(new Message() { body = message, Type = MessageType.Warning }); 
        }
        [KIgnore]
        public void warning(string message)
        {
            this.onWarning(message); 
        }

        public void addWarning(string message)
        {
            this.onWarning(message);
        }
    }
}
