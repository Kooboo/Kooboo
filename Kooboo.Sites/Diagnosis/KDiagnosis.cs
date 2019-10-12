//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.Diagnosis
{
    public class KDiagnosis
    {
        public DiagnosisSession session { get; set; }

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

        public void onError(string message)
        {
            this.session.AddMessage(new Message() { body = message, Type = MessageType.Critical });
        }

        public void error(string message)
        {
            onError(message);
        }

        public void addError(string message)
        {
            onError(message);
        }

        public void onInfo(string message)
        {
            this.session.AddMessage(new Message() { body = message, Type = MessageType.Information });
        }

        public void info(string message)
        {
            onInfo(message);
        }

        public void addInfo(string message)
        {
            onInfo(message);
        }

        public void onWarning(string message)
        {
            this.session.AddMessage(new Message() { body = message, Type = MessageType.Warning });
        }

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