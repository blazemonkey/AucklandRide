using AucklandRide.Services.WebClientService;
using AucklandRide.Updater.Models;
using AucklandRide.Updater.Services.RestService;
using AucklandRide.Updater.Services.SqlService;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater
{
    public class Updater
    {
        [Dependency]
        public IRestService RestService { get; set; }
        [Dependency]
        public ISqlService SqlService { get; set; }
        [Dependency]
        public IWebClientService WebClientService { get; set; }

        public async Task Run()
        {
            await SqlService.AddLogging(new Models.Logging("Run", Models.LoggingState.Started, "Begin Update"));
            var latestVers = await RestService.GetVersions();
            var dbVers = await SqlService.GetVersions();

            var updateReq = IsNewVersionDetected(dbVers, latestVers);
            if (!updateReq)
            {
                //var agencies = WebClientService.GetAgencies();
                //var calendars = WebClientService.GetCalendars();
                //var calendarDates = WebClientService.GetCalendarDates();
                //var routes = WebClientService.GetRoutes();
                //var shapes = WebClientService.GetShapes();
                //var stops = WebClientService.GetStops();
                //var stopTimes = WebClientService.GetStopTimes();
                //var trips = WebClientService.GetTrips();

                //await Task.WhenAll(agencies, calendars, calendarDates, routes, shapes, stops, stopTimes, trips);
                //await SqlService.DeleteAndReplaceAll(agencies.Result, calendars.Result, calendarDates.Result, routes.Result,
                //    shapes.Result, stops.Result, stopTimes.Result, trips.Result);

                var stops = await WebClientService.GetStops();
                await GetLocationFromGoogle(stops);

                //var routes = await WebClientService.GetRoutes();
                //await SqlService.DeleteAndReplaceAll(null, null, null, routes, null, null, null, null);


                await SqlService.UpdateTripsTime();
                await SqlService.AddVersions(latestVers);
            }

            await SqlService.AddLogging(new Models.Logging("Run", Models.LoggingState.Completed, "Update End"));
        }

        private bool IsNewVersionDetected(IEnumerable<Models.Version> dbVers, IEnumerable<Models.Version> latestVers)
        {
            if (latestVers == null)
                throw new Exception("Cannot retrieve Versions from Auckland Transport API");
            
            if (dbVers.Any())
            {
                var isEqual = latestVers.SequenceEqual(dbVers);
                return isEqual;
            }
            else
                return true;            
        }

        private async Task GetLocationFromGoogle(IEnumerable<Stop> stops)
        {
            var existingRegions = await SqlService.GetStopRegions();
            var toAdd = stops.Select(x => x.Id).Except(existingRegions.Select(x => x.StopId));

            var stopRegions = new List<StopRegion>();

            foreach (var stop in stops.Where(x => toAdd.Contains(x.Id)))
            {
                var region = await WebClientService.GetLocationFromGoogle(stop.Latitude, stop.Longitude);
                if (string.IsNullOrEmpty(region))
                    break;

                var stopRegion = new StopRegion
                {
                    StopId = stop.Id,
                    Name = region
                };

                stopRegions.Add(stopRegion);
            }

            if (stopRegions.Any())
                await SqlService.AddStopRegions(stopRegions);
        }
    }
}
