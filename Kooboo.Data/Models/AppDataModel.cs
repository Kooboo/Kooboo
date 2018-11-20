using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class AppDataModel
    {
        public byte[] Bytes { get; set; }

        private Guid _bodyhash;

        public Guid ByteHash
        {
            get
            {
                if (_bodyhash == default(Guid) && Bytes != null)
                {
                    _bodyhash = Lib.Security.Hash.ComputeGuid(Bytes);
                }
                return _bodyhash;
            }
            set { _bodyhash = value; }
        }

        public string Link { get; set; }

        public string Name { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid UserId { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }

    }
}
