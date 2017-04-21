using AucklandRide.Updater.Models;
using AucklandRide.Updater.Services.SqlService;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AucklandRide.Services.WebClientService
{
    public class WebClientService : IWebClientService
    {
        [Dependency]
        public ISqlService SqlService { get; set; }

        public async virtual Task<string> DownloadFile(string url)
        {
            try
            {
                var result = string.Empty;
                using (var client = new HttpClient() { Timeout = new TimeSpan(0, 10, 0) })
                {
                    result = await client.GetStringAsync(url);
                }

                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<string> GetLocationFromGoogle(decimal latitude, decimal longitude)
        {
            try
            {
                var url = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false", latitude, longitude);
                var result = string.Empty;
                using (var client = new HttpClient() { Timeout = new TimeSpan(0, 10, 0) })
                {
                    result = await client.GetStringAsync(url);
                }

                var reader = XmlReader.Create(new StringReader(result));
                var serializer = new XmlSerializer(typeof(Geocode));

                var de = serializer.Deserialize(reader) as Geocode;
                if (de.Status != "OK")
                    return string.Empty;

                var sublocality = de.Result.FirstOrDefault(x => x.AddressComponent.Exists(z => z.Type.Contains("sublocality")));
                string region = "";
                if (sublocality != null)
                    region = sublocality.AddressComponent.FirstOrDefault(x => x.Type.Contains("sublocality")).ShortName;
                
                if (string.IsNullOrEmpty(region))
                {
                    var locality = de.Result.FirstOrDefault(x => x.AddressComponent.Exists(z => z.Type.Contains("locality")));
                    if (locality != null)
                        region = locality.AddressComponent.FirstOrDefault(x => x.Type.Contains("locality")).ShortName;
                }

                return region;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<IEnumerable<Agency>> GetAgencies()
        {
            return await ExecuteGet<Agency, AgencyMap>("GetAgencies", "agency.txt");
        }

        public async Task<IEnumerable<Calendar>> GetCalendars()
        {
            return await ExecuteGet<Calendar, CalendarMap>("GetCalendars", "calendar.txt");
        }

        public async Task<IEnumerable<CalendarDate>> GetCalendarDates()
        {
            return await ExecuteGet<CalendarDate, CalendarDateMap>("GetCalendarDates", "calendar_dates.txt");
        }

        public async Task<IEnumerable<Route>> GetRoutes()
        {
            return await ExecuteGet<Route, RouteMap>("GetRoutes", "routes.txt");
        }

        public async Task<IEnumerable<Shape>> GetShapes()
        {
            return await ExecuteGet<Shape, ShapeMap>("GetShapes", "shapes.txt");
        }

        public async Task<IEnumerable<Stop>> GetStops()
        {
            return await ExecuteGet<Stop, StopMap>("GetStops", "stops.txt");
        }

        public async Task<IEnumerable<StopTime>> GetStopTimes()
        {
            return await ExecuteGet<StopTime, StopTimeMap>("GetStopTimes", "stop_times.txt");
        }

        public async Task<IEnumerable<Trip>> GetTrips()
        {
            return await ExecuteGet<Trip, TripMap>("GetTrips", "trips.txt");
        }

        private async Task<IEnumerable<T>> ExecuteGet<T, TMap>(string methodName, string fileName) where TMap : CsvClassMap
        {
            await SqlService.AddLogging(new Logging(methodName, LoggingState.Started));
            var result = await DownloadFile(string.Format("{0}{1}", "https://cdn01.at.govt.nz/data/", fileName));
            if (string.IsNullOrEmpty(result))
                return null;

            await SqlService.AddLogging(new Logging(methodName, LoggingState.Completed));
            return ParseResponse<T, TMap>(result);
        }

        private List<T> ParseResponse<T, TMap>(string result) where TMap : CsvClassMap
        {
            using (var tr = new StringReader(result))
            {
                var csv = new CsvReader(tr);
                csv.Configuration.RegisterClassMap<TMap>();
                var records = csv.GetRecords<T>().ToList();
                return records;
            }
        }
    }
}
