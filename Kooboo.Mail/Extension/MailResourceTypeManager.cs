using System.Collections.Generic;

namespace Kooboo.Mail.Extension
{

    public static class MailResourceTypeManager
    {
        public static List<MailResourceType> AvailableResource()
        {
            List<MailResourceType> result = new List<MailResourceType>();

            result.Add(new MailResourceType(EnumMailResourceType.css));
            result.Add(new MailResourceType(EnumMailResourceType.js));
            result.Add(new MailResourceType(EnumMailResourceType.read));
            result.Add(new MailResourceType(EnumMailResourceType.compose));
            result.Add(new MailResourceType(EnumMailResourceType.backend));
            result.Add(new MailResourceType(EnumMailResourceType.api));
            result.Add(new MailResourceType(EnumMailResourceType.img));
            result.Add(new MailResourceType(EnumMailResourceType.file));
            result.Add(new MailResourceType(EnumMailResourceType.root));

            return result;
        }


    }


}
