using System;

namespace Models
{
    public class MicrosoftEvent
    {
        public string odatacontext { get; set; }
        public Event[] value { get; set; }

        public class Event
        {
            public string odataetag { get; set; }
            public string id { get; set; }
            public string subject { get; set; }
            public Start start { get; set; }
            public End end { get; set; }
            public Organizer organizer { get; set; }
        }

        public class Start
        {
            public DateTime dateTime { get; set; }
            public string timeZone { get; set; }
        }

        public class End
        {
            public DateTime dateTime { get; set; }
            public string timeZone { get; set; }
        }

        public class Organizer
        {
            public Emailaddress emailAddress { get; set; }
        }

        public class Emailaddress
        {
            public string name { get; set; }
            public string address { get; set; }
        }

    }
}
