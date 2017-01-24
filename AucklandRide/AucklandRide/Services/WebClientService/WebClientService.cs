using AucklandRide.Updater.Models;
using CsvHelper;
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

        private async Task<string> DownloadFile(string url)
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

        public async Task<IEnumerable<Route>> GetRoutes()
        {
            var result = await DownloadFile(string.Format("{0}{1}", _url, "routes.txt"));
            if (result == string.Empty)
                return null;

            var routes = new List<Route>();
            using (var tr = new StringReader(result))
            {
                var csv = new CsvReader(tr);
                csv.Configuration.RegisterClassMap<RouteMap>();
                var records = csv.GetRecords<Route>().ToList();
                return records;
            }                
        }
    }
}
