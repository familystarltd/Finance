using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.Infrastructure.CrossCutting.Framework
{
    public class Event
    {
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
    }
    public class Calendar
    {
        #region PROTECTED FIELDS
        #endregion
        private async Task DownloadFile(string FileName, string DownloadFileOnline)
        {
            Uri uri;
            bool isUri = Uri.TryCreate(DownloadFileOnline, UriKind.Absolute, out uri)
                          && (uri.Scheme.ToLower() == "http"
                              || uri.Scheme.ToLower() == "https");
            using (HttpClient remoteClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                using (Stream contentStream = await (await remoteClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                    stream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(stream);
                }

                // Download the Web resource and save it into the current filesystem folder.
                //remoteClient.DownloadFile(uri, FileName);
            }
        }
        private async Task<string> GetContentFromOnlineAsync(string DownloadFileOnline)
        {
            Uri uri;
            bool isUri = Uri.TryCreate(DownloadFileOnline, UriKind.Absolute, out uri)
                          && (uri.Scheme.ToLower() == "http"
                              || uri.Scheme.ToLower() == "https");
            using (HttpClient remoteClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    return await (await remoteClient.SendAsync(request)).Content.ReadAsStringAsync();
                }

                // Download the Web resource and save it into the current filesystem folder.
                //remoteClient.DownloadFile(uri, FileName);
            }
        }

        private void SetEventData(Event calendarEvent, string Data)
        {
            char[] delim = { '\n' };
            delim[0] = ':';
            if (Data.Contains("SUMMARY:"))
            {
                calendarEvent.EventName = Regex.Replace(Data.Split(delim)[1], @"\t|\n|\r", "");
            }
            if (Data.Contains("DTSTART;VALUE=DATE:"))
            {
                calendarEvent.EventDate = DateTime.ParseExact(Regex.Replace(Data.Split(delim)[1], @"\t|\n|\r", ""), "yyyyMMdd", null);
            }
        }
        List<Event> eventData = new List<Event>();
        private void LoadCalendarData(string nameOrSourceFilePath)
        {
            string content = string.Empty;
            if (string.IsNullOrEmpty(nameOrSourceFilePath))
            {
                nameOrSourceFilePath = @"https://www.gov.uk/bank-holidays/england-and-wales.ics";
                content = Task<string>.Run(async () => await GetContentFromOnlineAsync(nameOrSourceFilePath)).Result;
            }
            else
            {
                using (FileStream fs = new FileStream(@nameOrSourceFilePath, FileMode.Open, FileAccess.Read))
                {
                    StreamReader sr = new StreamReader(fs);
                    content = sr.ReadToEnd();
                }
            }
            char[] delim = { '\n' };
            string[] lines = content.Split(delim);
            delim[0] = ':';
            Event calEvent = null;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("BEGIN:VEVENT"))
                {
                    calEvent = new Event();
                    eventData.Add(calEvent);
                }
                this.SetEventData(calEvent, lines[i]);
            }
        }
        
        public Calendar(string nameOrSourceFilePath)
        {
            this.LoadCalendarData(nameOrSourceFilePath);
        }
        public Calendar(string nameOrsourceFileOffline, string nameOrsourceFileOnline)
        {
            if (nameOrsourceFileOffline.Contains("|DataDirectory|"))
            {
                //nameOrsourceFileOffline = nameOrsourceFileOffline.Replace("|DataDirectory|", string.Format("{0}/", AppDomain.CurrentDomain.GetData("DataDirectory").ToString()));
            }
            if (!File.Exists(nameOrsourceFileOffline))
            {
                try
                {
                    if(string.IsNullOrEmpty(nameOrsourceFileOnline))
                        nameOrsourceFileOnline = @"https://www.gov.uk/bank-holidays/england-and-wales.ics";
                    Task.Run(async () => await DownloadFile(nameOrsourceFileOffline, nameOrsourceFileOnline));
                }
                catch { }
            }
            try
            {
                this.LoadCalendarData(nameOrsourceFileOffline);
            }
            catch { }
        }

        #region PROTECTED METHODS
        /// <summary>
        /// Gets a list of upcoming events (event that will occur within the
        /// next week).
        /// </summary>
        /// <returns>A list of events that will occur within the next week</returns>
        public IEnumerable<Event> GetEvents(int Year)
        {
            DateTime fromDate = new DateTime(Year, 1, 1);
            DateTime toDate = new DateTime(Year, 12, 31);
            return eventData.Where(e => e.EventDate >= fromDate && e.EventDate <= toDate);
        }

        /// <summary>
        /// Gets a list of upcoming events (event that will occur within the
        /// next week).
        /// </summary>
        /// <returns>A list of events that will occur within the next week</returns>
        public IEnumerable<Event> GetEvents(int Year, int Month)
        {
            DateTime fromDate = new DateTime(Year, Month, 1);
            DateTime toDate = new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));
            return eventData.Where(e => e.EventDate >= fromDate && e.EventDate <= toDate);
        }

        /// <summary>
        /// Gets a list of upcoming events (event that will occur within the
        /// next week).
        /// </summary>
        /// <returns>A list of events that will occur within the next week</returns>
        public IEnumerable<Event> GetEvents(DateTime fromDate, DateTime toDate)
        {
            return eventData.Where(e => e.EventDate >= fromDate && e.EventDate <= toDate);
        }
        #region METHODS
        /// <summary>
        /// Observe that the method calculates by including the start day, and end day
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public void GetDays(DateTime startDate, DateTime endDate,out int weekdays, out int saturdays, out int sundays,out int bankHolidays)
        {
            weekdays = 0;
            saturdays = -1;
            sundays = -1;
            bankHolidays = 0;
            endDate = endDate.AddDays(1);// include end day as well
            if (endDate < startDate)
                return;
            TimeSpan timeBetween = endDate.Subtract(startDate);
            int weekendsBetween = timeBetween.Days / 7;
            sundays = weekendsBetween;
            saturdays = weekendsBetween;
            int startDay = GetDayOfWeekNumber(startDate.DayOfWeek);
            int endDay = GetDayOfWeekNumber(endDate.DayOfWeek);
            if (startDay > endDay)
            {
                sundays++;
                saturdays += (startDay < 7) ? 1 : 0;
            }
            else if (startDay < endDay)
            {
                //We don't have to care about sundays (SUNDAY = 7) here, since we are excluding the last day
                //There will only be another saturday, if the end day is a sunday
                saturdays += (endDay == 7) ? 1 : 0;
            }
            int noOfWeekEnds = saturdays + sundays;
            bankHolidays = this.GetEvents(startDate.Date, endDate.Date).Count();
            int noOfDays = (int)(endDate.Date - startDate.Date).TotalDays;
            weekdays = noOfDays - (noOfWeekEnds + bankHolidays);
        }
        /// <summary>
        /// Since I don't think it's a good idea to rely on the face that the enums have specific values, I wrote this method
        /// If you are satisfied with using the integer value of the enums, just remember that Sundays will then have the value 0
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private static int GetDayOfWeekNumber(DayOfWeek day)
        {
            return day == DayOfWeek.Sunday ? ((int)DayOfWeek.Sunday) + 1 : (int)day;
        }
        #endregion
        #endregion
    }
}
