using System;

namespace Models
{
    public class TwilioMessage
    {
        public string FromNumber { get; set; } = Environment.GetEnvironmentVariable("DefaultTwilioFromNumber");
        public string ToNumber { get; set; } = Environment.GetEnvironmentVariable("DefaultTwilioToNumber");
        public string Message { get; set; }

    }
}
