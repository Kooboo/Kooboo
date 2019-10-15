using System;
using System.Collections.Generic;
using Kooboo.Data.Models;

namespace Kooboo.Data.Session
{
    public class UserSession
    {
        public static User User { get; set; }

        public string RSAPublicKey { get; set; }

        public string RSAPrivateKey { get; set; }

        public bool Rsa { get; set; }

        public Guid SessionKey { get; set; }

        public DateTime LastUse { get; set; }

        public string RemoteHost { get; set; }

        public string ClientIp { get; set; }

        private HashSet<Guid> _usedkey;

        public HashSet<Guid> UsedKey
        {
            get => _usedkey ?? (_usedkey = new HashSet<Guid>());
            set => _usedkey = value;
        }
    }
}