using System;

namespace Kooboo.Mail.Models
{

    public class MailModule
    {
        public MailModule()
        {

        }

        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Name);
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name { get; set; }   // Identifier..

        public string Settings { get; set; }

        public string TaskJs { get; set; }  // the module task js files. 
        public bool Online { get; set; } = true;   // default is true. 
    }

}
