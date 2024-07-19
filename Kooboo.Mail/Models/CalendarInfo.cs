using System;
using System.Collections.Generic;

namespace Kooboo.Mail.Models
{
    public class CalendarInfo
    {
        public string Id { get; set; }
        public string CalendarTitle { get; set; }
        public string Organizer { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<AttendeeModel> Attendees { get; set; }
        public string Location { get; set; }
        public string Mark { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }

    }
}
