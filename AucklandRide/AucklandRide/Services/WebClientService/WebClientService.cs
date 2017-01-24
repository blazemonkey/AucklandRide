using AucklandRide.Updater.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Services.WebClientService
{
    public class WebClientService : IWebClientService
    {
        private string _url = "https://cdn01.at.govt.nz/data/";

        public async virtual Task<string> DownloadFile(string url)
        {
            try
            {
                var result = string.Empty;
                using (var client = new HttpClient())
                {
                    result = await client.GetStringAsync(url);
                }

                return result;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public async Task<IEnumerable<Agency>> GetAgencies()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "agency.txt"));
            if (string.IsNullOrEmpty(result))
                return null;

            return ParseResponse<Agency, AgencyMap>(result);
        }

        public async Task<IEnumerable<Calendar>> GetCalendars()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "calendar.txt"));
            if (string.IsNullOrEmpty(result))
                return null;

            return ParseResponse<Calendar, CalendarMap>(result);
        }

        public async Task<IEnumerable<CalendarDate>> GetCalendarDates()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "calendar_dates.txt"));
            if (string.IsNullOrEmpty(result))
                return null;

            return ParseResponse<CalendarDate, CalendarDateMap>(result);
        }

        public async Task<IEnumerable<Route>> GetRoutes()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "routes.txt"));
            if (string.IsNullOrEmpty(result))
                return null;

            return ParseResponse<Route, RouteMap>(result);              
        }

        public async Task<IEnumerable<Shape>> GetShapes()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "shapes.txt"));
            if (string.IsNullOrEmpty(result))
                return null;

            return ParseResponse<Shape, ShapeMap>(result);
        }

        public async Task<IEnumerable<Stop>> GetStops()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "stops.txt"));
            if (string.IsNullOrEmpty(result))
                return null;

            return ParseResponse<Stop, StopMap>(result);
        }

        public async Task<IEnumerable<StopTime>> GetStopTimes()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "stop_times.txt"));
            if (string.IsNullOrEmpty(result))
                return null;

            return ParseResponse<StopTime, StopTimeMap>(result);
        }

        public async Task<IEnumerable<Trip>> GetTrips()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "trips.txt"));
            if (string.IsNullOrEmpty(result))
                return null;

            return ParseResponse<Trip, TripMap>(result);
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
