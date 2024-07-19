//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Newtonsoft.Json;

namespace Kooboo.Api
{
    public class ApiCall
    {
        private RenderContext _Context;
        public RenderContext Context
        {
            get
            {
                if (_Context == null)
                {
                    _Context = new RenderContext();
                }
                return _Context;
            }
            set { _Context = value; }
        }

        private ApiCommand _command;
        public ApiCommand Command
        {
            get
            {
                if (_command == null)
                {
                    _command = new ApiCommand();
                }
                return _command;
            }
            set
            {
                _command = value;
            }
        }

        private WebSite _website;
        public WebSite WebSite
        {
            get
            {
                if (_website == null)
                {
                    _website = this.Context.WebSite;
                }
                return _website;
            }
            set
            {
                _website = value;
            }
        }


        private Guid _objectid;

        public Guid ObjectId
        {
            get
            {
                if (_objectid == default(Guid))
                {
                    _objectid = TryGetId();
                }
                return _objectid;
            }
            set
            {
                _objectid = value;
            }
        }

        private string _NameOrId = null;
        public string NameOrId
        {
            get
            {
                if (_NameOrId == null)
                {
                    if (!string.IsNullOrEmpty(this.Command.Value))
                    {
                        _NameOrId = this.Command.Value;
                    }
                    else
                    {
                        _NameOrId = this.GetValue("name");
                    }

                }
                return _NameOrId;
            }
            set
            {
                _NameOrId = value;
            }
        }

        private Guid TryGetId()
        {
            Guid id = default(Guid);

            var strid = this.Context.Request.GetValue("id");
            if (!string.IsNullOrEmpty(strid))
            {
                if (System.Guid.TryParse(strid, out id))
                {
                    return id;
                }
            }

            if (!string.IsNullOrEmpty(this.Command.Value))
            {
                if (System.Guid.TryParse(this.Command.Value, out id))
                {
                    return id;
                }
            }

            if (!string.IsNullOrWhiteSpace(this.Context.Request.Body))
            {
                var data = Lib.Helper.JsonHelper.DeserializeJObject(this.Context.Request.Body);

                foreach (var item in data.Properties())
                {
                    if (item.Name.ToLower() == "id" && !string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        if (System.Guid.TryParse(item.Value.ToString(), out id))
                        {
                            return id;
                        }
                    }
                }


            }

            return id;
        }

        public string GetValue(string name)
        {
            return RequestManager.GetValue(this.Context.Request, name);
        }

        public T GetValue<T>(string name, T DefaultValue)
        {
            string Value = GetValue(name);
            if (string.IsNullOrEmpty(Value))
            {
                return DefaultValue;
            }
            else
            {
                return GetValue<T>(name);
            }
        }

        public T GetValue<T>(string name)
        {
            var type = typeof(T);
            string value = GetValue(name);
            if (type.IsArray)
            {
                if (string.IsNullOrWhiteSpace(value)) return default;
                return JsonConvert.DeserializeObject<T>(value);
            }
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            if (type == typeof(Guid))
            {
                Guid id = default(Guid);
                System.Guid.TryParse(value, out id);
                return (T)Convert.ChangeType(id, type);
            }
            else if (type == typeof(bool))
            {
                bool ok = false;
                bool.TryParse(value, out ok);
                return (T)Convert.ChangeType(ok, type);
            }
            else if (type == typeof(int))
            {
                int intvalue = 0;
                int.TryParse(value, out intvalue);
                return (T)Convert.ChangeType(intvalue, type);
            }
            else if (type == typeof(long))
            {
                long longvalue = 0;
                long.TryParse(value, out longvalue);
                return (T)Convert.ChangeType(longvalue, type);
            }
            else if (type == typeof(string))
            {
                return (T)Convert.ChangeType(value, type);
            }
            else if (type == typeof(decimal))
            {
                decimal decvalue = 0;
                decimal.TryParse(value, out decvalue);
                return (T)Convert.ChangeType(decvalue, type);

            }
            else
            {
                throw new Exception("type not supported");
            }

        }

        public Guid GetGuidValue(string name)
        {
            string value = GetValue(name);
            if (string.IsNullOrEmpty(value))
            {
                return default(Guid);
            }
            Guid id = default(Guid);

            System.Guid.TryParse(value, out id);
            return id;
        }

        public bool GetBoolValue(string name)
        {
            string value = GetValue(name);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            bool ok = false;

            bool.TryParse(value, out ok);
            return ok;
        }

        public long GetLongValue(string name)
        {
            string value = GetValue(name);
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            long longvalue = 0;

            long.TryParse(value, out longvalue);
            return longvalue;
        }

        public int GetIntValue(string name)
        {
            string value = GetValue(name);
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            int intvalue = 0;

            int.TryParse(value, out intvalue);
            return intvalue;
        }

        /// <summary>
        ///  get the value from query string or form only... 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetRequestValue(string Name)
        {
            return RequestManager.GetHttpValue(this.Context.Request, Name);
        }

        public string GetValue(params string[] names)
        {
            return RequestManager.GetValue(this.Context.Request, names);
        }

        // a fake request to fake data only.. 
        public bool IsFake
        {
            get
            {
                var fake = RequestManager.GetHttpValue(this.Context.Request, "fake");
                return fake != null;
            }
        }
    }


}
