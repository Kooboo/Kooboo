//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using System;
using System.Linq;

namespace Kooboo.Api
{
    public class ApiCall
    {
        private RenderContext _context;

        public RenderContext Context
        {
            get => _context ?? (_context = new RenderContext());
            set => _context = value;
        }

        private ApiCommand _command;

        public ApiCommand Command
        {
            get => _command ?? (_command = new ApiCommand());
            set => _command = value;
        }

        private WebSite _website;

        public WebSite WebSite
        {
            get => _website ?? (_website = Context.WebSite);
            set => _website = value;
        }

        private Guid _objectid;

        public Guid ObjectId
        {
            get
            {
                if (_objectid == default) _objectid = TryGetId();
                return _objectid;
            }
            set => _objectid = value;
        }

        private string _nameOrId;

        public string NameOrId
        {
            get =>
                _nameOrId ??
                (_nameOrId = !string.IsNullOrEmpty(Command.Value) ? Command.Value : GetValue("name"));
            set => _nameOrId = value;
        }

        private Guid TryGetId()
        {
            Guid id = default;

            var strid = Context.Request.GetValue("id");
            if (!string.IsNullOrEmpty(strid))
                if (Guid.TryParse(strid, out id))
                    return id;

            if (!string.IsNullOrEmpty(Command.Value))
                if (Guid.TryParse(Command.Value, out id))
                    return id;

            if (!string.IsNullOrWhiteSpace(Context.Request.Body))
            {
                var data = Lib.Helper.JsonHelper.DeserializeJObject(Context.Request.Body);

                if (data.Properties().Any(item => item.Name.ToLower() == "id" && !string.IsNullOrEmpty(item.Value.ToString()) && Guid.TryParse(item.Value.ToString(), out id)))
                {
                    return id;
                }
            }

            return id;
        }

        public string GetValue(string name)
        {
            return RequestManager.GetValue(Context.Request, name);
        }

        public T GetValue<T>(string name)
        {
            var type = typeof(T);
            var value = GetValue(name);
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            if (type == typeof(Guid))
            {
                Guid.TryParse(value, out Guid id);
                return (T)Convert.ChangeType(id, type);
            }
            else if (type == typeof(bool))
            {
                bool.TryParse(value, out bool ok);
                return (T)Convert.ChangeType(ok, type);
            }
            else if (type == typeof(int))
            {
                int.TryParse(value, out int intvalue);
                return (T)Convert.ChangeType(intvalue, type);
            }
            else if (type == typeof(long))
            {
                long.TryParse(value, out long longvalue);
                return (T)Convert.ChangeType(longvalue, type);
            }
            else if (type == typeof(string))
            {
                return (T)Convert.ChangeType(value, type);
            }
            else if (type == typeof(decimal))
            {
                decimal.TryParse(value, out decimal decvalue);
                return (T)Convert.ChangeType(decvalue, type);
            }
            else
            {
                throw new Exception("type of not supported");
            }
        }

        public Guid GetGuidValue(string name)
        {
            var value = GetValue(name);
            if (string.IsNullOrEmpty(value))
                return default;
            Guid.TryParse(value, out Guid id);
            return id;
        }

        public bool GetBoolValue(string name)
        {
            var value = GetValue(name);
            if (string.IsNullOrEmpty(value))
                return false;

            bool.TryParse(value, out bool ok);
            return ok;
        }

        public long GetLongValue(string name)
        {
            var value = GetValue(name);
            if (string.IsNullOrEmpty(value))
                return 0;

            long.TryParse(value, out long longvalue);
            return longvalue;
        }

        public int GetIntValue(string name)
        {
            var value = GetValue(name);
            if (string.IsNullOrEmpty(value))
                return 0;

            int.TryParse(value, out int intvalue);
            return intvalue;
        }

        /// <summary>
        ///  get the value from query string or form only...
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetRequestValue(string name)
        {
            return RequestManager.GetHttpValue(Context.Request, name);
        }

        public string GetValue(params string[] names)
        {
            return RequestManager.GetValue(Context.Request, names);
        }

        // a fake request to fake data only..
        public bool IsFake
        {
            get
            {
                var fake = RequestManager.GetHttpValue(Context.Request, "fake");
                return fake != null;
            }
        }
    }
}