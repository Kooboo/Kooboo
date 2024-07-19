using System;
using System.Collections.Generic;
using System.Linq;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Kooboo.Mail.Models;
using MimeKit;
using NUglify.Helpers;

namespace Kooboo.Mail.Utility
{
    public static class ICalendarUtility
    {
        public static string GenerateICalendarContent(ICalendarModel model)
        {
            CalendarEvent calendarEvent = new CalendarEvent()
            {
                DtStart = new CalDateTime(model.Start),
                DtEnd = new CalDateTime(model.End),
                DtStamp = CalDateTime.Now,
                Summary = model.Summary,
                Description = model.Description,
                Location = model.Location,
                Organizer = model.Organizer,
                //Attendees = model.Attendees,
                Uid = model.Uid,
                Created = CalDateTime.Now,
                LastModified = CalDateTime.Now,
                Transparency = "OPAQUE",
                Status = "CONFIRMED"
            };


            SetICalObjectAttendees(model, calendarEvent);

            calendarEvent.Alarms.Add(new Alarm()
            {
                Action = "DISPLAY",
                Description = "This is an event reminder",
                Trigger = new Trigger("-P0DT0H30M0S"),
            });

            var iCal = new Calendar();
            iCal.AddLocalTimeZone(new DateTime(DateTime.UtcNow.Year, 1, 1), true);
            iCal.Calendar.Method = "REQUEST";
            iCal.Scale = "GREGORIAN";
            iCal.Events.Add(calendarEvent);

            var serializer = new CalendarSerializer(iCal);
            string iCalContext = serializer.SerializeToString();
            return iCalContext;
        }

        public static List<ICalendarViewModel> AnalysisICalendarContent(string content)
        {
            if (string.IsNullOrEmpty(content)) return null;

            Calendar calendar = Calendar.Load(content);
            if (!calendar.Method.Equals("REQUEST", StringComparison.OrdinalIgnoreCase)) return null;
            List<ICalendarViewModel> calendarViewModels = new List<ICalendarViewModel>();
            foreach (CalendarEvent calendarEvent in calendar.Events)
            {
                ICalendarViewModel calendarViewModel = new ICalendarViewModel();
                calendarViewModel.ConvertByCalendarEvent(calendarEvent);
                calendarViewModel.Uid = calendarEvent.Uid;
                calendarViewModel.Start = calendarEvent.Start.AsUtc;
                calendarViewModel.End = calendarEvent.End.AsUtc;
                calendarViewModels.Add(calendarViewModel);
            }

            return calendarViewModels;
        }

        public static string ReplyCalendarPartStat(string icsBody, int partStat, string mailFrom)
        {
            Calendar calendar = Calendar.Load(icsBody);
            CalendarEvent calendarEvent = calendar.Events[0];
            calendarEvent.Calendar.Method = "REPLY";
            calendarEvent.DtStamp = CalDateTime.Now;

            List<Attendee> attendees = calendarEvent.Attendees.ToList();

            var mailboxAddress = MailboxAddress.Parse(mailFrom);
            Attendee selfAttendee = new Attendee($"mailto:{mailboxAddress.Address}");
            selfAttendee.Rsvp = true;
            selfAttendee.Role = "REQ-PARTICIPANT";
            selfAttendee.CommonName = mailboxAddress.Address;
            selfAttendee.Type = "INDIVIDUAL";
            if (partStat == 1)
            {
                selfAttendee.ParticipationStatus = "ACCEPTED";
            }
            else if (partStat == 2)
            {
                selfAttendee.ParticipationStatus = "DECLINED";
            }
            else if (partStat == 3)
            {
                selfAttendee.ParticipationStatus = "TENTATIVE";
            }
            calendarEvent.Attendees.Clear();
            calendarEvent.Attendees.Add(selfAttendee);

            var serializer = new CalendarSerializer(calendar);
            return serializer.SerializeToString();
        }

        public static string UpdateAndCancelICalendar(ICalendarModel model, bool isCancel)
        {
            CalendarEvent calendarEvent = new CalendarEvent()
            {
                DtStart = new CalDateTime(model.Start),
                DtEnd = new CalDateTime(model.End),
                DtStamp = CalDateTime.Now,
                Summary = model.Summary,
                Description = model.Description,
                Location = model.Location,
                Organizer = model.Organizer,
                //Attendees = model.Attendees,
                Uid = model.Uid,
                Created = CalDateTime.Now,
                LastModified = CalDateTime.Now,
            };

            SetICalObjectAttendees(model, calendarEvent);

            var iCal = new Calendar();
            iCal.AddLocalTimeZone(new DateTime(DateTime.UtcNow.Year, 1, 1), true);
            if (isCancel)
            {
                iCal.Calendar.Method = "CANCEL";
                calendarEvent.Status = "CANCELLED";
                calendarEvent.Sequence = 1;
            }
            else
            {
                iCal.Calendar.Method = "REQUEST";
                calendarEvent.Status = "CONFIRMED";
                calendarEvent.Sequence = 0;
            }
            iCal.Events.Add(calendarEvent);

            var serializer = new CalendarSerializer(iCal);
            string iCalContext = serializer.SerializeToString();
            return iCalContext;
        }

