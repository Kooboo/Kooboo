//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.FrontEvent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Models
{
    public class BusinessRule : CoreObject
    {
        public BusinessRule()
        {
            // TODO: set the business rule type here... 
            this.ConstType = ConstObjectType.BusinessRule;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public FrontEvent.enumEventType EventType { get; set; }

        private IFElseRule _rule;

        public IFElseRule Rule
        {
            get
            {
                if (_rule == null)
                {
                    _rule = new IFElseRule();
                }
                return _rule;
            }
            set
            {
                _rule = value;
            }
        }
    }

    public class IFElseRule
    {
        private List<Condition> _if;

        public List<Condition> IF
        {
            get
            {
                if (_if == null)
                {
                    _if = new List<Condition>();
                }
                return _if;
            }
            set
            {
                _if = value;
            }
        }

        public List<IFElseRule> Then
        {
            get; set;
        }

        public List<IFElseRule> Else
        {
            get; set;
        }

        private List<Action> _do;
        public List<Action> Do
        {
            get
            {
                if (_do == null)
                {
                    _do = new List<Action>();
                }
                return _do;
            }
            set
            {
                _do = value;
            }
        }

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
            set
            {
                _id = value;
            }
        }

        public override int GetHashCode()
        {
            string unique = string.Empty;
            if (_if != null && _if.Count > 0)
            {
                foreach (var item in _if)
                {
                    unique = item.Left + item.Operator + item.Right;
                }
            }

            if (this.Then != null && this.Then.Count > 0)
            {
                foreach (var item in this.Then)
                {
                    unique += item.GetHashCode().ToString();
                }
            }

            if (this.Else != null && this.Else.Count > 0)
            {
                foreach (var item in this.Else)
                {
                    unique += item.GetHashCode().ToString();
                }
            }

            if (_do != null && _do.Count > 0)
            {
                foreach (var item in _do)
                {
                    unique += item.CodeId.ToString();
                    if (item.Setting != null && item.Setting.Count > 0)
                    {
                        foreach (var dict in item.Setting)
                        {
                            unique += dict.Key + dict.Value;
                        }
                    }
                }
            }

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }

    public class Action
    {
        public Guid CodeId { get; set; }

        public Dictionary<string, string> Setting { get; set; }
    }

}
