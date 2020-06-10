using Models;
using Models.Harvest;
using Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
    public class CreateTimeEntries : ICreateTimeEntries
    {
        private readonly IGetOffice365Events _getOffice365Events;
        private readonly IGetHarvestProjects _getHarvestProjects;
        private readonly IGenericHTTPClient _genericHTTPClient;
        private readonly IHarvestURLBuilder _harvestURLBuilder;
        private string PTOCode = TaskTypeEnum.PTO.ToString();
        private string HolidayCode = TaskTypeEnum.HOLIDAY.ToString();
        private string BillableCode = TaskTypeEnum.B2C.ToString();
        public CreateTimeEntries(
            IGetOffice365Events getOffice365Events,
            IGetHarvestProjects getHarvestProjects,
            IHarvestURLBuilder harvestURLBuilder,
            IGenericHTTPClient genericHTTPClient)
        {
            _getOffice365Events = getOffice365Events;
            _getHarvestProjects = getHarvestProjects;
            _harvestURLBuilder = harvestURLBuilder;
            _genericHTTPClient = genericHTTPClient;
        }
        public async Task<TwilioMessage> CreateTimeEntriesByRange(string fromDate, string toDate, List<TimeEntryFlattened> existingTimeEntries)
        {
            var projects = await _getHarvestProjects.GetTaskProjectAssignmentGroupings();
            var newTimeEntries = new List<TimeEntryPost>();
            var datesWithoutTime = GetDatesWithoutTimeEntries(existingTimeEntries, 6);

            await AddTimeEntriesFromCalendarEvents(fromDate, toDate, projects, newTimeEntries, datesWithoutTime);

            CreateStandardBillableEntries(projects, newTimeEntries, datesWithoutTime);

            if (newTimeEntries.Any())
            {
                var results = await PostTimeEntries(newTimeEntries);
                if (results.All(x => x.IsSuccessStatusCode))
                {
                    return new TwilioMessage
                    {
                        Message = "40 hours of billable time were successfully entered for the week, review time entries and submit time."
                    };
                }
                else
                {
                    return new TwilioMessage
                    {
                        Message = "One or more time entries failed for this week, please manually review and submit time."
                    };
                }
            }
            return new TwilioMessage
            {
                Message = "No automated time entries were created this week, please manually reivew and submit your week."
            };

        }

        private void CreateStandardBillableEntries(List<TaskProjectAssignmentGrouping> projects, List<TimeEntryPost> newTimeEntries, List<DateTime> datesWithoutTime)
        {
            var billableProject = projects.FirstOrDefault(x => x.ProjectName.Contains(BillableCode));
            foreach (var item in datesWithoutTime)
            {
                newTimeEntries.Add(new TimeEntryPost
                {
                    hours = 8,
                    spent_date = item.ToString("yyyy-MM-dd"),
                    project_id = billableProject.ProjectID,
                    task_id = billableProject.TaskID,
                });
            }
        }

        private async Task AddTimeEntriesFromCalendarEvents(
            string fromDate,
            string toDate,
            List<TaskProjectAssignmentGrouping> projects,
            List<TimeEntryPost> newTimeEntries,
            List<DateTime> datesWithoutTime)
        {
            var events = await _getOffice365Events.GetOffice365EventsFlattened(fromDate, toDate);

            if (events.Any())
            {
                var ptoProject = projects.FirstOrDefault(x => x.TaskName.Contains(PTOCode));
                var holidayProject = projects.FirstOrDefault(x => x.TaskName.Contains(HolidayCode));
                var datesToRemove = new List<DateTime>();
                foreach (var item in datesWithoutTime)
                {
                    if (events.Any(x => item.Date >= x.StartDate.Date && item.Date < x.EndDate.Date))
                    {
                        var eventToAdd = events.FirstOrDefault(x => x.StartDate.Date == item.Date);
                        if (eventToAdd != null)
                        {
                            newTimeEntries.Add(new TimeEntryPost
                            {
                                hours = 8,
                                spent_date = item.ToString("yyyy-MM-dd"),
                                project_id = eventToAdd.Subject.Contains(HolidayCode) ? holidayProject.ProjectID : ptoProject.ProjectID,
                                task_id = eventToAdd.Subject.Contains(HolidayCode) ? holidayProject.TaskID : ptoProject.TaskID,
                            });
                        }
                        //Assume PTO Range where event falls somewhere in that range of days
                        else
                        {
                            newTimeEntries.Add(new TimeEntryPost
                            {
                                hours = 8,
                                spent_date = item.ToString("yyyy-MM-dd"),
                                project_id = ptoProject.ProjectID,
                                task_id = ptoProject.TaskID
                            });
                        }
                        datesToRemove.Add(item);
                    }
                }
                foreach (var item in datesToRemove)
                {
                    datesWithoutTime.Remove(item);
                }
            }
            return;
        }

        private List<DateTime> GetDatesWithoutTimeEntries(List<TimeEntryFlattened> existingTimeEntries, int daysToSubtract)
        {
            var datesWithoutTime = new List<DateTime>();
            var dateTimeLoopIncrementer = DateTime.UtcNow.AddDays(-daysToSubtract);
            var now = DateTime.UtcNow;
            while (dateTimeLoopIncrementer.Date < now.Date)
            {
                //Filter out weekends and any dates that already exist in the range
                if (!existingTimeEntries
                    .Any(x => x.TimeEntryDate.Date == dateTimeLoopIncrementer.Date) 
                    && dateTimeLoopIncrementer.DayOfWeek != DayOfWeek.Saturday 
                    && dateTimeLoopIncrementer.DayOfWeek != DayOfWeek.Sunday)
                {
                    datesWithoutTime.Add(dateTimeLoopIncrementer);
                }
                dateTimeLoopIncrementer = dateTimeLoopIncrementer.AddDays(1);
            }

            return datesWithoutTime;
        }

        private async Task<List<HttpResponseMessage>> PostTimeEntries(List<TimeEntryPost> newTimeEntries)
        {
            var results = new List<HttpResponseMessage>();
            var harvestURLTimeEntry = _harvestURLBuilder.GetHarvestURL(Enums.HarvestHttpClientEnum.PostTimeEntry);
            foreach (var item in newTimeEntries)
            {
                results.Add(
                    await _genericHTTPClient.Post<TimeEntryPost>(Enums.GenericHttpClientEnum.HarvestClient, harvestURLTimeEntry, item)
                    );
            }
            return results;
        }
    }
}