        private static void SetICalObjectAttendees(ICalendarModel model, CalendarEvent calendarEvent)
        {
            if (model.Attendees != null)
            {
                var attendees = new List<Attendee>();
                foreach (var item in model.Attendees)
                {
                    var attendee = new Attendee(item.Value)
                    {
                        Type = item.Type,
                        CommonName = item.CommonName,
                        ParticipationStatus = item.ParticipationStatus,
                        Role = item.Role,
                        Rsvp = item.Rsvp
                    };
                    attendees.Add(attendee);
                }
                calendarEvent.Attendees = attendees;
            }
        }

        public class ICalendarModel
        {
            public DateTime Start { get; set; }

            public DateTime End { get; set; }

            public string Summary { get; set; }

            public string Description { get; set; }

            public Organizer Organizer { get; set; }

            public List<AttendeeModel> Attendees { get; set; }

            public string Location { get; set; }

            public string Uid { get; set; }

            public ICalendarModel ConvertByCalendarInfo(CalendarInfo calendarInfo)
            {
                Start = calendarInfo.Start;
                End = calendarInfo.End;
                Summary = calendarInfo.CalendarTitle;
                Description = calendarInfo.Mark;
                if (calendarInfo.Organizer != null)
                {
                    Organizer = new Organizer()
                    {
                        CommonName = calendarInfo.Organizer
                    };
                }
                var attendees = new List<AttendeeModel>();
                var originEmail = MailboxAddress.Parse(calendarInfo.Organizer);
                calendarInfo.Attendees.ForEach(v =>
                {
                    if (!originEmail.Address.Equals(AddressUtility.GetAddress(v.Value.ToString().Replace("mailto:", "")), StringComparison.OrdinalIgnoreCase))
                    {
                        AttendeeModel attendee = new AttendeeModel(v.Value)
                        {
                            Type = "INDIVIDUAL",
                            Role = "REQ-PARTICIPANT",
                            CommonName = v.CommonName,
                            Rsvp = true,
                            ParticipationStatus = string.IsNullOrEmpty(v.ParticipationStatus) ? "NEEDS-ACTION" : v.ParticipationStatus,
                        };
                        attendees.Add(attendee);
                    }
                });
                Attendees = attendees;
                Location = calendarInfo.Location;
                Uid = calendarInfo.Id;
                return this;
            }

            public CalendarInfo ConvertToCalendarInfo()
            {
                CalendarInfo calendarInfo = new CalendarInfo()
                {
                    Id = Uid,
                    Start = Start,
                    End = End,
                    Attendees = this.Attendees,
                    Organizer = this.Organizer.Value.ToString().Replace("mailto:", ""),
                    CalendarTitle = Summary,
                    Mark = Description,
                    Location = Location
                };
                return calendarInfo;
            }

        }

        public class ICalendarViewModel
        {
            public DateTime Start { get; set; }

            public DateTime End { get; set; }

            public string Organizer { get; set; }

            public List<string> Attendees { get; set; }

            public string Summary { get; set; }

            public string Location { get; set; }

            public string Description { get; set; }

            public string Uid { get; set; }

            public bool IsExpired { get; set; }

            public ICalendarViewModel ConvertByCalendarEvent(CalendarEvent calendarEvent)
            {
                Description = calendarEvent.Description;
                Summary = calendarEvent.Summary;
                Start = calendarEvent.Start.AsUtc;
                End = calendarEvent.End.AsUtc;
                Uid = calendarEvent.Uid;
                Location = calendarEvent.Location;
                List<string> tempAttendees = new List<string>();
                if (calendarEvent.Organizer != null)
                {
                    Organizer = calendarEvent.Organizer.Value.ToString().Replace("mailto:", "");
                    tempAttendees.Add(Organizer);
                }
                if (calendarEvent.Attendees != null)
                {
                    calendarEvent.Attendees.ForEach(v =>
                    {
                        var organizerAddress = MailboxAddress.Parse(Organizer).Address;
                        var attendeeAddress = v.Value!.ToString().Replace("mailto:", "");
                        if (!organizerAddress.Equals(MailboxAddress.Parse(attendeeAddress).Address))
                        {
                            tempAttendees.Add(attendeeAddress);
                        }
                    });
                    Attendees = tempAttendees;
                }
                IsExpired = calendarEvent.Start.AsUtc.ToLocalTime() < DateTime.UtcNow.ToLocalTime();
                return this;
            }
        }
    }
}
