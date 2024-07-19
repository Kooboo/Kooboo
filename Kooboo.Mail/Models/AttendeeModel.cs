using System;

namespace Kooboo.Mail.Models
{
    public class AttendeeModel
    {
        public AttendeeModel() { }

        public AttendeeModel(string mailto)
        {
            this.Value = new Uri(mailto);
        }

        public AttendeeModel(Uri mailto)
        {
            this.Value = mailto;
        }

        public Uri Value { get; set; }

        public string Type { get; set; }
        public string Role { get; set; }
        public string CommonName { get; set; }
        public bool Rsvp { get; set; }
        public string ParticipationStatus { get; set; }
    }
}
