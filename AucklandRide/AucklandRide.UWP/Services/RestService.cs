using AucklandRide.UWP.Models;
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
        private const string SERVER_URL = "http://aucklandrideapi.azurewebsites.net/api/";

        private static void ClientHeaderInfo(HttpClient client)
        {
            client.BaseAddress = new Uri(SERVER_URL);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static async Task<T> Get<T>(string resourceUri)
        {
            var handler = new HttpClientHandler { UseDefaultCredentials = true };
            using (var client = new HttpClient(handler))
            {
                ClientHeaderInfo(client);
                try
                {
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

        private static async Task<List<T>> GetResources<T>(string uri)
        {
            return await Get<List<T>>(uri);
        }

        private static async Task<T> GetResource<T>(string uri, string id)
        {
            return await Get<T>(uri);
        }

        public static async Task<List<Stop>> GetStops()
        {
            return await GetResources<Stop>("stops");
        }
    }
}
