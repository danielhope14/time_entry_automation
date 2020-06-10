using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class GetOffice365Events : IGetOffice365Events
    {
        private readonly ICreateMicrosoftTokenRefresh _createMicrosoftTokenRefresh;
        private readonly IGenericHTTPClient _genericHTTPClient;
        public GetOffice365Events(ICreateMicrosoftTokenRefresh createMicrosoftTokenRefresh, IGenericHTTPClient genericHTTPClient)
        {
            _createMicrosoftTokenRefresh = createMicrosoftTokenRefresh;
            _genericHTTPClient = genericHTTPClient;
        }

        public async Task<List<MicrosoftEventFlattened>> GetOffice365EventsFlattened(string fromDate, string toDate)
        {
            var formData = await _createMicrosoftTokenRefresh.GetMicrosoftTokenRefreshData();
            var microsoftToken = await _genericHTTPClient.PostForm<MicrosoftTokenRefresh>(
                Enums.GenericHttpClientEnum.MicrosoftTokenRefreshClient,
                " https://login.microsoftonline.com/organizations/oauth2/v2.0/token", formData);
            var events = new List<MicrosoftEventFlattened>();
            var myEvents = await _genericHTTPClient.Get<MicrosoftEvent>(
                $"https://graph.microsoft.com/v1.0/me/calendar/calendarView?startDateTime={fromDate}&endDateTime={toDate}&$top=100&$select=subject,start,end,organizer",
                microsoftToken.access_token);
            var companyEvents = await _genericHTTPClient.Get<MicrosoftEvent>(
                $"https://graph.microsoft.com/v1.0/users/hello@allata.com/calendar/calendarView?startDateTime={fromDate}&endDateTime={toDate}&$top=100&$select=subject,start,end,organizer",
                microsoftToken.access_token);
            //Get everything that's a holiday or PTO from my calendar and the company's calendar and merge them into one list
            //I'm being fairly defensive on these binds because Microsofts APIs are a little sketchy
            events.AddRange(myEvents?.value?.Where(x => x.subject.ToUpper().Contains("PTO") && x.organizer.emailAddress.address == Environment.GetEnvironmentVariable("MyEmail"))
                ?.Select(x => new MicrosoftEventFlattened
                {
                    EndDate = x.end.dateTime,
                    StartDate = x.start.dateTime,
                    Subject = x.subject.ToUpper(),
                })?.ToList() ?? new List<MicrosoftEventFlattened>());
            events.AddRange(companyEvents?.value?.Where(x => x.subject.ToUpper().Contains("HOLIDAY"))
                ?.Select(x => new MicrosoftEventFlattened
                {
                    EndDate = x.end.dateTime,
                    StartDate = x.start.dateTime,
                    Subject = x.subject.ToUpper(),
                })?.ToList() ?? new List<MicrosoftEventFlattened>());
            return events;
        }
    }
}
