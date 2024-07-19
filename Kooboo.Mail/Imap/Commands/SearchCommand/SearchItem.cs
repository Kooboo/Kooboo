//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Mail.Imap.Commands.SearchCommand
{
    public class SearchItem
    {
        public SearchType Type { get; set; } = SearchType.Normal;

        public string Name { get; set; }

        public bool IsSequence { get; set; }

        public bool AsCollection { get; set; }

        public SearchItem NOT { get; set; }

        public SearchItem OROne { get; set; }

        public SearchItem ORTwo { get; set; }

        private Dictionary<string, object> _parameters;
        public Dictionary<string, object> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        public int SeqCompareUid { get; set; } = -1;

    }
}
