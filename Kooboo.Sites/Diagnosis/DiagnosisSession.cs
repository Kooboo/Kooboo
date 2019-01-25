//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Sites.Diagnosis
{
    public class DiagnosisSession
    {
        public Progress Progress { get; set; } = new Progress();

        public RenderContext context { get; set; }

        // all checkers that will be checking... 
        public List<DiagnosisChecker> AllCheckers { get; set; } = new List<DiagnosisChecker>();

        public DiagnosisChecker Current { get; set; }

        public string Headline { get; set; }

        public bool IsFinished { get; set; }

        public int CriticalCount { get; set; } = 0;

        public int informationCount { get; set; } = 0;

        public int WarningCount { get; set; } = 0;

        public List<Message> Messages = new List<Message>();
         
        public void AddMessage(string head, string body, MessageType type)
        {
            string msg = head + ": " + body;
            AddMessage(new Message() { body = msg, Type = type });
        }
         
        private object _locker = new object();

        public void AddMessage(Message message)
        {
            lock (_locker)
            {
                message.CheckerId = this.Current.Id; 
                this.Messages.Add(message);
                if (message.Type == MessageType.Critical)
                {
                    this.CriticalCount += 1;
                }
                else if (message.Type == MessageType.Warning)
                {
                    this.WarningCount += 1;
                }
                else
                {
                    this.informationCount += 1;
                }
            } 
        }

        public List<Message> FlushMessage()
        {
            lock (_locker)
            {
                var old = this.Messages;
                this.Messages = new List<Message>();
                return old;
            }
        }

    }
}
