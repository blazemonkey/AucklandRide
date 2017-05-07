using AucklandRide.UWP.Models;
using AucklandRide.UWP.Models.AT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Services
{
    public class RestService
    {
        private const string SUBSCRIPTION_KEY = "633dea42ee4c4a46a7ff49d70921664d";
        private const string SERVER_URL = "http://aucklandrideapi.azurewebsites.net/api/";
        private const string AT_SERVER_URL = "https://api.at.govt.nz/v2/";

        private static void ClientHeaderInfo(HttpClient client, List<KeyValuePair<string, string>> headers = null)
        {
            client.BaseAddress = new Uri(SERVER_URL);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers != null)
            {
                foreach (var h in headers)
                {
                    client.DefaultRequestHeaders.Add(h.Key, h.Value);
                }
            }
        }

        private static async Task<T> Get<T>(string resourceUri, List<KeyValuePair<string, string>> headers = null, List < KeyValuePair<string, string>> parameters = null)
        {
            var handler = new HttpClientHandler { UseDefaultCredentials = true };
            using (var client = new HttpClient(handler))
            {
                client.Timeout = new TimeSpan(0, 10, 0);
                ClientHeaderInfo(client, headers);
                try
                {
                    if (parameters != null)
                    {
                        resourceUri = resourceUri + "?";
                        foreach (var p in parameters)
                        {
                            resourceUri = resourceUri + p.Key + "=" + p.Value + "&";
                        }
                    }

                    var result = await client.GetAsync(resourceUri);
                    var content = await result.Content.ReadAsStringAsync();
                    var container = JsonConvert.DeserializeObject<T>(content);
                    return container;
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
        }

        private static async Task<List<T>> GetResources<T>(string uri, string id = "")
        {
            uri = string.Format("{0}/{1}", uri, id);
            return await Get<List<T>>(uri);
        }

        private static async Task<T> GetResource<T>(string uri, string id)
        {
            uri = string.Format("{0}/{1}", uri, id);
            return await Get<T>(uri);
        }

        private static async Task<T> GetResourceFromAT<T>(string uri, List<KeyValuePair<string, string>> parameters = null)
        {
            var headers = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY) };
            uri = string.Format("{0}{1}", AT_SERVER_URL, uri);
            return await Get<T>(uri, headers, parameters);
        }

        public static async Task<Calendar> GetCalendarByServiceId(string serviceId)
        {
            return await GetResource<Calendar>("calendars", serviceId);
        }

        public static async Task<List<Route>> GetRoutes()
        {
            return await GetResources<Route>("routes");
        }

        public static async Task<Route> GetRouteById(string routeId)
        {
            return await GetResource<Route>("routes", routeId);
        }

        public static async Task<List<Shape>> GetShapesById(string shapeId)
        {
            return await GetResources<Shape>("shapes", shapeId);
        }

        public static async Task<List<Stop>> GetStops()
        {
            return await GetResources<Stop>("stops");
        }

        public static async Task<Stop> GetStopById(string stopId)
        {
            return await GetResource<Stop>("stops", stopId);
        }

        public static async Task<List<StopTime>> GetStopTimesByTripId(string tripId)
        {
            return await GetResources<StopTime>("stopTimes", tripId);
        }

        public static async Task<ActiveTrip> GetActiveTrips()
        {
            return await GetResourceFromAT<ActiveTrip>("public/realtime/tripupdates");
        }

        public static async Task<ActiveTrip> GetActiveTripsByTripId(string tripId)
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("tripid", tripId)
            };
            return await GetResourceFromAT<ActiveTrip>("public/realtime/tripupdates", parameters);
        }

        public static async Task<ActiveVehicle> GetActiveVehicleByVehicleId(string vehicleId)
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("vehicleid", vehicleId)
            };
            return await GetResourceFromAT<ActiveVehicle>("public/realtime/vehiclelocations", parameters);
        }
    }
}
